using System;
using AuthorizationServer.TokenManagement;

namespace AuthorizationServer.Flows
{
    public class AuthorizationCodeFlow : IAuthorizationEndpointFlow
    {
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;

        public AuthorizationCodeFlow(IAuthorizationCodeGenerator authorizationCodeGenerator)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
        }

        public Uri BuildRedirectionUri(AuthorizationFlowModel model)
        {
            var authCode = _authorizationCodeGenerator.Generate(model.ClientId);
            return new Uri(model.RedirectUri + "?code=" + authCode);
        }
    }
}