using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;
using DeltaShell.Plugins.DemoGuiPlugin.Controllers;

namespace DeltaShell.Plugins.DemoGuiPlugin.Views
{
    public partial class WTIProjectView : UserControl, IView
    {
        private readonly WTIProjectViewController controller;

        public WTIProjectView()
        {
            InitializeComponent();
            controller = new WTIProjectViewController(this);
        }

        public object Data
        {
            get { return controller.GetViewData(); }
            set { controller.SetViewData(value); }
        }

        public Image Image
        {
            get { return Properties.Resources.projection_screen; }
            set {  }
        }

        public ViewInfo ViewInfo { get; set; }

        public void EnsureVisible(object item) { }

        public void SetProjectName(string name)
        {
            textBoxName.Text = name;
        }

        private void textBoxName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                // Commits value and removes active cursor from TextBox:
                backgroundPanel.Select();
            }
        }

        private void textBoxName_Leave(object sender, System.EventArgs e)
        {
            // Commits value:
            controller.SetProjectName(textBoxName.Text);
        }
    }
}
