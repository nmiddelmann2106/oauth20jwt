namespace Oauth20jwt.Exceptions
{
    public class TokenStringNichtValideException : ArgumentException
    {
        public new string Message { get; set; }
        private string Token { get; set; }
        public TokenStringNichtValideException()
        {
        }

        public TokenStringNichtValideException(string message)
            : base(message)
        {
            this.Message = message;
        }

        public TokenStringNichtValideException(string message, ArgumentException inner)
            : base(message, inner)
        {
            message = "Token nicht valide. ";
            this.Message = message;
        }

        public TokenStringNichtValideException(string message, string token)
            : base(message)
        {
            this.Message = message;
            this.Token = token;
        }
    }
}
