using Compiler_Lab1.LexicalAnalyzer;

namespace Compiler_Lab1.ASTBuild
{
    internal interface IASTBuilder
    {
        string FormAST(List<LexicalAnalyzer.IToken> tokens);
        List<SemanticError> SemanticError { get; }
    }

    internal class ASTBuilder : IASTBuilder
    {
        private List<SemanticError> _semanticErrors = new List<SemanticError>();
        public List<SemanticError> SemanticError { get { return _semanticErrors; } }
        public string FormAST(List<IToken> tokens)
        {
            string AST = "";
            List<List<IToken>> splitTokens = new List<List<IToken>>();
            splitTokens = TokensSplitter(tokens);

            foreach (List<IToken> token in splitTokens)
            {
                var checker = new SemanticCheckerVisitor();
                IASTNode forLoop = ParseForLoop(token);
                forLoop.Accept(checker);

                if (checker.SemanticError.Count == 0)
                {
                    var printer = new PrintAstVisitor();
                    forLoop.Accept(printer);
                    AST += printer.ASTstr;
                }
                else
                {
                    _semanticErrors.AddRange(checker.SemanticError);
                }
            }

            return AST;
        }

        private List<List<IToken>> TokensSplitter(List<IToken> tokens)
        {
            List<List<IToken>> splitTokens = new List<List<IToken>>();
            List<IToken> currentBlock = new List<IToken>();

            foreach (var token in tokens)
            {
                currentBlock.Add(token);

                if (token.GetTokenTypeEnum() == TokenType.DELIMITER_SEMICOLON)
                {
                    splitTokens.Add(currentBlock);
                    currentBlock = new List<IToken>();
                }
            }

            return splitTokens;
        }

        private IASTNode ParseForLoop(List<IToken> tokens)
        {
            IToken identifier = tokens[2];
            IToken start = tokens[4];
            IToken end = tokens[6];

            HeaderNode header = new HeaderNode(identifier, start, end);

            IToken varToPrint = tokens[11];
            PrintlnNode println = new PrintlnNode(varToPrint);

            BodyNode body = new BodyNode(println);

            return new ForLoopNode(header, body);
        }
    }

}
