using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Forms
{
    public partial class RichTextView : UserControl, IView
    {
        private RichTextFile richTextFile;

        public RichTextView()
        {
            InitializeComponent();
        }

        #region IView Members

        public object Data
        {
            get
            {
                return richTextFile;
            }
            set
            {
                richTextFile = value as RichTextFile;
                if (richTextFile != null && File.Exists(richTextFile.FilePath))
                {
                    richTextBox1.LoadFile(richTextFile.FilePath);
                }
                else
                {
                    richTextBox1.Clear();
                }
            }
        }

        public Image Image { get; set; }

        public void EnsureVisible(object item) {}

        public ViewInfo ViewInfo { get; set; }

        #endregion
    }
}