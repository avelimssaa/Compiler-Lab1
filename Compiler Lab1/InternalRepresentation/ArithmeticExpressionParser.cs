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

        private int _index = 0;
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
                if (Current.Type == ArithmeticTokenType.IDENTIFIER || Current.Type == ArithmeticTokenType.NUMBER)
                {
                    ParseT();
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

                ParseE();

                if (Current.Type != ArithmeticTokenType.EOF && Current.Type == ArithmeticTokenType.RIGHT_BRACE)
                {
                    _index++;
                }
                else
                {
                    AddError("Нет закрывающей скобки.");
                }
            }

            else
            {
                AddError("Ожидалось число или идентификатор.");
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
                if (Current.Type == ArithmeticTokenType.IDENTIFIER || Current.Type == ArithmeticTokenType.NUMBER)
                {
                    ParseF();
                    ParseB();
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