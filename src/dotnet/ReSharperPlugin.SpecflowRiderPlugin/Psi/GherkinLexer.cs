using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.Util;
using ReSharperPlugin.SpecflowRiderPlugin.Caching.SpecflowJsonSettings;

namespace ReSharperPlugin.SpecflowRiderPlugin.Psi
{
    public class GherkinLexer : ILexer
    {
        private int _currentPosition;
        private int _myCurrentTokenStart;
        private uint _myState;
        private int _myEndOffset;
        private string _myCurLanguage;
        
        private IReadOnlyCollection<string> _myKeywords;
        private readonly GherkinKeywordProvider _keywordProvider;

        // ReSharper disable InconsistentNaming
        private const int STATE_DEFAULT = 0;
        private const int STATE_AFTER_KEYWORD = 1;
        private const int STATE_TABLE = 2;
        private const int STATE_AFTER_KEYWORD_WITH_PARAMETER = 3;
        private const int STATE_INSIDE_PYSTRING = 5;

        private const int STATE_PARAMETER_INSIDE_PYSTRING = 6;
        private const int STATE_PARAMETER_INSIDE_STEP = 7;

        private const string PYSTRING_MARKER = "\"\"\"";
        // ReSharper restore InconsistentNaming

        public GherkinLexer(IBuffer buffer, GherkinKeywordProvider keywordProvider, SpecflowSettingsProvider settingsProvider)
        {
            Buffer = buffer;
            _keywordProvider = keywordProvider;
            
            var settings = settingsProvider.GetDefaultSettings();
            UpdateLanguage(settings.Language.NeutralFeature);
        }

        public void Start()
        {
            _myEndOffset = Buffer.Length;
            _currentPosition = 0;
            _myState = 0;
            Advance();
        }

        public void UpdateLanguage(string language)
        {
            _myCurLanguage = language;
            _myKeywords = _keywordProvider.GetAllKeywords(language);
        }

