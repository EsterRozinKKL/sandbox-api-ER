using System.Net;
using Dropbox.Api;
using Newtonsoft.Json.Linq;
using sandboxEr.Repositories.Interfaces;
namespace sandboxEr.Repositories.Implementations
{
    public class GetToken:IGetToken
    {
        private const string LoopbackHost = "http://127.0.0.1:52475/";
        private string authorization_code;
        // URL to receive OAuth 2 redirect from Dropbox server.
        // You also need to register this redirect URL on https://www.dropbox.com/developers/apps.
        private readonly Uri RedirectUri = new Uri(LoopbackHost + "authorize");

        // URL to receive access token from JS.
        private readonly Uri JSRedirectUri = new Uri(LoopbackHost + "token");

  public  async Task<OAuth2Response> GenerateToken(TokenParam token)
        {
        if (string.IsNullOrEmpty(token.code))
            {
                throw new ArgumentException("missing authorization_code");
            }
            else if (string.IsNullOrEmpty(token.appKey))
            {
                throw new ArgumentNullException("missing appKey");
            }
            else if (string.IsNullOrEmpty(token.appSecret) && string.IsNullOrEmpty(token.codeVerifier))
            {
                throw new ArgumentNullException(" missing appSecret or codeVerifier");
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

             //   if (!string.IsNullOrEmpty(token.redirectUri))
                {
                    parameters["redirect_uri"] = token.redirectUri;
                }

                var content = new FormUrlEncodedContent(parameters);
                var response = httpClient.PostAsync("https://api.dropbox.com/oauth2/token", content);

                var raw = response.Result.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(raw.Result);
              
                if (response.Result.StatusCode != HttpStatusCode.OK)
                {
              throw new OAuth2Exception( json["error_description"].ToString());
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



public  async Task<string> GenerateCode()
    {

        var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(oauthResponseType:OAuthResponseType.Code,
													   clientId: "mg8u3pah8i07t3q",
													   redirectUri: "http://127.0.0.1:52475/",
													   state: "Israel",
													   tokenAccessType: TokenAccessType.Offline,
													   scopeList: null,
													   includeGrantedScopes: IncludeGrantedScopes.None);

      var http=new HttpListener();
      http.Prefixes.Add(LoopbackHost);
      http.Start();
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(){FileName=authorizeUri.ToString(),UseShellExecute=true} );
      await HandleOAuth2Redirect(http);
      return authorization_code;
    }

     /// <summary>
        /// Handles the redirect from Dropbox server. Because we are using token flow, the local
        /// http server cannot directly receive the URL fragment. We need to return a HTML page with
        /// inline JS which can send URL fragment to local server as URL parameter.
        /// </summary>
        /// <param name="http">The http listener.</param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task HandleOAuth2Redirect(HttpListener http)
        {
            var context = await http.GetContextAsync();
           authorization_code=context.Request.Url.Query;
authorization_code=authorization_code.Replace("?code=","");
authorization_code=authorization_code.Split("&")[0];

        }



    }
}


        