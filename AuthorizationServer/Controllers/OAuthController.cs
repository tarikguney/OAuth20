using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using AuthorizationServer.Flows;
using AuthorizationServer.IdentityManagement;
using AuthorizationServer.Models;
using AuthorizationServer.TokenManagement;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Controllers
{
    [Route("api/oauth2")]
    public class OAuthController : Controller
    {
        private readonly IClientManager _clientManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IAuthorizationCodeValidator _authorizationCodeValidator;
        private readonly IReadOnlyDictionary<AuthorizationFlowType, IGrantFlow> _authFlowDictionary;

        public OAuthController(IClientManager clientManager, IJwtGenerator jwtGenerator,
            IReadOnlyDictionary<AuthorizationFlowType, IGrantFlow> authFlowDictionary,
            IAuthorizationCodeValidator authorizationCodeValidator)
        {
            _clientManager = clientManager;
            _jwtGenerator = jwtGenerator;
            _authFlowDictionary = authFlowDictionary;
            _authorizationCodeValidator = authorizationCodeValidator;
        }

        [HttpGet("authorize")]
        public IActionResult ShowLoginPage()
        {
            var responseTypes = Request.Query["response_type"];
            var clientIds = Request.Query["client_id"];
            var redirectUris = Request.Query["redirect_uri"];

            if (responseTypes.Count != 1 || clientIds.Count != 1 || redirectUris.Count != 1)
            {
                var error = new JsonResult(new ErrorResponse
                {
                    Error = ErrorTypeEnum.InvalidRequest
                }) {StatusCode = (int) HttpStatusCode.Unauthorized};
                return error;
            }

            if (!_clientManager.IsValidClient(clientIds[0]))
            {
                var error = new JsonResult(new ErrorResponse
                {
                    Error = ErrorTypeEnum.InvalidClient
                }) {StatusCode = (int) HttpStatusCode.Unauthorized};
                return error;
            }

            //todo Check if the responseType is in the recognized set of values. Otherwise, return invalid grant type response.

            ViewData["RedirectUri"] = redirectUris[0];
            ViewData["ResponseType"] = responseTypes[0];
            ViewData["ClientId"] = clientIds[0];

            return View("AuthorizationLogin");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Username and password fields are required!");
            }

            if (model.Username != "tarik" || model.Password != "guney")
            {
                return Unauthorized("You are not a valid user in the system. Please check your username and password.");
            }

            if (model.ResponseType == "implicit")
            {
                var flow = _authFlowDictionary[AuthorizationFlowType.Implicit];
                return Redirect(flow.BuildRedirectionUri(model).AbsoluteUri);
            }

            if (model.ResponseType == "code")
            {
                var flow = _authFlowDictionary[AuthorizationFlowType.AuthorizationCode];
                return Redirect(flow.BuildRedirectionUri(model).AbsoluteUri);
            }

            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.InvalidRequest
            }) {StatusCode = (int) HttpStatusCode.Unauthorized};
            return error;
        }


        [HttpPost("token")]
        public IActionResult GetAccessToken()
        {
            var grantType = Request.Form["grant_type"];

            switch (grantType)
            {
                case "client_credentials":
                {
                    var (clientSecret, validCredentials) = ExtractAndValidateClientCredentials();

                    if (!validCredentials)
                    {
                        return InvalidClient();
                    }

                    var success = new JsonResult(new AccessTokenResponse
                    {
                        AccessToken = _jwtGenerator.GenerateToken(clientSecret),
                        ExpiresIn = (int) TimeSpan.FromMinutes(10).TotalSeconds,
                        TokenType = "Bearer"
                    }) {StatusCode = (int) HttpStatusCode.OK};
                    return success;
                }
                case "authorization_code":
                {
                    var code = Request.Form["code"];
                    var redirectUri = Request.Form["redirect_uri"];
                    var clientId = Request.Form["client_id"];

                    if (string.IsNullOrWhiteSpace(clientId) || !_clientManager.IsValidClient(clientId))
                    {
                        return InvalidClient();
                    }

                    if (!_clientManager.AllowedToUseGrantType(clientId, GrantType.AuthorizationCode))
                    {
                        return UnauthorizedClient();
                    }

                    if (string.IsNullOrWhiteSpace(redirectUri) || string.IsNullOrWhiteSpace(code) ||
                        !_authorizationCodeValidator.IsValidAuthorizationCode(code, clientId))
                    {
                        return InvalidRequest();
                    }

                    return AccessToken(code);
                }
                case "password":
                {
                    var (clientSecret, validCredentials) = ExtractAndValidateClientCredentials();

                    if (!validCredentials)
                    {
                        return InvalidClient();
                    }

                    if (!Request.Form.ContainsKey("username") ||
                        !Request.Form.ContainsKey("password") ||
                        string.IsNullOrWhiteSpace(Request.Form["username"]) ||
                        string.IsNullOrWhiteSpace(Request.Form["password"]))
                    {
                        return InvalidRequest();
                    }

                    var username = Request.Form["username"];
                    var password = Request.Form["password"];

                    // todo check if the username and password are valid. Create a new service that verifies
                    // the username and password if passed.

                    var success = new JsonResult(new AccessTokenResponse
                    {
                        AccessToken = _jwtGenerator.GenerateToken(clientSecret),
                        ExpiresIn = (int) TimeSpan.FromMinutes(10).TotalSeconds,
                        TokenType = "Bearer"
                    }) {StatusCode = (int) HttpStatusCode.OK};
                    return success;
                }
                default:
                    return UnsupportedGrantType();
            }
        }

        private (string clientSecret, bool validCredentials) ExtractAndValidateClientCredentials()
        {
            var clientCredentials = Request.Headers["Authorization"][0];
            clientCredentials = clientCredentials.Replace("Basic ", "");
            var extractedCredentials =
                Encoding.UTF8.GetString(Convert.FromBase64String(clientCredentials)).Split(':');
            var clientId = extractedCredentials[0];
            var clientSecret = extractedCredentials[1];

            var validCredentials = _clientManager.ValidateClientCredentials(clientId, clientSecret);
            return (clientSecret, validCredentials);
        }

        private IActionResult AccessToken(string secret)
        {
            return new JsonResult(new AccessTokenResponse
            {
                AccessToken = _jwtGenerator.GenerateToken(secret),
                ExpiresIn = (int) TimeSpan.FromMinutes(10).TotalSeconds,
                TokenType = "Bearer"
            }) {StatusCode = (int) HttpStatusCode.OK};
        }

        private IActionResult InvalidClient()
        {
            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.InvalidClient
            }) {StatusCode = (int) HttpStatusCode.BadRequest};
            return error;
        }

        private IActionResult UnauthorizedClient()
        {
            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.UnauthorizedClient
            }) {StatusCode = (int) HttpStatusCode.BadRequest};
            return error;
        }

        private IActionResult InvalidRequest()
        {
            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.InvalidRequest
            }) {StatusCode = (int) HttpStatusCode.BadRequest};
            return error;
        }

        private IActionResult UnsupportedGrantType()
        {
            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.UnsupportedGrantType
            }) {StatusCode = (int) HttpStatusCode.BadRequest};
            return error;
        }
    }
}