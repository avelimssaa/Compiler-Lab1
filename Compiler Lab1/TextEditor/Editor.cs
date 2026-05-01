using Antlr4.Runtime;
using Compiler_Lab1.ASTBuild;
using Compiler_Lab1.GRAMMAR;
using Compiler_Lab1.LexicalAnalyzer;
using Compiler_Lab1.Parser;
using Compiler_Lab1.RegEx;
using Compiler_Lab1.TextEditor;
using Compiler_Lab1.FlexBisonParser;
using FastColoredTextBoxNS;
using System.Diagnostics;
using System.Text;
using Compiler_Lab1.InternalRepresentation;

namespace Compiler_Lab1
{
    public partial class textEditor : Form
    {
        private bool _isUndoRedoOperation = false;

        private Dictionary<TabPage, TabFileInfo> _tabFileInfo = new Dictionary<TabPage, TabFileInfo>();

        private int _pagesCount;

        private ILocalization _localization;

        private IAnalyzer _analyzer;

        public textEditor()
        {
            InitializeComponent();
            _localization = new Localization();
            _pagesCount = 0;
            CreateFile();
            _analyzer = new Analyzer();
        }

        private void CreateFile()
        {
            _pagesCount++;

            TabPage newTab = new TabPage();
            newTab.Location = new Point(4, 29);
            newTab.Name = _localization.Get("Untitled") + $"{_pagesCount}.txt";
            newTab.Size = new Size(774, 209);
            newTab.TabIndex = _pagesCount - 1;
            newTab.Text = newTab.Name;
            newTab.UseVisualStyleBackColor = true;
            tabControlEditor.TabPages.Add(newTab);

            FastColoredTextBox textBox = new FastColoredTextBox();
            textBox.Dock = DockStyle.Fill;
            textBox.TextChanged += TextBox_TextChanged;
            textBox.Name = $"textbox{_pagesCount}";
            textBox.TabIndex = newTab.TabIndex;
            textBox.Text = "";
            textBox.Language = Language.CSharp;
            textBox.ShowLineNumbers = true;
            textBox.HighlightingRangeType = HighlightingRangeType.VisibleRange;
            newTab.Controls.Add(textBox);

            TabFileInfo fileInfo = new TabFileInfo();

            fileInfo.FilePath = null;
            fileInfo.FileName = newTab.Name;
            fileInfo.IsChanged = false;


            tabControlEditor.SelectedTab = newTab;

            _tabFileInfo[newTab] = fileInfo;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            FastColoredTextBox textBox = sender as FastColoredTextBox;
            if (textBox == null) return;

            TabPage parentTab = textBox.Parent as TabPage;
            if (parentTab != null && _tabFileInfo.ContainsKey(parentTab))
            {
                TabFileInfo fileInfo = _tabFileInfo[parentTab];

                fileInfo.IsChanged = true;

                if (!parentTab.Text.EndsWith("*"))
                {
                    parentTab.Text = parentTab.Text + "*";
                }

                UpdateStatusInfo(parentTab);
            }
        }

