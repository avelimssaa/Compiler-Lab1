using Compiler_Lab1.LexicalAnalyzer;

namespace Compiler_Lab1.Parser
{
    public interface IParser
    {
        void ParseStart();
        IReadOnlyList<SyntaxError> Errors { get; }
        List<IToken> Tokens { get; }
    }

    public class CodeParser : IParser
    {
        private readonly List<IToken> _tokens;
        public List<IToken> Tokens { get { return _tokens; } }
        private int _position;

        private readonly List<SyntaxError> _errors = new();
        public IReadOnlyList<SyntaxError> Errors => _errors;
        private IToken Current =>
    _position < _tokens.Count ? _tokens[_position] : null;


        public CodeParser(List<IToken> tokens)
        {
            _tokens = tokens.Where(t =>
    t.GetTokenTypeEnum() != TokenType.DELIMITER_SPACE &&
    t.GetTokenTypeEnum() != TokenType.DELIMITER_TABULATION && 
    t.GetTokenTypeEnum() != TokenType.DELIMITER_NEWLINE)
    .ToList();
            _position = 0;
        }

        private void AddError(string message)
        {
            var token = Current ?? _tokens.LastOrDefault();

            var err = new SyntaxError
            {
                UnexpectedLexeme = token?.GetLexeme() ?? "EOF",
                Message = message,
                Line = token?.GetLine() ?? 0,
                Column = token?.GetStartColumn() ?? 0
            };

            _errors.Add(err);
        }

        private bool TryConsumeToken(TokenType expectedType, TokenType nextToken, string errorMessage)
        {
            if (Current != null && Current.GetTokenTypeEnum() == expectedType)
            {
                _position++;
                return true;
            }
            else
            {

                AddError(errorMessage);
                while (Current != null)
                {
                    if (Current.GetTokenTypeEnum() == nextToken)
                    {
                        return true;
                    }
                    _position++;
                }
                return false;
            }
        }

        public void ParseStart()
        {
            if (TryConsumeToken(TokenType.KEYWORD_FOR, TokenType.DELIMITER_LPAREN, "Ожидалось ключевое слово 'for'"))
            {
                ParseKeywordFor();
            }
            else
            {
                return;
            }
        }

        private void ParseKeywordFor()
        {
            if (TryConsumeToken(TokenType.DELIMITER_LPAREN, TokenType.IDENTIFIER, "Ожидался '(' после 'for'"))
            {
                ParseOpenBracketCycle();
            }
            else
            {
                return;
            }
        }

        private void ParseOpenBracketCycle()
        {
            if (TryConsumeToken(TokenType.IDENTIFIER, TokenType.OPERATOR_ARROW, "Ожидался идентификатор в заголовке цикла for"))
            {
                ParseIdCycle();
            }
            else
            {
                return;
            }
        }

        private void ParseIdCycle()
        {
            if (TryConsumeToken(TokenType.OPERATOR_ARROW, TokenType.DIGIT, "Ожидался оператор '<-'"))
            {
                ParseExpression();
            }
            else
            {
                return;
            }
        }

        private void ParseExpression()
        {
            if (TryConsumeToken(TokenType.DIGIT, TokenType.KEYWORD_TO, "Ожидалась цифра в выражении после '<-'"))
            {
                ParseBeginNumber();
            }
            else
            {
                return;
            }
        }

        private void ParseBeginNumber()
        {
            if (TryConsumeToken(TokenType.KEYWORD_TO, TokenType.DIGIT, "Ожидалось ключевое слово 'to'"))
            {
                ParseTo();
            }
            else
            {
                return;
            }
        }

        private void ParseTo()
        {
            if (TryConsumeToken(TokenType.DIGIT, TokenType.DELIMITER_RPAREN, "Ожидалась цифра после 'to'"))
            {
                ParseEndNumber();
            }
            else
            {
                return;
            }
        }

        private void ParseEndNumber()
        {
            if (TryConsumeToken(TokenType.DELIMITER_RPAREN, TokenType.DELIMITER_LBRACE, "Ожидалась закрывающая скобка ')' после конечного числа"))
            {
                ParseCloseBracketCycle();
            }
            else
            {
                return;
            }
        }

        private void ParseCloseBracketCycle()
        {
            if (TryConsumeToken(TokenType.DELIMITER_LBRACE, TokenType.KEYWORD_PRINTLN, "Ожидался '{' после заголовка цикла"))
            {
                ParseOpenCurly();
            }
            else
            {
                return;
            }
        }

        private void ParseOpenCurly()
        {
            if (TryConsumeToken(TokenType.KEYWORD_PRINTLN, TokenType.DELIMITER_LPAREN, "Ожидался оператор 'println'"))
            {
                ParsePrintln();
            }
            else
            {
                return;
            }
        }

        private void ParsePrintln()
        {
            if (TryConsumeToken(TokenType.DELIMITER_LPAREN, TokenType.IDENTIFIER, "Ожидался '(' после 'println'"))
            {
                ParseOpenBracePrintln();
            }
            else
            {
                return;
            }
        }

        private void ParseOpenBracePrintln()
        {
            if (TryConsumeToken(TokenType.IDENTIFIER, TokenType.DELIMITER_RPAREN, "Ожидался идентификатор в аргументе println"))
            {
                ParseIdCycleBody();
            }
            else
            {
                return;
            }
        }

        private void ParseIdCycleBody()
        {
            if (TryConsumeToken(TokenType.DELIMITER_RPAREN, TokenType.DELIMITER_RBRACE, "Ожидалась ')'"))
            {
                ParseCloseBracePrintln();
            }
            else
            {
                return;
            }
        }

        private void ParseCloseBracePrintln()
        {
            if (TryConsumeToken(TokenType.DELIMITER_RBRACE, TokenType.DELIMITER_SEMICOLON, "Ожидалася '}' после println(...)"))
            {
                ParseSemicolon();
            }
            else
            {
                return;
            }
        }

        private void ParseSemicolon()
        {
            if (TryConsumeToken(TokenType.DELIMITER_SEMICOLON, TokenType.DELIMITER_NEWLINE, "Ожидался ';' после '}'"))
            {
                if (Current != null)
                {
                    if (Current.GetTokenTypeEnum() == TokenType.DELIMITER_NEWLINE)
                    {
                        _position++;
                        if (Current != null && Current.GetTokenTypeEnum() != TokenType.DELIMITER_NEWLINE)
                        {
                            ParseStart();
                        }
                    }
                    else
                    {
                        AddError("Ожидался перевод строки.");
                    }
                }
            }

            else
            {
                return;
            }
        }
    }
}