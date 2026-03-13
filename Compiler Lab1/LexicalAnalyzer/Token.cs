namespace Compiler_Lab1.LexicalAnalyzer
{
    public class Token
    {
        public TokenType _type;
        public string _lexeme;
        public int _line;
        public int _startColumn;
        public int _endColumn;

        public bool _isError;
        public string _errorMessage;

        public int _code;
        public string _typeDescription;
        public string _location;

        public Token(TokenType type, string lexeme, int line, int startColumn, int endColumn, bool isError, string errorMessage)
        {
            _type = type;
            _lexeme = lexeme;
            _line = line;
            _startColumn = startColumn;
            _endColumn = endColumn;
            _isError = isError;
            _errorMessage = errorMessage;
        }
    }
}
