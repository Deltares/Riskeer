using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Table.Filtering
{
    public partial class FilterNumericControl : UserControl, IFilterControl
    {
        private enum FilterOptionNumeric
        {
            Equal,
            NotEqual,
            GreaterThen,
            SmallerThen
        }

        private string filter;

        public FilterNumericControl()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(Enum.GetValues(typeof(FilterOptionNumeric)).OfType<object>().ToArray());
        }

        public string Filter
        {
            get
            {
                switch ((FilterOptionNumeric) comboBox1.SelectedItem)
                {
                    case FilterOptionNumeric.Equal:
                        return string.Format("= {0}", textBox1.Text);
                    case FilterOptionNumeric.NotEqual:
                        return string.Format("<> {0}", textBox1.Text);
                    case FilterOptionNumeric.GreaterThen:
                        return string.Format("> {0}", textBox1.Text);
                    case FilterOptionNumeric.SmallerThen:
                        return string.Format("< {0}", textBox1.Text);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                filter = value;

                comboBox1.SelectedItem = GetFilterOption();
                textBox1.Text = GetFilterText();
            }
        }

        private string GetFilterText()
        {
            if (filter == null)
            {
                return "";
            }

            var numberOfCharToTrim = comboBox1.SelectedItem == (object) FilterOptionNumeric.NotEqual ? 3 : 2;
            return filter.Substring(numberOfCharToTrim, filter.Length - numberOfCharToTrim);
        }

        private FilterOptionNumeric GetFilterOption()
        {
            if (filter == null)
            {
                return FilterOptionNumeric.Equal;
            }

            if (filter.StartsWith("<>"))
            {
                return FilterOptionNumeric.NotEqual;
            }
            if (filter.StartsWith("="))
            {
                return FilterOptionNumeric.Equal;
            }
            if (filter.StartsWith(">"))
            {
                return FilterOptionNumeric.GreaterThen;
            }
            if (filter.StartsWith("<"))
            {
                return FilterOptionNumeric.SmallerThen;
            }

            return FilterOptionNumeric.Equal;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            var numberDecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != numberDecimalSeparator)
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == numberDecimalSeparator && textBox1.Text.IndexOf(numberDecimalSeparator) > -1)
            {
                e.Handled = true;
            }
        }
    }
}