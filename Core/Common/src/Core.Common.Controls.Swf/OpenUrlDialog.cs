using System.Linq;
using System.Windows.Forms;

namespace Core.Common.Controls.Swf
{
    public partial class OpenUrlDialog : Form
    {
        public OpenUrlDialog()
        {
            InitializeComponent();
        }

        public string Url
        {
            get
            {
                return urlComboBox.Text;
            }
            set
            {
                urlComboBox.Text = value;
            }
        }

        public string[] Urls
        {
            get
            {
                return urlComboBox.Items.Cast<object>().Select(i => i.ToString()).ToArray();
            }
            set
            {
                urlComboBox.Items.Clear();
                urlComboBox.Items.AddRange(value);
            }
        }
    }
}