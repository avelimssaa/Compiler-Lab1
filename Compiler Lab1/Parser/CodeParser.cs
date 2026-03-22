using Compiler_Lab1.LexicalAnalyzer;

namespace Compiler_Lab1.Parser
{
    public enum NonTerminal
    {
        START,
        KEYWORD_FOR,
        OPERATOR_OPEN_BRACKET_CYCLE,
        ID_CYCLE,
        OPERATOR_EXPRESSION,
        BEGIN_NUMBER,
        OPERATOR_TO,
        END_NUMBER,
        OPERATOR_OPEN_CURLY_BRACE,
        OPERATOR_NEWLINE,
        CYCLE_BODY,
        KEYWORD_PRINTLN,
        OPERATOR_OPEN_BRACE_PRINTLN,
        ID_CYCLE_BODY,
        OPERATOR_CLOSE_BRACE_PRINTLN,
        OPERATOR_CLOSE_CURLY_BRACE,
        OPERATOR_SEMICOLON
    }

    internal class SyntheticToken : IToken
    {
        private readonly TokenType _type;
        private readonly string _lexeme;
        private readonly int _line;
        private readonly int _startColumn;
        private readonly TokenDescription _description = new();

        public SyntheticToken(TokenType type, string lexeme, int line, int startColumn)
        {
            _type = type;
            _lexeme = lexeme;
            _line = line;
            _startColumn = startColumn;
        }

        public int GetConditionCode() => (int)_type;
        public string GetTokenType() => _description.GetTokenDescription(_type);
        public string GetLexeme() => _lexeme;
        public string GetLocation() => $"строка {_line}, {_startColumn} - {_startColumn}";
        public bool IsError() => false;
        public string GetMessageDescription() => "";
        public int GetStartColumn() => _startColumn;
        public int GetLine() => _line;
        public int GetEndColumn() => _startColumn;
        public TokenType GetTokenTypeEnum() => _type;
    }

    public interface IParser
    {
        void ParseStart();
        IReadOnlyList<SyntaxError> Errors { get; }
        IReadOnlyList<IToken> ValidTokens { get; }
    }

    public class CodeParser : IParser
    {
        private readonly List<IToken> _tokens;
        private int _position;

        private readonly List<SyntaxError> _errors = new();
        public IReadOnlyList<SyntaxError> Errors => _errors;

        private readonly List<IToken> _validTokens = new();
        public IReadOnlyList<IToken> ValidTokens => _validTokens;

        private readonly Stack<NonTerminal> _ntStack = new();

        private static readonly Dictionary<NonTerminal, List<TokenType>> ExpectedTokens = new()
        {
            [NonTerminal.START] = new() { TokenType.KEYWORD_FOR },
            [NonTerminal.KEYWORD_FOR] = new() { TokenType.DELIMITER_LPAREN },
            [NonTerminal.OPERATOR_OPEN_BRACKET_CYCLE] = new() { TokenType.IDENTIFIER },
            [NonTerminal.ID_CYCLE] = new() { TokenType.OPERATOR_ARROW },
            [NonTerminal.OPERATOR_EXPRESSION] = new() { TokenType.DIGIT },
            [NonTerminal.BEGIN_NUMBER] = new() { TokenType.KEYWORD_TO },
            [NonTerminal.OPERATOR_TO] = new() { TokenType.DIGIT },
            [NonTerminal.END_NUMBER] = new() { TokenType.DELIMITER_RPAREN },
            [NonTerminal.OPERATOR_OPEN_CURLY_BRACE] = new() { TokenType.DELIMITER_LBRACE },
            [NonTerminal.OPERATOR_NEWLINE] = new() { TokenType.DELIMITER_NEWLINE },
            [NonTerminal.CYCLE_BODY] = new() { TokenType.KEYWORD_PRINTLN },
            [NonTerminal.KEYWORD_PRINTLN] = new() { TokenType.DELIMITER_LPAREN },
            [NonTerminal.OPERATOR_OPEN_BRACE_PRINTLN] = new() { TokenType.IDENTIFIER },
            [NonTerminal.ID_CYCLE_BODY] = new() { TokenType.DELIMITER_RPAREN },
            [NonTerminal.OPERATOR_CLOSE_BRACE_PRINTLN] = new() { TokenType.DELIMITER_NEWLINE },
            [NonTerminal.OPERATOR_CLOSE_CURLY_BRACE] = new() { TokenType.DELIMITER_SEMICOLON },
            [NonTerminal.OPERATOR_SEMICOLON] = new() { TokenType.DELIMITER_SEMICOLON }
        };