        public void Advance()
        {
            if (_currentPosition >= _myEndOffset)
            {
                TokenType = null;
                return;
            }

            _myCurrentTokenStart = _currentPosition;
            char c = Buffer[_currentPosition];
            if (_myState != STATE_INSIDE_PYSTRING && char.IsWhiteSpace(c))
            {
                AdvanceOverWhitespace();
                TokenType = GherkinTokenTypes.WHITE_SPACE;
                while (_currentPosition < _myEndOffset && char.IsWhiteSpace(Buffer[_currentPosition]))
                {
                    AdvanceOverWhitespace();
                }
            }
            else if (c == '|' && _myState != STATE_INSIDE_PYSTRING)
            {
                TokenType = GherkinTokenTypes.PIPE;
                _currentPosition++;
                _myState = STATE_TABLE;
            }
            else if (_myState == STATE_PARAMETER_INSIDE_PYSTRING)
            {
                if (c == '>')
                {
                    _myState = STATE_INSIDE_PYSTRING;
                    _currentPosition++;
                    TokenType = GherkinTokenTypes.STEP_PARAMETER_BRACE;
                }
                else
                {
                    AdvanceToParameterEnd(PYSTRING_MARKER, new List<char>{'>'});
                    TokenType = GherkinTokenTypes.STEP_PARAMETER_TEXT;
                }
            }
            else if (_myState == STATE_INSIDE_PYSTRING)
            {
                if (IsStringAtPosition(PYSTRING_MARKER))
                {
                    _currentPosition += 3 /* marker length */;
                    TokenType = GherkinTokenTypes.PYSTRING;
                    _myState = STATE_DEFAULT;
                }
                else
                {
                    if (Buffer[_currentPosition] == '<')
                    {
                        if (IsStepParameterInMultilineString(PYSTRING_MARKER))
                        {
                            _currentPosition++;
                            _myState = STATE_PARAMETER_INSIDE_PYSTRING;
                            TokenType = GherkinTokenTypes.STEP_PARAMETER_BRACE;
                        }
                        else
                        {
                            _currentPosition++;
                            AdvanceToParameterOrSymbol(PYSTRING_MARKER, STATE_INSIDE_PYSTRING, false);
                            TokenType = GherkinTokenTypes.PYSTRING_TEXT;
                        }
                    }
                    else
                    {
                        AdvanceToParameterOrSymbol(PYSTRING_MARKER, STATE_INSIDE_PYSTRING, false);
                        TokenType = GherkinTokenTypes.PYSTRING_TEXT;
                    }
                }
            }
            else if (_myState == STATE_TABLE)
            {
                TokenType = GherkinTokenTypes.TABLE_CELL;
                while (_currentPosition < _myEndOffset)
                {
                    // Cucumber: 0.7.3 Table cells can now contain escaped bars - \| and escaped backslashes - \\
                    if (Buffer[_currentPosition] == '\\')
                    {
                        int nextPos = _currentPosition + 1;
                        if (nextPos < _myEndOffset)
                        {
                            char nextChar = Buffer[nextPos];
                            if (nextChar == '|' || nextChar == '\\')
                            {
                                _currentPosition += 2;
                                continue;
                            }

                            // else - common case
                        }
                    }
                    else if (Buffer[_currentPosition] == '|' || Buffer[_currentPosition] == '\n')
                    {
                        break;
                    }

                    _currentPosition++;
                }

                while (_currentPosition > 0 && char.IsWhiteSpace(Buffer[_currentPosition - 1]))
                {
                    _currentPosition--;
                }
            }
            else if (c == '#')
            {
                TokenType = GherkinTokenTypes.COMMENT;
                AdvanceToEol();

                string commentText = Buffer.GetText(new TextRange(_myCurrentTokenStart + 1, _currentPosition)).Trim();
                string language = FetchLocationLanguage(commentText);
                if (language != null)
                {
                    UpdateLanguage(language);
                }
            }
            else if (c == ':')
            {
                TokenType = GherkinTokenTypes.COLON;
                _currentPosition++;
            }
            else if (c == '@')
            {
                TokenType = GherkinTokenTypes.TAG;
                _currentPosition++;
                while (_currentPosition < _myEndOffset && IsValidTagChar(Buffer[_currentPosition]))
                {
                    _currentPosition++;
                }
            }
            else if (IsStringAtPosition(PYSTRING_MARKER))
            {
                TokenType = GherkinTokenTypes.PYSTRING;
                _myState = STATE_INSIDE_PYSTRING;
                _currentPosition += 3;
            }
            else
            {
                if (_myState == STATE_DEFAULT)
                {
                    foreach (var keyword in _myKeywords)
                    {
                        int length = keyword.Length;
                        if (IsStringAtPosition(keyword))
                        {
                            if (_keywordProvider.IsSpaceRequiredAfterKeyword(_myCurLanguage, keyword) &&
                                _myEndOffset - _currentPosition > length &&
                                char.IsLetterOrDigit(Buffer[_currentPosition + length]))
                            {
                                continue;
                            }

                            char followedByChar = _currentPosition + length < _myEndOffset ? Buffer[_currentPosition + length] : (char)0;
                            TokenType = _keywordProvider.GetTokenType(_myCurLanguage, keyword);
                            if (TokenType == GherkinTokenTypes.STEP_KEYWORD)
                            {
                                bool followedByWhitespace = char.IsWhiteSpace(followedByChar) && followedByChar != '\n';
                                if (followedByWhitespace != _keywordProvider.IsSpaceRequiredAfterKeyword(_myCurLanguage, keyword))
                                {
                                    TokenType = GherkinTokenTypes.TEXT;
                                }
                            }

                            _currentPosition += length;
                            if (TokenType == GherkinTokenTypes.STEP_KEYWORD || TokenType == GherkinTokenTypes.SCENARIO_OUTLINE_KEYWORD)
                            {
                                _myState = STATE_AFTER_KEYWORD_WITH_PARAMETER;
                            }
                            else
                            {
                                _myState = STATE_AFTER_KEYWORD;
                            }

                            return;
                        }
                    }
                }

                if (_myState == STATE_PARAMETER_INSIDE_STEP)
                {
                    switch (c)
                    {
                        case '>':
                            _myState = STATE_AFTER_KEYWORD_WITH_PARAMETER;
                            _currentPosition++;
                            TokenType = GherkinTokenTypes.STEP_PARAMETER_BRACE;
                            break;
                        case '"':
                            _myState = STATE_AFTER_KEYWORD_WITH_PARAMETER;
                            _currentPosition++;
                            TokenType = GherkinTokenTypes.STEP_PARAMETER_QUOTATION_MARKER;
                            break;
                        case '\'':
                            _myState = STATE_AFTER_KEYWORD_WITH_PARAMETER;
                            _currentPosition++;
                            TokenType = GherkinTokenTypes.STEP_PARAMETER_APOSTROPHE_MARKER;
                            break;
                        default:
                                AdvanceToParameterEnd("\n", new List<char>{'>','"','\''});
                                TokenType = GherkinTokenTypes.STEP_PARAMETER_TEXT;    
                            break;
                    }

                    return;
                }
                if (_myState == STATE_AFTER_KEYWORD_WITH_PARAMETER)
                {
                    if (_currentPosition < _myEndOffset && Buffer[_currentPosition] == '<' && IsStepParameter("\n"))
                    {
                        _myState = STATE_PARAMETER_INSIDE_STEP;
                        _currentPosition++;
                        TokenType = GherkinTokenTypes.STEP_PARAMETER_BRACE;
                    }
                    else if (_currentPosition < _myEndOffset && Buffer[_currentPosition] == '"' && IsStepParameter("\n"))
                    {
                        _myState = STATE_PARAMETER_INSIDE_STEP;
                        _currentPosition++;
                        TokenType = GherkinTokenTypes.STEP_PARAMETER_QUOTATION_MARKER;
                    }
                    else if (_currentPosition < _myEndOffset && Buffer[_currentPosition] == '\'' && IsStepParameter("\n"))
                    {
                        _myState = STATE_PARAMETER_INSIDE_STEP;
                        _currentPosition++;
                        TokenType = GherkinTokenTypes.STEP_PARAMETER_APOSTROPHE_MARKER;
                    }
                    else if (_currentPosition < _myEndOffset && char.IsNumber(Buffer[_currentPosition]))
                    {
                        TokenType = GherkinTokenTypes.NUMBER;
                        _currentPosition++;
                    }
                    else
                    {
                        TokenType = GherkinTokenTypes.TEXT;
                        AdvanceToParameterOrSymbol("\n", STATE_AFTER_KEYWORD_WITH_PARAMETER, true);
                    }

                    return;
                }

                TokenType = GherkinTokenTypes.TEXT;
                AdvanceToEol();
            }
        }

