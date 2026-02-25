using System.Diagnostics;
using System.Text;

namespace Compiler_Lab1
{
    public partial class textEditor : Form
    {
        private bool isUndoRedoOperation = false;

        private Dictionary<TabPage, TabFileInfo> tabFileInfo = new Dictionary<TabPage, TabFileInfo>();

        private int pagesCount;

        private ILocalization _localization;

        public textEditor()
        {
            InitializeComponent();
            _localization = new Localization();
            pagesCount = 0;
            CreateFile();
        }

        private void CreateFile()
        {
            pagesCount++;

            TabPage newTab = new TabPage();

            RichTextBox textBox = new RichTextBox();

            newTab.Controls.Add(textBox);
            newTab.Location = new Point(4, 29);
            newTab.Name = "Без имени" + $"{pagesCount}.txt";
            newTab.Padding = new Padding(3);
            newTab.Size = new Size(774, 209);
            newTab.TabIndex = pagesCount - 1;
            newTab.Text = newTab.Name;
            newTab.UseVisualStyleBackColor = true;


            textBox.Dock = DockStyle.Fill;
            textBox.Location = new Point(3, 3);
            textBox.TextChanged += TextBox_TextChanged;
            textBox.Name = $"textbox{pagesCount}";
            textBox.Size = new Size(768, 203);
            textBox.TabIndex = newTab.TabIndex;
            textBox.Text = "";

            TabFileInfo fileInfo = new TabFileInfo();

            fileInfo.FilePath = null;
            fileInfo.FileName = newTab.Name;
            fileInfo.IsChanged = false;
            fileInfo.PushUndoStack("");

            tabControlEditor.TabPages.Add(newTab);
            tabControlEditor.SelectedTab = newTab;

            tabFileInfo[newTab] = fileInfo;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            RichTextBox textBox = sender as RichTextBox;
            if (textBox == null) return;

            TabPage parentTab = textBox.Parent as TabPage;
            if (parentTab != null && tabFileInfo.ContainsKey(parentTab))
            {
                TabFileInfo fileInfo = tabFileInfo[parentTab];

                if (!isUndoRedoOperation)
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

            TabFileInfo fileInfo = tabFileInfo[currentTab];

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
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

            string filePath = tabFileInfo[currentTab].FilePath;

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

                if (tabFileInfo.ContainsKey(currentTab))
                {
                    tabFileInfo[currentTab].FilePath = filePath;
                    tabFileInfo[currentTab].FileName = Path.GetFileName(filePath);
                    tabFileInfo[currentTab].IsChanged = false;

                    tabFileInfo[currentTab].ClearUndoStack();
                    tabFileInfo[currentTab].ClearRedoStack();
                    tabFileInfo[currentTab].PushUndoStack(textBox.Text);
                }
                currentTab.Text = Path.GetFileName(filePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при сохранении:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void OpenFile()
        {
            string fileContent = "", currentFilePath = null, currentFileName = "Без имени.txt";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
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
                        MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            pagesCount++;

            TabPage newTab = new TabPage();

            RichTextBox textBox = new RichTextBox();

            newTab.Controls.Add(textBox);
            newTab.Location = new Point(4, 29);
            newTab.Name = currentFileName;
            newTab.Padding = new Padding(3);
            newTab.Size = new Size(774, 209);
            newTab.TabIndex = pagesCount - 1;
            newTab.Text = newTab.Name;
            newTab.UseVisualStyleBackColor = true;


            textBox.Dock = DockStyle.Fill;
            textBox.Location = new Point(3, 3);
            textBox.TextChanged += TextBox_TextChanged;
            textBox.Name = $"textbox{pagesCount}";
            textBox.Size = new Size(768, 203);
            textBox.TabIndex = newTab.TabIndex;
            textBox.Text = fileContent;

            TabFileInfo fileInfo = new TabFileInfo();

            fileInfo.FilePath = currentFilePath;
            fileInfo.FileName = newTab.Name;
            fileInfo.IsChanged = false;
            fileInfo.PushUndoStack(fileContent);

            tabControlEditor.TabPages.Add(newTab);
            tabControlEditor.SelectedTab = newTab;

            tabFileInfo[newTab] = fileInfo;
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
                if (tabFileInfo[tab].IsChanged)
                {
                    tabControlEditor.SelectedTab = tab;

                    DialogResult result = MessageBox.Show(
                        $"Сохранить изменения в файле \"{tabFileInfo[tab].FileName}\"?",
                        "Выход из программы",
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

            RichTextBox textBox = currentTab.Controls.OfType<RichTextBox>().FirstOrDefault();
            if (textBox == null) return;

            var fileInfo = tabFileInfo[currentTab];

            if (fileInfo.GetUndoStackCount() > 0)
            {
                isUndoRedoOperation = true;

                fileInfo.PushRedoStack(textBox.Text);

                string previousText = fileInfo.PopUndoStack();
                textBox.Text = previousText;

                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;

                isUndoRedoOperation = false;
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

            var fileInfo = tabFileInfo[currentTab];

            if (fileInfo.GetRedoStackCount() > 0)
            {
                isUndoRedoOperation = true;

                fileInfo.PushUndoStack(textBox.Text);

                string nextText = fileInfo.PopRedoStack();
                textBox.Text = nextText;

                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;

                isUndoRedoOperation = false;

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
                string helpPath = Path.Combine(Application.StartupPath, "Help.html");

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
                    MessageBox.Show("Файл справки не найден.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия справки: {ex.Message}", "Ошибка",
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

                if (tabFileInfo[currentTab].IsChanged)
                {
                    DialogResult result = MessageBox.Show(
                        $"Сохранить изменения в файле \"{tabFileInfo[currentTab].FileName}\"?",
                        "Закрытие вкладки",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                        Save();
                    else if (result == DialogResult.Cancel)
                        return;
                }

                tabControlEditor.TabPages.Remove(currentTab);
                tabFileInfo.Remove(currentTab);

                pagesCount--;

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
            UpdateUILanguage();
        }

        private void UpdateUILanguage()
        {
            Text = _localization.Get("Editor");

            UpdateToolStripDown();

            //UpdateToolbarTexts();

            //UpdateTabTitles();
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


        }

        private void UpdateToolbarTexts()
        {

        }

        private void UpdateTabTitles()
        {

        }
    }
}
