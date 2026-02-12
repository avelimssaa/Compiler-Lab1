using System.Text;

namespace Compiler_Lab1
{
    public partial class textEditor : Form
    {
        private string currentFilePath = null;
        private string currentFileName = "Без имени.txt";
        private bool isTextChanged = false;

        public textEditor()
        {
            InitializeComponent();

            mainText.TextChanged += (s, e) =>
            {
                isTextChanged = true;
                UpdateTitle();
            };
        }

        private void UpdateTitle()
        {
            string mark = isTextChanged ? "*" : "";
            this.Text = $"Текстовый редактор - {currentFileName}{mark}";
        }

        private void createFile_Click(object sender, EventArgs e)
        {
            if (!FileSaveConfirm("Создать файл"))
            {
                return;
            }

            mainText.Clear();
            currentFilePath = null;
            currentFileName = "Без имени.txt";
            isTextChanged = false;
            UpdateTitle();

            MessageBox.Show($"[{DateTime.Now:HH:mm:ss}] Создан новый документ\r\n");
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
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.AddExtension = true;

                saveFileDialog.FileName = currentFileName;

                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveFileToPath(saveFileDialog.FileName);

                    currentFilePath = saveFileDialog.FileName;
                    currentFileName = Path.GetFileName(currentFilePath);
                    UpdateTitle();
                }
            }
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAs();
            }
            else
            {
                SaveFileToPath(currentFilePath);
            }
        }

        private void SaveFileToPath(string filePath)
        {
            try
            {
                string textToSave = mainText.Text;

                File.WriteAllText(filePath, textToSave, Encoding.UTF8);

                isTextChanged = false;
                UpdateTitle();

                MessageBox.Show($"[{DateTime.Now:HH:mm:ss}] Файл сохранен: {Path.GetFileName(filePath)}\r\n");
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

        private bool FileSaveConfirm(string action)
        {
            DialogResult actionConfirm = MessageBox.Show(
$"Вы уверены, что хотите выполнить следующее действие: {action}?",
action,
MessageBoxButtons.YesNo,
MessageBoxIcon.Question
);
            if (actionConfirm == DialogResult.No)
            {
                return false;
            }

            DialogResult result = MessageBox.Show(
            "Сохранить изменения в текущем файле?",
            action,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

            if (result == DialogResult.Yes)
            {
                Save();
            }
            return true;
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {

            if (!FileSaveConfirm("Открыть файл"))
            {
                return;
            }

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
                        string fileContent = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);
                        mainText.Text = fileContent;

                        currentFilePath = openFileDialog.FileName;
                        currentFileName = Path.GetFileName(currentFilePath);
                        isTextChanged = false;
                        UpdateTitle();

                        MessageBox.Show($"[{DateTime.Now:HH:mm:ss}] Открыт файл: {currentFileName}\r\n");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            if (!FileSaveConfirm("Выйти"))
            {
                return;
            }

            Application.Exit();
        }
    }
}
