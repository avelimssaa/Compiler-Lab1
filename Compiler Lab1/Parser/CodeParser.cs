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

            _position++;
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


            TryConsumeToken(TokenType.KEYWORD_FOR, "Ожидалось ключевое слово 'for'");
            ParseKeywordFor();

            _ntStack.Pop();

            CompleteMissingTokens();
        }

        private void CompleteMissingTokens()
        {
            if (_validTokens.Count == 0)
                return;

            var lastToken = _validTokens.LastOrDefault();
            if (lastToken != null && lastToken.GetTokenTypeEnum() != TokenType.DELIMITER_SEMICOLON)
            {
                if (lastToken.GetTokenTypeEnum() != TokenType.DELIMITER_RBRACE)
                {
                    InsertSyntheticToken(TokenType.DELIMITER_SEMICOLON, ";");
                }
            }

            CheckForCycleStructure();

            CheckCycleBodyStructure();

            CheckPrintlnStructure();

            
        }

        private void CheckForCycleStructure()
        {
            int forIndex = FindTokenIndex(TokenType.KEYWORD_FOR);
            if (forIndex == -1) return;

            if (!HasTokenAfterPosition(TokenType.DELIMITER_LPAREN, forIndex))
            {
                InsertSyntheticTokenAtPosition(forIndex + 1, TokenType.DELIMITER_LPAREN, "(");
            }

            int lparenIndex = FindTokenIndexAfter(TokenType.DELIMITER_LPAREN, forIndex);
            if (lparenIndex != -1 && !HasTokenAfterPosition(TokenType.IDENTIFIER, lparenIndex))
            {
                InsertSyntheticTokenAtPosition(lparenIndex + 1, TokenType.IDENTIFIER, "<identifier>");
            }

            int identIndex = FindTokenIndexAfter(TokenType.IDENTIFIER, forIndex);
            if (identIndex != -1 && !HasTokenAfterPosition(TokenType.OPERATOR_ARROW, identIndex))
            {
                InsertSyntheticTokenAtPosition(identIndex + 1, TokenType.OPERATOR_ARROW, "<-");
            }

            int arrowIndex = FindTokenIndexAfter(TokenType.OPERATOR_ARROW, forIndex);
            if (arrowIndex != -1 && !HasTokenAfterPosition(TokenType.DIGIT, arrowIndex))
            {
                InsertSyntheticTokenAtPosition(arrowIndex + 1, TokenType.DIGIT, "<number>");
            }

            int startNumberIndex = FindTokenIndexAfter(TokenType.DIGIT, forIndex);
            if (startNumberIndex != -1 && !HasTokenAfterPosition(TokenType.KEYWORD_TO, startNumberIndex))
            {
                InsertSyntheticTokenAtPosition(startNumberIndex + 1, TokenType.KEYWORD_TO, "to");
            }

            int toIndex = FindTokenIndexAfter(TokenType.KEYWORD_TO, forIndex);
            if (toIndex != -1 && !HasTokenAfterPosition(TokenType.DIGIT, toIndex))
            {
                InsertSyntheticTokenAtPosition(toIndex + 1, TokenType.DIGIT, "<number>");
            }

            int endNumberIndex = FindTokenIndexAfter(TokenType.DIGIT, toIndex);
            if (endNumberIndex != -1 && !HasTokenAfterPosition(TokenType.DELIMITER_RPAREN, endNumberIndex))
            {
                InsertSyntheticTokenAtPosition(endNumberIndex + 1, TokenType.DELIMITER_RPAREN, ")");
            }

            int rparenIndex = FindTokenIndexAfter(TokenType.DELIMITER_RPAREN, forIndex);
            if (rparenIndex != -1 && !HasTokenAfterPosition(TokenType.DELIMITER_LBRACE, rparenIndex))
            {
                InsertSyntheticTokenAtPosition(rparenIndex + 1, TokenType.DELIMITER_LBRACE, "{");
            }

            int lbraceIndex = FindTokenIndexAfter(TokenType.DELIMITER_LBRACE, forIndex);
            if (lbraceIndex != -1 && !HasTokenAfterPosition(TokenType.DELIMITER_NEWLINE, lbraceIndex))
            {
                InsertSyntheticTokenAtPosition(lbraceIndex + 1, TokenType.DELIMITER_NEWLINE, "\n");
            }
        }

        private void CheckCycleBodyStructure()
        {
            int lbraceIndex = FindTokenIndex(TokenType.DELIMITER_LBRACE);
            if (lbraceIndex == -1) return;

            int rbraceIndex = FindTokenIndexAfter(TokenType.DELIMITER_RBRACE, lbraceIndex);

            if (rbraceIndex == -1)
            {
                int printlnIndex = FindTokenIndexAfter(TokenType.KEYWORD_PRINTLN, lbraceIndex);
                if (printlnIndex == -1)
                {
                    InsertSyntheticTokenAtPosition(lbraceIndex + 1, TokenType.KEYWORD_PRINTLN, "println");
                    InsertSyntheticTokenAtPosition(lbraceIndex + 2, TokenType.DELIMITER_LPAREN, "(");
                    InsertSyntheticTokenAtPosition(lbraceIndex + 3, TokenType.IDENTIFIER, "<identifier>");
                    InsertSyntheticTokenAtPosition(lbraceIndex + 4, TokenType.DELIMITER_RPAREN, ")");
                    InsertSyntheticTokenAtPosition(lbraceIndex + 5, TokenType.DELIMITER_NEWLINE, "\n");
                }

                int lastPos = _validTokens.Count;
                InsertSyntheticTokenAtPosition(lastPos, TokenType.DELIMITER_RBRACE, "}");

                InsertSyntheticTokenAtPosition(lastPos + 1, TokenType.DELIMITER_SEMICOLON, ";");
            }
        }

        private void CheckPrintlnStructure()
        {
            for (int i = 0; i < _validTokens.Count; i++)
            {
                if (_validTokens[i].GetTokenTypeEnum() == TokenType.KEYWORD_PRINTLN)
                {
                    if (i + 1 >= _validTokens.Count ||
                        _validTokens[i + 1].GetTokenTypeEnum() != TokenType.DELIMITER_LPAREN)
                    {
                        InsertSyntheticTokenAtPosition(i + 1, TokenType.DELIMITER_LPAREN, "(");
                    }

                    int lparenPos = i + 1;
                    if (lparenPos < _validTokens.Count &&
                        _validTokens[lparenPos].GetTokenTypeEnum() == TokenType.DELIMITER_LPAREN)
                    {
                        if (lparenPos + 1 >= _validTokens.Count ||
                            _validTokens[lparenPos + 1].GetTokenTypeEnum() != TokenType.IDENTIFIER)
                        {
                            InsertSyntheticTokenAtPosition(lparenPos + 1, TokenType.IDENTIFIER, "<identifier>");
                        }

                        int identPos = lparenPos + 1;
                        if (identPos < _validTokens.Count &&
                            _validTokens[identPos].GetTokenTypeEnum() == TokenType.IDENTIFIER)
                        {
                            if (identPos + 1 >= _validTokens.Count ||
                                _validTokens[identPos + 1].GetTokenTypeEnum() != TokenType.DELIMITER_RPAREN)
                            {
                                InsertSyntheticTokenAtPosition(identPos + 1, TokenType.DELIMITER_RPAREN, ")");
                            }
                        }
                    }
                }
            }
        }


        private int FindTokenIndex(TokenType type)
        {
            for (int i = 0; i < _validTokens.Count; i++)
            {
                if (_validTokens[i].GetTokenTypeEnum() == type)
                    return i;
            }
            return -1;
        }

        private int FindTokenIndexAfter(TokenType type, int startIndex)
        {
            for (int i = startIndex + 1; i < _validTokens.Count; i++)
            {
                if (_validTokens[i].GetTokenTypeEnum() == type)
                    return i;
            }
            return -1;
        }

        private bool HasTokenAfterPosition(TokenType type, int position)
        {
            for (int i = position + 1; i < _validTokens.Count; i++)
            {
                if (_validTokens[i].GetTokenTypeEnum() == type)
                    return true;
            }
            return false;
        }

        private void InsertSyntheticTokenAtPosition(int position, TokenType type, string lexeme)
        {
            if (position < 0) position = 0;
            if (position > _validTokens.Count) position = _validTokens.Count;

            var syntheticToken = new SyntheticToken(
                type,
                lexeme,
                GetLineForPosition(position),
                GetColumnForPosition(position)
            );

            _validTokens.Insert(position, syntheticToken);
        }

        private int GetLineForPosition(int position)
        {
            if (position > 0 && position <= _validTokens.Count)
            {
                return _validTokens[position - 1].GetLine();
            }
            else if (_validTokens.Count > 0)
            {
                return _validTokens.Last().GetLine();
            }
            return 1;
        }

        private int GetColumnForPosition(int position)
        {
            if (position > 0 && position <= _validTokens.Count)
            {
                var prevToken = _validTokens[position - 1];
                return prevToken.GetStartColumn() + prevToken.GetLexeme().Length;
            }
            else if (_validTokens.Count > 0)
            {
                var lastToken = _validTokens.Last();
                return lastToken.GetStartColumn() + lastToken.GetLexeme().Length;
            }
            return 1;
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

            TryConsumeToken(TokenType.OPERATOR_ARROW, "Ожидался оператор '<-'");
            ParseExpression();

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

            TryConsumeToken(TokenType.KEYWORD_TO, "Ожидалось ключевое слово 'to'");
            ParseTo();

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

            TryConsumeToken(TokenType.DELIMITER_RPAREN, "Ожидалась закрывающая скобка ')' после конечного числа");
            ParseOpenCurly();

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
                ParseNewline();
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

            TryConsumeToken(TokenType.DELIMITER_RPAREN, "Ожидалась ')'");
            ParseCloseBracePrintln();

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