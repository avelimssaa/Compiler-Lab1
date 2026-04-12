using Compiler_Lab1.LexicalAnalyzer;

namespace Compiler_Lab1.Parser
{

    internal class StateMachineParser : IParser
    {
        private readonly List<IToken> _tokens;
        private int _position, _state;

        private readonly List<SyntaxError> _errors = new();
        public IReadOnlyList<SyntaxError> Errors => _errors;
        private IToken Current =>
    _position < _tokens.Count ? _tokens[_position] : null;

        public StateMachineParser(List<IToken> tokens)
        {
            _tokens = tokens.Where(t =>
t.GetTokenTypeEnum() != TokenType.DELIMITER_SPACE &&
t.GetTokenTypeEnum() != TokenType.DELIMITER_TABULATION &&
t.GetTokenTypeEnum() != TokenType.DELIMITER_NEWLINE)
.ToList();
            _position = 0;
            _state = 0;
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

         private bool TryConsumeToken(TokenType nextToken, string errorMessage)
        {
            AddError(errorMessage);
            while (Current != null)
            {
                if (Current.GetTokenTypeEnum() == nextToken)
                {
                    _state++;
                    return true;
                }
                _position++;
            }
            return false;
        }

        public void ParseStart()
        {
            while (true)
            { 
                switch (_state)
                {
                    case 0:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.KEYWORD_FOR)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DELIMITER_LPAREN, "Ожидалось ключевое слово 'for'."))
                                return;
                        }
                        break;

                    case 1:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_LPAREN)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.IDENTIFIER, "Ожидалась '('."))
                                return;
                        }
                        break;

                    case 2:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.IDENTIFIER)
                        {
                            _state++; 
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.OPERATOR_ARROW, "Ожидался идентификатор."))
                                return;
                        }
                        break;

                    case 3:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.OPERATOR_ARROW)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DIGIT, "Ожидалось '<-'."))
                                return;
                        }
                        break;

                    case 4:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.DIGIT)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.KEYWORD_TO, "Ожидалась целое без знака."))
                                return;
                        }
                        break;

                    case 5:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.KEYWORD_TO)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DIGIT, "Ожидалось ключевое слово 'to'"))
                                return;
                        }
                        break;

                    case 6:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.DIGIT)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DELIMITER_RPAREN, "Ожидалось целое без знака."))
                                return;
                        }
                        break;

                    case 7:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_RPAREN)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DELIMITER_LBRACE, "Ожидалась ')'."))
                                return;
                        }
                        break;

                    case 8:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_LBRACE)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.KEYWORD_PRINTLN, "Ожидалась '{'."))
                                return;
                        }
                        break;

                    case 9:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.KEYWORD_PRINTLN)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DELIMITER_LPAREN, "Ожидался оператор 'println'."))
                                return;
                        }
                        break;

                    case 10:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_LPAREN)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.IDENTIFIER, "Ожидалась '('."))
                                return;
                        }
                        break;

                    case 11:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.IDENTIFIER)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DELIMITER_RPAREN, "Ожидался идентификатор."))
                                return;
                        }
                        break;

                    case 12:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_RPAREN)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DELIMITER_RBRACE, "Ожидалась ')'."))
                                return;
                        }
                        break;

                    case 13:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_RBRACE)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DELIMITER_SEMICOLON, "Ожидалась '}'."))
                                return;
                        }
                        break;

                    case 14:
                        if (Current != null && Current.GetTokenTypeEnum() == TokenType.DELIMITER_SEMICOLON)
                        {
                            _state++;
                            _position++;
                        }
                        else
                        {
                            if (!TryConsumeToken(TokenType.DELIMITER_SEMICOLON, "Ожидалась ';'."))
                                return;
                        }
                        break;

                    case 15:
                        _state = 0;
                        if (Current == null)
                            return;
                        break;
                }
            }
        }
    }
}
