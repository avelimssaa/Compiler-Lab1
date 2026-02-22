using System.Diagnostics;
using System.Text;

namespace Compiler_Lab1
{
    public partial class textEditor : Form
    {
        //  private string currentFilePath = null;
        // private string currentFileName = "Без имени.txt";
        //  private bool isTextChanged = false;


        private Stack<string> undoStack = new Stack<string>();
        private Stack<string> redoStack = new Stack<string>();
        private bool isUndoRedoOperation = false;

        private Dictionary<TabPage, TabFileInfo> tabFileInfo = new Dictionary<TabPage, TabFileInfo>();

        private int pagesCount;

        public textEditor()
        {

            InitializeComponent();
            pagesCount = 0;
            CreateFile();
            //mainText.TextChanged += (s, e) =>
            //{
            //    isTextChanged = true;
            //    UpdateTitle();

            //    if (!isUndoRedoOperation)
            //    {
            //        undoStack.Push(mainText.Text);
            //        redoStack.Clear();
            //    }
            //};
        }

        private void UpdateTitle()
        {
            //string mark = isTextChanged ? "*" : "";
            //this.Text = $"Текстовый редактор - {currentFileName}{mark}";
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

            tabControlEditor.TabPages.Add(newTab);
            tabControlEditor.SelectedTab = newTab;

            tabFileInfo[newTab] = fileInfo;

            // 
            // tabPage1
            // 
            //tabPage1.Controls.Add(richTextBox1);
            // tabPage1.Location = new Point(4, 29);
            //tabPage1.Name = "tabPage1";
            //tabPage1.Padding = new Padding(3);
            //tabPage1.Size = new Size(774, 209);
            //tabPage1.TabIndex = 0;
            //tabPage1.Text = "tabPage1";
            //tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            //richTextBox1.Dock = DockStyle.Fill;
            // richTextBox1.Location = new Point(3, 3);
            //richTextBox1.Name = "richTextBox1";
            //richTextBox1.Size = new Size(768, 203);
            //richTextBox1.TabIndex = 0;
            //richTextBox1.Text = "";

            //UpdateTitle();

            //if (!FileSaveConfirm("Создать файл"))
            //{
            //    return;
            //}

            //mainText.Clear();
            //currentFilePath = null;
            //currentFileName = "Без имени.txt";
            //isTextChanged = false;
            //UpdateTitle();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            RichTextBox textBox = sender as RichTextBox;
            if (textBox == null) return;

            TabPage parentTab = textBox.Parent as TabPage;
            if (parentTab != null && tabFileInfo.ContainsKey(parentTab))
            {
                tabFileInfo[parentTab].IsChanged = true;

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
            //using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            //{
            //    saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            //    saveFileDialog.FilterIndex = 1;
            //    saveFileDialog.DefaultExt = "txt";
            //    saveFileDialog.AddExtension = true;

            //    saveFileDialog.FileName = currentFileName;

            //    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //    if (saveFileDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        SaveFileToPath(saveFileDialog.FileName);

            //        currentFilePath = saveFileDialog.FileName;
            //        currentFileName = Path.GetFileName(currentFilePath);
            //        UpdateTitle();
            //    }
            //}

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
            //string filePath = tabFileInfo[tabControlEditor.SelectedTab].FilePath;
            //if (string.IsNullOrEmpty(filePath))
            //{
            //    SaveAs();
            //}
            //else
            //{
            //    SaveFileToPath(filePath);
            //}

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
            //try
            //{
            //    string textToSave = mainText.Text;

            //    File.WriteAllText(filePath, textToSave, Encoding.UTF8);

            //    isTextChanged = false;
            //    UpdateTitle();

            //    MessageBox.Show($"[{DateTime.Now:HH:mm:ss}] Файл сохранен: {Path.GetFileName(filePath)}\r\n");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(
            //        $"Неожиданная ошибка при сохранении:\n{ex.Message}",
            //        "Ошибка",
            //        MessageBoxButtons.OK,
            //        MessageBoxIcon.Error
            //    );
            //}

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
                }
                currentTab.Text = Path.GetFileName(filePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Неожиданная ошибка при сохранении:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

//        private bool FileSaveConfirm(string action)
//        {
//            DialogResult actionConfirm = MessageBox.Show(
//$"Вы уверены, что хотите выполнить следующее действие: {action}?",
//action,
//MessageBoxButtons.YesNo,
//MessageBoxIcon.Question
//);
//            if (actionConfirm == DialogResult.No)
//            {
//                return false;
//            }

//            DialogResult result = MessageBox.Show(
//            "Сохранить изменения в текущем файле?",
//            action,
//            MessageBoxButtons.YesNo,
//            MessageBoxIcon.Question
//        );

//            if (result == DialogResult.Yes)
//            {
//                Save();
//            }
//            return true;
//        }

        private void OpenFile()
        {
            //if (!FileSaveConfirm("Открыть файл"))
            //{
            //    return;
            //}

            string fileContent = "", currentFilePath = null, currentFileName = "Без имени.txt";
            bool isTextChanged;

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
            //if (undoStack.Count > 0)
            //{
            //    isUndoRedoOperation = true;

            //    redoStack.Push(mainText.Text);

            //    string previousText = undoStack.Pop();
            //    mainText.Text = previousText;

            //    isUndoRedoOperation = false;
            //    isTextChanged = true;
            //    UpdateTitle();
            //}
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void Redo()
        {
            //if (redoStack.Count > 0)
            //{
            //    isUndoRedoOperation = true;

            //    undoStack.Push(mainText.Text);

            //    string nextText = redoStack.Pop();
            //    mainText.Text = nextText;

            //    isUndoRedoOperation = false;
            //    isTextChanged = true;
            //    UpdateTitle();
            //}
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void Cut()
        {
            //if (!string.IsNullOrEmpty(mainText.SelectedText))
            //{
            //    mainText.Cut();
            //}
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void Copy()
        {
            //if (!string.IsNullOrEmpty(mainText.SelectedText))
            //{
            //    mainText.Copy();
            //}
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void Paste()
        {
            //try
            //{
            //    if (Clipboard.ContainsText())
            //    {
            //        int selectionStart = mainText.SelectionStart;
            //        mainText.Text = mainText.Text.Insert(selectionStart, Clipboard.GetText());
            //        mainText.SelectionStart = selectionStart + Clipboard.GetText().Length;

            //        isTextChanged = true;
            //        UpdateTitle();

            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка при вставке: {ex.Message}", "Ошибка",
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void Delete()
        {
            //try
            //{
            //    if (mainText.SelectionLength > 0)
            //    {
            //        int selectionStart = mainText.SelectionStart;
            //        mainText.Cut();
            //        mainText.SelectionStart = selectionStart;
            //        isTextChanged = true;
            //        UpdateTitle();
            //    }
            //    else
            //    {
            //        if (mainText.SelectionStart > 0)
            //        {
            //            int cursorPos = mainText.SelectionStart;
            //            mainText.Text = mainText.Text.Remove(cursorPos - 1, 1);
            //            mainText.SelectionStart = cursorPos - 1;

            //            isTextChanged = true;
            //            UpdateTitle();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void SelectAll()
        {
            //if (!string.IsNullOrEmpty(mainText.Text))
            //{
            //    mainText.SelectAll();
            //}
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
            //if (mainText != null)
            //{
            //    mainText.Font = new Font(mainText.Font.FontFamily, size);
            //}

            //if (outputText != null)
            //{
            //    outputText.Font = new Font(outputText.Font.FontFamily, size);
            //}
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
    }
}
