using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Csv.FormatPickers
{
    public class DoubleCsvFormatPickerProvider : CsvFormatPickerProvider
    {
        private ComboBox numberFormatComboBox;

        public override string Label
        {
            get
            {
                return "Number format";
            }
        }

        public override Control GetFormatPicker()
        {
            numberFormatComboBox = CreateCombobox(GenerateNumberSeparators().Cast<object>().ToArray());
            numberFormatComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            numberFormatComboBox.Font = new Font(numberFormatComboBox.Font, FontStyle.Bold);
            numberFormatComboBox.SelectedIndexChanged += numberFormatComboBox_SelectedIndexChanged;
            return numberFormatComboBox;
        }

        public override IFormatProvider GetFormatProvider()
        {
            return ((NumberSeparator) numberFormatComboBox.SelectedItem).FormatInfo;
        }

        public override void SetFormatPickerToInitialGuess(IEnumerable<string> exampleStrings)
        {
            double result;
            var matchingFormat = GenerateNumberSeparators()
                .FirstOrDefault(format =>
                                exampleStrings.All(
                                    example =>
                                    double.TryParse(example, NumberStyles.Number, format.FormatInfo, out result)));

            if (matchingFormat != null)
            {
                numberFormatComboBox.SelectedIndex = numberFormatComboBox.Items.IndexOf(matchingFormat);
            }
            else
            {
                FireUserSelectionChanged();
            }
        }

        private void numberFormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FireUserSelectionChanged();
        }

        private static IEnumerable<NumberSeparator> GenerateNumberSeparators()
        {
            yield return new NumberSeparator(".", "");
            yield return new NumberSeparator(".", ",");
            yield return new NumberSeparator(",", "");
            yield return new NumberSeparator(",", ".");
            yield return new NumberSeparator(".", " ");
            yield return new NumberSeparator(",", " ");
        }

        private class NumberSeparator
        {
            public NumberSeparator(string decimalSeparator, string groupSeparator)
            {
                var customNFormat = (NumberFormatInfo) CultureInfo.InvariantCulture.NumberFormat.Clone();
                customNFormat.NumberDecimalSeparator = decimalSeparator;
                customNFormat.NumberGroupSeparator = groupSeparator;
                customNFormat.NumberGroupSizes = new[]
                {
                    3
                };

                FormatInfo = customNFormat;
            }

            public NumberFormatInfo FormatInfo { get; private set; }

            public override string ToString()
            {
                return String.Format(FormatInfo, "{0:N}", 12345.67);
            }

            public override bool Equals(object obj)
            {
                var ns = obj as NumberSeparator;

                if (ns == null)
                {
                    return false;
                }

                return ns.FormatInfo.NumberDecimalSeparator == FormatInfo.NumberDecimalSeparator &&
                       ns.FormatInfo.NumberGroupSeparator == FormatInfo.NumberGroupSeparator;
            }
        }
    }
}