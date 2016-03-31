using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// A <see cref="UserControl"/> which can be used to display a list of <see cref="RingtoetsPipingSurfaceLine"/>
    /// from which a selection can be made.
    /// </summary>
    public partial class PipingSurfaceLineSelectionView : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSurfaceLineSelectionView"/>. The given
        /// <paramref name="surfaceLines"/> is used to fill the datagrid.
        /// </summary>
        /// <param name="surfaceLines">The surface lines to present in the view.</param>
        public PipingSurfaceLineSelectionView(IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines)
        {
            if (surfaceLines == null)
            {
                throw new ArgumentNullException("surfaceLines");
            }
            InitializeComponent();

            SurfaceLineDataGrid.DataSource = surfaceLines.Select(sl => new SurfaceLineSelectionRow(sl)).ToList();
        }

        /// <summary>
        /// Gets the currently selected surface lines from the data grid view.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="RingtoetsPipingSurfaceLine"/>
        /// which were selected in the view.</returns>
        public IEnumerable<RingtoetsPipingSurfaceLine> GetSelectedSurfaceLines()
        {
            return SurfaceLines.Where(sl => sl.Selected).Select(sl => sl.SurfaceLine).ToList();
        }

        private IEnumerable<SurfaceLineSelectionRow> SurfaceLines
        {
            get
            {
                return (IEnumerable<SurfaceLineSelectionRow>) SurfaceLineDataGrid.DataSource;
            }
        }

        private void OnSelectAllClick(object sender, EventArgs e)
        {
            foreach (var item in SurfaceLines)
            {
                item.Selected = true;
            }
            SurfaceLineDataGrid.Invalidate();
        }

        private void OnSelectNoneClick(object sender, EventArgs e)
        {
            foreach (var item in SurfaceLines)
            {
                item.Selected = false;
            }
            SurfaceLineDataGrid.Invalidate();
        }

        private class SurfaceLineSelectionRow
        {
            public SurfaceLineSelectionRow(RingtoetsPipingSurfaceLine surfaceLine)
            {
                Selected = false;
                Name = surfaceLine.Name;
                SurfaceLine = surfaceLine;
            }

            public bool Selected { get; set; }
            public string Name { get; private set; }
            public readonly RingtoetsPipingSurfaceLine SurfaceLine;
        }
    }
}