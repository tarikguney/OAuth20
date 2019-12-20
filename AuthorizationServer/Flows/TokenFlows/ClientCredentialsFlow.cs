using System;
using System.Net;
using System.Text;
using AuthorizationServer.Flows.FlowResponses;
using AuthorizationServer.IdentityManagement;
using AuthorizationServer.Models;
using AuthorizationServer.TokenManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Flows.TokenFlows
{
    public class ClientCredentialsFlow : ITokenFlow
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IFlowResponses _flowResponses;
        private readonly IClientManager _clientManager;

        public ClientCredentialsFlow(IJwtGenerator jwtGenerator,
            IFlowResponses flowResponses,
            IClientManager clientManager)
        {
            _jwtGenerator = jwtGenerator;
            _flowResponses = flowResponses;
            _clientManager = clientManager;
        }

        public IActionResult ProcessFlow(HttpRequest request)
        {
            var (clientSecret, validCredentials) = ExtractAndValidateClientCredentials(request);

            if (!validCredentials)
            {
                return _flowResponses.InvalidClient();
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