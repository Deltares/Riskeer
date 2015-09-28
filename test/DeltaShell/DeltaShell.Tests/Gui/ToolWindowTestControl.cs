using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;

namespace DeltaShell.Tests.Gui
{
    public partial class ToolWindowTestControl : UserControl, IView
    {
        private Image image;

        public ToolWindowTestControl(string name)
        {
            Name = name;
            Initialize();
        }

        public ToolWindowTestControl()
        {
            Initialize();
        }

        #region IView Members

        public object Data
        {
            get; set;
        }

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        public void EnsureVisible(object item) { }
        public ViewInfo ViewInfo { get; set; }

        #endregion

        private void Initialize()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = propertyGrid1;
        }
    }
}