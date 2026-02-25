namespace Compiler_Lab1
{
    public interface ILocalization
    {
        string Get(string key);
        string CurrentLanguage { get; set; }
    }

    public class Localization : ILocalization
    {
        private string _currentLanguage;
        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set { _currentLanguage = value; }
        }

        private Dictionary<string, Dictionary<string, string>> translations =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["ru"] = new Dictionary<string, string>
                {
                    ["Editor"] = "Текстовый редактор",

                    ["File"] = "Файл",
                    ["Create"] = "Создать",
                    ["Open"] = "Открыть",
                    ["Save"] = "Сохранить",
                    ["SaveAs"] = "Сохранить как",
                    ["CloseTab"] = "Закрыть вкладку",
                    ["Exit"] = "Выход",
                    ["Edit"] = "Правка",
                    ["Undo"] = "Отменить",
                    ["Redo"] = "Вернуть",
                    ["Cut"] = "Вырезать",
                    ["Copy"] = "Копировать",
                    ["Paste"] = "Вставить",
                    ["Delete"] = "Удалить",
                    ["SelectAll"] = "Выделить все",

                    ["Text"] = "Text",
                    ["MissionStatement"] = "Постановка задачи",
                    ["Grammar"] = "Грамматика",
                    ["Grammar classification"] = "Классификаия грамматики",
                    ["Method of analysis"] = "Метод анализа",
                    ["Test case"] = "Тестовый пример",
                    ["List of literature"] = "Список литературы",
                    ["Source code"] = "Исходный код программы",

                    ["Start"] = "Пуск",

                    ["Certificate"] = "Справка",
                    ["Help"] = "Руководство пользователя",
                    ["About"] = "О программе",

                    ["View"] = "Вид",
                    ["Help font"] = "Выберите размер шрифта",
                    ["Font size"] = "Размер шрифта",
                    ["Choose local"] = "Выберите локализацию",

                    ["Language"] = "Язык",

                    ["SaveChanges"] = "Сохранить изменения в файле",
                    ["ExitProgram"] = "Выход из программы",
                    ["CloseTabTitle"] = "Закрытие вкладки",
                    ["UnsavedChanges"] = "Несохраненные изменения",
                    ["Confirm"] = "Подтверждение",
                    ["Error"] = "Ошибка",
                    ["Warning"] = "Предупреждение",
                    ["Info"] = "Информация",

                    ["NoTextToDelete"] = "Нет текста для удаления",
                    ["NoTextSelected"] = "Нет выделенного текста",
                    ["ClipboardEmpty"] = "Буфер обмена пуст",
                    ["FileNotFound"] = "Файл не найден",
                    ["SaveSuccess"] = "Файл успешно сохранен",
                    ["OpenError"] = "Ошибка при открытии файла",
                    ["SaveError"] = "Ошибка при сохранении файла",

                    ["Untitled"] = "Без имени",

                    ["TextFiles"] = "Текстовые файлы",
                    ["AllFiles"] = "Все файлы"
                },

                ["en"] = new Dictionary<string, string>
                {
                    ["Editor"] = "Text editor",

                    ["File"] = "File",
                    ["Create"] = "New",
                    ["Open"] = "Open",
                    ["Save"] = "Save",
                    ["SaveAs"] = "Save As",
                    ["CloseTab"] = "Close Tab",
                    ["Exit"] = "Exit",

                    ["Edit"] = "Edit",
                    ["Undo"] = "Undo",
                    ["Redo"] = "Redo",
                    ["Cut"] = "Cut",
                    ["Copy"] = "Copy",
                    ["Paste"] = "Paste",
                    ["Delete"] = "Delete",
                    ["SelectAll"] = "Select All",

                    ["Text"] = "Text",
                    ["MissionStatement"] = "Mission statement",
                    ["Grammar"] = "Grammar",
                    ["Grammar classification"] = "Grammar classification",
                    ["Method of analysis"] = "Method of analysis",
                    ["Test case"] = "Test case",
                    ["List of literature"] = "List of literature",
                    ["Source code"] = "The source code of the program",

                    ["Start"] = "Start",

                    ["Certificate"] = "Certificate",
                    ["Help"] = "Help",
                    ["About"] = "About",

                    ["View"] = "View",
                    ["Help font"] = "Choose the font size",
                    ["Font size"] = "Font size",
                    ["Choose local"] = "Choose a localization",

                    ["Language"] = "Language",

                    ["SaveChanges"] = "Save changes in file",
                    ["ExitProgram"] = "Exit Program",
                    ["CloseTabTitle"] = "Close Tab",
                    ["UnsavedChanges"] = "Unsaved changes",
                    ["Confirm"] = "Confirm",
                    ["Error"] = "Error",
                    ["Warning"] = "Warning",
                    ["Info"] = "Information",

                    ["NoTextToDelete"] = "No text to delete",
                    ["NoTextSelected"] = "No text selected",
                    ["ClipboardEmpty"] = "Clipboard is empty",
                    ["FileNotFound"] = "File not found",
                    ["SaveSuccess"] = "File saved successfully",
                    ["OpenError"] = "Error opening file",
                    ["SaveError"] = "Error saving file",

                    ["Untitled"] = "Untitled",

                    ["TextFiles"] = "Text files",
                    ["AllFiles"] = "All files"
                }
            };

        public Localization()
        {
            _currentLanguage = "ru";
        }

        public string Get(string key)
        {
            if (translations.ContainsKey(CurrentLanguage) &&
                translations[CurrentLanguage].ContainsKey(key))
            {
                return translations[CurrentLanguage][key];
            }

            return translations["en"].ContainsKey(key) ? translations["en"][key] : key;
        }
    }
}
