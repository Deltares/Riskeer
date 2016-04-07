using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms
{
    /// <summary>
    /// A dialog which allows the user to make a selection form a given set of <see cref="RingtoetsPipingSurfaceLine"/>. Upon
    /// closing of the dialog, the selected <see cref="RingtoetsPipingSurfaceLine"/> can be obtained.
    /// </summary>
    public partial class PipingSurfaceLineSelectionDialog : DialogBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSurfaceLineSelectionDialog"/>.
        /// </summary>
        /// <param name="dialogParent">The parent of the dialog.</param>
        /// <param name="surfaceLines">The collection of <see cref="RingtoetsPipingSurfaceLine"/> to show in the dialog.</param>
        public PipingSurfaceLineSelectionDialog(IWin32Window dialogParent, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines)
            : base(dialogParent, Resources.GeneratePipingCalculationsIcon, 300, 400)
        {
            InitializeComponent();

            PipingSurfaceLineSelectionView = new PipingSurfaceLineSelectionView(surfaceLines)
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(PipingSurfaceLineSelectionView);
            SelectedSurfaceLines = new List<RingtoetsPipingSurfaceLine>();
        }

        /// <summary>
        /// Gets a collection of selected <see cref="RingtoetsPipingSurfaceLine"/> if they were selected
        /// in the dialog and a confirmation was given. If no confirmation was given or no 
        /// <see cref="RingtoetsPipingSurfaceLine"/> was selected, then an empty collection is returned.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> SelectedSurfaceLines { get; private set; }

        protected override Button GetCancelButton()
        {
            return CustomCancelButton;
        }

        private PipingSurfaceLineSelectionView PipingSurfaceLineSelectionView { get; set; }

        private void OkButtonOnClick(object sender, EventArgs e)
        {
            SelectedSurfaceLines = PipingSurfaceLineSelectionView.GetSelectedSurfaceLines();
            Close();
        }

        private void CancelButtonOnClick(object sender, EventArgs eventArgs)
        {
            Close();
        }
    }
}