namespace Compiler_Lab1.InternalRepresentation
{
    internal enum ArithmeticTokenType
    {
        IDENTIFIER = 1,
        NUMBER = 2,
        SUM = 3,
        DIF = 4,
        MULTIPLICATION = 5,
        DIVISION = 6,
        REMAINDER = 7,
        UNKNOWN = 8,
        SPACE = 9,
        NEW_LINE = 10
    }
    internal interface IArithToken
    {
        int Line { get; }
        int Column { get; }
        string Word { get; }
        ArithmeticTokenType Type { get; }
    }
    internal class ArithmeticToken : IArithToken
    {
        private readonly int _line = 0, _column = 0;
        private readonly string _word = "";
        private readonly ArithmeticTokenType _type;

        public int Line { get { return _line; } }
        public int Column { get { return _column; } }
        public string Word { get { return _word; } }
        public ArithmeticTokenType Type { get { return _type; }  }

        public ArithmeticToken(int line, int column, string word, ArithmeticTokenType type)
        {
            _line = line;
            _column = column;
            _word = word;
            _type = type;
        }
    }
}
