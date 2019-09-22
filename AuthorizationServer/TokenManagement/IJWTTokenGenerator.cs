using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace AuthorizationServer.TokenManagement
{
    public interface IJWTTokenGenerator
    {
        string GenerateToken(string secret);
    }

    class JwtTokenGenerator : IJWTTokenGenerator
    {
        public string GenerateToken(string secret)
        {
            var headerJson = JsonConvert.DeserializeObject($"{{'alg':'{HmacAlgorithm.HS256}', 'typ': 'JWT'}}").ToString();
            var headerBytes = Encoding.UTF8.GetBytes(headerJson);
            var headerBase = Base64UrlEncoder.Encode(headerBytes);

            var payloadBytes =
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new JwtToken() { Name = "Kazim Ozkurt" }));
            var payloadBase = Base64UrlEncoder.Encode(payloadBytes);

            var hmac = new HMACSHA256();
            hmac.Key = Encoding.UTF8.GetBytes("youtube");
            var payload = Encoding.UTF8.GetBytes(headerBase + "." + payloadBase);
            var signatureBytes = hmac.ComputeHash(payload);
            var signatureBase64 = Base64UrlEncoder.Encode(signatureBytes);
            Console.WriteLine(headerBase + "." + payloadBase + "." + signatureBase64);
        }
    }

}