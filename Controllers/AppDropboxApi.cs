using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;


namespace kkl.Services.Controllers
{ 

    [ApiController]

    public class AppDropboxApiController : ControllerBase
    {
        [HttpPost]
        [Route("token")]
        [Consumes("multipart/form-data","application/json", "application/xml")]
        public async Task < IActionResult > Token([FromBody]TokenParam? token) { 
     try
            {
            var response=  GenerateToken(token);
                  return Ok(response);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
}   
 
  
  
       


     public static async Task<OAuth2Response> GenerateToken(TokenParam token)
        {
        if (string.IsNullOrEmpty(token.code))
            {
                throw new ArgumentNullException("code");
            }
            else if (string.IsNullOrEmpty(token.appKey))
            {
                throw new ArgumentNullException("appKey");
            }
            else if (string.IsNullOrEmpty(token.appSecret) && string.IsNullOrEmpty(token.codeVerifier))
            {
                throw new ArgumentNullException("appSecret or codeVerifier");
            }

            var httpClient = new HttpClient();

            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "code", token.code },
                    { "grant_type", "authorization_code" },
                    { "client_id", token.appKey },
                };

                if (!string.IsNullOrEmpty(token.appSecret))
                {
                    parameters["client_secret"] = token.appSecret;
                }

                if (!string.IsNullOrEmpty(token.codeVerifier))
                {
                    parameters["code_verifier"] = token.codeVerifier;
                }

                if (!string.IsNullOrEmpty(token.redirectUri))
                {
                    parameters["redirect_uri"] = token.redirectUri;
                }

                var content = new FormUrlEncodedContent(parameters);
                var response = httpClient.PostAsync("https://api.dropbox.com/oauth2/token", content);

                var raw = response.Result.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(raw.Result);
  
                if (response.Result.StatusCode != HttpStatusCode.OK)
                {
              throw new OAuth2Exception(json["error"].ToString(), json.Value<string>("error_description"));
                 }

                string refreshToken = null;
                if (json.Value<string>("refresh_token") != null)
                {
                    refreshToken = json["refresh_token"].ToString();
                }

                int expiresIn = -1;
                if (json.Value<string>("expires_in") != null)
                {
                    expiresIn = json["expires_in"].ToObject<int>();
                }

                string[] scopeList = null;
                if (json.Value<string>("scope") != null)
                {
                    scopeList = json["scope"].ToString().Split(' ');
                }

                if (expiresIn == -1)
                {
                    return new OAuth2Response(
                        json["access_token"].ToString(),
                        json["uid"].ToString(),
                        null,
                        json["token_type"].ToString());
                }

                return new OAuth2Response(
                    json["access_token"].ToString(),
                    refreshToken,
                    json["uid"].ToString(),
                    null,
                    json["token_type"].ToString(),
                    expiresIn,
                    scopeList);
            }
            finally
            {
          
            }
           
        }
      


 
    }
}