        private static string FetchLocationLanguage(string commentText)
        {
            return commentText.StartsWith("language:") ? commentText.Substring(9).Trim() : null;
        }
        
        private void AdvanceOverWhitespace() {
            if (Buffer[_currentPosition] == '\n')
                _myState = STATE_DEFAULT;

            _currentPosition++;
        }
        
        private bool IsStringAtPosition(string keyword) {
            int length = keyword.Length;
            return _myEndOffset - _currentPosition >= length && Buffer.GetText(new TextRange(_currentPosition, _currentPosition + length)).Equals(keyword, StringComparison.Ordinal);
        }

        private bool IsStringAtPosition(string keyword, int position) {
            int length = keyword.Length;
            return _myEndOffset - position >= length && Buffer.GetText(new TextRange(position, position + length)).Equals(keyword, StringComparison.Ordinal);
        }

        private static bool IsValidTagChar(char c) {
            return !char.IsWhiteSpace(c) && c != '@';
        }

        private void AdvanceToParameterEnd(string endSymbol, List<char> closingCharacters) {
            _currentPosition++;
            int mark = _currentPosition;
            while (_currentPosition < _myEndOffset && !IsStringAtPosition(endSymbol) && !closingCharacters.Contains(Buffer[_currentPosition])) {
                _currentPosition++;
            }

            if (_currentPosition < _myEndOffset) {
                if (IsStringAtPosition(endSymbol)) {
                    _myState = STATE_DEFAULT;
                }
            }

            ReturnWhitespace(mark);
        }

        private void ReturnWhitespace(int mark)
        {
            while (_currentPosition > mark && char.IsWhiteSpace(Buffer[_currentPosition - 1]))
                _currentPosition--;
        }
        
        private void AdvanceToEol() {
            _currentPosition++;
            int mark = _currentPosition;
            while (_currentPosition < _myEndOffset && Buffer[_currentPosition] != '\n') {
                _currentPosition++;
            }
            ReturnWhitespace(mark);
            _myState = STATE_DEFAULT;
        }
        
        private void AdvanceToParameterOrSymbol(string s, uint parameterState, bool shouldReturnWhitespace) {
            int mark = _currentPosition;

            while (_currentPosition < _myEndOffset && !IsStringAtPosition(s) &&
                   (parameterState == STATE_AFTER_KEYWORD_WITH_PARAMETER ? !IsStepParameter(s) && !IsNumber() : !IsStepParameterInMultilineString(s)))
            {
                _currentPosition++;
            }

            if (shouldReturnWhitespace) {
                _myState = STATE_DEFAULT;
                if (_currentPosition < _myEndOffset) {
                    if (!IsStringAtPosition(s)) {
                        _myState = parameterState;
                    }
                }

                ReturnWhitespace(mark);
            }
        }

        private bool IsNumber()
        {
            return int.TryParse(Buffer[_currentPosition].ToString(), out _);
        }

        private bool IsStepParameter(string currentElementTerminator) {
            int pos = _currentPosition;

            switch (Buffer[pos])
            {
                case '<':
                {
                    return IsStepParameter(currentElementTerminator, pos, '>');
                }
                case '"':
                {
                    return IsStepParameter(currentElementTerminator, pos, '"');
                }
                case '\'':
                {
                    return IsStepParameter(currentElementTerminator, pos, '\'');
                }
                default:
                    return false;
            }

        }
        
        private bool IsStepParameterInMultilineString(string currentElementTerminator)
        {
            int pos = _currentPosition;

            if (Buffer[pos] == '<')
                return IsStepParameter(currentElementTerminator, pos, '>');
            return false;

        }

        private bool IsStepParameter(string currentElementTerminator, int pos, char stepParameterMark)
        {

            while (pos < _myEndOffset && Buffer[pos] != '\n' && Buffer[pos] != stepParameterMark && !IsStringAtPosition(currentElementTerminator, pos))
            {
                pos++;
            }

            return pos < _myEndOffset && Buffer[pos] == stepParameterMark;
        }


        public object CurrentPosition { get => _currentPosition; set => _currentPosition = (int)value; }
        
        public TokenNodeType TokenType { get; private set; }
        
        public int TokenStart => _myCurrentTokenStart;
        
        public int TokenEnd => _currentPosition;
        
        public IBuffer Buffer { get; }

        public uint State => _myState;
    }
}