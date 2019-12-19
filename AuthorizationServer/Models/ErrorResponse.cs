using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AuthorizationServer.Models
{
    public class ErrorResponse
    {
        [JsonProperty("error")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorTypeEnum Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("error_uri")]
        public string ErrorUri { get; set; }
    }
}