using System;
using System.Text;
using NeoSmart.Utils;

namespace AuthorizationServer.TokenManagement
{
    public class AuthorizationCodeGenerator: IAuthorizationCodeGenerator
    {
        public string Generate(string clientId)
        {
            return UrlBase64.Encode(Encoding.UTF8.GetBytes(clientId));
        }
    }
}