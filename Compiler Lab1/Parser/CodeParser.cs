using Compiler_Lab1.LexicalAnalyzer;

namespace Compiler_Lab1.Parser
{
    public interface IParser
    {
        void ParseStart();
        IReadOnlyList<SyntaxError> Errors { get; }
    }

    public class CodeParser : IParser
    {
        private readonly List<IToken> _tokens;
        private int _position;

        private readonly List<SyntaxError> _errors = new();
        public IReadOnlyList<SyntaxError> Errors => _errors;

        public CodeParser(List<IToken> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        private IToken Current =>
            _position < _tokens.Count ? _tokens[_position] : null;

        public void ParseStart()
        {
            SkipSpaces();
            Consume(TokenType.KEYWORD_FOR);
            ParseKeywordFor();
        }

        private void ParseKeywordFor()
        {
            SkipSpaces();
            Consume(TokenType.DELIMITER_LPAREN);
            ParseOpenBracketCycle();
        }

        private void ParseOpenBracketCycle()
        {
            SkipSpaces();
            Consume(TokenType.IDENTIFIER);
            ParseIdCycle();
        }

        private void ParseIdCycle()
        {
            while (true)
            {
                SkipSpaces();

                if (Current == null)
                {
                    Error("Ожидался идентификатор или <-, но вход закончился");
                    return;
                }

                var t = Current.GetTokenTypeEnum();

                if (t == TokenType.IDENTIFIER || t == TokenType.DIGIT)
                {
                    Consume(t);
                    continue;
                }

                if (t == TokenType.OPERATOR_ARROW)
                {
                    Consume(TokenType.OPERATOR_ARROW);
                    ParseExpression();
                    return;
                }

                Error($"Ожидался идентификатор или <-, найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                return;
            }
        }

        private void ParseExpression()
        {
            SkipSpaces();
            Consume(TokenType.DIGIT);
            ParseBeginNumber();
        }

        private void ParseBeginNumber()
        {
            while (true)
            {
                SkipSpaces();

                if (Current == null)
                {
                    Error("Ожидался digit или to, но вход закончился");
                    return;
                }

                var t = Current.GetTokenTypeEnum();

                if (t == TokenType.DIGIT)
                {
                    Consume(TokenType.DIGIT);
                    continue;
                }

                if (t == TokenType.KEYWORD_TO)
                {
                    Consume(TokenType.KEYWORD_TO);
                    ParseTo();
                    return;
                }

                Error($"Ожидался digit или to, найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                return;
            }
        }

        private void ParseTo()
        {
            SkipSpaces();
            Consume(TokenType.DIGIT);
            ParseEndNumber();
        }

        private void ParseEndNumber()
        {
            while (true)
            {
                SkipSpaces();

                if (Current == null)
                {
                    Error("Ожидался digit или ), но вход закончился");
                    return;
                }

                var t = Current.GetTokenTypeEnum();

                if (t == TokenType.DIGIT)
                {
                    Consume(TokenType.DIGIT);
                    continue;
                }

                if (t == TokenType.DELIMITER_RPAREN)
                {
                    Consume(TokenType.DELIMITER_RPAREN);
                    ParseOpenCurly();
                    return;
                }

                Error($"Ожидался digit или ), найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                return;
            }
        }

        private void ParseOpenCurly()
        {
            SkipSpaces();
            Consume(TokenType.DELIMITER_LBRACE);

            SkipSpaces();
            Consume(TokenType.DELIMITER_NEWLINE);

            ParseNewline();
        }

        private void ParseNewline()
        {
            while (true)
            {
                SkipSpaces();

                if (Current == null)
                {
                    Error("Ожидался println или }, но вход закончился");
                    return;
                }

                var t = Current.GetTokenTypeEnum();

                if (t == TokenType.KEYWORD_PRINTLN)
                {
                    ParseCycleBody();
                    return;
                }

                if (t == TokenType.DELIMITER_RBRACE)
                {
                    Consume(TokenType.DELIMITER_RBRACE);
                    ParseSemicolon();
                    return;
                }

                Error($"Ожидался println или } , найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                return;
            }
        }

        private void ParseCycleBody()
        {
            SkipSpaces();
            Consume(TokenType.KEYWORD_PRINTLN);
            ParsePrintln();
        }

        private void ParsePrintln()
        {
            SkipSpaces();
            Consume(TokenType.DELIMITER_LPAREN);

            SkipSpaces();
            Consume(TokenType.IDENTIFIER);

            SkipSpaces();
            Consume(TokenType.DELIMITER_RPAREN);

            SkipSpaces();
            Consume(TokenType.DELIMITER_NEWLINE);

            ParseNewline();
        }

        private void ParseSemicolon()
        {
            SkipSpaces();
            Consume(TokenType.DELIMITER_SEMICOLON);
        }

        private void SkipSpaces()
        {
            while (Current != null &&
                  (Current.GetTokenTypeEnum() == TokenType.DELIMITER_SPACE ||
                   Current.GetTokenTypeEnum() == TokenType.DELIMITER_TABULATION))
            {
                _position++;
            }
        }

        private void Consume(TokenType expected)
        {
            if (Current == null)
            {
                Error($"Ожидался {expected}, но вход закончился");
                return;
            }

            if (Current.GetTokenTypeEnum() != expected)
            {
                Error($"Ожидался {expected}, найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                return;
            }

            _position++;
        }

        private void Error(string message)
        {
            var err = new SyntaxError
            {
                UnexpectedLexeme = Current?.GetLexeme() ?? "<EOF>",
                Message = message,
                Line = Current?.GetLine() ?? 0,
                Column = Current?.GetStartColumn() ?? 0
            };

            _errors.Add(err);

            if (Current != null)
                _position++;
        }
    }
}
