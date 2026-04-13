using System.Diagnostics;

namespace Compiler_Lab1.TextEditor
{
    public interface IHTMLHelper
    {
        void CallHTML(string needHTML);
    }
    internal class HTMLHelper : IHTMLHelper
    {
        private ILocalization _localization;
        public HTMLHelper(ILocalization localization)
        {
            _localization = localization;
        }

        private void CallPage(string filePath)
        {
            try
            {
                string helpPath = Path.Combine(Application.StartupPath, filePath);

                if (File.Exists(helpPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = helpPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show(_localization.Get("HelpFileError"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{_localization.Get("HelpOpenError")}: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void CallHTML(string needHTML)
        {
            if (needHTML == null)
            {
                return;
            }
            else if (needHTML == "Help")
            {
                CallPage(_localization.Get("UserManual"));
            }
            else if (needHTML == "Mission")
            {
                CallPage("HTMLs/Mission.html");
            }
            else if (needHTML == "Grammar")
            {
                CallPage("HTMLs/Grammar.html");
            }
            else if (needHTML == "GrammarClassification")
            {
                CallPage("HTMLs/GrammarClassification.html");
            }
            else if (needHTML == "MethodOfAnalysis")
            {
                CallPage("HTMLs/MethodOfAnalysis.html");
            }
            else if (needHTML == "TestCase")
            {
                CallPage("HTMLs/TestCase.html");
            }
            else if (needHTML == "ListOfLiterature")
            {
                CallPage("HTMLs/ListOfLiterature.html");
            }
            else if (needHTML == "SourceCode")
                CallPage("HTMLs/SourceCode.html");
        }
    }
}
