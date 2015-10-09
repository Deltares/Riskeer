using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DelftTools.Utils;

namespace DelftTools.Controls.Swf.Csv.FormatPickers
{
    public class DateTimeCsvFormatPickerProvider : CsvFormatPickerProvider
    {
        private TextBox dateTimeFormatBox;

        public override string Label
        {
            get
            {
                return "Date time format";
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(DateTime);
            }
        }

        public override Control GetFormatPicker()
        {
            var panel = new Panel
            {
                Height = 25, Width = 400
            };

            dateTimeFormatBox = new TextBox
            {
                Left = 0, Width = 150
            };
            dateTimeFormatBox.Leave += dateTimeFormatBox_Leave;
            dateTimeFormatBox.Font = new Font(dateTimeFormatBox.Font, FontStyle.Bold);

            var tooltip = new ToolTip
            {
                IsBalloon = true
            };
            var caption = "y = year, M = month, d = day, h = hour (12), " + Environment.NewLine +
                          "H = hour (24), m = minute, s = second, " + Environment.NewLine +
                          "f = second fraction, F = second fraction (without trailing zeroes), " + Environment.NewLine +
                          "t = PM or AM, z = time zone";
            tooltip.SetToolTip(dateTimeFormatBox, caption);

            var applyButton = new Button
            {
                Left = 155, Width = 45, Height = dateTimeFormatBox.Height, Text = "Apply"
            };
            applyButton.Click += applyButton_Click;

            panel.Controls.Add(dateTimeFormatBox);
            panel.Controls.Add(applyButton);

            return panel;
        }

        public override IFormatProvider GetFormatProvider()
        {
            var customDTFormat = (DateTimeFormatInfo) CultureInfo.InvariantCulture.DateTimeFormat.Clone();
            customDTFormat.FullDateTimePattern = dateTimeFormatBox.Text;

            // HACK: this datetimeformatinfo object is not internally consistent, we only
            // use it to transport the FullDateTimePattern to the csv importer..
            return customDTFormat;
        }

        public override void SetFormatPickerToInitialGuess(IEnumerable<string> exampleStrings)
        {
            string format;
            if (DateTimeFormatGuesser.TryGuessDateTimeFormat(exampleStrings, out format))
            {
                dateTimeFormatBox.Text = format;
            }
            else
            {
                dateTimeFormatBox.Text = CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern
                                         + " " + CultureInfo.InvariantCulture.DateTimeFormat.ShortTimePattern;
            }
        }

        private void dateTimeFormatBox_Leave(object sender, EventArgs e)
        {
            FireUserSelectionChanged();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            //empty for reason (handled by leave event)
        }
    }
}