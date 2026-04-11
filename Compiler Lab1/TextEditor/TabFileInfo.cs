namespace Compiler_Lab1.TextEditor
{
    public class TabFileInfo
    {
        private string _filePath;
        public string FilePath { get { return _filePath; } set { _filePath = value; } }

        private string _fileName;
        public string FileName { get { return _fileName; } set { _fileName = value; } }

        private bool _isChanged;
        public bool IsChanged { get { return _isChanged;  } set { _isChanged = value; }  }
    }
}
