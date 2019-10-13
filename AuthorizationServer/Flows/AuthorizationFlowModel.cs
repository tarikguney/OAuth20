using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AuthorizationServer.Flows
{
    [BindProperties(SupportsGet = true)]
    public class AuthorizationFlowModel
    {
        [JsonProperty("response_type")]
        [BindProperty(Name = "response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("redirect_uri")]
        [BindProperty(Name = "redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonProperty("state")]
        [BindProperty(Name = "state")]
        public string State { get; set; }

        [JsonProperty("client_id")]
        [BindProperty(Name = "client_id")]
        public string ClientId { get; set; }

        [JsonProperty("scope")]
        [BindProperty(Name = "scope")]
        public string Scope { get; set; }

        [JsonProperty("username")]
        [BindProperty(Name = "username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        [BindProperty(Name = "password")]
        public string Password { get; set; }
    }
}