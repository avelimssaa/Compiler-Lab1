namespace Compiler_Lab1.InternalRepresentation
{
    internal interface ITetrads
    {
        List<ArithmeticExpressionTetrads> Tetrads { get; }
    }

    internal class ArithmeticExpressionTetradsGenerator : ITetrads
    {
        private readonly List<IArithToken> _tokens = [];
        private readonly Stack<IArithToken> _stackTokens = new();
        private int _varCount = 1;
        private readonly List<ArithmeticExpressionTetrads> _tetrads = [];
        public List<ArithmeticExpressionTetrads> Tetrads { get {  return _tetrads; }  }

        public ArithmeticExpressionTetradsGenerator(List<IArithToken> tokens) 
        {
            _tokens = tokens;
            Start();
        }

        private void Start()
        {
            foreach (IArithToken token in _tokens)
            {
                if (token.Type == ArithmeticTokenType.IDENTIFIER || token.Type == ArithmeticTokenType.NUMBER)
                {
                    _stackTokens.Push(token);
                }
                else
                {
                    string rightOperand = _stackTokens.Pop().Word, leftOpearnd = _stackTokens.Pop().Word,  temp = GetVarName();
                    ArithmeticExpressionTetrads tetrad = new ArithmeticExpressionTetrads(token.Word, leftOpearnd, rightOperand, temp);
                    _tetrads.Add(tetrad);
                    IArithToken synteticToken = new ArithmeticToken(0, 0, temp, ArithmeticTokenType.IDENTIFIER);
                    _stackTokens.Push(synteticToken);
                }
            }
        }

        private string GetVarName()
        {
            string name = "t" + _varCount.ToString();
            _varCount++;
            return name;
        }
    }
}
