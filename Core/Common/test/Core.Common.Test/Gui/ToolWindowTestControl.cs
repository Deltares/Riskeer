using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Views;

namespace Core.Common.Test.Gui
{
    public partial class ToolWindowTestControl : UserControl, IView
    {
        public ToolWindowTestControl()
        {
            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = propertyGrid1;
        }

        #region IView Members

        public object Data { get; set; }

        public Image Image { get; set; }

        public void EnsureVisible(object item) {}
        public ViewInfo ViewInfo { get; set; }

        #endregion
    }
}