using System;
using System.Net;
using AuthorizationServer.Models;
using AuthorizationServer.TokenManagement;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Flows.FlowResponses
{
    public class FlowResponses: IFlowResponses
    {
        private readonly IJwtGenerator _jwtGenerator;

        public FlowResponses(IJwtGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
        }
        
        public IActionResult AccessToken(string secret)
        {
            return new JsonResult(new AccessTokenResponse
            {
                AccessToken = _jwtGenerator.GenerateToken(secret),
                ExpiresIn = (int) TimeSpan.FromMinutes(10).TotalSeconds,
                TokenType = "Bearer"
            }) {StatusCode = (int) HttpStatusCode.OK};
        }

        public IActionResult InvalidClient()
        {
            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.InvalidClient
            }) {StatusCode = (int) HttpStatusCode.BadRequest};
            return error;
        }

        public IActionResult UnauthorizedClient()
        {
            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.UnauthorizedClient
            }) {StatusCode = (int) HttpStatusCode.BadRequest};
            return error;
        }

        public IActionResult InvalidRequest()
        {
            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.InvalidRequest
            }) {StatusCode = (int) HttpStatusCode.BadRequest};
            return error;
        }

        public IActionResult InvalidGrant()
        {
            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.InvalidGrant
            }) {StatusCode = (int) HttpStatusCode.BadRequest};
            return error;
        }

        public IActionResult UnsupportedGrantType()
        {
            var error = new JsonResult(new ErrorResponse
            {
                Error = ErrorTypeEnum.UnsupportedGrantType
            }) {StatusCode = (int) HttpStatusCode.BadRequest};
            return error;
        }
    }
}