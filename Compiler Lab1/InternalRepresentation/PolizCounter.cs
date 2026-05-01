namespace Compiler_Lab1.InternalRepresentation
{
    internal interface IPolizCounter
    {
        double Count();
    }
    internal class PolizCounter : IPolizCounter
    {
        private readonly List<IArithToken> _tokens = [];
        private readonly Stack<double> _polishResult = new();
        public PolizCounter(List<IArithToken> tokens)
        {
            _tokens = tokens;
        }

        public double Count() 
        {
            foreach (var token in _tokens)
            {
                if (token.Type == ArithmeticTokenType.NUMBER)
                {
                    _polishResult.Push(int.Parse(token.Word));
                }
                else
                {
                    double right = _polishResult.Pop(), left = _polishResult.Pop();

                    switch (token.Type)
                    {
                        case ArithmeticTokenType.SUM:
                            _polishResult.Push(left + right);
                            break;

                        case ArithmeticTokenType.DIF:
                            _polishResult.Push(left - right);
                            break;

                        case ArithmeticTokenType.MULTIPLICATION:
                            _polishResult.Push(left * right);
                            break;

                        case ArithmeticTokenType.DIVISION:
                            _polishResult.Push(left / right);
                            break;

                        case ArithmeticTokenType.REMAINDER:
                            _polishResult.Push(left % right);
                            break;
                    }
                }
            }

            return _polishResult.Pop();
        }
    }
}
