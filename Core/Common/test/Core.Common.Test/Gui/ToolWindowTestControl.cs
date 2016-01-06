using System.Windows.Forms;
using Core.Common.Forms.Views;

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

        #endregion
    }
}