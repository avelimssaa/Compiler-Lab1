namespace Compiler_Lab1.RegEx
{
    internal class StateMachine : IRegEx
    {
        private readonly List<ISubString> _subs;
        private readonly string _text;

        public StateMachine(string text)
        {
            _subs = [];
            _text = text;
        }

        public List<ISubString> SubStrings()
        {
            TextAnalyze();
            return _subs;
        }

        public void TextAnalyze()
        {
            if (string.IsNullOrEmpty(_text))
                return;

            int length = _text.Length, state = 0;
            string matchValue;
            ISubString sub;

            for (int i = 0; i < length; i++)
            {
                switch (state)
                {
                    case 0:
                        if (_text[i] == '6')
                            state++;
                        else
                            state = 0;
                        break;

                    case 1:
                        if (_text[i] == '2')
                            state++;
                        else
                            state = 0;
                        break;

                    case < 16:
                        if (char.IsDigit(_text[i]))
                            state++;
                        else
                            state = 0;
                        break;

                    case < 19:
                        if (char.IsDigit(_text[i]))
                            state++;
                        else
                        {
                            matchValue = _text.Substring(i - state, state);
                            sub = new SubStringInfo
                            {
                                RegEx = matchValue,
                                Length = state,
                                Line = GetLineNumber(i - state).ToString(),
                                Column = GetColumnPosition(i - state).ToString()
                            };
                            _subs.Add(sub);
                            i = i - state + 1;
                            state = 0;
                        }
                        break;

                    case 19:
                        matchValue = _text.Substring(i - state, state);
                        sub = new SubStringInfo
                        {
                            RegEx = matchValue,
                            Length = state,
                            Line = GetLineNumber(i - state).ToString(),
                            Column = GetColumnPosition(i - state).ToString()
                        };
                        _subs.Add(sub);
                        i = i - state + 1;
                        state = 0;
                        break;
                }
            }
            if (state >= 2 && state <= 19)
            {
                matchValue = _text.Substring(length - state, state);
                sub = new SubStringInfo
                {
                    RegEx = matchValue,
                    Length = state,
                    Line = GetLineNumber(length - state).ToString(),
                    Column = GetColumnPosition(length - state).ToString()
                };
                _subs.Add(sub);
            }
        }

        private int GetLineNumber(int position)
        {
            if (string.IsNullOrEmpty(_text)) return 1;

            int lineNumber = 1;
            for (int i = 0; i < position && i < _text.Length; i++)
            {
                if (_text[i] == '\n')
                    lineNumber++;
            }
            return lineNumber;
        }

        private int GetColumnPosition(int position)
        {
            if (string.IsNullOrEmpty(_text)) return 1;

            int lastNewLine = _text.LastIndexOf('\n', position);
            if (lastNewLine == -1)
                return position + 1;
            else
                return position - lastNewLine;
        }
    }
}
