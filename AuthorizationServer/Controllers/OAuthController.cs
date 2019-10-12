using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using AuthorizationServer.IdentityManagement;
using AuthorizationServer.TokenManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AuthorizationServer.Controllers
{
    [Route("api/oauth2")]
    public class OAuthController : Controller
    {
        private readonly IClientManager _clientManager;
        private readonly IJWTTokenGenerator _jwtTokenGenerator;
        private readonly IAuthorizationCodeGenerator _authCodeGenerator;

        public OAuthController(IClientManager clientManager, IJWTTokenGenerator jwtTokenGenerator, IAuthorizationCodeGenerator authCodeGenerator)
        {
            _clientManager = clientManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _authCodeGenerator = authCodeGenerator;
        }

        [HttpGet("authorize")]
        public IActionResult GetAuthorizationCode()
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

            if (responseTypes[0] == "code")
            {
                if (!_clientManager.IsValidClient(clientIds[0]))
                {
                    var error = new JsonResult(new ErrorResponse
                    {
                        Error = ErrorTypeEnum.InvalidClient
                    }) {StatusCode = (int) HttpStatusCode.Unauthorized};
                    return error;
                }

                ViewData["RedirectUri"] = redirectUris[0];
                return View("AuthorizationLogin", HttpUtility.UrlEncode(redirectUris[0]));
            }

            return null;
        }

        [HttpPost("login")]
        public IActionResult Login([FromForm] string username, 
            [FromForm] string password, [FromQuery(Name = "redirect_uri")] string redirectUri)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password fields are required!");
            }

            if (username == "tarik" && password == "guney")
            {
                var authCode = _authCodeGenerator.Generate(username);
                return Redirect(redirectUri+"?code=" + authCode);
            }

            return Unauthorized("You are not a valid user in the system. Please check your username and password.");
        }


        [HttpPost("token")]
        public IActionResult GetAccessToken()
        {
            var grantType = Request.Form["grant_type"];
            var clientCredentials = Request.Headers["Authorization"][0];
            if (grantType == "client_credentials")
            {
                clientCredentials = clientCredentials.Replace("Basic ", "");
                var extractedCredentials =
                    Encoding.UTF8.GetString(Convert.FromBase64String(clientCredentials)).Split(':');
                var clientId = extractedCredentials[0];
                var clientSecret = extractedCredentials[1];

                var validCredentials = _clientManager.ValidateClientCredentials(clientId, clientSecret);

                if (!validCredentials)
                {
                    var error = new JsonResult(new ErrorResponse
                    {
                        Error = ErrorTypeEnum.InvalidClient
                    }) {StatusCode = (int) HttpStatusCode.Unauthorized};
                    return error;
                }

                var success = new JsonResult(new AccessTokenResponse
                {
                    AccessToken = _jwtTokenGenerator.GenerateToken(clientSecret),
                    ExpiresIn = (int) TimeSpan.FromMinutes(10).TotalSeconds,
                    TokenType = "Bearer"
                }) {StatusCode = (int) HttpStatusCode.OK};
                return success;
            }

            return Ok();
        }


        public class ErrorResponse
        {
            [JsonProperty("error")]
            [JsonConverter(typeof(StringEnumConverter))]
            public ErrorTypeEnum Error { get; set; }

            [JsonProperty("error_description")] public string ErrorDescription { get; set; }

            [JsonProperty("error_uri")] public string ErrorUri { get; set; }
        }

        public enum ErrorTypeEnum
        {
            [EnumMember(Value = "invalid_request")]
            InvalidRequest,

            [EnumMember(Value = "invalid_client")] InvalidClient,

            [EnumMember(Value = "invalid_grant")] InvalidGrant,

            [EnumMember(Value = "unauthorized_client")]
            UnauthorizedClient,

            [EnumMember(Value = "unsupported_grant_type")]
            UnsupportedGrantType
        }

        public class AccessTokenResponse
        {
            [JsonProperty("access_token")] public string AccessToken { get; set; }

            [JsonProperty("token_type")] public string TokenType { get; set; }

            [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
        }
    }
}