        private static readonly Dictionary<NonTerminal, HashSet<TokenType>> L = new()
        {
            [NonTerminal.START] = new() { TokenType.KEYWORD_FOR },

            [NonTerminal.KEYWORD_FOR] = new() { TokenType.DELIMITER_LPAREN },

            [NonTerminal.OPERATOR_OPEN_BRACKET_CYCLE] = new() { TokenType.IDENTIFIER },

            [NonTerminal.ID_CYCLE] = new()
            {
                TokenType.IDENTIFIER,
                TokenType.DIGIT,
                TokenType.OPERATOR_ARROW
            },

            [NonTerminal.OPERATOR_EXPRESSION] = new() { TokenType.DIGIT },

            [NonTerminal.BEGIN_NUMBER] = new()
            {
                TokenType.DIGIT,
                TokenType.KEYWORD_TO
            },

            [NonTerminal.OPERATOR_TO] = new() { TokenType.DIGIT },

            [NonTerminal.END_NUMBER] = new()
            {
                TokenType.DIGIT,
                TokenType.DELIMITER_RPAREN
            },

            [NonTerminal.OPERATOR_OPEN_CURLY_BRACE] = new() { TokenType.DELIMITER_LBRACE },

            [NonTerminal.OPERATOR_NEWLINE] = new()
            {
                TokenType.KEYWORD_PRINTLN,
                TokenType.DELIMITER_RBRACE
            },

            [NonTerminal.CYCLE_BODY] = new() { TokenType.KEYWORD_PRINTLN },

            [NonTerminal.KEYWORD_PRINTLN] = new() { TokenType.DELIMITER_LPAREN },

            [NonTerminal.OPERATOR_OPEN_BRACE_PRINTLN] = new() { TokenType.IDENTIFIER },

            [NonTerminal.ID_CYCLE_BODY] = new()
            {
                TokenType.IDENTIFIER,
                TokenType.DIGIT,
                TokenType.DELIMITER_RPAREN
            },

            [NonTerminal.OPERATOR_CLOSE_BRACE_PRINTLN] = new() { TokenType.DELIMITER_NEWLINE },

            [NonTerminal.OPERATOR_CLOSE_CURLY_BRACE] = new() { TokenType.DELIMITER_SEMICOLON },

            [NonTerminal.OPERATOR_SEMICOLON] = new() { TokenType.DELIMITER_SEMICOLON }
        };

