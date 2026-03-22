namespace Compiler_Lab1.Parser
{
    public class SyntaxError
    {
        public string UnexpectedLexeme { get; set; }
        public string Message { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public string Location => $"строка {Line}, столбец {Column}";
    }

}
