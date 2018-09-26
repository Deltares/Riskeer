// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms
{
    /// <summary>
    /// Dialog for selecting a view for a given piece of data.
    /// </summary>
    public partial class SelectViewDialog : DialogBase
    {
        private List<string> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectViewDialog"/> class.
        /// </summary>
        /// <param name="dialogParent">The dialog parent for which this dialog should be
        /// shown on top.</param>
        public SelectViewDialog(IWin32Window dialogParent) : base(dialogParent, Resources.arrow_000_medium_question_mark, 350, 200)
        {
            InitializeComponent();

            Font lbFont = listBox.Font;
            Graphics g = listBox.CreateGraphics();

            using (var defaultItemFont = new Font(lbFont.FontFamily, lbFont.Size, FontStyle.Bold))
            {
                SizeF itemSize = g.MeasureString("TEST", defaultItemFont);
                listBox.ItemHeight = (int) itemSize.Height;
            }
        }

        /// <summary>
        /// Gets or sets the items to select from.
        /// </summary>
        public List<string> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                listBox.DataSource = items;
                listBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public string SelectedItem
        {
            get
            {
                return (string) listBox.SelectedItem;
            }
        }

        /// <summary>
        /// Gets or sets the name of the default view for the data.
        /// </summary>
        public string DefaultViewName { get; set; }

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }

        private void ListBoxDoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ListBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            string itemAsString = listBox.Items[e.Index].ToString();
            Font lbFont = listBox.Font;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            e.DrawBackground();

            if (itemAsString == DefaultViewName)
            {
                string defaultIndicatorText = Resources.SelectViewDialog_listBox_DrawItem_Default;
                var defaultItemFont = new Font(lbFont.FontFamily, lbFont.Size, FontStyle.Bold);

                SizeF itemSize = e.Graphics.MeasureString(itemAsString, defaultItemFont);
                SizeF indicatorSize = e.Graphics.MeasureString(defaultIndicatorText, defaultItemFont);

                var boundsIndicator = new RectangleF(e.Bounds.Left + itemSize.Width,
                                                     e.Bounds.Top, indicatorSize.Width, e.Bounds.Height);

                e.Graphics.DrawString(itemAsString, defaultItemFont, selected ? new SolidBrush(SystemColors.HighlightText) : Brushes.Black, e.Bounds);
                e.Graphics.DrawString(defaultIndicatorText, defaultItemFont, Brushes.LightGray, boundsIndicator);
            }
            else
            {
                e.Graphics.DrawString(itemAsString, lbFont, selected ? new SolidBrush(SystemColors.HighlightText) : Brushes.Black, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void ListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxDefault.Checked = listBox.SelectedItem != null &&
                                      listBox.SelectedItem.ToString() == DefaultViewName;
        }

        private void CheckBoxDefaultCheckedChanged(object sender, EventArgs e)
        {
            string previousName = DefaultViewName;
            if (listBox.SelectedItem.ToString() == DefaultViewName)
            {
                DefaultViewName = checkBoxDefault.Checked
                                      ? listBox.SelectedItem.ToString()
                                      : null;
            }
            else
            {
                if (checkBoxDefault.Checked)
                {
                    DefaultViewName = listBox.SelectedItem.ToString();
                }
            }

            if (previousName != DefaultViewName)
            {
                listBox.Refresh();
            }
        }
    }
}