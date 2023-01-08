public class TokenParam
    {
        public string appKey { get; set; }
        public string appSecret { get; set; }
        public string redirectUri { get; set; }=null;
        public string code { get; set; }
        public string codeVerifier { get; set; }=null;

    }
