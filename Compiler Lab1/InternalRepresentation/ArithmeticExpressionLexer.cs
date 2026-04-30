namespace Compiler_Lab1.InternalRepresentation
{
    internal interface IArithmeticExpressionLexer
    {
        List<IArithToken> Tokens { get; }
    }
    internal class ArithmeticExpressionLexer : IArithmeticExpressionLexer
    {
        private readonly List<IArithToken> _tokens = [];
        public List<IArithToken> Tokens { get  { return _tokens; } }

        private readonly string _text = "";
        private int _index = 0, _line = 1, _column = 1;


        public ArithmeticExpressionLexer(string text)
        {
            _text = text;
            ProcessTokens();
        }

        private void ProcessTokens()
        {
            while (_index < _text.Length)
            {
                char current = _text[_index];

                if (current == ' ')
                {
                    AddToken(" ", _line, _column, ArithmeticTokenType.SPACE);
                    _index++;
                    _column++;
                }

                else if (current == '+')
                {
                    AddToken("+", _line, _column, ArithmeticTokenType.SUM);
                    _index++;
                    _column++;
                }

                else if (current == '-')
                {
                    AddToken("-", _line, _column, ArithmeticTokenType.DIF);
                    _index++;
                    _column++;
                }

                else if (current == '*')
                {
                    AddToken("*", _line, _column, ArithmeticTokenType.MULTIPLICATION);
                    _index++;
                    _column++;
                }

                else if (current == '/')
                {
                    AddToken("/", _line, _column, ArithmeticTokenType.DIVISION);
                    _index++;
                    _column++;
                }

                else if (current == '%')
                {
                    AddToken("%", _line, _column, ArithmeticTokenType.REMAINDER);
                    _index++;
                    _column++;
                }

                else if (current == '\r' || current == '\t')
                {
                    _index++;
                    _column++;
                }

                else if (current == '\n')
                {
                    AddToken("\n", _line, _column, ArithmeticTokenType.NEW_LINE);
                    _index++;
                    _column = 1;
                    _line++;
                }

                else if (current == '(')
                {
                    AddToken("(", _line, _column, ArithmeticTokenType.LEFT_BRACE);
                    _index++;
                    _column++;
                }

                else if (current == ')')
                {
                    AddToken(")", _line, _column, ArithmeticTokenType.RIGHT_BRACE);
                    _index++;
                    _column++;
                }

                else if (char.IsDigit(current))
                {
                    ProcessNumber();
                }

                else if (char.IsLetter(current))
                {
                    ProcessIdentiier();
                }

                else
                {
                    AddToken(current.ToString(), _line, _column, ArithmeticTokenType.UNKNOWN);
                    _index++;
                    _column++;
                }
            }

            AddToken("EOF", _line, _column, ArithmeticTokenType.EOF);
        }

        private void ProcessNumber()
        {
            string digit = "";
            char current = _text[_index];

            while (_index < _text.Length && char.IsDigit(current))
            {
                digit += current;
                _index++;
                if (_index == _text.Length)
                    break;
                current = _text[_index];
            }

            if (!char.IsLetter(current))
            {
                AddToken(digit, _line, _column, ArithmeticTokenType.NUMBER);
            }
            else
            {
                while (char.IsLetterOrDigit(current))
                {
                    digit += current;
                    _index++;
                    if (_index == _text.Length)
                        break;
                    current = _text[_index];
                }

                AddToken(digit, _line, _column, ArithmeticTokenType.UNKNOWN);
            }

            _column += digit.Length;
        }

        private void ProcessIdentiier()
        {
            string identifier = "";

            while (_index < _text.Length && char.IsLetterOrDigit(_text[_index]))
            {
                char current = _text[_index];
                identifier += current;
                _index++;
                if (_index == _text.Length)
                    break;
            }

            AddToken(identifier, _line, _column, ArithmeticTokenType.IDENTIFIER);

            _column += identifier.Length;
        }

        private void AddToken(string lexeme, int line, int column, ArithmeticTokenType type)
        {
            ArithmeticToken token = new ArithmeticToken(line, column, lexeme, type);
            _tokens.Add(token);
        }
    }
}
