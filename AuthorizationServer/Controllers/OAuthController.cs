using System.Collections.Generic;
using System.Net;
using AuthorizationServer.Flows;
using AuthorizationServer.Flows.FlowResponses;
using AuthorizationServer.Flows.TokenFlows;
using AuthorizationServer.IdentityManagement;
using AuthorizationServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Controllers
{
    [Route("api/oauth2")]
    public class OAuthController : Controller
    {
        private readonly IClientManager _clientManager;
        private readonly IReadOnlyDictionary<string, ITokenFlow> _tokenFlowsByGrantTypes;
        private readonly IFlowResponses _flowResponses;
        private readonly IClientGrantManager _clientGrantManager;
        private readonly IReadOnlyDictionary<AuthorizationFlowType, IGrantFlow> _authFlowDictionary;

        public OAuthController(IClientManager clientManager,
            IReadOnlyDictionary<string, ITokenFlow> tokenFlowsByGrantTypes,
            IFlowResponses flowResponses,
            IClientGrantManager clientGrantManager,
            IReadOnlyDictionary<AuthorizationFlowType, IGrantFlow> authFlowDictionary)
        {
            _clientManager = clientManager;
            _tokenFlowsByGrantTypes = tokenFlowsByGrantTypes;
            _flowResponses = flowResponses;
            _clientGrantManager = clientGrantManager;
            _authFlowDictionary = authFlowDictionary;
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

            var clientId = clientIds[0];

            if (!_clientManager.IsValidClient(clientId))
            {
                var error = new JsonResult(new ErrorResponse
                {
                    Error = ErrorTypeEnum.InvalidClient
                }) {StatusCode = (int) HttpStatusCode.Unauthorized};
                return error;
            }

            var responseType = responseTypes[0];
            if (responseType != "token" || responseType == "implicit")
            {
                return _flowResponses.InvalidGrant();
            }

            // todo Think about moving such logic into its own scope. Maybe into the enum? Also, think about separating these to ResponseType and GrantType enums... Might be useful.
            var parsedGrantType = responseType switch
            {
                "token" => GrantType.Implicit,
                "code" => GrantType.AuthorizationCode
            };

            if (!_clientGrantManager.ClientHasGrantType(clientId, parsedGrantType))
            {
                return _flowResponses.InvalidGrant();
            }

            ViewData["RedirectUri"] = redirectUris[0];
            ViewData["ResponseType"] = responseTypes[0];
            ViewData["ClientId"] = clientId;

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
            return !_tokenFlowsByGrantTypes.ContainsKey(grantType)
                ? _flowResponses.UnsupportedGrantType()
                : _tokenFlowsByGrantTypes[grantType].ProcessFlow(Request);
        }
    }
}