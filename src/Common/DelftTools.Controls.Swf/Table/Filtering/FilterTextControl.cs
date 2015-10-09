using System;
using System.Linq;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Table.Filtering
{
    public partial class FilterTextControl : UserControl, IFilterControl
    {
        private enum FilterOption
        {
            Equal,
            NotEqual,
            Contains,
            StartsWith,
            EndsWith
        }

        private string filter;

        public FilterTextControl()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(Enum.GetValues(typeof(FilterOption)).OfType<object>().ToArray());
        }

        public string Filter
        {
            get
            {
                switch ((FilterOption) comboBox1.SelectedItem)
                {
                    case FilterOption.Equal:
                        return string.Format("= '{0}'", textBox1.Text);
                    case FilterOption.NotEqual:
                        return string.Format("<> '{0}'", textBox1.Text);
                    case FilterOption.Contains:
                        return string.Format("Like '%{0}%'", textBox1.Text);
                    case FilterOption.StartsWith:
                        return string.Format("Like '{0}%'", textBox1.Text);
                    case FilterOption.EndsWith:
                        return string.Format("Like '%{0}'", textBox1.Text);
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

            var start = filter.IndexOf("'") + 1;
            var end = filter.LastIndexOf("'");

            return filter.Substring(start, end - start).Trim('%');
        }

        private FilterOption GetFilterOption()
        {
            if (filter == null)
            {
                return FilterOption.Equal;
            }

            if (filter.StartsWith("<>"))
            {
                return FilterOption.NotEqual;
            }
            if (filter.StartsWith("="))
            {
                return FilterOption.Equal;
            }

            if (filter.StartsWith("Like '%") && filter.EndsWith("%'"))
            {
                return FilterOption.Contains;
            }
            if (filter.StartsWith("Like '") && filter.EndsWith("%'"))
            {
                return FilterOption.StartsWith;
            }
            if (filter.StartsWith("Like '%"))
            {
                return FilterOption.EndsWith;
            }

            return FilterOption.Equal;
        }
    }
}