using System;
using System.Linq;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Table.Filtering
{
    public partial class FilterDateTimeControl : UserControl, IFilterControl
    {
        private string filter;

        private enum FilterOptionDateTime
        {
            Equal,
            NotEqual,
            GreaterThen,
            SmallerThen
        }

        public FilterDateTimeControl()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(Enum.GetValues(typeof(FilterOptionDateTime)).OfType<object>().ToArray());
        }

        public string Filter
        {
            get
            {
                switch ((FilterOptionDateTime)comboBox1.SelectedItem)
                {
                    case FilterOptionDateTime.Equal: return string.Format("= #{0}#", dateTimePicker1.Value);
                    case FilterOptionDateTime.NotEqual: return string.Format("<> #{0}#", dateTimePicker1.Value);
                    case FilterOptionDateTime.GreaterThen: return string.Format("> #{0}#", dateTimePicker1.Value);
                    case FilterOptionDateTime.SmallerThen: return string.Format("< #{0}#", dateTimePicker1.Value);
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                filter = value;

                comboBox1.SelectedItem = GetFilterOption();
                dateTimePicker1.Text = GetFilterText();
            }
        }

        private string GetFilterText()
        {
            if (filter == null) return "";
            
            var start = filter.IndexOf("#") + 1;
            var end = filter.LastIndexOf("#");

            return filter.Substring(start, end - start);
        }

        private FilterOptionDateTime GetFilterOption()
        {
            if (filter == null) return FilterOptionDateTime.Equal;

            if (filter.StartsWith("<>")) return FilterOptionDateTime.NotEqual;
            if (filter.StartsWith("=")) return FilterOptionDateTime.Equal;
            if (filter.StartsWith(">")) return FilterOptionDateTime.GreaterThen;
            if (filter.StartsWith("<")) return FilterOptionDateTime.SmallerThen;

            return FilterOptionDateTime.Equal;
        }
    }
}