        private void createFile_Click(object sender, EventArgs e)
        {
            CreateFile();
        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveFileLike_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void SaveAs()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            TabFileInfo fileInfo = _tabFileInfo[currentTab];

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = $"{_localization.Get("TextFiles")} (*.txt)|*.txt|{_localization.Get("AllFiles")} (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.AddExtension = true;
                saveFileDialog.FileName = fileInfo.FileName ?? $"{_localization.Get("Untitled")}.txt";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveFileToPath(saveFileDialog.FileName);
                }
            }
        }

        private void Save()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            string filePath = _tabFileInfo[currentTab].FilePath;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                SaveAs();
            }
            else
            {
                SaveFileToPath(filePath);
            }
        }

        private void SaveFileToPath(string filePath)
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            try
            {
                string textToSave = textBox.Text;
                File.WriteAllText(filePath, textToSave, Encoding.UTF8);

                if (_tabFileInfo.ContainsKey(currentTab))
                {
                    _tabFileInfo[currentTab].FilePath = filePath;
                    _tabFileInfo[currentTab].FileName = Path.GetFileName(filePath);
                    _tabFileInfo[currentTab].IsChanged = false;
                }
                currentTab.Text = Path.GetFileName(filePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{_localization.Get("SaveError")}:\n{ex.Message}",
                    _localization.Get("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void OpenFile()
        {
            string fileContent = "", currentFilePath = null, currentFileName = _localization.Get("Untitled");

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = $"{_localization.Get("TextFiles")} (*.txt)|*.txt";
                openFileDialog.FilterIndex = 1;
                openFileDialog.DefaultExt = "txt";
                openFileDialog.InitialDirectory = Application.StartupPath;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        fileContent = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);
                        currentFilePath = openFileDialog.FileName;
                        currentFileName = Path.GetFileName(currentFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{_localization.Get("OpenError")}: {ex.Message}", _localization.Get("Error"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            _pagesCount++;

            TabPage newTab = new TabPage();

            FastColoredTextBox textBox = new FastColoredTextBox();

            newTab.Controls.Add(textBox);
            newTab.Location = new Point(4, 29);
            newTab.Name = currentFileName;
            newTab.Padding = new Padding(3);
            newTab.Size = new Size(774, 209);
            newTab.TabIndex = _pagesCount - 1;
            newTab.Text = newTab.Name;
            newTab.UseVisualStyleBackColor = true;

            textBox.Dock = DockStyle.Fill;
            textBox.Location = new Point(63, 0);
            textBox.TextChanged += TextBox_TextChanged;
            textBox.Name = $"textbox{_pagesCount}";
            textBox.Size = new Size(711, 209);
            textBox.TabIndex = newTab.TabIndex;
            textBox.Text = fileContent;
            textBox.Language = Language.CSharp;
            textBox.ShowLineNumbers = true;
            textBox.HighlightingRangeType = HighlightingRangeType.VisibleRange;

            TabFileInfo fileInfo = new TabFileInfo();

            fileInfo.FilePath = currentFilePath;
            fileInfo.FileName = newTab.Name;
            fileInfo.IsChanged = false;

            tabControlEditor.TabPages.Add(newTab);
            tabControlEditor.SelectedTab = newTab;

            _tabFileInfo[newTab] = fileInfo;
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void Exit()
        {
            Application.Exit();
        }

        private void textEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (TabPage tab in tabControlEditor.TabPages)
            {
                if (_tabFileInfo[tab].IsChanged)
                {
                    tabControlEditor.SelectedTab = tab;

                    DialogResult result = MessageBox.Show(
                        $"{_localization.Get("SaveFile")} \"{_tabFileInfo[tab].FileName}\"?",
                        _localization.Get("ExitApp"),
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        Save();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void Undo()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            textBox.Undo();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void Redo()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            textBox.Redo();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void Cut()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            if (textBox.SelectionLength > 0)
            {
                textBox.Cut();
            }
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void Copy()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            if (textBox.SelectionLength > 0)
            {
                textBox.Copy();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void Paste()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            if (Clipboard.ContainsText())
            {
                textBox.Paste();
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void Delete()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            if (textBox.SelectionLength > 0)
            {
                textBox.SelectedText = "";
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void SelectAll()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox != null)
            {
                textBox.SelectAll();
                textBox.Focus();
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void CallHelp()
        {
            IHTMLHelper htmlHelper = new HTMLHelper(_localization);
            htmlHelper.CallHTML("Help");
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            CallHelp();
        }

        private void CallAbout()
        {
            About about1 = new About();
            about1.ShowDialog();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            CallAbout();
        }

        private void FontSizeCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox? cmb = sender as ToolStripComboBox;
            if (cmb != null && cmb.SelectedItem != null)
            {

                float newSize = Convert.ToSingle(cmb.SelectedItem);
                SetFontSizeForAllWindows(newSize);
            }
        }

        private void SetFontSizeForAllWindows(float size)
        {
            foreach (TabPage tab in tabControlEditor.TabPages)
            {
                FastColoredTextBox textBox = tab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
                if (textBox != null)
                {
                    FontStyle currentStyle = textBox.Font.Style;
                    string fontFamily = textBox.Font.FontFamily.Name;

                    textBox.Font = new Font(fontFamily, size, currentStyle);
                }
            }

            if (tabPageResults != null)
            {
                FontStyle resultsStyle = tabPageResults.Font.Style;
                string resultsFontFamily = tabPageResults.Font.FontFamily.Name;
                tabPageResults.Font = new Font(resultsFontFamily, size, resultsStyle);
            }

            if (dgvResults != null)
            {
                FontStyle rtbResultsStyle = dgvResults.Font.Style;
                string rtbResultsFontFamily = dgvResults.Font.FontFamily.Name;
                dgvResults.Font = new Font(rtbResultsFontFamily, size, rtbResultsStyle);
            }
        }

        private void CloseTab()
        {
            if (tabControlEditor.SelectedTab != null)
            {
                TabPage currentTab = tabControlEditor.SelectedTab;

                if (_tabFileInfo[currentTab].IsChanged)
                {
                    DialogResult result = MessageBox.Show(
                        $"{_localization.Get("SaveFile")} \"{_tabFileInfo[currentTab].FileName}\"?",
                        _localization.Get("CloseTabTitle"),
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                        Save();
                    else if (result == DialogResult.Cancel)
                        return;
                }

                tabControlEditor.TabPages.Remove(currentTab);
                _tabFileInfo.Remove(currentTab);

                _pagesCount--;

                if (tabControlEditor.TabPages.Count == 0)
                    CreateFile();
            }
        }

        private void btnCloseTabQuick_Click(object sender, EventArgs e)
        {
            CloseTab();
        }

        private void btnCloseTab_Click(object sender, EventArgs e)
        {
            CloseTab();
        }

        private void cmbLocalization_SelectedIndexChanged(object sender, EventArgs e)
        {
            string newLang = cmbLocalization.SelectedIndex == 0 ? "ru" : "en";
            ChangeLanguage(newLang);
        }

        private void ChangeLanguage(string language)
        {
            if (_localization.CurrentLanguage == language) return;

            _localization.CurrentLanguage = language;

            if (tabControlEditor.SelectedTab != null)
            {
                UpdateStatusInfo(tabControlEditor.SelectedTab);
            }

            UpdateUILanguage();
        }

        private void UpdateUILanguage()
        {
            Text = _localization.Get("Editor");

            UpdateToolStripDown();

            UpdateToolStripQuickBtn();

            UpdateOutputWindow();
        }

        private void UpdateToolStripDown()
        {
            ddmFile.Text = _localization.Get("File");
            createFile.Text = _localization.Get("Create");
            btnOpenFile.Text = _localization.Get("Open");
            saveFile.Text = _localization.Get("Save");
            saveFileLike.Text = _localization.Get("SaveAs");
            btnCloseTab.Text = _localization.Get("Close Tab");
            exitBtn.Text = _localization.Get("Exit");

            ddmEdit.Text = _localization.Get("Edit");
            btnBack.Text = _localization.Get("Undo");
            btnForward.Text = _localization.Get("Redo");
            btnCut.Text = _localization.Get("Cut");
            btnCopy.Text = _localization.Get("Copy");
            btnPaste.Text = _localization.Get("Paste");
            btnDelete.Text = _localization.Get("Delete");
            btnSelectAll.Text = _localization.Get("SelectAll");

            ddmText.Text = _localization.Get("Text");
            btnMission.Text = _localization.Get("MissionStatement");
            btnGrammar.Text = _localization.Get("Grammar");
            btnGrammarClassification.Text = _localization.Get("Grammar classification");
            btnMethodOfAnalysis.Text = _localization.Get("Method of analysis");
            btnTestCase.Text = _localization.Get("Test case");
            btnListOfLiterature.Text = _localization.Get("List of literature");
            btnSourceCode.Text = _localization.Get("Source code");

            btnStart.Text = _localization.Get("Start");

            ddmCertificate.Text = _localization.Get("Certificate");
            btnHelp.Text = _localization.Get("Help");
            btnAbout.Text = _localization.Get("About");

            viewDropDownBtn.Text = _localization.Get("View");
            txtHelpFont.Text = _localization.Get("Help font");
            FontSizeCmb.Text = _localization.Get("Font size");
            txtHelpLocal.Text = _localization.Get("Choose local");
            cmbLocalization.Text = _localization.Get("UserManual");
        }

        private void UpdateToolStripQuickBtn()
        {
            createFileQuick.Text = _localization.Get("Create");
            openFileQuick.Text = _localization.Get("Open");
            saveFileQuick.Text = _localization.Get("Save");
            btnCloseTabQuick.Text = _localization.Get("CloseTab");
            btnBackQuick.Text = _localization.Get("Undo");
            btnForwardQuick.Text = _localization.Get("Redo");
            btnCopyQuick.Text = _localization.Get("Copy");
            btnCutQuick.Text = _localization.Get("Cut");
            btnPasteQuick.Text = _localization.Get("Paste");
            btnStartQuick.Text = _localization.Get("Start");
            btnHelpQuick.Text = _localization.Get("Help");
            btnAboutQuick.Text = _localization.Get("About");

            createFileQuick.ToolTipText = _localization.Get("Create");
            openFileQuick.ToolTipText = _localization.Get("Open");
            saveFileQuick.ToolTipText = _localization.Get("Save");
            btnCloseTabQuick.ToolTipText = _localization.Get("CloseTab");
            btnBackQuick.ToolTipText = _localization.Get("Undo");
            btnForwardQuick.ToolTipText = _localization.Get("Redo");
            btnCopyQuick.ToolTipText = _localization.Get("Copy");
            btnCutQuick.ToolTipText = _localization.Get("Cut");
            btnPasteQuick.ToolTipText = _localization.Get("Paste");
            btnStartQuick.ToolTipText = _localization.Get("Start");
            btnHelpQuick.ToolTipText = _localization.Get("Help");
            btnAboutQuick.ToolTipText = _localization.Get("About");
        }

        private void UpdateOutputWindow()
        {
            tabPageResults.Text = _localization.Get("ResultTab");
        }

        private void textEditor_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
    ? DragDropEffects.Copy
    : DragDropEffects.None;
        }

        private void textEditor_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files == null || files.Length == 0) return;

            foreach (string filePath in files)
            {
                bool alreadyOpen = false;
                foreach (var tab in _tabFileInfo)
                {
                    if (tab.Value.FilePath == filePath)
                    {
                        tabControlEditor.SelectedTab = tab.Key;
                        alreadyOpen = true;
                        break;
                    }
                }

                if (!alreadyOpen)
                {
                    OpenFileFromPath(filePath);
                }
            }
        }

        private void OpenFileFromPath(string filePath)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath, Encoding.UTF8);
                string fileName = Path.GetFileName(filePath);

                _pagesCount++;

                TabPage newTab = new TabPage();
                FastColoredTextBox textBox = new FastColoredTextBox();

                newTab.Controls.Add(textBox);
                newTab.Name = fileName;
                newTab.Text = fileName;
                newTab.Padding = new Padding(3);

                textBox.Dock = DockStyle.Fill;
                textBox.Location = new Point(63, 0);
                textBox.TextChanged += TextBox_TextChanged;
                textBox.Text = fileContent;

                textBox.Language = Language.CSharp;
                textBox.ShowLineNumbers = true;
                textBox.HighlightingRangeType = HighlightingRangeType.VisibleRange;

                TabFileInfo fileInfo = new TabFileInfo
                {
                    FilePath = filePath,
                    FileName = fileName,
                    IsChanged = false
                };

                tabControlEditor.TabPages.Add(newTab);
                tabControlEditor.SelectedTab = newTab;
                _tabFileInfo[newTab] = fileInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{_localization.Get("OpenError")}: {ex.Message}",
                    _localization.Get("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void UpdateStatusInfo(TabPage tab)
        {
            if (tab == null || !_tabFileInfo.ContainsKey(tab)) return;

            FastColoredTextBox textBox = tab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            var fileInfo = _tabFileInfo[tab];

            string language = _localization.CurrentLanguage == "ru" ? "Русский" : "English";
            labelLanguage.Text = $"{_localization.Get("Language")}: {language}";

            int charCount = textBox.Text.Length;
            string fileSize = GetFileSizeString(charCount);
            labelFileSize.Text = $"{_localization.Get("FileSize")}: {fileSize}";

            int lineCount = textBox.Lines.Count;
            labelLineCount.Text = $"{_localization.Get("Lines")}: {lineCount}";
        }

        private string GetFileSizeString(int charCount)
        {
            double bytes = charCount * 2;

            if (bytes < 1024)
                return $"{bytes} B";
            else if (bytes < 1024 * 1024)
                return $"{(bytes / 1024):F1} KB";
            else
                return $"{(bytes / (1024 * 1024)):F1} MB";
        }

        private void tabControlEditor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlEditor.SelectedTab != null)
            {
                UpdateStatusInfo(tabControlEditor.SelectedTab);
            }
        }

        private void textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isUndoRedoOperation) return;

            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.N:
                        CreateFile();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.O:
                        OpenFile();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.S:
                        Save();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.Shift | Keys.S:
                        SaveAs();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.W:
                        CloseTab();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.Z:
                        Undo();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.Y:
                        Redo();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.X:
                        Cut();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.C:
                        Copy();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.V:
                        Paste();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.A:
                        SelectAll();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.P:
                        AntlrParser();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.M:
                        StateMachineRegEx();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.B:
                        FlexBisonParser();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                }
            }

            if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.F:
                        ddmFile.ShowDropDown();
                        e.Handled = true;
                        break;

                    case Keys.E:
                        ddmEdit.ShowDropDown();
                        e.Handled = true;
                        break;

                    case Keys.V:
                        viewDropDownBtn.ShowDropDown();
                        e.Handled = true;
                        break;

                    case Keys.T:
                        ddmText.ShowDropDown();
                        e.Handled = true;
                        break;

                    case Keys.C:
                        ddmCertificate.ShowDropDown();
                        e.Handled = true;
                        break;
                }
            }

            switch (e.KeyCode)
            {
                case Keys.F1:
                    CallHelp();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.F5:
                    btnStart.PerformClick();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

                case Keys.F12:
                    CallAbout();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void Compile()
        {
            dgvResults.Rows.Clear();

            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            string textToAnalyze = textBox.Text;

            if (string.IsNullOrEmpty(textToAnalyze))
            {
                MessageBox.Show("Введена пустая строка.");
                return;
            }

            RunSyntaxAnalysis();
        }

        private void AntlrParser()
        {
            dgvResults.Rows.Clear();

            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            string code = textBox.Text;

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Введена пустая строка.");
                return;
            }

            var input = new AntlrInputStream(code);
            var lexer = new ForLangLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ForLangParser(tokens);

            var errorListener = new ForLangErrorListener();

            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);

            parser.start();

            foreach (var err in errorListener.Errors)
            {
                dgvResults.Rows.Add(err.Fragment, err.Location, err.Description);
            }

            if (errorListener.Errors.Count == 0)
            {
                dgvResults.Rows.Add("—", "—", "Ошибок не обнаружено");
            }
        }

        private void RunSyntaxAnalysis()
        {
            dgvResults.Rows.Clear();
            rtbAST.Clear();

            var textBox = tabControlEditor.SelectedTab.Controls
                .OfType<FastColoredTextBox>()
                .FirstOrDefault();

            if (textBox == null)
                return;

            string code = textBox.Text;

            var tokens = _analyzer.Scan(code);

            //var parser = new CodeParser(tokens);
            IParser parser = new StateMachineParser(tokens);
            parser.ParseStart();
            tokens = parser.Tokens;

            dgvResults.Rows.Clear();

            foreach (var err in parser.Errors)
            {
                int rowIndex = dgvResults.Rows.Add(
                    err.UnexpectedLexeme,
                    err.Location,
                    err.Message
                );

                dgvResults.Rows[rowIndex].Tag = err;
            }

            if (parser.Errors.Count == 0)
            {
                int rowIndex = dgvResults.Rows.Add(
                    "",
                    "",
                    "Синтаксический анализ завершён успешно"
                );
                dgvResults.Rows[rowIndex].Tag = null;

                PrintAST(tokens);
            }

            ErrorsCount.Text = $"Количество ошибок: {parser.Errors.Count}";
        }

        private void PrintAST(List<LexicalAnalyzer.IToken> tokens)
        {
            rtbAST.Clear();
            IASTBuilder builder = new ASTBuilder();
            string AST = builder.FormAST(tokens);
            rtbAST.Text = AST;

            dgvResults.Rows.Clear();

            foreach (var err in builder.SemanticError)
            {
                int rowIndex = dgvResults.Rows.Add(
                    err.UnexpectedLexeme,
                    err.Location,
                    err.Message
                );

                dgvResults.Rows[rowIndex].Tag = err;
            }

            if (builder.SemanticError.Count == 0)
            {
                int rowIndex = dgvResults.Rows.Add(
                    "",
                    "",
                    "Синтаксический и семантический анализ завершён успешно"
                );
                dgvResults.Rows[rowIndex].Tag = null;
            }

            else
            {
                int rowIndex = dgvResults.Rows.Add(
    "",
    "",
    $"Количество ошибок: {builder.SemanticError.Count}"
);
                dgvResults.Rows[rowIndex].Tag = null;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Compile();
        }

        private void btnStartQuick_Click(object sender, EventArgs e)
        {
            Compile();
        }

        private void dgvResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int line = 0, column = 0;
            if (e.RowIndex < 0 || e.RowIndex >= dgvResults.Rows.Count)
                return;

            var syntaxError = dgvResults.Rows[e.RowIndex].Tag as SyntaxError;
            var semanticError = dgvResults.Rows[e.RowIndex].Tag as SemanticError;
            var arithSyntaxError = dgvResults.Rows[e.RowIndex].Tag as IArithSyntax;

            if (syntaxError == null)
            {
                if (semanticError == null)
                {
                    if (arithSyntaxError == null)
                        return;
                }
            }

            var textBox = tabControlEditor.SelectedTab.Controls
                .OfType<FastColoredTextBox>()
                .FirstOrDefault();

            if (textBox == null)
                return;

            if (syntaxError != null)
            {
                line = syntaxError.Line;
                column = syntaxError.Column;
            }
            else if (semanticError != null)
            {
                line = semanticError.Line;
                column = semanticError.Column;
            }
            else if (arithSyntaxError != null)
            {
                line = arithSyntaxError.Line;
                column = arithSyntaxError.Column;
            }
            SetCursorPosition(textBox, line, column);
        }

        private void SetCursorPosition(FastColoredTextBox textBox, int line, int column)
        {
            if (textBox == null) return;

            line = Math.Max(1, Math.Min(line, textBox.LinesCount));
            column = Math.Max(1, column);

            var place = new Place(column - 1, line - 1);

            string currentLine = textBox.Lines[line - 1];
            if (place.iChar > currentLine.Length)
            {
                place = new Place(currentLine.Length, line - 1);
            }

            try
            {
                int position = textBox.PlaceToPosition(place);
                textBox.SelectionStart = position;
                textBox.SelectionLength = 0;

                var range = new FastColoredTextBoxNS.Range(textBox, place, place);
                textBox.DoRangeVisible(range, true);

                textBox.Focus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка установки позиции: {ex.Message}");
                try
                {
                    var fallbackPlace = new Place(0, line - 1);
                    textBox.SelectionStart = textBox.PlaceToPosition(fallbackPlace);
                    textBox.SelectionLength = 0;
                    textBox.DoRangeVisible(new FastColoredTextBoxNS.Range(textBox, fallbackPlace, fallbackPlace), true);
                    textBox.Focus();
                }
                catch { }
            }
        }

        private void ddmStartAnalysis_Click(object sender, EventArgs e)
        {
            RegEx();
        }

        private void btnSearchQuick_Click(object sender, EventArgs e)
        {
            RegEx();
        }

        private void RegEx()
        {

            string selectedRegEx = "Китайские почтовые индексы";

            dgvRegular.Rows.Clear();


            if (cmbRegEx.SelectedItem != null)
            {
                selectedRegEx = cmbRegEx.SelectedItem.ToString();
            }

            var textBox = tabControlEditor.SelectedTab.Controls
                .OfType<FastColoredTextBox>()
                .FirstOrDefault();

            if (textBox == null)
                return;


            string code = textBox.Text;

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Нет данных для поиска.");
            }

            IRegEx regEx = new RegularExpressionsFind(code, selectedRegEx);
            List<ISubString> subStrings = regEx.SubStrings();
            foreach (var regular in subStrings)
            {
                int rowIndex = dgvRegular.Rows.Add(
                    regular.RegEx,
                    regular.Position(),
                    regular.Length);

                dgvRegular.Rows[rowIndex].Tag = regular;
            }

            if (subStrings.Count == 0)
            {
                int rowIndex = dgvRegular.Rows.Add(
                    "",
                    "",
                    "Совпадений не найдено.");

                dgvRegular.Rows[rowIndex].Tag = null;
            }

            else if (subStrings.Count > 0)
            {
                int rowIndex = dgvRegular.Rows.Add(
    "",
    "",
    $"Совпадений найдено : {subStrings.Count}.");

                dgvRegular.Rows[rowIndex].Tag = null;
            }
        }

        private void dgvRegular_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvRegular.Rows.Count)
                return;

            var subString = dgvRegular.Rows[e.RowIndex].Tag as ISubString;
            if (subString == null)
                return;

            var textBox = tabControlEditor.SelectedTab.Controls
                .OfType<FastColoredTextBox>()
                .FirstOrDefault();

            if (textBox == null)
                return;

            if (!int.TryParse(subString.Line, out int line))
                return;

            if (!int.TryParse(subString.Column, out int column))
                return;

            var startPlace = new Place(column - 1, line - 1);
            var endPlace = new Place(column - 1 + subString.Length, line - 1);

            int startPosition = textBox.PlaceToPosition(startPlace);
            int endPosition = textBox.PlaceToPosition(endPlace);

            textBox.SelectionStart = startPosition;
            textBox.SelectionLength = endPosition - startPosition;

            textBox.DoSelectionVisible();

            var range = new FastColoredTextBoxNS.Range(textBox, startPlace, endPlace);
            textBox.DoRangeVisible(range, true);

            textBox.Focus();

            textBox.Invalidate();
        }

        private void StateMachineRegEx()
        {
            dgvRegular.Rows.Clear();

            var textBox = tabControlEditor.SelectedTab.Controls
    .OfType<FastColoredTextBox>()
    .FirstOrDefault();

            if (textBox == null)
                return;


            string code = textBox.Text;

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Нет данных для поиска.");
            }

            IRegEx regex = new StateMachine(code);

            List<ISubString> subStrings = regex.SubStrings();

            foreach (var regular in subStrings)
            {
                int rowIndex = dgvRegular.Rows.Add(
                    regular.RegEx,
                    regular.Position(),
                    regular.Length);

                dgvRegular.Rows[rowIndex].Tag = regular;
            }

            if (subStrings.Count == 0)
            {
                int rowIndex = dgvRegular.Rows.Add(
                    "",
                    "",
                    "Совпадений не найдено.");

                dgvRegular.Rows[rowIndex].Tag = null;
            }

            else if (subStrings.Count > 0)
            {
                int rowIndex = dgvRegular.Rows.Add(
    "",
    "",
    $"Совпадений найдено : {subStrings.Count}.");

                dgvRegular.Rows[rowIndex].Tag = null;
            }
        }

        private void FlexBisonParser()
        {
            dgvResults.Rows.Clear();

            var textBox = tabControlEditor.SelectedTab.Controls
                .OfType<FastColoredTextBox>()
                .FirstOrDefault();

            if (textBox == null)
                return;

            string code = textBox.Text;

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show(_localization.Get("EmptyString"));
                return;
            }

            var parser = new FlexBisonParser.FlexBisonParser();
            var tokens = parser.Scan(code);

            if (parser.Errors != null && parser.Errors.Count > 0)
            {
                foreach (var error in parser.Errors)
                {
                    int rowIndex = dgvResults.Rows.Add(
                        error.UnexpectedLexeme ?? "?",
                        error.Location,
                        error.Message
                    );
                    dgvResults.Rows[rowIndex].Tag = error;
                }
                ErrorsCount.Text = $"Количество ошибок: {parser.Errors.Count}";
            }
            else
            {
                int rowIndex = dgvResults.Rows.Add(
                    "",
                    "",
                    "Синтаксический анализ завершён успешно"
                );
                dgvResults.Rows[rowIndex].Tag = null;
                ErrorsCount.Text = "Количество ошибок: 0";
            }

            Debug.WriteLine($"Получено токенов: {tokens.Count}");
            foreach (var token in tokens.Take(10))
            {
            }
        }

        private void btnMission_Click(object sender, EventArgs e)
        {
            CallMission();
        }

        private void CallMission()
        {
            IHTMLHelper htmlHelper = new HTMLHelper(_localization);
            htmlHelper.CallHTML("Mission");
        }

        private void btnGrammar_Click(object sender, EventArgs e)
        {
            CallGrammar();
        }

        private void CallGrammar()
        {
            IHTMLHelper htmlHelper = new HTMLHelper(_localization);
            htmlHelper.CallHTML("Grammar");
        }

        private void btnGrammarClassification_Click(object sender, EventArgs e)
        {
            CallGrammarClassification();
        }

        private void CallGrammarClassification()
        {
            IHTMLHelper htmlHelper = new HTMLHelper(_localization);
            htmlHelper.CallHTML("GrammarClassification");
        }

        private void btnMethodOfAnalysis_Click(object sender, EventArgs e)
        {
            CallMethodOfAnalysis();
        }

        private void CallMethodOfAnalysis()
        {
            IHTMLHelper htmlHelper = new HTMLHelper(_localization);
            htmlHelper.CallHTML("MethodOfAnalysis");
        }

        private void btnTestCase_Click(object sender, EventArgs e)
        {
            CallTestCase();
        }

        private void CallTestCase()
        {
            IHTMLHelper htmlHelper = new HTMLHelper(_localization);
            htmlHelper.CallHTML("TestCase");
        }

        private void btnListOfLiterature_Click(object sender, EventArgs e)
        {
            CallListOfLiterature();
        }

        private void CallListOfLiterature()
        {
            IHTMLHelper htmlHelper = new HTMLHelper(_localization);
            htmlHelper.CallHTML("ListOfLiterature");
        }

        private void btnSourceCode_Click(object sender, EventArgs e)
        {
            CallSourceCode();
        }

        private void CallSourceCode()
        {
            IHTMLHelper htmlHelper = new HTMLHelper(_localization);
            htmlHelper.CallHTML("SourceCode");
        }

        private void btnPOLIZ_Click(object sender, EventArgs e)
        {
            dgvArithLexem.Rows.Clear();

            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            if (textBox == null) return;

            string textToAnalyze = textBox.Text;

            if (string.IsNullOrEmpty(textToAnalyze))
            {
                MessageBox.Show("Введена пустая строка.");
                return;
            }

            IArithmeticExpressionLexer lexer = new ArithmeticExpressionLexer(textToAnalyze);

            List<IArithToken> tokens = lexer.Tokens;

            foreach (IArithToken token in tokens)
            {
                int rowIndex = dgvArithLexem.Rows.Add(
                    token.Word,
                    token.Line,
                    token.Column,
                    token.Type
                    );

                dgvArithLexem.Rows[rowIndex].Tag = token;
            }

            IArithParser parser = new ArithmeticExpressionParser(tokens);

            if (parser.Errors.Count != 0)
            {
                dgvResults.Rows.Clear();

                foreach (var error in parser.Errors)
                {
                    int rowIndex = dgvResults.Rows.Add(
                        error.Token,
                        $"Строка {error.Line}, столбец {error.Column}",
                        error.Message);

                    dgvResults.Rows[rowIndex].Tag = error;
                }
            }
            else
            {
                tokens = parser.Tokens;
                dgvResults.Rows.Clear();

                dgvResults.Rows.Add(
    "",
    "",
    "Синтаксический анализ завершён успешно"
);
                IPOLIZ poliz = new ArithmeticExpressionPOLIZ(tokens);

                tokens = poliz.POLIZ;

                rtbPOLIZ.Text = "";

                foreach (IArithToken token in tokens)
                {
                    rtbPOLIZ.Text += token.Word + " ";
                }

                ITetrads tetradsGenerator = new ArithmeticExpressionTetradsGenerator(tokens);

                List<ArithmeticExpressionTetrads> tetrads = tetradsGenerator.Tetrads;

                dgvTetrads.Rows.Clear();
                foreach (var tetrad in tetrads)
                {
                    int rowIndex = dgvTetrads.Rows.Add(
                        tetrad.Operation,
                        tetrad.OperandLeft,
                        tetrad.OperandRight,
                        tetrad.Variable
                        );
                }
            }
        }
    }
}
