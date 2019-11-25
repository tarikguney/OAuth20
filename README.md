# OAuth 2.0 Authorization Server Implementation

The idea of this project is to write a simple OAuth 2.0 Authorization Server. Don't expect too much. It is progressing with baby steps.

## Specifications

This project uses the following RFC documents as the specification for its implementation. The idea is to comply with the specs as much as possible with little or no diversion. 

- [RFC 6749](https://tools.ietf.org/html/rfc6749) - The OAuth 2.0 Authorization Framework
- [RFC 7519](https://tools.ietf.org/html/rfc7519) - JSON Web Token (JWT)
- [RFC 6750](https://tools.ietf.org/html/rfc6750) - The OAuth 2.0 Authorization Framework: Bearer Token Usage

## Sample Postman Scripts

The postman scripts will get updated as the code progress. 

[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/0d9c03bf799e027cc8d0)

## Verify Access Tokens

The access token format this project uses is JWT bearer token. Therefore, you can verify the token there: https://www.jsonwebtoken.io/

The secret used in the HMAC signature digest is `client_id`. Silly, but it is how things work for now.

## Contribute

I always welcome PRs from the community. As you may know, this project is developed during live streams on my channel at http://youtube.com/tarikguneyphd. Currently, the videos are in Turkish but perhaps later we can have a summary video in English, too. Anyhow, please feel free to send me pull requests. 

## Suggestions

- Use `Guid.NewGuid().ToString("N")` to generate unique `authorization_code` `code` values.

## Changes `(order by timestamp desc)`

- **[11/25/2019]** - Implemented access token request for authorization code grant type, and renamed `IAuthorizationEndpointFlow` to `IGrant` along with some other small code clean up and refactorings.
- **[10/13/2019]** Implemented implicit flow, refactored the code a little bit, and extracted implicit and authorization code flows into their respective classes with one interface.
- **[10/12/2019]** Implemented a simple authorization endpoints. Allows people to log in to get the authorization code. 
- **[10/06/2019]** Returning correctly formatted response for `access_token` request for both successful and erroneous  situations.
- [Older Date] JWT Token generation is now working.
- [Older Date] Tested with Postman and works nice! Try it yourself