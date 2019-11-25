using System.Runtime.Serialization;

namespace AuthorizationServer.Controllers
{
    public enum ErrorTypeEnum
    {
        [EnumMember(Value = "invalid_request")]
        InvalidRequest,

        [EnumMember(Value = "invalid_client")]
        InvalidClient,

        [EnumMember(Value = "invalid_grant")]
        InvalidGrant,

        [EnumMember(Value = "unauthorized_client")]
        UnauthorizedClient,

        [EnumMember(Value = "unsupported_grant_type")]
        UnsupportedGrantType
    }
}