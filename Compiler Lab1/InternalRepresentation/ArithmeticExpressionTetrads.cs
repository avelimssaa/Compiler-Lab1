namespace Compiler_Lab1.InternalRepresentation
{
    internal class ArithmeticExpressionTetrads(string operation, string operandLeft, string operandRight, string variable)
    {
        public string Operation { get { return operation; } }

        public string OperandLeft { get { return operandLeft; } }

        public string OperandRight { get { return operandRight; } }

        public string Variable { get { return variable; } }
    }
}
