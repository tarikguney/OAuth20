using System;
using System.Net;
using System.Text;
using AuthorizationServer.Flows.FlowResponses;
using AuthorizationServer.IdentityManagement;
using AuthorizationServer.Models;
using AuthorizationServer.TokenManagement;
using AuthorizationServer.UserManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Flows.TokenFlows
{
    public class PasswordFlow : ITokenFlow
    {
        private readonly IClientManager _clientManager;
        private readonly IFlowResponses _flowResponses;
        private readonly IUserCredentialValidator _userCredentialValidator;
        private readonly IJwtGenerator _jwtGenerator;

        public PasswordFlow(IClientManager clientManager, IFlowResponses flowResponses,
            IUserCredentialValidator userCredentialValidator, IJwtGenerator jwtGenerator)
        {
            _clientManager = clientManager;
            _flowResponses = flowResponses;
            _userCredentialValidator = userCredentialValidator;
            _jwtGenerator = jwtGenerator;
        }

        public IActionResult ProcessFlow(HttpRequest request)
        {
            var (clientSecret, validCredentials) = ExtractAndValidateClientCredentials(request);

            if (!validCredentials)
            {
                return _flowResponses.InvalidClient();
            }

            if (!request.Form.ContainsKey("username") ||
                !request.Form.ContainsKey("password") ||
                string.IsNullOrWhiteSpace(request.Form["username"]) ||
                string.IsNullOrWhiteSpace(request.Form["password"]))
            {
                return _flowResponses.InvalidRequest();
            }

            var username = request.Form["username"];
            var password = request.Form["password"];

            if (!_userCredentialValidator.ValidateCredentials(username, password))
            {
                return _flowResponses.InvalidGrant();
            }

            var success = new JsonResult(new AccessTokenResponse
            {
                AccessToken = _jwtGenerator.GenerateToken(clientSecret),
                ExpiresIn = (int) TimeSpan.FromMinutes(10).TotalSeconds,
                TokenType = "Bearer"
            }) {StatusCode = (int) HttpStatusCode.OK};
            return success;
        }

        private (string clientSecret, bool validCredentials) ExtractAndValidateClientCredentials(HttpRequest request)
        {
            var clientCredentials = request.Headers["Authorization"][0];
            clientCredentials = clientCredentials.Replace("Basic ", "");
            var extractedCredentials =
                Encoding.UTF8.GetString(Convert.FromBase64String(clientCredentials)).Split(':');
            var clientId = extractedCredentials[0];
            var clientSecret = extractedCredentials[1];

            var validCredentials = _clientManager.ValidateClientCredentials(clientId, clientSecret);
            return (clientSecret, validCredentials);
        }
    }
}