using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Compiler_Lab1
{
    public partial class textEditor : Form
    {
        private bool _isUndoRedoOperation = false;

        private Dictionary<TabPage, TabFileInfo> _tabFileInfo = new Dictionary<TabPage, TabFileInfo>();

        private int _pagesCount;

        private ILocalization _localization;

        private CancellationTokenSource _scanningCTS;
        private Task _scanningTask;
        private ConcurrentDictionary<RichTextBox, string> _textSnapshots = new ConcurrentDictionary<RichTextBox, string>();
        private ConcurrentQueue<RichTextBox> _scanQueue = new ConcurrentQueue<RichTextBox>();
        private bool _isScanningActive = false;
        private readonly object _scanLock = new object();

        public textEditor()
        {
            InitializeComponent();
            _localization = new Localization();
            _pagesCount = 0;
            CreateFile();

            StartBackgroundScanning();
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

            RichTextBox textBox = new RichTextBox();
            textBox.Dock = DockStyle.Fill;
            textBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            textBox.TextChanged += TextBox_TextChanged;
            textBox.Name = $"textbox{_pagesCount}";
            textBox.TabIndex = newTab.TabIndex;
            textBox.Text = "";
            newTab.Controls.Add(textBox);

            Panel linePanel = new Panel();
            linePanel.Location = new Point(0, 0);
            linePanel.Size = new Size(57, 209);
            linePanel.Dock = DockStyle.Left;
            linePanel.BackColor = Color.FromArgb(240, 240, 240);

            newTab.Controls.Add(linePanel);

            linePanel.Paint += (s, e) => DrawLineNumbers(e, textBox, linePanel);






            _textSnapshots[textBox] = "";
            EnableLineNumbering(textBox, linePanel);

            TabFileInfo fileInfo = new TabFileInfo();

            fileInfo.FilePath = null;
            fileInfo.FileName = newTab.Name;
            fileInfo.IsChanged = false;
            fileInfo.PushUndoStack("");

 
            tabControlEditor.SelectedTab = newTab;

            _tabFileInfo[newTab] = fileInfo;
        }

        private void StartBackgroundScanning()
        {
            _scanningCTS = new CancellationTokenSource();
            _scanningTask = Task.Run(() => BackgroundScanningLoop(_scanningCTS.Token));
        }

        private async Task BackgroundScanningLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await CollectTextBoxesForScanning(token);

                    await Task.Delay(200, token);

                    await ProcessScanningQueue(token);

                    await Task.Delay(600, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    await Task.Delay(5000, token);
                }
            }
        }

        private async Task CollectTextBoxesForScanning(CancellationToken token)
        {
            if (_isScanningActive) return;

            lock (_scanLock)
            {
                if (_isScanningActive) return;
                _isScanningActive = true;
            }

            try
            {
                await Task.Run(() =>
                {
                    this.Invoke(new Action(() =>
                    {
                        if (token.IsCancellationRequested) return;

                        foreach (TabPage tab in tabControlEditor.TabPages)
                        {
                            if (token.IsCancellationRequested) return;

                            RichTextBox textBox = tab.Controls.OfType<RichTextBox>().FirstOrDefault();
                            if (textBox != null)
                            {
                                string currentText = textBox.Text;

                                if (_textSnapshots.TryGetValue(textBox, out string oldText))
                                {
                                    if (oldText != currentText)
                                    {
                                        _scanQueue.Enqueue(textBox);
                                        _textSnapshots[textBox] = currentText;
                                    }
                                }
                                else
                                {
                                    _textSnapshots[textBox] = currentText;
                                    _scanQueue.Enqueue(textBox);
                                }
                            }
                        }
                    }));
                }, token);
            }
            finally
            {
                lock (_scanLock)
                {
                    _isScanningActive = false;
                }
            }
        }

        private async Task ProcessScanningQueue(CancellationToken token)
        {
            var lastRequests = new Dictionary<RichTextBox, string>();

            while (_scanQueue.TryDequeue(out RichTextBox textBox))
            {
                if (token.IsCancellationRequested) break;
                if (textBox == null || textBox.IsDisposed) continue;

                if (_textSnapshots.TryGetValue(textBox, out string text))
                {
                    lastRequests[textBox] = text;
                }
            }

            foreach (var request in lastRequests)
            {
                if (token.IsCancellationRequested) break;

                var textBox = request.Key;
                var text = request.Value;

                if (textBox.IsDisposed) continue;

                await Task.Delay(50, token);

                var highlights = await Task.Run(() => AnalyzeTextForHighlighting(text), token);

                if (!token.IsCancellationRequested && !textBox.IsDisposed)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (token.IsCancellationRequested || textBox.IsDisposed) return;
                        ApplyHighlighting(textBox, highlights);
                    }));
                }
            }
        }

        private List<HighlightInfo> AnalyzeTextForHighlighting(string text)
        {
            var highlights = new List<HighlightInfo>();
            if (string.IsNullOrEmpty(text)) return highlights;

            var keywords = new HashSet<string>
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
        "char", "checked", "class", "const", "continue", "decimal", "default",
        "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
        "implicit", "in", "int", "interface", "internal", "is", "lock", "long",
        "namespace", "new", "null", "object", "operator", "out", "override",
        "params", "private", "protected", "public", "readonly", "ref", "return",
        "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
        "struct", "switch", "this", "throw", "true", "try", "typeof", "uint",
        "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void",
        "volatile", "while"
    };

            var types = new HashSet<string>
    {
        "bool", "byte", "char", "decimal", "double", "float", "int", "long",
        "object", "sbyte", "short", "string", "uint", "ulong", "ushort", "void"
    };

            int index = 0;
            while (index < text.Length)
            {
                if (char.IsLetter(text[index]) || text[index] == '_')
                {
                    int start = index;
                    while (index < text.Length && (char.IsLetterOrDigit(text[index]) || text[index] == '_'))
                        index++;

                    string word = text.Substring(start, index - start);

                    if (keywords.Contains(word))
                    {
                        highlights.Add(new HighlightInfo
                        {
                            Start = start,
                            Length = word.Length,
                            Color = types.Contains(word) ? Color.Blue : Color.FromArgb(0, 0, 139),
                            Style = FontStyle.Bold
                        });
                    }
                }
                else if (text[index] == '"')
                {
                    int start = index;
                    index++;
                    while (index < text.Length && text[index] != '"')
                    {
                        if (text[index] == '\\' && index + 1 < text.Length)
                            index += 2;
                        else
                            index++;
                    }
                    if (index < text.Length) index++;

                    highlights.Add(new HighlightInfo
                    {
                        Start = start,
                        Length = index - start,
                        Color = Color.Brown,
                        Style = FontStyle.Regular
                    });
                }
                else if (text[index] == '/' && index + 1 < text.Length)
                {
                    if (text[index + 1] == '/')
                    {
                        int start = index;
                        index += 2;
                        while (index < text.Length && text[index] != '\n')
                            index++;

                        highlights.Add(new HighlightInfo
                        {
                            Start = start,
                            Length = index - start,
                            Color = Color.Green,
                            Style = FontStyle.Italic
                        });
                    }
                    else if (text[index + 1] == '*')
                    {
                        int start = index;
                        index += 2;
                        while (index < text.Length && !(text[index] == '*' && index + 1 < text.Length && text[index + 1] == '/'))
                            index++;
                        if (index < text.Length) index += 2;

                        highlights.Add(new HighlightInfo
                        {
                            Start = start,
                            Length = index - start,
                            Color = Color.Green,
                            Style = FontStyle.Italic
                        });
                    }
                    else
                    {
                        index++;
                    }
                }
                else
                {
                    index++;
                }
            }

            return highlights;
        }

        private void ApplyHighlighting(RichTextBox textBox, List<HighlightInfo> highlights)
        {
            if (textBox == null || textBox.IsDisposed) return;

            try
            {
                int selectionStart = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;

                textBox.SuspendLayout();

                textBox.SelectAll();
                textBox.SelectionColor = Color.Black;
                textBox.SelectionFont = new Font(textBox.Font, FontStyle.Regular);

                foreach (var highlight in highlights)
                {
                    if (highlight.Start >= 0 && highlight.Start + highlight.Length <= textBox.Text.Length)
                    {
                        textBox.Select(highlight.Start, highlight.Length);
                        textBox.SelectionColor = highlight.Color;
                        if (highlight.Style != FontStyle.Regular)
                        {
                            textBox.SelectionFont = new Font(textBox.Font, highlight.Style);
                        }
                    }
                }

                if (selectionStart <= textBox.Text.Length)
                {
                    textBox.Select(selectionStart, selectionLength);
                }

                textBox.ResumeLayout();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка применения подсветки: {ex.Message}");
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            RichTextBox textBox = sender as RichTextBox;
            if (textBox == null) return;

            TabPage parentTab = textBox.Parent as TabPage;
            if (parentTab != null && _tabFileInfo.ContainsKey(parentTab))
            {
                TabFileInfo fileInfo = _tabFileInfo[parentTab];

                if (!_isUndoRedoOperation)
                {
                    fileInfo.PushUndoStack(textBox.Text);
                    fileInfo.ClearRedoStack();
                    fileInfo.UndoStackLimitation();
                }

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
                saveFileDialog.FileName = fileInfo.FileName ?? "Без имени.txt";
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

            RichTextBox textBox = currentTab.Controls.OfType<RichTextBox>().FirstOrDefault();
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

                    _tabFileInfo[currentTab].ClearUndoStack();
                    _tabFileInfo[currentTab].ClearRedoStack();
                    _tabFileInfo[currentTab].PushUndoStack(textBox.Text);
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

            RichTextBox textBox = new RichTextBox();

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

            Panel linePanel = new Panel();
            linePanel.Location = new Point(0, 0);
            linePanel.Size = new Size(57, 209);
            linePanel.Dock = DockStyle.Left;
            linePanel.BackColor = Color.FromArgb(240, 240, 240);
            linePanel.Paint += (s, e) => DrawLineNumbers(e, textBox, linePanel);
            newTab.Controls.Add(linePanel);

            EnableLineNumbering(textBox, linePanel);

            TabFileInfo fileInfo = new TabFileInfo();

            fileInfo.FilePath = currentFilePath;
            fileInfo.FileName = newTab.Name;
            fileInfo.IsChanged = false;
            fileInfo.PushUndoStack(fileContent);

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
            _scanningCTS?.Cancel();

            try
            {
                if (_scanningTask != null)
                {
                    _scanningTask.Wait(1000); 
                }
            }
            catch (AggregateException)
            {
               
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

            RichTextBox textBox = currentTab.Controls.OfType<RichTextBox>().FirstOrDefault();
            if (textBox == null) return;

            var fileInfo = _tabFileInfo[currentTab];

            if (fileInfo.GetUndoStackCount() > 0)
            {
                _isUndoRedoOperation = true;

                fileInfo.PushRedoStack(textBox.Text);

                string previousText = fileInfo.PopUndoStack();
                textBox.Text = previousText;

                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;

                _isUndoRedoOperation = false;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void Redo()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            RichTextBox textBox = currentTab.Controls.OfType<RichTextBox>().FirstOrDefault();
            if (textBox == null) return;

            var fileInfo = _tabFileInfo[currentTab];

            if (fileInfo.GetRedoStackCount() > 0)
            {
                _isUndoRedoOperation = true;

                fileInfo.PushUndoStack(textBox.Text);

                string nextText = fileInfo.PopRedoStack();
                textBox.Text = nextText;

                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;

                _isUndoRedoOperation = false;

                fileInfo.IsChanged = true;
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void Cut()
        {
            TabPage currentTab = tabControlEditor.SelectedTab;
            if (currentTab == null) return;

            RichTextBox textBox = currentTab.Controls.OfType<RichTextBox>().FirstOrDefault();
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

            RichTextBox textBox = currentTab.Controls.OfType<RichTextBox>().FirstOrDefault();
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

            RichTextBox textBox = currentTab.Controls.OfType<RichTextBox>().FirstOrDefault();
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

            RichTextBox textBox = currentTab.Controls.OfType<RichTextBox>().FirstOrDefault();
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

            RichTextBox textBox = currentTab.Controls.OfType<RichTextBox>().FirstOrDefault();
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
                RichTextBox textBox = tab.Controls.OfType<RichTextBox>().FirstOrDefault();
                if (textBox != null)
                {
                    FontStyle currentStyle = textBox.Font.Style;
                    string fontFamily = textBox.Font.FontFamily.Name;

                    textBox.Font = new Font(fontFamily, size, currentStyle);
                }
            }

            if (dgvOutput != null)
            {
                FontStyle outputStyle = dgvOutput.Font.Style;
                string outputFontFamily = dgvOutput.Font.FontFamily.Name;
                dgvOutput.Font = new Font(outputFontFamily, size, outputStyle);
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

        private void EnableLineNumbering(RichTextBox textBox, Panel linePanel)
        {
            textBox.TextChanged += (s, e) => linePanel.Invalidate();
            textBox.VScroll += (s, e) => linePanel.Invalidate();
            textBox.FontChanged += (s, e) => linePanel.Invalidate();
        }

        private void DrawLineNumbers(PaintEventArgs e, RichTextBox textBox, Panel panel)
        {
            e.Graphics.Clear(panel.BackColor);

            using (Pen pen = new Pen(Color.Gray))
            {
                e.Graphics.DrawLine(pen, panel.Width - 1, 0, panel.Width - 1, panel.Height);
            }

            if (textBox == null || textBox.Lines.Length == 0) return;

            Point firstVisiblePos = textBox.GetPositionFromCharIndex(0);

            for (int i = 0; i < textBox.Lines.Length; i++)
            {
                int charIndex = textBox.GetFirstCharIndexFromLine(i);
                if (charIndex < 0) continue;

                Point linePos = textBox.GetPositionFromCharIndex(charIndex);

                if (linePos.Y >= 0 && linePos.Y < textBox.Height)
                {
                    Rectangle rect = new Rectangle(0, linePos.Y, panel.Width - 2, textBox.Font.Height);

                    using (StringFormat format = new StringFormat())
                    {
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Center;

                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
                        {
                            e.Graphics.DrawString((i + 1).ToString(), textBox.Font, brush, rect, format);
                        }
                    }
                }
            }
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
                RichTextBox textBox = new RichTextBox();

                newTab.Controls.Add(textBox);
                newTab.Name = fileName;
                newTab.Text = fileName;
                newTab.Padding = new Padding(3);

                textBox.Dock = DockStyle.Right;
                textBox.Location = new Point(63, 0);
                textBox.Size = new Size(711, 209);
                textBox.TextChanged += TextBox_TextChanged;
                textBox.Text = fileContent;

                Panel linePanel = new Panel();
                linePanel.Location = new Point(0, 0);
                linePanel.Size = new Size(57, 209);
                linePanel.Dock = DockStyle.Left;
                linePanel.BackColor = Color.FromArgb(240, 240, 240);
                linePanel.Paint += (s, e) => DrawLineNumbers(e, textBox, linePanel);
                newTab.Controls.Add(linePanel);

                _textSnapshots[textBox] = fileContent;

                EnableLineNumbering(textBox, linePanel);

                TabFileInfo fileInfo = new TabFileInfo
                {
                    FilePath = filePath,
                    FileName = fileName,
                    IsChanged = false
                };
                fileInfo.PushUndoStack(fileContent);

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

            RichTextBox textBox = tab.Controls.OfType<RichTextBox>().FirstOrDefault();
            if (textBox == null) return;

            var fileInfo = _tabFileInfo[tab];

            string language = _localization.CurrentLanguage == "ru" ? "Русский" : "English";
            labelLanguage.Text = $"{_localization.Get("Language")}: {language}";

            int charCount = textBox.Text.Length;
            string fileSize = GetFileSizeString(charCount);
            labelFileSize.Text = $"{_localization.Get("FileSize")}: {fileSize}";

            int lineCount = textBox.Lines.Length;
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

        private async void Compile()
        {
            tabControlResults.SelectedTab = tabPageResults;
            rtbResults.Text = "";
            rtbResults.AppendText(_localization.Get("Compile"));
            rtbResults.AppendText("\n");
            await Task.Delay(2000);
            rtbResults.AppendText(_localization.Get("Ready"));
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
