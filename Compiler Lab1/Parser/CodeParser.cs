using Compiler_Lab1.LexicalAnalyzer;

namespace Compiler_Lab1.Parser
{
    public interface IParser
    {
        void ParseStart();
    }

    public class CodeParser : IParser
    {
        private readonly List<IToken> _tokens;
        private int _position;

        private readonly List<SyntaxError> _errors = new();
        public IReadOnlyList<SyntaxError> Errors => _errors;
        private IToken Current =>
    _position < _tokens.Count ? _tokens[_position] : null;


        public CodeParser(List<IToken> tokens)
        {
            _tokens = tokens.Where(t =>
    t.GetTokenTypeEnum() != TokenType.DELIMITER_SPACE &&
    t.GetTokenTypeEnum() != TokenType.DELIMITER_TABULATION)
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

        private bool TryConsumeToken(TokenType expectedType, TokenType nextToken, string errorMessage, string errorMessage2 = null)
        {
            if (Current != null && Current.GetTokenTypeEnum() == expectedType)
            {
                _position++;
                return true;
            }
            else
            {

                AddError(errorMessage);
                bool flag = true;
                while (Current != null)
                {
                    if (flag == true && errorMessage2 != null)
                    {
                        AddError(errorMessage2);
                        flag = false;
                    }
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
            if (TryConsumeToken(TokenType.KEYWORD_FOR, TokenType.DELIMITER_LPAREN, "Ожидалось ключевое слово 'for'", "Ожидался '(' после 'for'"))
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
            if (TryConsumeToken(TokenType.DELIMITER_LPAREN, TokenType.IDENTIFIER, "Ожидался '(' после 'for'", "Ожидался идентификатор в заголовке цикла for"))
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
            if (TryConsumeToken(TokenType.IDENTIFIER, TokenType.OPERATOR_ARROW, "Ожидался идентификатор в заголовке цикла for", "Ожидался оператор '<-'"))
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
            if (TryConsumeToken(TokenType.OPERATOR_ARROW, TokenType.DIGIT, "Ожидался оператор '<-'", "Ожидалась цифра в выражении после '<-'"))
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
            if (TryConsumeToken(TokenType.DIGIT, TokenType.KEYWORD_TO, "Ожидалась цифра в выражении после '<-'", "Ожидалось ключевое слово 'to'"))
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
            if (TryConsumeToken(TokenType.KEYWORD_TO, TokenType.DIGIT, "Ожидалось ключевое слово 'to'", "Ожидалась цифра после 'to'"))
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
            if (TryConsumeToken(TokenType.DIGIT, TokenType.DELIMITER_RPAREN, "Ожидалась цифра после 'to'", "Ожидалась закрывающая скобка ')' после конечного числа"))
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
            if (TryConsumeToken(TokenType.DELIMITER_RPAREN, TokenType.DELIMITER_LBRACE, "Ожидалась закрывающая скобка ')' после конечного числа", "Ожидался '{' после заголовка цикла"))
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
            if (TryConsumeToken(TokenType.DELIMITER_LBRACE, TokenType.DELIMITER_NEWLINE, "Ожидался '{' после заголовка цикла", "Ожидался перевод строки после '{'"))
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
            if (TryConsumeToken(TokenType.DELIMITER_NEWLINE, TokenType.DELIMITER_RBRACE, "Ожидался перевод строки после '{'"))
            {
                ParseNewline();
            }
            else
            {
                return;
            }
        }

        private void ParseNewline()
        {
            if (Current.GetTokenTypeEnum() == TokenType.KEYWORD_PRINTLN || Current.GetTokenTypeEnum() == TokenType.DELIMITER_RBRACE)
            {
                if (Current.GetTokenTypeEnum() == TokenType.KEYWORD_PRINTLN)
                {
                    _position++;
                    ParsePrintln();
                }
                else
                {
                    _position++;
                    ParseSemicolon();
                }
            }

            else
            {
                AddError("Ожидался оператор println() или }}");
                _position++;

                while (Current != null && Current.GetTokenTypeEnum() != TokenType.KEYWORD_PRINTLN && Current.GetTokenTypeEnum() != TokenType.DELIMITER_RBRACE)
                {
                    _position++;
                }
                if (Current == null)
                {
                    return;
                }
                else if (Current.GetTokenTypeEnum() == TokenType.KEYWORD_PRINTLN)
                {
                    _position++;
                    ParsePrintln();
                }
                else
                {
                    _position++;
                    ParseSemicolon();
                }
            }
        }

        private void ParsePrintln()
        {
            if (TryConsumeToken(TokenType.DELIMITER_LPAREN, TokenType.IDENTIFIER, "Ожидался '(' после 'println'", "Ожидался идентификатор в аргументе println"))
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
            if (TryConsumeToken(TokenType.IDENTIFIER, TokenType.DELIMITER_RPAREN, "Ожидался идентификатор в аргументе println", "Ожидалась ')'"))
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
            if (TryConsumeToken(TokenType.DELIMITER_RPAREN, TokenType.DELIMITER_NEWLINE, "Ожидалась ')'", "Ожидался перевод строки после println(...)"))
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
            if (TryConsumeToken(TokenType.DELIMITER_NEWLINE, TokenType.DELIMITER_RBRACE, "Ожидался перевод строки после println(...)"))
            {
                ParseNewline();
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