        public CodeParser(List<IToken> tokens)
        {
            _tokens = tokens;
            _position = 0;
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

        private void AddValidToken(IToken token)
        {
            if (token != null && !token.IsError())
            {
                _validTokens.Add(token);
            }
        }

        private void InsertSyntheticToken(TokenType type, string lexeme)
        {
            var syntheticToken = new SyntheticToken(
                type,
                lexeme,
                Current?.GetLine() ?? 0,
                Current?.GetStartColumn() ?? 0);

            _tokens.Insert(_position, syntheticToken);
            _validTokens.Add(syntheticToken);
        }

        private bool TryConsumeToken(TokenType expectedType, string errorMessage, Action onSuccess = null)
        {
            SkipSpaces();

            if (Current != null && Current.GetTokenTypeEnum() == expectedType)
            {
                AddValidToken(Current);
                _position++;
                onSuccess?.Invoke();
                return true;
            }
            else
            {
                AddError(errorMessage);
                return false;
            }
        }

        private void IronsRecover()
        {
            while (Current != null)
            {
                var currentType = Current.GetTokenTypeEnum();

                var broken = FindBrokenNonTerminalFromTop(currentType);

                if (broken != null)
                {
                    if (ExpectedTokens.TryGetValue(broken.Value, out var expectedSeq) && expectedSeq.Count > 0)
                    {
                        foreach (var expectedToken in expectedSeq)
                        {
                            InsertSyntheticToken(expectedToken, $"<{expectedToken}>");
                        }
                    }

                    return;
                }

                _position++;
            }
        }

        private NonTerminal? FindBrokenNonTerminalFromTop(TokenType j)
        {
            foreach (var nt in _ntStack.Reverse())
            {
                if (L.TryGetValue(nt, out var set) && set.Contains(j))
                    return nt;
            }

            return null;
        }

        public void ParseStart()
        {
            _ntStack.Push(NonTerminal.START);

            if (TryConsumeToken(TokenType.KEYWORD_FOR, "Ожидалось ключевое слово 'for'"))
            {
                ParseKeywordFor();
            }
            else
            {
                IronsRecover();
            }

            _ntStack.Pop();

            CompleteMissingTokens();
        }

        private void CompleteMissingTokens()
        {
            var lastToken = _validTokens.LastOrDefault();
            if (lastToken != null && lastToken.GetTokenTypeEnum() != TokenType.DELIMITER_SEMICOLON)
            {
                InsertSyntheticToken(TokenType.DELIMITER_SEMICOLON, ";");
            }
        }

        private void ParseKeywordFor()
        {
            _ntStack.Push(NonTerminal.KEYWORD_FOR);

            TryConsumeToken(TokenType.DELIMITER_LPAREN, "Ожидался '(' после 'for'");
            ParseOpenBracketCycle();

            _ntStack.Pop();
        }

        private void ParseOpenBracketCycle()
        {
            _ntStack.Push(NonTerminal.OPERATOR_OPEN_BRACKET_CYCLE);

            TryConsumeToken(TokenType.IDENTIFIER, "Ожидался идентификатор в заголовке цикла for");
            ParseIdCycle();

            _ntStack.Pop();
        }

        private void ParseIdCycle()
        {
            _ntStack.Push(NonTerminal.ID_CYCLE);

            if (TryConsumeToken(TokenType.OPERATOR_ARROW, "Ожидался оператор '<-'"))
            {
                ParseExpression();
            }
            else
            {
                IronsRecover();
            }

            _ntStack.Pop();
        }

        private void ParseExpression()
        {
            _ntStack.Push(NonTerminal.OPERATOR_EXPRESSION);

            TryConsumeToken(TokenType.DIGIT, "Ожидалась цифра в выражении после '<-'");
            ParseBeginNumber();

            _ntStack.Pop();
        }

        private void ParseBeginNumber()
        {
            _ntStack.Push(NonTerminal.BEGIN_NUMBER);

            if (TryConsumeToken(TokenType.KEYWORD_TO, "Ожидалось ключевое слово 'to'"))
            {
                ParseTo();
            }
            else
            {
                IronsRecover();
            }

            _ntStack.Pop();
        }

        private void ParseTo()
        {
            _ntStack.Push(NonTerminal.OPERATOR_TO);

            TryConsumeToken(TokenType.DIGIT, "Ожидалась цифра после 'to'");
            ParseEndNumber();

            _ntStack.Pop();
        }

        private void ParseEndNumber()
        {
            _ntStack.Push(NonTerminal.END_NUMBER);

            if (TryConsumeToken(TokenType.DELIMITER_RPAREN, "Ожидалась закрывающая скобка ')' после конечного числа"))
            {
                ParseOpenCurly();
            }
            else
            {
                IronsRecover();
            }

            _ntStack.Pop();
        }

        private void ParseOpenCurly()
        {
            _ntStack.Push(NonTerminal.OPERATOR_OPEN_CURLY_BRACE);

            TryConsumeToken(TokenType.DELIMITER_LBRACE, "Ожидался '{' после заголовка цикла");

            SkipSpaces();
            TryConsumeToken(TokenType.DELIMITER_NEWLINE, "Ожидался перевод строки после '{'");

            ParseNewline();

            _ntStack.Pop();
        }

        private void ParseNewline()
        {
            _ntStack.Push(NonTerminal.OPERATOR_NEWLINE);

            SkipSpaces();
            var t = Current?.GetTokenTypeEnum();

            if (t == TokenType.KEYWORD_PRINTLN)
            {
                TryConsumeToken(TokenType.KEYWORD_PRINTLN, "");
                ParsePrintln();
            }
            else if (t == TokenType.DELIMITER_RBRACE)
            {
                TryConsumeToken(TokenType.DELIMITER_RBRACE, "");
                ParseSemicolon();
            }
            else
            {
                AddError($"Ожидался 'println' или '}}', найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                IronsRecover();
            }

            _ntStack.Pop();
        }

        private void ParsePrintln()
        {
            _ntStack.Push(NonTerminal.KEYWORD_PRINTLN);

            TryConsumeToken(TokenType.DELIMITER_LPAREN, "Ожидался '(' после 'println'");
            ParseOpenBracePrintln();

            _ntStack.Pop();
        }

        private void ParseOpenBracePrintln()
        {
            _ntStack.Push(NonTerminal.OPERATOR_OPEN_BRACE_PRINTLN);

            TryConsumeToken(TokenType.IDENTIFIER, "Ожидался идентификатор в аргументе println");
            ParseIdCycleBody();

            _ntStack.Pop();
        }

        private void ParseIdCycleBody()
        {
            _ntStack.Push(NonTerminal.ID_CYCLE_BODY);

            if (TryConsumeToken(TokenType.DELIMITER_RPAREN, "Ожидалась ')'"))
            {
                ParseCloseBracePrintln();
            }
            else
            {
                AddError($"Ожидалась ')', найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                IronsRecover();
            }

            _ntStack.Pop();
        }

        private void ParseCloseBracePrintln()
        {
            _ntStack.Push(NonTerminal.OPERATOR_CLOSE_BRACE_PRINTLN);

            TryConsumeToken(TokenType.DELIMITER_NEWLINE, "Ожидался перевод строки после println(...)");
            ParseNewline();

            _ntStack.Pop();
        }

        private void ParseSemicolon()
        {
            _ntStack.Push(NonTerminal.OPERATOR_CLOSE_CURLY_BRACE);

            TryConsumeToken(TokenType.DELIMITER_SEMICOLON, "Ожидался ';' после '}'");

            _ntStack.Pop();
        }

        private IToken Current =>
            _position < _tokens.Count ? _tokens[_position] : null;
    }
}