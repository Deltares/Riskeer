// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms
{
    /// <summary>
    /// Dialog that can be used to ask the user to select from a collection of options.
    /// </summary>
    public partial class SelectItemDialog : DialogBase
    {
        private readonly string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectItemDialog"/> class.
        /// </summary>
        /// <param name="dialogParent">The dialog parent for which this dialog should be 
        /// shown on top.</param>
        /// <param name="text">The text to show in the dialog.</param>
        public SelectItemDialog(IWin32Window dialogParent, string text) : base(dialogParent, Resources.ExportIcon, 320, 220)
        {
            InitializeComponent();

            this.text = text;
        }

        /// <summary>
        /// Gets the data object corresponding to the item selected by the user or null if
        /// no selection was made.
        /// </summary>
        public object SelectedItemTag
        {
            get
            {
                return SelectedItem?.Tag;
            }
        }

        /// <summary>
        /// Gets the name of the selected item or null if no selection was made.
        /// </summary>
        public string SelectedItemTypeName
        {
            get
            {
                return SelectedItem?.Name;
            }
        }

        /// <summary>
        /// Adds an option element to the dialog.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="category">The category of the element.</param>
        /// <param name="image">The image of the element.</param>
        /// <param name="tag">The data corresponding to the element.</param>
        public void AddItemType(string name, string category, Image image, object tag)
        {
            if (!ContainsCategory(category))
            {
                listViewItemTypes.Groups.Add(new ListViewGroup(category, category));
            }

            ListViewGroup group = listViewItemTypes.Groups[category];

            imageList.Images.Add(category + "-" + name, image);
            listViewItemTypes.Items.Add(name, name, imageList.Images.Count - 1);
            listViewItemTypes.Items[listViewItemTypes.Items.Count - 1].Group = group;
            listViewItemTypes.Items[listViewItemTypes.Items.Count - 1].Tag = tag;
        }

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }

        protected override void OnLoad(EventArgs e)
        {
            Text = text;

            base.OnLoad(e);
        }

        private ListViewItem SelectedItem
        {
            get
            {
                if (listViewItemTypes.SelectedIndices.Count == 0)
                {
                    return null;
                }

                int selectedIndex = listViewItemTypes.SelectedIndices[0];
                return listViewItemTypes.Items[selectedIndex];
            }
        }

        private bool ContainsCategory(string category)
        {
            return listViewItemTypes.Groups
                                    .Cast<ListViewGroup>()
                                    .Any(listViewGroup => listViewGroup.Header == category);
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show(Resources.SelectItemDialog_buttonOk_Click_Please_select_an_item,
                                Resources.SelectItemDialog_buttonOk_Click_Error,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void ListViewItemTypesDoubleClick(object sender, EventArgs e)
        {
            buttonOk.PerformClick();
        }
    }
}