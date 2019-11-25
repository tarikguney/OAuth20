using System;
using AuthorizationServer.TokenManagement;

namespace AuthorizationServer.Flows
{
    public interface IGrantFlow
    {
        Uri BuildRedirectionUri(AuthorizationFlowModel model);
    }

    public class ImplicitFlow : IGrantFlow
    {
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;
        private readonly IJWTGenerator _jwtGenerator;

        public ImplicitFlow(IAuthorizationCodeGenerator authorizationCodeGenerator,
            IJWTGenerator jwtGenerator)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _jwtGenerator = jwtGenerator;
        }

        public Uri BuildRedirectionUri(AuthorizationFlowModel model)
        {
            var accessToken = _jwtGenerator.GenerateToken($"{model.Username}_{model.Password}");
            var expiresIn = (int) TimeSpan.FromMinutes(10).TotalSeconds;
            var tokenType = "Bearer";

            return new Uri($"{model.RedirectUri}#access_token={accessToken}&expires_in={expiresIn}&token_type={tokenType}");
        }
    }
}