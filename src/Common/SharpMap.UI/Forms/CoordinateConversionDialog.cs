using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace SharpMap.UI.Forms
{
    public partial class CoordinateConversionDialog : Form
    {
        private ICoordinateSystem fromCS;
        private ICoordinateSystem toCS;

        public ICoordinateSystem FromCS
        {
            get { return fromCS; }
            set
            {
                fromCS = value;
                txtFromCS.Text = fromCS != null ? fromCS.Name : "<none>";
            }
        }

        public ICoordinateSystem ToCS
        {
            get { return toCS; }
            set
            {
                toCS = value;
                txtToCS.Text = toCS != null ? toCS.Name : "<none>";
            }
        }

        private IList<ICoordinateSystem> AvailableCoordinateSystems { get; set; }

        private Func<ICoordinateSystem, ICoordinateSystem, ICoordinateTransformation> CreateTransformation { get; set; }
        
        private Func<ICoordinateSystem, bool> CoordinateSystemFilter { get; set; }

        public CoordinateConversionDialog(ICoordinateSystem targetCS, ICoordinateSystem sourceCS,
                                          IList<ICoordinateSystem> availableCoordinateSystems,
                                          Func<ICoordinateSystem, ICoordinateSystem, ICoordinateTransformation>
                                              createTransformation)
        {
            InitializeComponent();
            FromCS = sourceCS;
            AvailableCoordinateSystems = availableCoordinateSystems;
            CreateTransformation = createTransformation;
            ToCS = targetCS;
        }

        public void SwitchToExportDialog()
        {
            rbAsIs.Text=rbAsIs.Text.Replace("Import", "Export");
        }

        public ICoordinateTransformation ResultTransformation
        {
            get
            {
                return rbDoTransformation.Checked && FromCS != null && ToCS != null
                           ? CreateTransformation(FromCS, ToCS)
                           : null;
            }
        }

        private void btnChooseFromCS_Click(object sender, EventArgs e)
        {
            ICoordinateSystem newSystem = null;
            if (LetUserPickCoordinateSystem(ref newSystem))
                FromCS = newSystem;
        }

        private void btnChooseToCS_Click(object sender, EventArgs e)
        {
            ICoordinateSystem newSystem = null;
            if (LetUserPickCoordinateSystem(ref newSystem))
                ToCS = newSystem;
        }

        private bool LetUserPickCoordinateSystem(ref ICoordinateSystem newSystem)
        {
            using (
                var selectDialog = new SelectCoordinateSystemDialog(AvailableCoordinateSystems, Map.CoordinateSystemFactory.CustomCoordinateSystems)
                    {
                        CoordinateSystemFilter = CoordinateSystemFilter
                    })
            {
                if (selectDialog.ShowDialog() == DialogResult.OK)
                {
                    newSystem = selectDialog.SelectedCoordinateSystem;
                    return true;
                }
            }
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void rbDoTransformation_CheckedChanged(object sender, EventArgs e)
        {
            txtFromCS.Enabled = rbDoTransformation.Checked;
            txtToCS.Enabled = rbDoTransformation.Checked;
            btnChooseFromCS.Enabled = rbDoTransformation.Checked;
            btnChooseToCS.Enabled = rbDoTransformation.Checked;
        }
    }
}
