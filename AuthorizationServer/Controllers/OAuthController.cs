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
        private readonly IReadOnlyDictionary<AuthorizationFlowType, IGrantFlow> _authFlowDictionary;

        public OAuthController(IClientManager clientManager,
            IReadOnlyDictionary<string, ITokenFlow> tokenFlowsByGrantTypes,
            IFlowResponses flowResponses,
            IReadOnlyDictionary<AuthorizationFlowType, IGrantFlow> authFlowDictionary)
        {
            _clientManager = clientManager;
            _tokenFlowsByGrantTypes = tokenFlowsByGrantTypes;
            _flowResponses = flowResponses;
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
            return !_tokenFlowsByGrantTypes.ContainsKey(grantType)
                ? _flowResponses.UnsupportedGrantType()
                : _tokenFlowsByGrantTypes[grantType].ProcessFlow(Request);
        }
    }
}