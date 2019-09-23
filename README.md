# OAuth 2.0 Authorization Server Implementation

The idea of this project is to write a simple OAuth 2.0 Authorization Server. Don't expect too much. It is progressing with baby steps.

## Sample Postman Scripts

The postman scripts will get updated as the code progress. 

[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/0d9c03bf799e027cc8d0)

## Verify Access Tokens

The access token format this project uses is JWT bearer token. Therefore, you can verify the token there: https://www.jsonwebtoken.io/

The secret used in the HMAC signature digest is `guney`. Silly but works for now.

## Changes `(order by timestamp desc)`

- JWT Token generation is now working.
- Tested with Postman and works nice! Try it yourself