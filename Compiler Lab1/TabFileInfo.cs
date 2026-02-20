
namespace Compiler_Lab1
{
    public class TabFileInfo
    {
        private string _filePath;
        public string FilePath { get { return _filePath; } set { _filePath = value; } }
        private string _fileName;
        public string FileName { get { return _fileName; } set { _filePath = value; } }
        private bool _isChanged;
        public bool IsChanged { get { return _isChanged;  } set { _isChanged = value; }  }

        private Stack<string> _undoStack = new Stack<string>();
        private Stack<string> _redoStack = new Stack<string>();
        private bool isUndoRedoOperation = false;

        public Stack<string> GetUndoStack()
        {
            return _undoStack;
        }

        public Stack<string> GetRedoStack() { return _redoStack; }

        public bool GetIsUndoRedoOperation() {  return isUndoRedoOperation; }
    }
}
