using Compiler_Lab1.LexicalAnalyzer;

namespace Compiler_Lab1.ASTBuild
{
    internal interface IASTNode
    {
        void Accept(IVisitor visitor);
    }
    internal class ForLoopNode : IASTNode
    {
        private readonly IASTNode _headerNode, _bodyNode;

        public IASTNode HeaderNode
        {
            get { return _headerNode; }
        }

        public IASTNode BodyNode
        {
            get { return _bodyNode; }
        }

        internal ForLoopNode(IASTNode headerNode, IASTNode bodyNode)
        {
            _bodyNode = bodyNode;
            _headerNode = headerNode;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    internal class HeaderNode : IASTNode
    {
        private readonly IToken _identifier, _startDigit, _endDigit;

        public IToken Identifier
        {
            get { return _identifier; }
        }

        public IToken StartDigit
        {
            get { return _startDigit; }
        }

        public IToken EndDigit
        {
            get { return _endDigit; }
        }

        public HeaderNode(IToken identifier, IToken startDigit, IToken endDigit)
        {
            _endDigit = endDigit;
            _identifier = identifier;
            _startDigit = startDigit;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    internal class BodyNode : IASTNode
    {
        private readonly IASTNode _printlnNode;

        public IASTNode PrintlnNode
        {
            get { return _printlnNode; }
        }

        public BodyNode(IASTNode printlnNode)
        {
            _printlnNode = printlnNode;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    internal class PrintlnNode : IASTNode
    {
        private readonly IToken _identifier;

        public IToken Identifier
        {
            get { return _identifier; }
        }

        public PrintlnNode(IToken identifier)
        {
            _identifier = identifier;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
