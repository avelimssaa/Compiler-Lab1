namespace Compiler_Lab1.InternalRepresentation
{
    internal interface IPOLIZ
    {
        List<IArithToken> POLIZ {  get; }
        bool HasID { get; }
    }

    internal class ArithmeticExpressionPOLIZ : IPOLIZ
    {
        private readonly List<IArithToken> _poliz = [];
        public List<IArithToken> POLIZ { get { return _poliz; } }

        private readonly List<IArithToken> _inputString = [];

        private readonly Stack<IArithToken> _opStack = new();

        private bool _hasID = false;

        public bool HasID { get { return _hasID; } }

        public ArithmeticExpressionPOLIZ(List<IArithToken> inputString)
        {
            _inputString = inputString;
            Start();
        }

        private void Start()
        {
            foreach (var token in _inputString)
            {
                if (token.Type == ArithmeticTokenType.IDENTIFIER || token.Type == ArithmeticTokenType.NUMBER)
                {
                    _poliz.Add(token);
                    if (!_hasID && token.Type == ArithmeticTokenType.IDENTIFIER)
                        _hasID = true;
                }
                else if (token.Type == ArithmeticTokenType.LEFT_BRACE)
                {
                    _opStack.Push(token);
                }
                else if (token.Type == ArithmeticTokenType.RIGHT_BRACE)
                {
                    while (_opStack.Peek().Type != ArithmeticTokenType.LEFT_BRACE)
                    {
                        _poliz.Add(_opStack.Pop());
                    }
                    _opStack.Pop();
                }
                else if (token.Type == ArithmeticTokenType.DIF || token.Type == ArithmeticTokenType.SUM || token.Type == ArithmeticTokenType.MULTIPLICATION || token.Type == ArithmeticTokenType.DIVISION || token.Type == ArithmeticTokenType.REMAINDER)
                {
                    if (_opStack.Count == 0 || (GetPriority(token.Type) > GetPriority(_opStack.Peek().Type)))
                    {
                        _opStack.Push(token);
                    }
                    else
                    {
                        _poliz.Add(_opStack.Pop());
                        while (_opStack.Count != 0 && GetPriority(_opStack.Peek().Type) >= GetPriority(token.Type))
                        {
                            _poliz.Add(_opStack.Pop());
                        }
                        _opStack.Push(token);
                    }
                }
            }

            foreach (var op in _opStack) 
            {
                _poliz.Add(op);
            }
        }

        private short GetPriority(ArithmeticTokenType type)
        {
            short priority = 0;
            if (type == ArithmeticTokenType.LEFT_BRACE)
            {
                priority = 0;
            }
            else if (type == ArithmeticTokenType.RIGHT_BRACE)
            {
                priority = 1;
            }
            else if (type == ArithmeticTokenType.DIF || type == ArithmeticTokenType.SUM)
            {
                priority = 7;
            }
            else if (type == ArithmeticTokenType.MULTIPLICATION || type == ArithmeticTokenType.DIVISION || type == ArithmeticTokenType.REMAINDER)
            {
                priority = 8;
            }
            return priority;
        }
    }
}
