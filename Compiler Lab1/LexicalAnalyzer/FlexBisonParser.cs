using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Compiler_Lab1.LexicalAnalyzer
{
    internal class FlexBisonParser : IAnalyzer
    {
        private readonly string _parserPath;
        private List<IToken> _currentTokens;

        public FlexBisonParser()
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectDir = Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\"));

            _parserPath = Path.Combine(projectDir, "ScalaParser", "parser.exe");

            _currentTokens = new List<IToken>();
        }

        public List<IToken> Scan(string input)
        {
            _currentTokens.Clear();

            try
            {
                string tempFile = Path.GetTempFileName();

                File.WriteAllText(tempFile, input, new UTF8Encoding(false));

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = _parserPath,
                    Arguments = $"\"{tempFile}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };

                using (Process process = Process.Start(startInfo))
                {
                    string jsonOutput = process.StandardOutput.ReadToEnd();
                    string errorOutput = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(errorOutput))
                    {

                    }

                    if (!string.IsNullOrEmpty(jsonOutput))
                    {
                        _currentTokens = ParseJsonToTokens(jsonOutput);
                    }
                }

                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
            }

            return _currentTokens;
        }

        private List<IToken> ParseJsonToTokens(string json)
        {
            var tokens = new List<IToken>();

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    foreach (JsonElement element in doc.RootElement.EnumerateArray())
                    {
                        int bisonCode = element.GetProperty("code").GetInt32();
                        string type = element.GetProperty("type").GetString();
                        string lexeme = element.GetProperty("lexeme").GetString();
                        int line = element.GetProperty("line").GetInt32();
                        int startColumn = element.GetProperty("start_column").GetInt32();
                        int endColumn = element.GetProperty("end_column").GetInt32();
                        bool isError = element.GetProperty("is_error").GetBoolean();
                        string errorMessage = element.GetProperty("error_message").GetString();

                        TokenType tokenType = MapBisonCodeToTokenType(bisonCode);

                        Token token = new Token(
                            tokenType,
                            lexeme,
                            line,
                            startColumn,
                            endColumn,
                            isError,
                            errorMessage
                        );

                        tokens.Add(token);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return tokens;
        }

        private TokenType MapBisonCodeToTokenType(int bisonCode)
        {
            return bisonCode switch
            {
                258 => TokenType.KEYWORD_PRINTLN,
                259 => TokenType.IDENTIFIER,
                260 => TokenType.KEYWORD_FOR,
                261 => TokenType.KEYWORD_TO,
                262 => TokenType.DELIMITER_LPAREN,
                263 => TokenType.DELIMITER_RPAREN,
                264 => TokenType.DELIMITER_LBRACE,
                265 => TokenType.DELIMITER_RBRACE,
                266 => TokenType.DELIMITER_SEMICOLON,
                267 => TokenType.OPERATOR_ARROW,
                268 => TokenType.DIGIT,
                269 => TokenType.DELIMITER_SPACE,
                270 => TokenType.DELIMITER_TABULATION,
                271 => TokenType.DELIMITER_NEWLINE,
                272 => TokenType.UNKNOWN,
                273 => TokenType.UNKNOWN,
                _ => TokenType.UNKNOWN
            };
        }
    }
}
