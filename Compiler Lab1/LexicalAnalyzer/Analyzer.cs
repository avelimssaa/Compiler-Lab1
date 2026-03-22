namespace Compiler_Lab1.LexicalAnalyzer
{
    public interface IAnalyzer
    {
        List<IToken> Scan(string input);
    }

    internal class Analyzer : IAnalyzer
    {
        private string _input;
        private int _position;
        private int _line;
        private int _column;

        private readonly List<IToken> _tokens;

        public Analyzer()
        {
            _tokens = new List<IToken>();
        }

        public List<IToken> Scan(string input)
        {
            _input = input;
            _position = 0;
            _line = 1;
            _column = 1;
            _tokens.Clear();

            while (_position < _input.Length)
            {
                char current = _input[_position];

                if (current == ' ')
                {
                    AddToken(TokenType.DELIMITER_SPACE, " ", 1);
                }
                else if (current == '\n')
                {
                    AddToken(TokenType.DELIMITER_NEWLINE, "\n", 1);
                    _line++;
                    _column = 1;
                }
                else if (current == '\r')
                {
                    _position++;
                }
                else if (current == '\t')
                {
                    AddToken(TokenType.DELIMITER_TABULATION, "\t", 1);
                }
                else if (char.IsDigit(current))
                {
                    ProcessNumber();
                }
                else if (char.IsLetter(current))
                {
                    ProcessIdentifierOrKeyword();
                }
                else
                {
                    ProcessOperatorOrDelimiter();
                }
            }

            return _tokens;
        }

        private void ProcessNumber()
        {
            int startPos = _position;
            int startCol = _column;

            while (_position < _input.Length &&
                   char.IsDigit(_input[_position]))
            {
                _position++;
                _column++;
            }

            string number = _input.Substring(startPos, _position - startPos);
            _tokens.Add(new Token(TokenType.DIGIT, number, _line, startCol, _column - 1, false, ""));
        }

        private void ProcessIdentifierOrKeyword()
        {
            int startPos = _position;
            int startCol = _column;

            while (_position < _input.Length &&
                  (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
            {
                _position++;
                _column++;
            }

            string lexeme = _input.Substring(startPos, _position - startPos);

            if (lexeme == "for")
            {
                if (_position >= _input.Length || !char.IsLetterOrDigit(_input[_position]))
                {
                    _tokens.Add(new Token(TokenType.KEYWORD_FOR, lexeme, _line, startCol, _column - 1, false, ""));
                    return;
                }
            }

            if (lexeme == "to")
            {
                if (_position >= _input.Length || !char.IsLetterOrDigit(_input[_position]))
                {
                    _tokens.Add(new Token(TokenType.KEYWORD_TO, lexeme, _line, startCol, _column - 1, false, ""));
                    return;
                }
            }

            if (lexeme == "println")
            {
                _tokens.Add(new Token(TokenType.KEYWORD_PRINTLN, lexeme, _line, startCol, _column - 1, false, ""));
                return;
            }

            _tokens.Add(new Token(TokenType.IDENTIFIER, lexeme, _line, startCol, _column - 1, false, ""));
        }

        private void ProcessOperatorOrDelimiter()
        {
            char current = _input[_position];

            switch (current)
            {
                case '(':
                    AddToken(TokenType.DELIMITER_LPAREN, "(", 1);
                    return;

                case ')':
                    AddToken(TokenType.DELIMITER_RPAREN, ")", 1);
                    return;

                case '{':
                    AddToken(TokenType.DELIMITER_LBRACE, "{", 1);
                    return;

                case '}':
                    AddToken(TokenType.DELIMITER_RBRACE, "}", 1);
                    return;

                case ';':
                    AddToken(TokenType.DELIMITER_SEMICOLON, ";", 1);
                    return;
            }

            if (_position + 1 < _input.Length)
            {
                string two = _input.Substring(_position, 2);

                if (two == "<-")
                {
                    AddToken(TokenType.OPERATOR_ARROW, two, 2);
                    return;
                }
            }

            AddToken(TokenType.UNKNOWN, current.ToString(), 1, true, $"Недопустимый символ '{current}'");
        }

        private void AddToken(TokenType type, string lexeme, int length, bool isError = false, string message = "")
        {
            _tokens.Add(new Token(type, lexeme, _line, _column, _column + length - 1, isError, message));
            _position += length;
            _column += length;
        }
    }
}
