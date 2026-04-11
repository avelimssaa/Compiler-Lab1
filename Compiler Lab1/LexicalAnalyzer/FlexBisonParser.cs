using Compiler_Lab1.Parser;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Compiler_Lab1.LexicalAnalyzer
{
    public class ScanResult
    {
        public List<IToken> Tokens { get; set; }
        public List<string> Errors { get; set; }
        public bool HasErrors => Errors != null && Errors.Count > 0;

        public ScanResult()
        {
            Tokens = new List<IToken>();
            Errors = new List<string>();
        }

        public void PrintErrors()
        {

        }
    }

    internal class FlexBisonParser : IAnalyzer
    {
    private readonly string _parserPath;
        private List<IToken> _currentTokens;
        public List<SyntaxError> Errors { get; private set; }

        public FlexBisonParser()
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectDir = Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\"));

            _parserPath = Path.Combine(projectDir, "ScalaParser", "parser.exe");

            _currentTokens = new List<IToken>();
            Errors = new List<SyntaxError>();
        }

        public List<IToken> Scan(string input)
        {
            var result = ScanWithErrors(input);
            _currentTokens = result.Tokens;
            Errors = result.Errors.Select(e => new SyntaxError
            {
                Message = e,
                Line = GetLineFromError(e),
                Column = GetColumnFromError(e),
                UnexpectedLexeme = GetLexemeFromError(e)
            }).ToList();

            return _currentTokens;
        }

        private int GetLineFromError(string error)
        {
            var match = Regex.Match(error, @"Line (\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int line))
                return line;
            return 1;
        }

        private int GetColumnFromError(string error)
        {
            var match = Regex.Match(error, @"Column (\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int column))
                return column;
            return 1;
        }

        private string GetLexemeFromError(string error)
        {
            var match = Regex.Match(error, @"unexpected '(.+?)'");
            if (match.Success)
                return match.Groups[1].Value;
            return "?";
        }

        public ScanResult ScanWithErrors(string input)
        {
            var result = new ScanResult();

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
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using (Process process = Process.Start(startInfo))
                {
                    string jsonOutput = process.StandardOutput.ReadToEnd();
                    string errorOutput = process.StandardError.ReadToEnd();
                    int exitCode = process.ExitCode;

                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(errorOutput))
                    {
                        var syntaxErrors = ParseErrorOutput(errorOutput);
                        result.Errors.AddRange(syntaxErrors);
                    }

                    if (!string.IsNullOrEmpty(jsonOutput))
                    {
                        var tokensAndErrors = ParseJsonToTokensWithErrors(jsonOutput);
                        result.Tokens = tokensAndErrors.Tokens;
                        result.Errors.AddRange(tokensAndErrors.Errors);
                    }

                    if (exitCode != 0 && string.IsNullOrEmpty(errorOutput) && string.IsNullOrEmpty(jsonOutput))
                    {
                        result.Errors.Add($"Parser process exited with code {exitCode}");
                    }
                }

                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Internal error while running parser: {ex.Message}");
            }

            return result;
        }

        private List<string> ParseErrorOutput(string errorOutput)
        {
            var errors = new List<string>();
            var lines = errorOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.Contains("syntax error"))
                {
                    var error = ParseSyntaxErrorMessage(line);
                    errors.Add(error);
                }
                else if (line.Contains("parse error"))
                {
                    errors.Add($"[Parse Error] {line}");
                }
                else if (line.Contains("unexpected"))
                {
                    errors.Add($"[Syntax Error] {line}");
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    errors.Add($"[Parser] {line}");
                }
            }

            return errors;
        }

        private string ParseSyntaxErrorMessage(string line)
        {
            try
            {
                var parts = line.Split(':');
                if (parts.Length >= 3)
                {
                    string location = parts[1].Trim();
                    string message = string.Join(":", parts.Skip(2)).Trim();
                    return $"Line {location}: {message}";
                }
                return line;
            }
            catch
            {
                return line;
            }
        }

        private (List<IToken> Tokens, List<string> Errors) ParseJsonToTokensWithErrors(string json)
        {
            var tokens = new List<IToken>();
            var errors = new List<string>();

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

                        if (isError && !string.IsNullOrEmpty(errorMessage))
                        {
                            errors.Add($"Line {line}, Column {startColumn}: {errorMessage}");
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                errors.Add($"Failed to parse JSON output: {ex.Message}");
                errors.Add($"Raw JSON: {json}");
            }
            catch (Exception ex)
            {
                errors.Add($"Error processing parser output: {ex.Message}");
            }

            return (tokens, errors);
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