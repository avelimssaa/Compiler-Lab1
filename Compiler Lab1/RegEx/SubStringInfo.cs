namespace Compiler_Lab1.RegEx
{
    public interface ISubString
    {
        string RegEx { get; set; }
        string Line { get; set; }
        string Column { get; set; }
        int Length { get; set; }
        string Position();
    }
    internal class SubStringInfo : ISubString
    {
        private string _regEx;
        public string RegEx
        {
            get { return _regEx; }
            set { _regEx = value; }
        }

        private string _line;
        public string Line
        {
            get { return _line; }
            set { _line = value; }
        }

        private string _column;
        public string Column
        {
            get { return _column; }
            set { _column = value; }
        }

        private int _length;
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public string Position()
        {
            return $"Строка: {_line}, столбец {_column}";
        }
    }
}
