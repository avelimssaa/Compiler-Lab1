using FastColoredTextBoxNS;
using System.Text.RegularExpressions;

namespace Compiler_Lab1.RegEx
{
    internal interface IRegEx
    {
        List<ISubString> SubStrings();
    }
    internal class RegularExpressionsFind : IRegEx
    {
        private readonly List<ISubString> _subs; 
        private readonly string _text;
        private readonly string _needMatch;

        public RegularExpressionsFind(string text, string needMatch)
        {
            _needMatch = needMatch;
            _text = text;
            _subs = [];
        }

        public List<ISubString> SubStrings()
        {
            if (_needMatch == "Китайские почтовые индексы")
                MatchChineseZIP();
            else if (_needMatch == "UnionPay")
                MatchUnionPay();
            else if (_needMatch == "Таблица Менделеева")
                MatchMendeleevTable();

            return _subs;
        }

        private void MatchChineseZIP()
        {
            string pattern = @"[1-9]\d{5}";
            Regex regex = new Regex(pattern);

            MatchCollection matches = regex.Matches(_text);

            foreach (Match match in matches)
            {
                ISubString sub = new SubStringInfo
                {
                    RegEx = match.Value,
                    Length = match.Length,
                    Line = GetLineNumber(match.Index).ToString(),
                    Column = GetColumnPosition(match.Index).ToString()
                };
                _subs.Add(sub);
            }
        }

        private void MatchUnionPay()
        {
            string pattern = @"62\d{14,17}";
            Regex regex = new(pattern);

            MatchCollection matches = regex.Matches(_text);

            foreach (Match match in matches)
            {
                ISubString sub = new SubStringInfo
                {
                    RegEx = match.Value,
                    Length = match.Length,
                    Line = GetLineNumber(match.Index).ToString(),
                    Column = GetColumnPosition(match.Index).ToString()
                };
                _subs.Add(sub);
            }
        }

        private void MatchMendeleevTable()
        {
            string pattern = @"\b(H|He|
Li|Be|B|C|N|O|F|Ne|
Na|Mg|Al|Si|P|S|Cl|Ar|
K|Ca|Sc|Ti|V|Cr|Mn|Fe|Co|Ni|
Cu|Zn|Ga|Ge|As|Se|Br|Kr|
Rb|Sr|Y|Zr|Nb|Mo|Tc|Ru|Rh|Pd|
Ag|Cd|In|Sn|Sb|Te|I|Xe|
Cs|Ba|La|
Ce|Pr|Nd|Pm|Sm|Eu|Gd|Tb|Dy|Ho|Er|Tm|Yb|Lu|
Hf|Ta|W|Re|Os|Ir|Pt|
Au|Hg|Tl|Pb|Bi|Po|At|Rn|
Fr|Ra|Ac|
Th|Pa|U|Np|Pu|Am|Cm|Bk|Cf|Es|Fm|Md|No|Lr|
Rf|Db|Sg|Bh|Hs|Mt|Ds|
Rg|Cn|Nh|Fl|Mc|Lv|Ts|Og)\b";

            Regex regex = new(pattern);

            MatchCollection matches = regex.Matches(_text);

            foreach (Match match in matches)
            {
                ISubString sub = new SubStringInfo
                {
                    RegEx = match.Value,
                    Length = match.Length,
                    Line = GetLineNumber(match.Index).ToString(),
                    Column = GetColumnPosition(match.Index).ToString()
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
