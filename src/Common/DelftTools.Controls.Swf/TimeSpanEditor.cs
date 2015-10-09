using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    public class TimeSpanEditor : MaskedTextBox
    {
        public event EventHandler ValueChanged;
        private bool includeTensOfSeconds;
        private bool includeDays;

        private TimeSpan previousValue;

        public TimeSpanEditor()
        {
            TextMaskFormat = MaskFormat.IncludePromptAndLiterals;
            SetMask();
            KeyDown += TimeSpanEditor_KeyDown;
            Validating += TimeSpanEditor_Validating;
            GotFocus += TimeSpanEditor_GotFocus;
            LostFocus += TimeSpanEditor_LostFocus;
            InsertKeyMode = InsertKeyMode.Overwrite;
            Value = new TimeSpan(0);
        }

        public bool IncludeTensOfSeconds
        {
            get
            {
                return includeTensOfSeconds;
            }
            set
            {
                includeTensOfSeconds = value;
                SetMask();
            }
        }

        public bool IncludeDays
        {
            get
            {
                return includeDays;
            }
            set
            {
                includeDays = value;
                SetMask();
            }
        }

        public TimeSpan Value
        {
            get
            {
                var timespan = TryParseTimeSpan();
                return timespan.HasValue ? timespan.Value : default(TimeSpan);
            }
            set
            {
                var format = GenerateDateTimeFormat();
                Text = value.ToString(format, CultureInfo.InvariantCulture);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                TimeSpanEditor_KeyDown(this, new KeyEventArgs(keyData));
                return true;
            }
            if (keyData == Keys.Delete || keyData == Keys.Back)
            {
                TimeSpanEditor_KeyDown(this, new KeyEventArgs(keyData));
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private IDictionary<char, int> TimeSymbols
        {
            get
            {
                var res = new Dictionary<char, int>();
                if (IncludeDays)
                {
                    res.Add('d', 99);
                }
                res.Add('h', 23);
                res.Add('m', 59);
                res.Add('s', 59);
                if (IncludeTensOfSeconds)
                {
                    res.Add('f', 9);
                }
                return res;
            }
        }

        private void TimeSpanEditor_KeyDown(object sender, KeyEventArgs e)
        {
            var format = Regex.Replace(GenerateDateTimeFormat(), "\\\\", "");
            var timeSymbol = GetTimeSymbolAtPosition(format, SelectionStart);

            if (e.KeyCode == Keys.Escape)
            {
                Value = previousValue;
                Parent.Focus();
            }
            else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                if (e.KeyCode == Keys.Back && SelectionStart <= 0)
                {
                    return;
                }

                var symbolToReset = e.KeyCode == Keys.Delete
                                        ? timeSymbol
                                        : GetTimeSymbolAtPosition(format, SelectionStart - 1);
                SetBlockValue(format, symbolToReset, 0);

                // manually jump to begin of block:
                SelectionStart = format.IndexOf(symbolToReset);

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                var direction = 1;
                if (e.KeyCode == Keys.Left)
                {
                    direction = -1;
                }

                var targetTimeSymbol = GetTargetTimeSymbol(timeSymbol, direction);

                // jump to begin of block:
                SelectionStart = format.IndexOf(targetTimeSymbol);

                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                var increment = e.KeyCode == Keys.Up ? 1 : -1;
                var maxValueForSymbol = TimeSymbols[timeSymbol];

                var startIndex = format.IndexOf(timeSymbol);
                var endIndex = format.LastIndexOf(timeSymbol) + 1;
                var length = endIndex - startIndex;

                var valueUnderSelection = Text.Substring(startIndex, length);
                int value;
                Int32.TryParse(valueUnderSelection, out value); //we don't care if it succeeds or not -> resets to 0

                value += increment;
                if (value > maxValueForSymbol)
                {
                    value = 0;
                }
                if (value < 0)
                {
                    value = maxValueForSymbol;
                }

                SetBlockValue(format, timeSymbol, value);

                e.Handled = true;
            }
        }

        private char GetTimeSymbolAtPosition(string format, int position)
        {
            if (position == Text.Length)
            {
                position--;
            }

            var timeSymbol = Char.IsLetter(format[position])
                                 ? format[position]
                                 : format[position - 1];

            if (!Char.IsLetter(timeSymbol) || !TimeSymbols.ContainsKey(timeSymbol))
            {
                throw new InvalidOperationException("Datetime format not as expected");
            }
            return timeSymbol;
        }

        private void SetBlockValue(string timeFormat, char timeSymbol, int value)
        {
            var startIndex = timeFormat.IndexOf(timeSymbol);
            var endIndex = timeFormat.LastIndexOf(timeSymbol) + 1;
            var length = endIndex - startIndex;

            var partBefore = Text.Substring(0, startIndex);
            var partAfter = Text.Substring(endIndex);

            var paddedNumber = value.ToString(CultureInfo.InvariantCulture).PadLeft(length, '0');

            var oldPos = SelectionStart;
            Text = partBefore + paddedNumber + partAfter;
            SelectionStart = oldPos;
        }

        private char GetTargetTimeSymbol(char timeSymbol, int direction)
        {
            var symbols = TimeSymbols.Keys.ToList();

            var indexOfSymbol = symbols.IndexOf(timeSymbol);

            var indexOfTarget = indexOfSymbol + direction;

            if (indexOfTarget < 0)
            {
                indexOfTarget = symbols.Count - 1;
            }
            if (indexOfTarget >= symbols.Count)
            {
                indexOfTarget = 0;
            }

            return symbols[indexOfTarget];
        }

        private TimeSpan? TryParseTimeSpan()
        {
            TimeSpan result;
            if (!string.IsNullOrEmpty(Text) && TimeSpan.TryParseExact(Text, GenerateDateTimeFormat(), CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return null;
        }

        private void TimeSpanEditor_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !TryParseTimeSpan().HasValue;
        }

        private void TimeSpanEditor_GotFocus(object sender, EventArgs e)
        {
            previousValue = Value;
        }

        private void TimeSpanEditor_LostFocus(object sender, EventArgs e)
        {
            if (Equals(Value, previousValue))
            {
                return;
            }

            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }

        private void SetMask()
        {
            var dtFormat = GenerateDateTimeFormat();
            var mask = new StringBuilder();

            // replace letters by #, keep rest as-is
            // so dd hh:mm:ss becomes ## ##:##:##
            foreach (var c in dtFormat)
            {
                mask.Append(Char.IsLetter(c) ? '#' : c);
            }

            var oldVar = Value;
            Mask = mask.ToString();
            Value = oldVar;
        }

        private string GenerateDateTimeFormat()
        {
            var format = @"hh\:mm\:ss";
            if (includeDays)
            {
                format = @"dd\ " + format;
            }
            if (includeTensOfSeconds)
            {
                format = format + @"\.f";
            }
            return format;
        }
    }
}