using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Table.Filtering
{
    public partial class FilterBooleanControl : UserControl, IFilterControl
    {
        public FilterBooleanControl()
        {
            InitializeComponent();
        }

        public string Filter
        {
            get { return "= " + (checkBox1.Checked ? "TRUE" : "FALSE"); }
            set
            {
                if (value != null && value.EndsWith("TRUE"))
                {
                    checkBox1.Checked = true;
                }
            }
        }
    }
}
