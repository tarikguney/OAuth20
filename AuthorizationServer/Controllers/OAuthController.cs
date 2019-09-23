using System;
using System.Text;
using AuthorizationServer.IdentityManagement;
using AuthorizationServer.TokenManagement;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Controllers
{
    [Route("api/as/token.oauth2")]
    public class OAuthController: Controller
    {
        private readonly IClientManager _clientManager;
        private readonly IJWTTokenGenerator _jwtTokenGenerator;

        public OAuthController(IClientManager clientManager, IJWTTokenGenerator jwtTokenGenerator)
        {
            _clientManager = clientManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        
        [HttpPost]
        public ActionResult GetAccessToken()
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
                    return BadRequest();
                }
                return Ok(_jwtTokenGenerator.GenerateToken(clientSecret));
                
            }

            return Ok();
        }
    }
}