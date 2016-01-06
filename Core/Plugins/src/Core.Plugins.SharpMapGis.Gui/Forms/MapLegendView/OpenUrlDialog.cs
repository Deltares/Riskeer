using System.Linq;
using System.Windows.Forms;
using Core.Common.Forms.Dialogs;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public partial class OpenUrlDialog : DialogBase
    {
        public OpenUrlDialog(IWin32Window owner) : base(owner, Resources.buttonAddLayer_Image, 418, 100)
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

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }
    }
}