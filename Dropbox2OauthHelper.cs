using System;
using System.Text;
using Dropbox.Api;

public sealed class OAuth2Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Response"/> class.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="state">The state.</param>
        /// <param name="tokenType">The token type.</param>
        /// <param name="expiresIn">The duration until token expiry in seconds.</param>
        /// <param name="scopeList">The scope list.</param>
        internal OAuth2Response(string accessToken, string refreshToken, string uid, string state, string tokenType, int expiresIn, string[] scopeList)
        {
          
            this.AccessToken = accessToken;
            this.Uid = uid;
            this.State = state;
            this.TokenType = tokenType;
            this.RefreshToken = refreshToken;
            this.ExpiresAt = DateTime.Now.AddSeconds(expiresIn);
            this.ScopeList = scopeList;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Response"/> class.
        /// </summary>
        /// <param name="accessToken">The access_token.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="state">The state.</param>
        /// <param name="tokenType">The token_type.</param>
        internal OAuth2Response(string accessToken, string uid, string state, string tokenType)
        {
         

            this.AccessToken = accessToken;
            this.Uid = uid;
            this.State = state;
            this.TokenType = tokenType;
            this.RefreshToken = null;
            this.ExpiresAt = null;
            this.ScopeList = null;
        }
  /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Response"/> class.
        /// </summary>
        /// <param name="message">message</param>
        internal OAuth2Response(string message)
        {
            this.AccessToken = null;
            this.Uid = null;
            this.State = null;
            this.TokenType = null;
            this.RefreshToken = null;
            this.ExpiresAt = null;
            this.ScopeList = null;
        }
        /// <summary>
        /// Gets the access token, a token which can be used to make calls to the Dropbox API.
        /// </summary>
        /// <remarks>
        /// Pass this as the <c>oauth2AccessToken</c> argument when creating an instance
        /// of <see cref="DropboxClient"/>.</remarks>
        /// <value>
        /// A token which can be used to make calls to the Dropbox API.
        /// </value>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the Dropbox user ID of the authorized user.
        /// </summary>
        /// <value>
        /// The Dropbox user ID of the authorized user.
        /// </value>
        public string Uid { get; private set; }

        /// <summary>
        /// Gets the state content, if any, originally passed to the authorize URI.
        /// </summary>
        /// <value>
        /// The state content, if any, originally passed to the authorize URI.
        /// </value>
        public string State { get; private set; }

        /// <summary>
        /// Gets the type of the token, which will always be <c>bearer</c> if set.
        /// </summary>
        /// <value>
        /// This will always be <c>bearer</c> if set.
        /// </value>
        public string TokenType { get; private set; }

        /// <summary>
        /// Gets the refresh token, if offline or online access type was selected.
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Gets the time of expiration of the access token, if the token will expire.
        /// This is only filled if offline or online access type was selected.
        /// </summary>
        public DateTime? ExpiresAt { get; private set; }

        /// <summary>
        /// Gets list of scopes this oauth2 request granted the user.
        /// </summary>
        public string[] ScopeList { get; private set; }
    }
 public sealed class OAuth2Exception : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Exception"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorDescription">The error description.</param>
        public OAuth2Exception(string message, string errorDescription = null)
            : base(message)
        {
            this.ErrorDescription = errorDescription;
        }

        /// <summary>
        /// Gets the error description.
        /// </summary>
        public string ErrorDescription { get; private set; }
    }
    

    public static class DropboxOAuth2Helper
    {
        /// <summary>
        /// Length of Code Verifier for PKCE to be used in OAuth Flow.
        /// </summary>
        private static readonly int PKCEVerifierLength = 128;

    /// <summary>
        /// Gets the URI used to start the OAuth2.0 authorization flow.
        /// </summary>
        /// <param name="oauthResponseType">The grant type requested, either <c>Token</c> or <c>Code</c>.</param>
        /// <param name="clientId">The apps key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">Where to redirect the user after authorization has completed. This must be the exact URI
        /// registered in the <a href="https://www.dropbox.com/developers/apps">App Console</a>; even <c>localhost</c>
        /// must be listed if it is used for testing. A redirect URI is required for a token flow, but optional for code.
        /// If the redirect URI is omitted, the code will be presented directly to the user and they will be invited to enter
        /// the information in your app.</param>
        /// <param name="state">Up to 500 bytes of arbitrary data that will be passed back to <paramref name="redirectUri"/>.
        /// This parameter should be used to protect against cross-site request forgery (CSRF).</param>
        /// <param name="forceReapprove">Whether or not to force the user to approve the app again if they've already done so.
        /// If <c>false</c> (default), a user who has already approved the application may be automatically redirected to
        /// <paramref name="redirectUri"/>If <c>true</c>, the user will not be automatically redirected and will have to approve
        /// the app again.</param>
        /// <param name="disableSignup">When <c>true</c> (default is <c>false</c>) users will not be able to sign up for a
        /// Dropbox account via the authorization page. Instead, the authorization page will show a link to the Dropbox
        /// iOS app in the App Store. This is only intended for use when necessary for compliance with App Store policies.</param>
        /// <param name="requireRole">If this parameter is specified, the user will be asked to authorize with a particular
        /// type of Dropbox account, either work for a team account or personal for a personal account. Your app should still
        /// verify the type of Dropbox account after authorization since the user could modify or remove the require_role
        /// parameter.</param>
        /// <param name="forceReauthentication"> If <c>true</c>, users will be signed out if they are currently signed in.
        /// This will make sure the user is brought to a page where they can create a new account or sign in to another account.
        /// This should only be used when there is a definite reason to believe that the user needs to sign in to a new or
        /// different account.</param>
        /// <param name="tokenAccessType">Determines the type of token to request.  See <see cref="TokenAccessType" />
        /// for information on specific types available.  If none is specified, this will use the legacy type.</param>
        /// <param name="scopeList">list of scopes to request in base oauth flow.  If left blank, will default to all scopes for app.</param>
        /// <param name="includeGrantedScopes">which scopes to include from previous grants. Note: if this user has never linked the app, include_granted_scopes must be None.</param>
        /// <param name="codeChallenge">If using PKCE, please us the PKCEOAuthFlow object.</param>
        /// <returns>The uri of a web page which must be displayed to the user in order to authorize the app.</returns>
        public static Uri GetAuthorizeUri(OAuthResponseType oauthResponseType, string clientId, string redirectUri = null, string state = null, bool forceReapprove = false, bool disableSignup = false, string requireRole = null, bool forceReauthentication = false, TokenAccessType tokenAccessType = TokenAccessType.Legacy, string[] scopeList = null, IncludeGrantedScopes includeGrantedScopes = IncludeGrantedScopes.None, string codeChallenge = null)
        {
            var uri = string.IsNullOrEmpty(redirectUri) ? null : new Uri(redirectUri);

            return GetAuthorizeUri(oauthResponseType, clientId, uri, state, forceReapprove, disableSignup, requireRole, forceReauthentication, tokenAccessType, scopeList, includeGrantedScopes, codeChallenge);
        }
          /// <summary>
        /// Gets the URI used to start the OAuth2.0 authorization flow.
        /// </summary>
        /// <param name="oauthResponseType">The grant type requested, either <c>Token</c> or <c>Code</c>.</param>
        /// <param name="clientId">The apps key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">Where to redirect the user after authorization has completed. This must be the exact URI
        /// registered in the <a href="https://www.dropbox.com/developers/apps">App Console</a>; even <c>localhost</c>
        /// must be listed if it is used for testing. A redirect URI is required for a token flow, but optional for code.
        /// If the redirect URI is omitted, the code will be presented directly to the user and they will be invited to enter
        /// the information in your app.</param>
        /// <param name="state">Up to 500 bytes of arbitrary data that will be passed back to <paramref name="redirectUri"/>.
        /// This parameter should be used to protect against cross-site request forgery (CSRF).</param>
        /// <param name="forceReapprove">Whether or not to force the user to approve the app again if they've already done so.
        /// If <c>false</c> (default), a user who has already approved the application may be automatically redirected to
        /// <paramref name="redirectUri"/>If <c>true</c>, the user will not be automatically redirected and will have to approve
        /// the app again.</param>
        /// <param name="disableSignup">When <c>true</c> (default is <c>false</c>) users will not be able to sign up for a
        /// Dropbox account via the authorization page. Instead, the authorization page will show a link to the Dropbox
        /// iOS app in the App Store. This is only intended for use when necessary for compliance with App Store policies.</param>
        /// <param name="requireRole">If this parameter is specified, the user will be asked to authorize with a particular
        /// type of Dropbox account, either work for a team account or personal for a personal account. Your app should still
        /// verify the type of Dropbox account after authorization since the user could modify or remove the require_role
        /// parameter.</param>
        /// <param name="forceReauthentication"> If <c>true</c>, users will be signed out if they are currently signed in.
        /// This will make sure the user is brought to a page where they can create a new account or sign in to another account.
        /// This should only be used when there is a definite reason to believe that the user needs to sign in to a new or
        /// different account.</param>
        /// <param name="tokenAccessType">Determines the type of token to request.  See <see cref="TokenAccessType" />
        /// for information on specific types available.  If none is specified, this will use the legacy type.</param>
        /// <param name="scopeList">list of scopes to request in base oauth flow.  If left blank, will default to all scopes for app.</param>
        /// <param name="includeGrantedScopes">which scopes to include from previous grants. Note: if this user has never linked the app, include_granted_scopes must be None.</param>
        /// <param name="codeChallenge">If using PKCE, please us the PKCEOAuthFlow object.</param>
        /// <returns>The uri of a web page which must be displayed to the user in order to authorize the app.</returns>
        public static Uri GetAuthorizeUri(OAuthResponseType oauthResponseType, string clientId, Uri redirectUri = null, string state = null, bool forceReapprove = false, bool disableSignup = false, string requireRole = null, bool forceReauthentication = false, TokenAccessType tokenAccessType = TokenAccessType.Legacy, string[] scopeList = null, IncludeGrantedScopes includeGrantedScopes = IncludeGrantedScopes.None, string codeChallenge = null)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException("clientId");
            }

            if (redirectUri == null && oauthResponseType != OAuthResponseType.Code)
            {
                throw new ArgumentNullException("redirectUri");
            }

            var queryBuilder = new StringBuilder();

            queryBuilder.Append("response_type=");
            switch (oauthResponseType)
            {
                case OAuthResponseType.Token:
                    queryBuilder.Append("token");
                    break;
                case OAuthResponseType.Code:
                    queryBuilder.Append("code");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("oauthResponseType");
            }

            queryBuilder.Append("&client_id=").Append(Uri.EscapeDataString(clientId));

            if (redirectUri != null)
            {
                queryBuilder.Append("&redirect_uri=").Append(Uri.EscapeDataString(redirectUri.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                queryBuilder.Append("&state=").Append(Uri.EscapeDataString(state));
            }

            if (forceReapprove)
            {
                queryBuilder.Append("&force_reapprove=true");
            }

            if (disableSignup)
            {
                queryBuilder.Append("&disable_signup=true");
            }

            if (!string.IsNullOrWhiteSpace(requireRole))
            {
                queryBuilder.Append("&require_role=").Append(requireRole);
            }

            if (forceReauthentication)
            {
                queryBuilder.Append("&force_reauthentication=true");
            }

            queryBuilder.Append("&token_access_type=").Append(tokenAccessType.ToString().ToLower());

            if (scopeList != null)
            {
                queryBuilder.Append("&scope=").Append(string.Join(" ", scopeList));
            }

            if (includeGrantedScopes != IncludeGrantedScopes.None)
            {
                queryBuilder.Append("&include_granted_scopes=").Append(includeGrantedScopes.ToString().ToLower());
            }

            if (codeChallenge != null)
            {
                queryBuilder.Append("&code_challenge_method=S256&code_challenge=").Append(codeChallenge);
            }

            var uriBuilder = new UriBuilder("https://www.dropbox.com/oauth2/authorize")
            {
                Query = queryBuilder.ToString(),
            };

            return uriBuilder.Uri;
        }

    }

