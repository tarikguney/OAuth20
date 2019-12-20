using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Flows.FlowResponses
{
    public interface IFlowResponses
    {
        IActionResult AccessToken(string secret);
        IActionResult InvalidClient();
        IActionResult UnauthorizedClient();
        IActionResult InvalidRequest();
        IActionResult InvalidGrant();
        IActionResult UnsupportedGrantType();
    }
}