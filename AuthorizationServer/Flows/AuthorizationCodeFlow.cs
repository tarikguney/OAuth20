using System;
using AuthorizationServer.TokenManagement;

namespace AuthorizationServer.Flows
{
    public class AuthorizationCodeFlow : IGrantFlow, IAuthorizationCodeValidator
    {
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;

        public AuthorizationCodeFlow(IAuthorizationCodeGenerator authorizationCodeGenerator)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
        }

        public Uri BuildRedirectionUri(LoginModel model)
        {
            var authCode = _authorizationCodeGenerator.Generate(model.ClientId);
            return new Uri(model.RedirectUri + "?code=" + authCode);
        }

        public bool IsValidAuthorizationCode(string code, string clientId)
        {
            return true;
        }
        
    }
}