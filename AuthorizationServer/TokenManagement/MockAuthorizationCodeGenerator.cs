using System.Text;
using NeoSmart.Utils;

namespace AuthorizationServer.TokenManagement
{
    public class MockAuthorizationCodeGenerator: IAuthorizationCodeGenerator
    {
        public string Generate(string clientId)
        {
            return UrlBase64.Encode(Encoding.UTF8.GetBytes(clientId));
        }
    }
}