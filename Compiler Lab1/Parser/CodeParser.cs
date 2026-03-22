using Compiler_Lab1.LexicalAnalyzer;
using System.Text;

namespace Compiler_Lab1.Parser
{
    public interface IParser
    {
        void ParseStart();
        IReadOnlyList<SyntaxError> Errors { get; }
        string GetRecoveredSource();
    }

    internal class SyntheticToken : IToken
    {
        public bool IsSynthetic { get; } = true;

        private readonly TokenType _type;
        private readonly string _lexeme;
        private readonly int _line;
        private readonly int _column;

        public SyntheticToken(TokenType type, string lexeme, int line, int column)
        {
            _type = type;
            _lexeme = lexeme;
            _line = line;
            _column = column;
        }

        public TokenType GetTokenTypeEnum() => _type;
        public string GetTokenType() => _type.ToString();
        public string GetLexeme() => _lexeme;
        public int GetLine() => _line;
        public int GetStartColumn() => _column;
        public int GetEndColumn() => _column;
        public bool IsError() => false;
        public string GetMessageDescription() => "";
        public int GetConditionCode() => 0;
        public string GetLocation() => $"{_line}:{_column}";
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
            _tokens = tokens;
            _position = 0;
        }

        public void ParseStart()
        {
            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.KEYWORD_FOR)
            {
                AddError($"Ожидался ключевое слово for, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                while (Current != null && Current.GetTokenTypeEnum() != TokenType.KEYWORD_FOR)
                    _position++;
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.KEYWORD_FOR)
                _position++;

            ParseKeywordFor();
        }

