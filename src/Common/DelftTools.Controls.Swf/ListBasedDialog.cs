using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    /// <summary>
    /// TODO: rename to SelectItemDialog!
    /// </summary>
    public partial class ListBasedDialog : Form
    {
        public ListBasedDialog()
        {
            InitializeComponent();
        }

        public SelectionMode SelectionMode
        {
            get
            {
                return listBox.SelectionMode;
            }
            set
            {
                listBox.SelectionMode = value;
            }
        }

        ///<summary>
        /// Set this property to the object to be displayed in the list box.
        ///</summary>
        public IList DataSource
        {
            get
            {
                return listBox.DataSource as IList;
            }
            set
            {
                listBox.DataSource = value;
            }
        }

        ///<summary>
        /// Use this property to specify which member from the DataSource will be displayed in the list box.
        ///</summary>
        public string DisplayMember
        {
            get
            {
                return listBox.DisplayMember;
            }
            set
            {
                listBox.DisplayMember = value;
            }
        }

        public IList<bool> CheckedItems
        {
            get
            {
                int itemsCount = listBox.Items.Count;
                IList<bool> items = new bool[listBox.Items.Count];
                for (int i = 0; i < itemsCount; i++)
                {
                    items[i] = listBox.GetSelected(i);
                }
                return items;
            }
        }

        public IList SelectedItems
        {
            get
            {
                IList dataSourceList = DataSource;
                if (dataSourceList == null)
                {
                    return null;
                }

                IList resultList = new ArrayList();
                for (int i = 0; i < dataSourceList.Count; i++)
                {
                    if (listBox.GetSelected(i))
                    {
                        resultList.Add(DataSource[i]);
                    }
                }
                return resultList;
            }
        }

        public IEnumerable<int> SelectedItemIndices
        {
            get
            {
                IList dataSourceList = DataSource;
                if (dataSourceList == null)
                {
                    yield break;
                }

                for (int i = 0; i < dataSourceList.Count; i++)
                {
                    if (listBox.GetSelected(i))
                    {
                        yield return i;
                    }
                }
            }
        }

        /// <summary>
        /// Double click is handled as select item and ok for single selection listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((listBox.SelectionMode == SelectionMode.One) && (listBox.SelectedItems.Count != 0))
            {
                buttonOk.PerformClick();
            }
        }
    }
}