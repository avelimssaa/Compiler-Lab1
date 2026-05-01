namespace Compiler_Lab1.InternalRepresentation
{
    internal interface IArithParser
    {
        List<IArithToken> Tokens { get; }
        List<IArithSyntax> Errors {  get; }
    }
    internal class ArithmeticExpressionParser : IArithParser
    {
        private readonly List<IArithToken> _tokens;
        public List<IArithToken> Tokens {  get  { return _tokens; } }

        private readonly List<IArithSyntax> _errors = [];

        public List<IArithSyntax> Errors => _errors;

        private int _index = 0, _openBraceCount = 0;
        private IArithToken Current { get
            {
                if (_index >= _tokens.Count)
                {
                    return null;
                }
                return _tokens[_index];
            } }

        public ArithmeticExpressionParser(List<IArithToken> tokens)
        {
            _tokens = [.. tokens.Where(t => t.Type != ArithmeticTokenType.SPACE && t.Type != ArithmeticTokenType.NEW_LINE)];
            ParseE();
        }

        private void ParseE()
        {
            ParseT();
            ParseA();
        }

        private void ParseA()
        {
            if ((Current.Type != ArithmeticTokenType.EOF) && (Current.Type == ArithmeticTokenType.SUM || Current.Type == ArithmeticTokenType.DIF))
            {
                _index++;
                ParseT();
                ParseA();
            }

            else if ((Current.Type != ArithmeticTokenType.EOF) && (Current.Type != ArithmeticTokenType.RIGHT_BRACE))
            {
                AddError("Ожидался оператор.");
                while (Current.Type != ArithmeticTokenType.EOF && (Current.Type != ArithmeticTokenType.SUM && Current.Type != ArithmeticTokenType.DIF))
                    _index++;
                if (Current.Type != ArithmeticTokenType.EOF)
                {
                    ParseA();
                }
            }
        }

        private void ParseT()
        {
            ParseF();
            ParseB();
        }

        private void ParseF()
        {
            if ((Current.Type != ArithmeticTokenType.EOF) && (Current.Type == ArithmeticTokenType.IDENTIFIER || Current.Type == ArithmeticTokenType.NUMBER))
            {
                _index++;
            }

            else if (Current.Type != ArithmeticTokenType.EOF && Current.Type == ArithmeticTokenType.LEFT_BRACE)
            {
                _index++;
                _openBraceCount++;

                ParseE();

                if (Current.Type != ArithmeticTokenType.EOF && Current.Type == ArithmeticTokenType.RIGHT_BRACE)
                {
                    _index++;
                    _openBraceCount--;
                }
                else
                {
                    AddError("Нет закрывающей скобки.");
                }
            }

            else
            {
                AddError("Ожидалось число или идентификатор.");
                while ((Current.Type != ArithmeticTokenType.EOF) && Current.Type != ArithmeticTokenType.IDENTIFIER && Current.Type != ArithmeticTokenType.NUMBER && Current.Type != ArithmeticTokenType.LEFT_BRACE)
                {
                    _index++;
                }
                if (Current.Type != ArithmeticTokenType.EOF)
                    ParseF();
            }
        }

        private void ParseB()
        {
            if ((Current.Type != ArithmeticTokenType.EOF) && (Current.Type == ArithmeticTokenType.MULTIPLICATION || Current.Type == ArithmeticTokenType.DIVISION || Current.Type == ArithmeticTokenType.REMAINDER))
            {
                _index++;
                ParseF();
                ParseB();
            }
            else if (Current.Type != ArithmeticTokenType.EOF && Current.Type != ArithmeticTokenType.SUM && Current.Type != ArithmeticTokenType.DIF && Current.Type != ArithmeticTokenType.RIGHT_BRACE)
            {
                AddError("Ожидался оператор.");
                while (Current.Type != ArithmeticTokenType.EOF && Current.Type != ArithmeticTokenType.IDENTIFIER && Current.Type != ArithmeticTokenType.NUMBER && Current.Type != ArithmeticTokenType.LEFT_BRACE)
                    _index++;
                if (Current.Type != ArithmeticTokenType.EOF)
                {
                    ParseF();
                    ParseB();
                }
            }
            else if (Current.Type == ArithmeticTokenType.RIGHT_BRACE)
            {
                if (_openBraceCount == 0)
                {
                    AddError("Ожидался оператор.");
                    while (Current.Type != ArithmeticTokenType.EOF && Current.Type != ArithmeticTokenType.IDENTIFIER && Current.Type != ArithmeticTokenType.NUMBER && Current.Type != ArithmeticTokenType.LEFT_BRACE)
                        _index++;
                    if (Current.Type != ArithmeticTokenType.EOF)
                    {
                        ParseF();
                        ParseB();
                    }
                }
            }
        }

        private void AddError(string message)
        {
            IArithSyntax error = new ArithmeticExpressionSyntaxError(Current.Word, Current.Line, Current.Column, message);
            Errors.Add(error);
        }
    }
}