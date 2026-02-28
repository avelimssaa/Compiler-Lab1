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

                    ["Text"] = "Текст",
                    ["MissionStatement"] = "Постановка задачи",
                    ["Grammar"] = "Грамматика",
                    ["Grammar classification"] = "Классификаия грамматики",
                    ["Method of analysis"] = "Метод анализа",
                    ["Test case"] = "Тестовый пример",
                    ["List of literature"] = "Список литературы",
                    ["Source code"] = "Исходный код программы",

                    ["Start"] = "Компиляция",

                    ["Certificate"] = "Справка",
                    ["Help"] = "Руководство пользователя",
                    ["About"] = "О программе",

                    ["View"] = "Вид",
                    ["Help font"] = "Выберите размер шрифта",
                    ["Font size"] = "Размер шрифта",
                    ["Choose local"] = "Выберите локализацию",
                    ["UserManual"] = "Help_ru.html",

                    ["Untitled"] = "Без имени",
                    ["AllFiles"] = "Все файлы",
                    ["TextFiles"] = "Текстовые файлы",
                    ["Error"] = "Ошибка",
                    ["SaveError"] = "Ошибка при сохранении",
                    ["OpenError"] = "Ошибка прии открытии файла",
                    ["HelpFileError"] = "Файл справки не найден",
                    ["HelpOpenError"] = "Ошибка открытия справки",

                    ["SaveFile"] = "Сохранить изменения в файле",
                    ["ExitApp"] = "Выход из программы",
                    ["CloseTabTitle"] = "Закрытие вкладки",

                    ["Language"] = "Язык",
                    ["FileSize"] = "Размер файла",
                    ["Lines"] = "Строк",
                    ["Chars"] = "Символов",

                    ["Compile"] = "Компиляция...",
                    ["Ready"] = "Готово."
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

                    ["Start"] = "Compile",

                    ["Certificate"] = "Certificate",
                    ["Help"] = "Help",
                    ["About"] = "About",
                    ["UserManual"] = "Help_en.html",

                    ["View"] = "View",
                    ["Help font"] = "Choose the font size",
                    ["Font size"] = "Font size",
                    ["Choose local"] = "Choose a localization",

                    ["Untitled"] = "Untitled",
                    ["TextFiles"] = "Text files",
                    ["AllFiles"] = "All files",
                    ["SaveError"] = "Save error",
                    ["OpenError"] = "File open error",
                    ["HelpFileError"] ="The help file was not found.",
                    ["HelpOpenError"] = "Error opening help", 

                    ["SaveFile"] = "Save changes to a file",
                    ["ExitApp"] = "Exiting Program",
                    ["CloseTabTitle"] = "Close Tab",

                    ["Language"] = "Language",
                    ["FileSize"] = "File size",
                    ["Lines"] = "Lines",
                    ["Chars"] = "Chars",

                    ["Compile"] = "Compile...",
                    ["Ready"] = "Ready."
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
