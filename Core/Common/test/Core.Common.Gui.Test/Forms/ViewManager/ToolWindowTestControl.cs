using System.Windows.Forms;

using Core.Common.Controls.Views;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    public partial class ToolWindowTestControl : UserControl, IView
    {
        public ToolWindowTestControl()
        {
            Initialize();
        }

        #region IView Members

        public object Data { get; set; }

        #endregion

        private void Initialize()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = propertyGrid1;
        }
    }
}