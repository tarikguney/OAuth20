using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AuthorizationServer.Flows
{
    [BindProperties(SupportsGet = true)]
    public class AuthorizationFlowModel
    {
        [JsonProperty("response_type")]
        [FromQuery(Name="response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("redirect_uri")]
        [FromQuery(Name="redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonProperty("state")]
        [FromQuery(Name="state")]
        public string State { get; set; }

        [JsonProperty("client_id")]
        [FromQuery(Name="client_id")]
        public string ClientId { get; set; }

        [JsonProperty("scope")]
        [FromQuery(Name="scope")]
        public string Scope { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}