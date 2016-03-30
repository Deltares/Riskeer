using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms
{
    public partial class PipingSurfaceLineSelectionDialog : DialogBase
    {
        public PipingSurfaceLineSelectionDialog(IWin32Window dialogParent, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines)
            : base(dialogParent, Resources.GeneratePipingCalculationsIcon, 300, 600)
        {
            InitializeComponent();

            PipingSurfaceLineSelectionView = new PipingSurfaceLineSelectionView(surfaceLines)
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(PipingSurfaceLineSelectionView);
        }

        private PipingSurfaceLineSelectionView PipingSurfaceLineSelectionView { get; set; }

        public IEnumerable<RingtoetsPipingSurfaceLine> SelectedSurfaceLines { get; set; }

        protected override Button GetCancelButton()
        {
            return CancelButton;
        }

        private void OkButtonOnClick(object sender, EventArgs e)
        {
            SelectedSurfaceLines = PipingSurfaceLineSelectionView.GetSelectedSurfaceLines();
            Close();
        }

        private void CancelButtonOnClick(object sender, EventArgs eventArgs)
        {
            SelectedSurfaceLines = new List<RingtoetsPipingSurfaceLine>();
            Close();
        }
    }
}