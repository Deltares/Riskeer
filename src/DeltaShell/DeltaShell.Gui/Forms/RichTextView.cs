using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DelftTools.Controls;
using DeltaShell.Gui.Properties;
using log4net;

namespace DeltaShell.Gui.Forms
{
    public partial class RichTextView : UserControl, IView
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (RichTextView));
        private Image image;

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
                log.WarnFormat(Resources.RichTextView_RichTextView_The_license_file__0__cannot_be_found_on_the_computer_, Path.GetFileName(path));
            }
            Name = name;
            base.Text = name;
        }

        #region IView Members

        public object Data
        {
            get { return Text; }
            set { /* ignore; cleanup process may set data to null */ }
        }


        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        public void EnsureVisible(object item) { }
        public ViewInfo ViewInfo { get; set; }

        #endregion
    }
}