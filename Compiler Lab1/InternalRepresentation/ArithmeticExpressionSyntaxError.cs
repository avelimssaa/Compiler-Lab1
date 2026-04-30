namespace Compiler_Lab1.InternalRepresentation
{
    internal interface IArithSyntax
    {
        string Token { get; }
        int Line { get; }
        int Column { get; }
        string Message { get; }
    }
    internal class ArithmeticExpressionSyntaxError(string token, int line, int column, string message) : IArithSyntax
    {
        public string Token { get { return token; } }

        public int Line { get { return line; } }

        public int Column { get { return column; } }

        public string Message { get { return message; } }
    }
}
