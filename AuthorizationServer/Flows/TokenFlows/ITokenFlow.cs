using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Flows.TokenFlows
{
    public interface ITokenFlow
    {
        IActionResult ProcessFlow(HttpRequest request);
    }
}