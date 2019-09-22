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

            var payload = headerBase + "." + payloadBase;
            Console.WriteLine(payload);

        }
    }

}