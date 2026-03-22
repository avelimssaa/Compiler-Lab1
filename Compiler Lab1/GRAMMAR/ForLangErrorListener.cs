using Antlr4.Runtime;

namespace Compiler_Lab1.GRAMMAR
{
    public class ForLangErrorListener : BaseErrorListener
    {
        public class ErrorInfo
        {
            public string Fragment { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
        }

        public List<ErrorInfo> Errors { get; } = new List<ErrorInfo>();

        public override void SyntaxError(
            TextWriter output,
            IRecognizer recognizer,
            IToken offendingSymbol,
            int line,
            int charPositionInLine,
            string msg,
            RecognitionException e)
        {
            string fragment = offendingSymbol?.Text ?? "<EOF>";
            string location = $"Строка {line}, позиция {charPositionInLine}";
            string description = msg;

            Errors.Add(new ErrorInfo
            {
                Fragment = fragment,
                Location = location,
                Description = description
            });
        }
    }
}
