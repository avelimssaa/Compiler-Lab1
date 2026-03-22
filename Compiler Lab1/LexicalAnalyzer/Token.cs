namespace Compiler_Lab1.LexicalAnalyzer
{
    public interface IToken
    {
        int GetConditionCode();
        string GetTokenType();
        string GetLexeme();
        string GetLocation();
        bool IsError();
        string GetMessageDescription();
        int GetStartColumn();
        int GetLine();
        int GetEndColumn();
        TokenType GetTokenTypeEnum();
    }
    public class Token : IToken
    {
        private TokenType _type;
        private string _lexeme;
        private int _line;
        private int _startColumn;
        private int _endColumn;

        private bool _isError;
        private string _errorMessage;

        ITokenDescription _description;

        public Token(TokenType type, string lexeme, int line, int startColumn, int endColumn, bool isError, string errorMessage)
        {
            _type = type;
            _lexeme = lexeme;
            _line = line;
            _startColumn = startColumn;
            _endColumn = endColumn;
            _isError = isError;
            _errorMessage = errorMessage;
            _description = new TokenDescription(); 
        }

        public int GetConditionCode()
        {
            return (int)_type;
        }
        
        public string GetTokenType()
        {
            return _description.GetTokenDescription(_type);
        }

        public string GetLexeme()
        {
            return _lexeme;
        }

        public string GetLocation()
        {
            return $"строка {_line}, {_startColumn} - {_endColumn}";
        }

        public bool IsError()
        {
            return _isError;
        }

        public string GetMessageDescription()
        {
            return _errorMessage;
        }

        public int GetStartColumn()
        {
            return _startColumn;
        }

        public int GetLine()
        {
            return _line;
        }

        public int GetEndColumn()
        {
            return _endColumn;
        }

        public TokenType GetTokenTypeEnum()
        {
            return _type;
        }
    }
}
