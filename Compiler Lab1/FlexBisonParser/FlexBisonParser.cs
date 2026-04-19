using Compiler_Lab1.LexicalAnalyzer;
using Compiler_Lab1.Parser;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Compiler_Lab1.FlexBisonParser
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

            _parserPath = Path.Combine(projectDir, "FlexBisonParser", "parser.exe");

            _currentTokens = new List<IToken>();
            Errors = new List<SyntaxError>();
        }

        public List<IToken> Scan(string input)
        {
            var result = ScanWithErrors(input);

            _currentTokens = result.Tokens;

            Errors = result.Errors.Select(e => new SyntaxError
            {
                Message = ExtractCleanMessage(e),
                Line = GetLineFromError(e),
                Column = 1,
                UnexpectedLexeme = "?"
            }).ToList();

            return _currentTokens;
        }

        private string ExtractCleanMessage(string error)
        {
            var match = Regex.Match(error, @"Строка \d+:\s*(.+)");
            if (match.Success)
                return match.Groups[1].Value;
            return error;
        }

        private int GetLineFromError(string error)
        {
            var match = Regex.Match(error, @"Строка (\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int line))
                return line;
            return 1;
        }

        public ScanResult ScanWithErrors(string input)
        {
            var result = new ScanResult();
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

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

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
                    process.ErrorDataReceived += (s, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }

                string textOutput = outputBuilder.ToString();
                string errorOutput = errorBuilder.ToString();

                if (!string.IsNullOrEmpty(textOutput))
                {
                    var lines = textOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    bool isErrorReport = false;

                    foreach (var line in lines)
                    {
                        if (line.Contains("--- ОТЧЕТ ОБ ОШИБКАХ ---"))
                        {
                            isErrorReport = true;
                            continue;
                        }

                        if (isErrorReport && Regex.IsMatch(line, @"\[\d+\] Строка"))
                        {
                            result.Errors.Add(line);
                        }
                        else if (line.Contains("Лексическая ошибка") || line.Contains("Критическая синтаксическая ошибка"))
                        {
                            if (!isErrorReport) result.Errors.Add(line);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(errorOutput))
                {
                    var errorLines = errorOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var errLine in errorLines)
                    {
                        result.Errors.Add($"[Системная ошибка парсера]: {errLine}");
                    }
                }

                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Внутренняя ошибка при запуске парсера: {ex.Message}");
            }

            return result;
        }
    }
}