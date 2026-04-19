namespace Compiler_Lab1.ASTBuild
{
    internal class SymbolTable
    {
        private Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();

        public bool CheckDuplicate(string name)
        {
            return _symbols.ContainsKey(name);
        }

        public void Declare(string name, string type, object value)
        {
            if (CheckDuplicate(name))
            {
                return;
            }

            _symbols.Add(name, new Symbol(name, type, value));
        }

        public Symbol Lookup(string name)
        {
            if (!_symbols.ContainsKey(name))
            {
                return null;
            }
            return _symbols[name];
        }
    }

    internal class Symbol
    {
        private readonly string _name, _type;
        private readonly Object _value;
        public string Name
        {
            get { return _name; }
        }
        public string Type
        {
            get { return _type; }
        }
        public Object Value
        {
            get { return _value; }
        }

        public Symbol(string name, string type, Object value)
        {
            _value = value;
            _type = type;
            _name = name;
        }
    }
}
