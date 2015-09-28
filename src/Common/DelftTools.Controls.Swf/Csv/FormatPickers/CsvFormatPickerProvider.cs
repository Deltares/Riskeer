using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Csv.FormatPickers
{
    public abstract class CsvFormatPickerProvider
    {
        public abstract string Label { get; }
        public abstract Type ValueType { get; }
        public abstract Control GetFormatPicker();
        public abstract IFormatProvider GetFormatProvider();

        public event EventHandler UserSelectionChanged;

        protected void FireUserSelectionChanged()
        {
            if (UserSelectionChanged != null)
            {
                UserSelectionChanged(this, EventArgs.Empty);
            }
        }

        public abstract void SetFormatPickerToInitialGuess(IEnumerable<string> exampleStrings);
        
        protected static ComboBox CreateCombobox(object[] items)
        {
            var combobox = new ComboBox
            {
                Width = 200,
            };
            combobox.Items.AddRange(items);
            combobox.SelectedIndex = 0;
            return combobox;
        }
    }
}