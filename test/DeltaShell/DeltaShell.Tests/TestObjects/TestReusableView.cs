using System;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;

namespace DeltaShell.Tests.TestObjects
{
    public partial class TestReusableView : UserControl,IReusableView
    {
        
        public TestReusableView()
        {
            InitializeComponent();
        }

        public object Data { get; set; }
        public Image Image { get; set; }
        public void EnsureVisible(object item) { }
        public ViewInfo ViewInfo { get; set; }

        private bool locked;
        public bool Locked
        {
            get { return locked; }
            set
            {
                locked = value;
                if (LockedChanged != null)
                {
                    LockedChanged(this,new EventArgs());
                }
            }
        }

        public event EventHandler LockedChanged;
    }
}
