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
            if (string.IsNullOrEmpty(accessToken) || uid == null)
            {
                throw new ArgumentException("Invalid OAuth 2.0 response, missing access_token and/or uid.");
            }

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
            if (string.IsNullOrEmpty(accessToken) || uid == null)
            {
                throw new ArgumentException("Invalid OAuth 2.0 response, missing access_token and/or uid.");
            }

            this.AccessToken = accessToken;
            this.Uid = uid;
            this.State = state;
            this.TokenType = tokenType;
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
