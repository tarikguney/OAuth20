using AuthorizationServer.Flows.FlowResponses;
using AuthorizationServer.IdentityManagement;
using AuthorizationServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Flows.TokenFlows
{
    public class AuthorizationCodeFlow : ITokenFlow
    {
        private readonly IClientManager _clientManager;
        private readonly IAuthorizationCodeValidator _authorizationCodeValidator;
        private readonly IFlowResponses _flowResponses;

        public AuthorizationCodeFlow(IClientManager clientManager, 
            IAuthorizationCodeValidator authorizationCodeValidator,
            IFlowResponses flowResponses)
        {
            _clientManager = clientManager;
            _authorizationCodeValidator = authorizationCodeValidator;
            _flowResponses = flowResponses;
        }

        public IActionResult ProcessFlow(HttpRequest request)
        {
            var code = request.Form["code"];
            var redirectUri = request.Form["redirect_uri"];
            var clientId = request.Form["client_id"];

            if (string.IsNullOrWhiteSpace(clientId) || !_clientManager.IsValidClient(clientId))
            {
                return _flowResponses.InvalidClient();
            }

            if (!_clientManager.AllowedToUseGrantType(clientId, GrantType.AuthorizationCode))
            {
                return _flowResponses.UnauthorizedClient();
            }

            if (string.IsNullOrWhiteSpace(redirectUri) || string.IsNullOrWhiteSpace(code) ||
                !_authorizationCodeValidator.IsValidAuthorizationCode(code, clientId))
            {
                return _flowResponses.InvalidRequest();
            }

            return _flowResponses.AccessToken(code);
        }
    }
}