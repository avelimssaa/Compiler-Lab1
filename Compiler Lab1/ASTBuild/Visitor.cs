namespace Compiler_Lab1.ASTBuild
{
    internal interface IVisitor
    {
        void Visit(ForLoopNode node);

        void Visit(HeaderNode node);

        void Visit(BodyNode node);

        void Visit(PrintlnNode node);
    }

    internal class PrintAstVisitor : IVisitor
    {
        private string _indent = "";
        private string _ASTstr = "";

        public string ASTstr { get { return _ASTstr; } }

        private void AppendLine(string text)
        {
            _ASTstr += _indent + text + "\n";
        }

        public void Visit(ForLoopNode node)
        {
            AppendLine("├── ForLoop");

            string oldIndent = _indent;
            _indent += "│   ";

            node.HeaderNode.Accept(this);
            node.BodyNode.Accept(this);

            _indent = oldIndent;
        }

        public void Visit(HeaderNode node)
        {
            AppendLine("├── HeaderNode");

            string subIndent = _indent + "│   ";
            string tempIndent = _indent;

            _indent = subIndent;
            AppendLine($"├── Identifier : {node.Identifier.GetLexeme()}");
            AppendLine($"├── Start digit: {node.StartDigit.GetLexeme()}");
            AppendLine($"└── End digit: {node.EndDigit.GetLexeme()}");
            _indent = tempIndent;
        }

        public void Visit(BodyNode node)
        {
            AppendLine("└── BodyNode");

            string oldIndent = _indent;
            _indent += "    ";

            node.PrintlnNode.Accept(this);

            _indent = oldIndent;
        }

        public void Visit(PrintlnNode node)
        {
            AppendLine("├── PrintlnNode");

            string subIndent = _indent + "│   ";
            string tempIndent = _indent;

            _indent = subIndent;
            AppendLine($"└── Identifier: {node.Identifier.GetLexeme()}");
            _indent = tempIndent;
        }
    }

    internal class SemanticCheckerVisitor : IVisitor
    {
        private SymbolTable _symbolTable = new SymbolTable();
        private List<SemanticError> _semanticErrors = new List<SemanticError>();
        public List<SemanticError> SemanticError { get { return _semanticErrors; } }

        public void Visit(ForLoopNode node)
        {
            node.HeaderNode.Accept(this);
            node.BodyNode.Accept(this);
        }

        public void Visit(HeaderNode node)
        {
            _symbolTable.Declare(node.Identifier.GetLexeme(), "int", 0);
            if (int.Parse(node.StartDigit.GetLexeme()) < 0)
            {
                SemanticError error = new SemanticError();
                error.Message = $"Ошибка: число начала цикла меньше нуля!";
                error.UnexpectedLexeme = node.StartDigit.GetLexeme();
                error.Line = node.StartDigit.GetLine();
                error.Column = node.StartDigit.GetStartColumn();
                _semanticErrors.Add(error);
            }
            if (int.Parse(node.EndDigit.GetLexeme()) < int.Parse(node.StartDigit.GetLexeme()))
            {
                SemanticError error = new SemanticError();
                error.Message = $"Ошибка: число конца цикла меньше числа начала цикла!";
                error.UnexpectedLexeme = node.EndDigit.GetLexeme();
                error.Line = node.EndDigit.GetLine();
                error.Column = node.EndDigit.GetStartColumn();
                _semanticErrors.Add(error);
            }
        }

        public void Visit(BodyNode node)
        {
            node.PrintlnNode.Accept(this);
        }

        public void Visit(PrintlnNode node)
        {
            if (_symbolTable.Lookup(node.Identifier.GetLexeme()) == null)
            {
                SemanticError error = new SemanticError();
                error.Message = $"Ошибка: Переменная '{node.Identifier.GetLexeme()}' не объявлена!";
                error.UnexpectedLexeme = node.Identifier.GetLexeme();
                error.Line = node.Identifier.GetLine();
                error.Column = node.Identifier.GetStartColumn();
                _semanticErrors.Add(error);
            }
        }
    }
}
