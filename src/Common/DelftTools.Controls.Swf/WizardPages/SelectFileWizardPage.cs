using System;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.WizardPages
{
    ///<summary>
    /// Use to add select file page to a wizard
    ///</summary>
    public partial class SelectFileWizardPage : UserControl, IWizardPage
    {
        private string fileName;

        public event EventHandler FileSelected;

        ///<summary>
        ///</summary>
        public SelectFileWizardPage()
        {
            InitializeComponent();
        }
        ///<summary>
        /// File used in the openFileDialog
        ///</summary>
        public virtual string Filter
        {
            get { return openFileDialog.Filter; }
            set { openFileDialog.Filter = value; }
        }

        public virtual string FileName 
        {
            get { return fileName; }
            set {}
        }
        
        public bool CanFinish()
        {
            return CanDoNext();
        }

        public virtual bool CanDoNext()
        {
            return textBox1.Text != string.Empty && labelErrorMessage.Text == string.Empty;
        }

        ///<summary>
        ///</summary>
        public string FileDescription
        {
            get { return lblDescription.Text; }
            set { lblDescription.Text = value; }
        }

        public bool CanDoPrevious()
        {
            return true;
        }
        
        
        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fileName = openFileDialog.FileName;
                    textBox1.Text = openFileDialog.FileName;
                    labelErrorMessage.Text = "";
                }
                catch (Exception exception)
                {
                    fileName = "";
                    textBox1.Text = "";
                    labelErrorMessage.Text = exception.Message;

                }
                OnFileSelected();
            }
        }

        protected virtual void OnFileSelected()
        {
            if (FileSelected != null)
            {
                FileSelected(this, new EventArgs());
            }
        }
    }
}