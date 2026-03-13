namespace Compiler_Lab1.LexicalAnalyzer
{
    public enum TokenType
    {
        KEYWORD_PRINTLN = 1, 
        IDENTIFIER = 2,
        KEYWORD_FOR = 3,
        KEYWORD_TO = 4,

        DELIMITER_LPAREN = 5,    
        DELIMITER_RPAREN = 6,    

        DELIMITER_LBRACE = 7,    
        DELIMITER_RBRACE = 8,    

        DELIMITER_SEMICOLON = 9,

        OPERATOR_ARROW = 10,

        DIGIT = 11,

        DELIMITER_SPACE = 12,     
        DELIMITER_TABULATION = 13,
        DELIMITER_NEWLINE = 14,

        UNKNOWN = 99
    }

    public interface ITokenDescription
    {
        string GetTokenDescription(TokenType type);
    }

    internal class TokenDescription : ITokenDescription
    {
        public TokenDescription()
        {

        }

        public string GetTokenDescription(TokenType type)
        {
            switch (type)
            {
                case TokenType.DIGIT:
                    return "целое без знака";
                case TokenType.IDENTIFIER:
                    return "идентификатор";
                case TokenType.KEYWORD_FOR:
                    return "ключевое слово for";
                case TokenType.KEYWORD_TO:
                    return "ключевое слово to";
                case TokenType.KEYWORD_PRINTLN:
                    return "ключевое слово println";
                case TokenType.OPERATOR_ARROW:
                    return "оператор стрелка";
                case TokenType.DELIMITER_LPAREN:
                    return "открывающая круглая скобка";
                case TokenType.DELIMITER_RPAREN:
                    return "закрывающая круглая скобка";
                case TokenType.DELIMITER_LBRACE:
                    return "открывающая фигурная скобка";
                case TokenType.DELIMITER_RBRACE:
                    return "закрывающая фигурная скобка";
                case TokenType.DELIMITER_SEMICOLON:
                    return "конец оператора";
                case TokenType.DELIMITER_SPACE:
                    return "пробел";
                case TokenType.DELIMITER_NEWLINE:
                    return "перевод строки";
                case TokenType.UNKNOWN:
                    return "недопустимый символ";
                default:
                    return type.ToString();
            }
        }
    }
}
