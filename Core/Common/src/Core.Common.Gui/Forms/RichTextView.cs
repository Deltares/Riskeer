using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui.Properties;
using log4net;

namespace Core.Common.Gui.Forms
{
    public partial class RichTextView : UserControl, IView
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RichTextView));

        public RichTextView()
        {
            InitializeComponent();
            richTextBox1.ReadOnly = true;
        }

        public RichTextView(string name, string path) : this()
        {
            //InitializeComponent();
            if (File.Exists(path))
            {
                richTextBox1.LoadFile(path);
            }
            else
            {
                log.WarnFormat(Resources.RichTextView_RichTextView_The_license_file_0_cannot_be_found_on_the_computer, Path.GetFileName(path));
            }
            Name = name;
            Text = name;
        }

        #region IView Members

        public object Data
        {
            get
            {
                return Text;
            }
            set
            {
                /* ignore; cleanup process may set data to null */
            }
        }

        public Image Image { get; set; }

        public void EnsureVisible(object item) {}
        public ViewInfo ViewInfo { get; set; }

        #endregion
    }
}