        private void ParseKeywordFor()
        {
            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.DELIMITER_LPAREN)
            {
                AddError($"Ожидался DELIMITER_LPAREN, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.DELIMITER_LPAREN, "(");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_LPAREN)
                _position++;

            ParseOpenBracketCycle();
        }

        private void ParseOpenBracketCycle()
        {
            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.IDENTIFIER)
            {
                AddError($"Ожидался IDENTIFIER, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.IDENTIFIER, "i");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.IDENTIFIER)
                _position++;

            ParseIdCycle();
        }

        private void ParseIdCycle()
        {
            while (true)
            {
                SkipSpaces();

                if (Current == null)
                {
                    AddError("Ожидался идентификатор или <-, но вход закончился");
                    return;
                }

                var t = Current.GetTokenTypeEnum();

                if (t == TokenType.IDENTIFIER || t == TokenType.DIGIT)
                {
                    _position++;
                    continue;
                }

                if (t == TokenType.OPERATOR_ARROW)
                {
                    _position++;
                    ParseExpression();
                    return;
                }

                AddError($"Ожидался идентификатор или <-, найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                _position++;
            }
        }

        private void ParseExpression()
        {
            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.DIGIT)
            {
                AddError($"Ожидался DIGIT, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.DIGIT, "0");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.DIGIT)
                _position++;

            ParseBeginNumber();
        }

        private void ParseBeginNumber()
        {
            while (true)
            {
                SkipSpaces();

                if (Current == null)
                {
                    AddError("Ожидался digit или to, но вход закончился");
                    return;
                }

                var t = Current.GetTokenTypeEnum();

                if (t == TokenType.DIGIT)
                {
                    _position++;
                    continue;
                }

                if (t == TokenType.KEYWORD_TO)
                {
                    _position++;
                    ParseTo();
                    return;
                }

                AddError($"Ожидался digit или to, найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                _position++;
            }
        }

        private void ParseTo()
        {
            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.DIGIT)
            {
                AddError($"Ожидался DIGIT, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.DIGIT, "0");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.DIGIT)
                _position++;

            ParseEndNumber();
        }

        private void ParseEndNumber()
        {
            while (true)
            {
                SkipSpaces();

                if (Current == null)
                {
                    AddError("Ожидался digit или ), но вход закончился");
                    return;
                }

                var t = Current.GetTokenTypeEnum();

                if (t == TokenType.DIGIT)
                {
                    _position++;
                    continue;
                }

                if (t == TokenType.DELIMITER_RPAREN)
                {
                    _position++;
                    ParseOpenCurly();
                    return;
                }

                AddError($"Ожидался digit или ), найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                _position++;
            }
        }

        private void ParseOpenCurly()
        {
            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.DELIMITER_LBRACE)
            {
                AddError($"Ожидался DELIMITER_LBRACE, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.DELIMITER_LBRACE, "{");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_LBRACE)
                _position++;

            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.DELIMITER_NEWLINE)
            {
                AddError($"Ожидался DELIMITER_NEWLINE, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.DELIMITER_NEWLINE, "\n");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_NEWLINE)
                _position++;

            ParseNewline();
        }

        private void ParseNewline()
        {
            while (true)
            {
                SkipSpaces();

                if (Current == null)
                {
                    AddError("Ожидался println или }, но вход закончился");
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
                    _position++;
                    ParseSemicolon();
                    return;
                }

                AddError($"Ожидался println или }} , найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                _position++;
            }
        }

        private void ParseCycleBody()
        {
            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.KEYWORD_PRINTLN)
            {
                AddError($"Ожидался println, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.KEYWORD_PRINTLN, "println");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.KEYWORD_PRINTLN)
                _position++;

            ParsePrintln();
        }

        private void ParsePrintln()
        {
            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.DELIMITER_LPAREN)
            {
                AddError($"Ожидался DELIMITER_LPAREN, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.DELIMITER_LPAREN, "(");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_LPAREN)
                _position++;

            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.IDENTIFIER)
            {
                AddError($"Ожидался IDENTIFIER, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.IDENTIFIER, "i");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.IDENTIFIER)
                _position++;

            ParseIdCycleBody();
        }

        private void ParseIdCycleBody()
        {
            while (true)
            {
                SkipSpaces();

                if (Current == null)
                {
                    AddError("Ожидался идентификатор или ), но вход закончился");
                    return;
                }

                var t = Current.GetTokenTypeEnum();

                if (t == TokenType.IDENTIFIER || t == TokenType.DIGIT)
                {
                    _position++;
                    continue;
                }

                if (t == TokenType.DELIMITER_RPAREN)
                {
                    _position++;
                    SkipSpaces();

                    if (Current == null || Current.GetTokenTypeEnum() != TokenType.DELIMITER_NEWLINE)
                    {
                        AddError($"Ожидался DELIMITER_NEWLINE, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                        InsertSyntheticToken(TokenType.DELIMITER_NEWLINE, "\n");
                    }

                    if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_NEWLINE)
                        _position++;

                    ParseNewline();
                    return;
                }

                AddError($"Ожидался идентификатор или ), найдено {Current.GetTokenType()} ('{Current.GetLexeme()}')");
                _position++;
            }
        }

        private void ParseSemicolon()
        {
            SkipSpaces();

            if (Current == null || Current.GetTokenTypeEnum() != TokenType.DELIMITER_SEMICOLON)
            {
                AddError($"Ожидался DELIMITER_SEMICOLON, найдено {Current?.GetTokenType()} ('{Current?.GetLexeme()}')");
                InsertSyntheticToken(TokenType.DELIMITER_SEMICOLON, ";");
            }

            if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_SEMICOLON)
                _position++;
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
            var err = new SyntaxError
            {
                UnexpectedLexeme = Current?.GetLexeme() ?? "<EOF>",
                Message = message,
                Line = Current?.GetLine() ?? 0,
                Column = Current?.GetStartColumn() ?? 0
            };

            _errors.Add(err);
        }

        private void InsertSyntheticToken(TokenType type, string lexeme)
        {
            int line = Current?.GetLine() ?? 0;
            int col = Current?.GetStartColumn() ?? 0;
            var fake = new SyntheticToken(type, lexeme, line, col);
            _tokens.Insert(_position, fake);
        }

        public string GetRecoveredSource()
        {
            var sb = new StringBuilder();

            foreach (var t in _tokens)
            {
                if (t.GetTokenTypeEnum() == TokenType.UNKNOWN)
                    continue;

                sb.Append(t.GetLexeme());
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }
    }
}
