using System;
using System.Security.Cryptography;
using System.Text;
using NeoSmart.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AuthorizationServer.TokenManagement
{
    class JwtTokenGenerator : IJWTTokenGenerator
    {
        public string GenerateToken(string secret)
        {
            var headerJson = JObject.Parse($"{{'alg':'{HmacAlgorithm.HS256}','typ':'JWT'}}");
            var headerBytes = Encoding.UTF8.GetBytes(headerJson.ToString());
            Console.WriteLine(HmacAlgorithm.HS256);
            var headerBase64 = UrlBase64.Encode(headerBytes);
	
            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new JwtToken { Name = "tarik guney" }));
            var payloadBase64 = UrlBase64.Encode(payloadBytes);
	
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var signatureContent = Encoding.UTF8.GetBytes(headerBase64 + "." + payloadBase64);
            var signatureBytes = hmac.ComputeHash(signatureContent);
            var signatureBase64 = UrlBase64.Encode(signatureBytes);

            return $"{headerBase64}.{payloadBase64}.{signatureBase64}";
        }
    }
}