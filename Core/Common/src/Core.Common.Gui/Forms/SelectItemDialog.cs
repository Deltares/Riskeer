// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms
{
    public partial class SelectItemDialog : DialogBase
    {
        public SelectItemDialog(IWin32Window dialogParent) : base(dialogParent, Resources.plus, 320, 220)
        {
            InitializeComponent();

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

        protected override Button GetCancelButton()
        {
            return buttonCancel;
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
    }
}