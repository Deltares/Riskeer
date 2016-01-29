// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// All rights preserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms
{
    public partial class SelectViewDialog : DialogBase
    {
        private IList<string> items;

        public SelectViewDialog(IWin32Window owner) : base(owner, Resources.arrow_000_medium_question_mark, 350, 200)
        {
            InitializeComponent();

            Font lbFont = listBox.Font;
            var defaultItemFont = new Font(lbFont.FontFamily, lbFont.Size, FontStyle.Bold);
            var g = listBox.CreateGraphics();
            var itemSize = g.MeasureString("TEST", defaultItemFont);

            listBox.ItemHeight = (int) itemSize.Height;
        }

        public IList<string> Items
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

        public string SelectedItem
        {
            get
            {
                return (string) listBox.SelectedItem;
            }
            set
            {
                listBox.SelectedItem = value;
            }
        }

        public string DefaultViewName { get; set; }

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            string itemAsString = listBox.Items[e.Index].ToString();
            Font lbFont = listBox.Font;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            e.DrawBackground();

            if (itemAsString == DefaultViewName)
            {
                string defaultIndicatorText = Resources.SelectViewDialog_listBox_DrawItem_Default;
                var defaultItemFont = new Font(lbFont.FontFamily, lbFont.Size, FontStyle.Bold);

                var itemSize = e.Graphics.MeasureString(itemAsString, defaultItemFont);
                var indicatorSize = e.Graphics.MeasureString(defaultIndicatorText, defaultItemFont);

                var boundsIndicator = new RectangleF(e.Bounds.Left + itemSize.Width,
                                                     e.Bounds.Top, indicatorSize.Width, e.Bounds.Height);

                e.Graphics.DrawString(itemAsString, defaultItemFont, (selected) ? new SolidBrush(SystemColors.HighlightText) : Brushes.Black, e.Bounds);
                e.Graphics.DrawString(defaultIndicatorText, defaultItemFont, Brushes.LightGray, boundsIndicator);
            }
            else
            {
                e.Graphics.DrawString(itemAsString, lbFont, (selected) ? new SolidBrush(SystemColors.HighlightText) : Brushes.Black, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxDefault.Checked = (listBox.SelectedItem != null &&
                                       listBox.SelectedItem.ToString() == DefaultViewName);
        }

        private void checkBoxDefault_CheckedChanged(object sender, EventArgs e)
        {
            string previousName = DefaultViewName;
            if (listBox.SelectedItem.ToString() == DefaultViewName)
            {
                DefaultViewName = (checkBoxDefault.Checked)
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