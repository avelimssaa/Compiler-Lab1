using System.Diagnostics;
using System.Text;
using FastColoredTextBoxNS;
using Compiler_Lab1.LexicalAnalyzer;

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
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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
            try
            {
                string helpPath = Path.Combine(Application.StartupPath, _localization.Get("UserManual"));

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
            TabPage currentTab = tabControlEditor.SelectedTab;
            FastColoredTextBox textBox = currentTab.Controls.OfType<FastColoredTextBox>().FirstOrDefault();
            string textToAnalyze = textBox.Text;

            //_analyzer.GetText(textToAnalyze);
            List<IToken> tokens = new List<IToken>();
            tokens = _analyzer.Scan(textToAnalyze);
            DisplayTokensInDataGridView(tokens);
        }

        private void DisplayTokensInDataGridView(List<IToken> tokens)
        {
            dgvResults.Rows.Clear();

            foreach (Token token in tokens)
            {
                dgvResults.Rows.Add(
                    token.GetConditionCode(),
                    token.GetTokenType(),
                    token.GetLexeme(),
                    token.GetLocation()
                    //token.IsError() ? "Ошибка" : "",
                    //token.GetMessageDescription()
                );
            }

            //foreach (DataGridViewRow row in dgvResults.Rows)
            //{
            //    if (row.Cells[4].Value?.ToString() == "Ошибка")
            //    {
            //        row.DefaultCellStyle.BackColor = Color.LightCoral;
            //        row.DefaultCellStyle.ForeColor = Color.White;
            //    }
            //}
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Compile();
        }

        private void btnStartQuick_Click(object sender, EventArgs e)
        {
            Compile();
        }
    }
}
