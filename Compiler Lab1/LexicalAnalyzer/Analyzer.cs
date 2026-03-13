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

        private List<IToken> _tokens;


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
                    _tokens.Add(new Token(TokenType.DELIMITER_SPACE, " ", _line, _column, _column, false, ""));
                    _position++;
                    _column++;
                }
                else if (current == '\n')
                {
                    _tokens.Add(new Token(TokenType.DELIMITER_NEWLINE, "\n", _line, _column, _column, false, ""));
                    _position++;
                    _line++;
                    _column = 1;
                }
                else if (current == '\r')
                {
                    _position++;
                }
                else if (current == '\t')
                {
                    _tokens.Add(new Token(TokenType.DELIMITER_TABULATION, "\t", _line, _column, _column, false, ""));
                    _position++;
                    _column++;
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

            while (_position < _input.Length && char.IsDigit(_input[_position]))
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

            TokenType type;

            switch (lexeme)
            {
                case "for":
                    type = TokenType.KEYWORD_FOR;
                    break;
                case "to":
                    type = TokenType.KEYWORD_TO;
                    break;
                case "println":
                    type = TokenType.KEYWORD_PRINTLN;
                    break;
                default:
                    type = TokenType.IDENTIFIER;
                    break;
            }

            _tokens.Add(new Token(type, lexeme, _line, startCol, _column - 1, false, ""));
        }

        private void ProcessOperatorOrDelimiter()
        {
            char current = _input[_position];
            int startCol = _column;

            if (_position + 1 < _input.Length)
            {
                string twoChars = current.ToString() + _input[_position + 1];

                if (twoChars == "<-")
                {
                    _tokens.Add(new Token(TokenType.OPERATOR_ARROW, twoChars, _line, startCol, startCol + 1, false, ""));
                    _position += 2;
                    _column += 2;
                    return;
                }
            }

            TokenType? type = null;

            switch (current)
            {
                case '(':
                    type = TokenType.DELIMITER_LPAREN;
                    break;
                case ')':
                    type = TokenType.DELIMITER_RPAREN;
                    break;
                case '{':
                    type = TokenType.DELIMITER_LBRACE;
                    break;
                case '}':
                    type = TokenType.DELIMITER_RBRACE;
                    break;
                case ';':
                    type = TokenType.DELIMITER_SEMICOLON;
                    break;
            }

            if (type.HasValue)
            {
                _tokens.Add(new Token(type.Value, current.ToString(), _line, startCol, startCol, false, ""));
                _position++;
                _column++;
            }
            else
            {
                type = TokenType.UNKNOWN;
                _tokens.Add(new Token(type.Value, current.ToString(), _line, startCol, startCol, true, $"Недопустимый символ '{current}'"));
                _position++;
                _column++;
            }
        }
    }
}
