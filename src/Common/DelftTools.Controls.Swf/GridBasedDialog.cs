using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    /// <summary>
    /// TODO: this class name seems to be very non-intuitive, find a better name, discuss design.
    /// GridBasedDialog implements a simple form with2 grids with a master slave relation
    /// The slave grid is optional.
    /// Typically the grids are bound to a datasource. The caller is responsible for responding
    /// to selection changes.
    /// </summary>
    public partial class GridBasedDialog : Form
    {
        ///<summary>
        /// This event is raised when the selection in the master grid changes
        ///</summary>
        public event System.EventHandler MasterSelectionChanged;

        ///<summary>
        /// This event is raised when the selection in the slave grid changes
        ///</summary>
        public event System.EventHandler SlaveSelectionChanged;

        ///<summary>
        /// Set to true to support multiple selection in the master grid; default is false.
        ///</summary>
        public bool MasterMultiSelect
        {
            get { return dataGridViewMaster.MultiSelect; }
            set { dataGridViewMaster.MultiSelect = value; }
        }

        ///<summary>
        /// Set to true to support multiple selection in the slave grid; default is false.
        ///</summary>
        public bool SlaveMultiSelect
        {
            get { return dataGridViewSlave.MultiSelect; }
            set { dataGridViewSlave.MultiSelect = value; }
        }

        /// <summary>
        /// The text used to name the master grid in the form
        /// </summary>
        public string MasterTitle
        {
            get { return groupBoxMaster.Text; }
            set { groupBoxMaster.Text = value; }
        }

        /// <summary>
        /// The text used to name the slave grid in the form
        /// </summary>
        public string SlaveTitle
        {
            get { return groupBoxSlave.Text; }
            set { groupBoxSlave.Text = value; }
        }
        ///<summary>
        /// Set to true if doubleclick in a grid should close the form with DialogResult.OK if there is 
        /// valid selection.
        ///</summary>
        public bool MouseDoubleClickIsOk { get; set; }

        ///<summary>
        /// Default contructor for the form
        ///</summary>
        public GridBasedDialog()
        {
            InitializeComponent();
            dataGridViewMaster.SelectionChanged += MasterDataGridViewSelectionChanged;
            dataGridViewSlave.SelectionChanged += SlaveDataGridViewSelectionChanged;
        }

        ///<summary>
        /// Set to true to only use the master grid
        ///</summary>
        public bool SingleList
        {
            get { return splitContainer1.Panel2Collapsed; }
            set { splitContainer1.Panel2Collapsed = value; }
        }

        void MasterDataGridViewSelectionChanged(object sender, System.EventArgs e)
        {
            if (null != MasterSelectionChanged)
            {
                MasterSelectionChanged(this, e);
            }
        }

        void SlaveDataGridViewSelectionChanged(object sender, System.EventArgs e)
        {
            if (null != SlaveSelectionChanged)
            {
                SlaveSelectionChanged(this, e);
            }
        }

        ///<summary>
        /// Set this property to the objects to be displayed in the master grid.
        ///</summary>
        public IEnumerable MasterDataSource
        {
            get { return (IEnumerable) dataGridViewMaster.DataSource; }
            set { dataGridViewMaster.DataSource = value; }
        }

        ///<summary>
        /// Set this property to the objects to be displayed in the slave grid.
        /// Typically the caller of the form set this property when the selection
        /// in the master grid changes.
        ///</summary>
        public IEnumerable SlaveDataSource
        {
            get { return (IEnumerable)dataGridViewSlave.DataSource; }
            set { dataGridViewSlave.DataSource = value; }
        }

        private static IEnumerable<int> DataGridViewSelectedIndices(DataGridView dataGridView)
        {
            IList<int> selectedIndices = new List<int>();
            
            foreach (DataGridViewRow s in dataGridView.SelectedRows)
            {
                selectedIndices.Add(dataGridView.Rows.IndexOf(s));
            }
            return selectedIndices;
        }

        ///<summary>
        /// The indices of the rows that are selected in the master grid
        ///</summary>
        public IEnumerable<int> MasterSelectedIndices
        {
            get { return DataGridViewSelectedIndices(dataGridViewMaster); }
        }

        ///<summary>
        /// The indices of the rows that are selected in the slave grid
        ///</summary>
        public IEnumerable<int> SlaveSelectedIndices
        {
            get { return DataGridViewSelectedIndices(dataGridViewSlave); }
        }

        private void dataGridViewSlave_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((!MouseDoubleClickIsOk) || (!MasterSelectedIndices.Any()))
                return;
            if ((!SingleList) && ((!SlaveSelectedIndices.Any())))
                return;
            DialogResult = DialogResult.OK;
            Close();
        }

        public void SetMasterSelectedIndices(IEnumerable<int> pSelectedIndices)
        {
            var selectedIndices = pSelectedIndices as int[] ?? pSelectedIndices.ToArray();

            foreach (DataGridViewRow row in dataGridViewMaster.Rows)
            {
                row.Selected = selectedIndices.Contains(row.Index);
            }
        }
    }
}