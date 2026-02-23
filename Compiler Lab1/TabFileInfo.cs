
namespace Compiler_Lab1
{
    public class TabFileInfo
    {
        private string _filePath;
        public string FilePath { get { return _filePath; } set { _filePath = value; } }

        private string _fileName;
        public string FileName { get { return _fileName; } set { _fileName = value; } }

        private bool _isChanged;
        public bool IsChanged { get { return _isChanged;  } set { _isChanged = value; }  }

        private Stack<string> _undoStack = new Stack<string>();
        private Stack<string> _redoStack = new Stack<string>();
        private bool _isUndoRedoOperation = false;

        public Stack<string> GetUndoStack()
        {
            return _undoStack;
        }

        public void PushUndoStack(string element)
        {
            _undoStack.Push(element);
        }

        public void ClearUndoStack()
        {
            _undoStack.Clear(); 
        }

        public void UndoStackLimitation()
        {
            if (GetUndoStackCount() > 100)
            {
                List<string> list = _undoStack.ToList();
                list.RemoveAt(list.Count - 1);
                _undoStack = new Stack<string>(list);
            }
        }

        public int GetUndoStackCount()
        {
            return _undoStack.Count;
        }

        public string PopUndoStack()
        {
            return _undoStack.Pop();
        }

        public Stack<string> GetRedoStack() { return _redoStack; }

        public void PushRedoStack(string element)
        {
            _redoStack.Push(element); 
        }

        public void ClearRedoStack()
        {
            _redoStack.Clear(); 
        }

        public int GetRedoStackCount()
        {
            return _redoStack.Count; 
        }

        public string PopRedoStack()
        {
            return _redoStack.Pop();
        }

        public bool GetIsUndoRedoOperation() {  return _isUndoRedoOperation; }
    }
}
