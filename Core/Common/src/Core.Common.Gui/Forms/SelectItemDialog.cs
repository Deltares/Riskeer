using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Swf;
using Core.Common.Gui.Properties;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

//using System.Windows.Forms;

namespace Core.Common.Gui.Forms
{
    public partial class SelectItemDialog : Form
    {
        private string newItemName;

        private int previousSelectedItemIndex;

        public SelectItemDialog()
        {
            InitializeComponent();

            ShowExampleCheckBox = false;

            imageList.Images.Clear();
            listViewItemTypes.Clear();
            listViewItemTypes.Groups.Clear();
            listViewItemTypes.HandleCreated += ListViewItemTypesOnHandleCreated;

            // http://blogs.msdn.com/b/oldnewthing/archive/2005/05/03/414317.aspx
            // WM_CHANGEUISTATE, UIS_INITIALIZE
            // Sometimes required by controls, e.g. ListView. If this is not called - dotted focus rectangle is not drawn.
            ControlHelper.SendMessage(Handle, 0x127, 0x30001, 0);
        }

        public object SelectedItemTag
        {
            get
            {
                return SelectedItem.Tag;
            }
        }

        public string SelectedItemTypeName
        {
            get
            {
                return SelectedItem != null ? SelectedItem.Name : null;
            }
        }

        public Func<string, bool> ItemSupportsExample { get; set; }

        public bool IsExample
        {
            get
            {
                return checkBoxExample.Checked;
            }
            set
            {
                checkBoxExample.Checked = value;
            }
        }

        public bool ShowExampleCheckBox
        {
            get
            {
                return checkBoxExample.Visible;
            }
            set
            {
                checkBoxExample.Visible = value;
            }
        }

        public void AddItemType(string name, string category, Image image, object tag)
        {
            if (!ContainsCategory(category))
            {
                listViewItemTypes.Groups.Add(new ListViewGroup(category, category));
            }

            var group = listViewItemTypes.Groups[category];

            imageList.Images.Add(category + "-" + name, image);
            listViewItemTypes.Items.Add(name, name, imageList.Images.Count - 1);
            listViewItemTypes.Items[listViewItemTypes.Items.Count - 1].Group = group;
            listViewItemTypes.Items[listViewItemTypes.Items.Count - 1].Tag = tag;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Down)
            {
                //return true;
            }
            if (keyData == Keys.Up)
            {
                //return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private ListViewItem SelectedItem
        {
            get
            {
                if (listViewItemTypes.SelectedIndices.Count > 1)
                {
                    throw new InvalidOperationException(Resources.SelectItemDialog_SelectedItem_Number_of_selected_items_must_be_1);
                }

                if (listViewItemTypes.SelectedIndices.Count == 0)
                {
                    return null;
                }

                int selectedIndex = listViewItemTypes.SelectedIndices[0];

                return listViewItemTypes.Items[selectedIndex];
            }
        }

        private void ListViewItemTypesOnHandleCreated(object sender, EventArgs eventArgs)
        {
            ControlHelper.SetWindowTheme(listViewItemTypes.Handle, Resources.SelectItemDialog_ListViewItemTypesOnHandleCreated_Explorer, null);
        }

        private bool ContainsCategory(string category)
        {
            foreach (ListViewGroup listViewGroup in listViewItemTypes.Groups)
            {
                if (listViewGroup.Header == category)
                {
                    return true;
                }
            }

            return false;
        }

        private void NewDataDialog_Validating(object sender, CancelEventArgs e) {}

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show(Resources.SelectItemDialog_buttonOk_Click_Please_select_an_item, Resources.SelectItemDialog_buttonOk_Click_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void listViewItemTypes_DoubleClick(object sender, EventArgs e)
        {
            buttonOk.PerformClick();
        }

        private void listViewItemTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItemName = "";

            if (SelectedItem == null)
            {
                newItemName = "";
            }
            else
            {
                previousSelectedItemIndex = listViewItemTypes.SelectedIndices[0];

                selectedItemName = SelectedItem.Text;
                newItemName = Resources.SelectItemDialog_listViewItemTypes_SelectedIndexChanged_New + selectedItemName;
            }

            if (string.IsNullOrEmpty(newItemName))
            {
                ShowExampleCheckBox = false;
            }
            else if (ItemSupportsExample != null)
            {
                ShowExampleCheckBox = ItemSupportsExample(selectedItemName);
            }
        }

        private void checkBoxDemo_CheckedChanged(object sender, EventArgs e) {}
    }
}