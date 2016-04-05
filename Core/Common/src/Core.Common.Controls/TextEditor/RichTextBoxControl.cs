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
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Core.Common.Controls.TextEditor
{
    /// <summary>
    /// Wrapper for the <see cref="RichTextBox"/>.
    /// </summary>
    public partial class RichTextBoxControl : UserControl
    {
        /// <summary>
        /// The event which is send when the text changes.
        /// </summary>
        public event EventHandler TextBoxValueChanged;

        /// <summary>
        /// Creates a new instance of <see cref="RichTextBoxControl"/>.
        /// </summary>
        public RichTextBoxControl()
        {
            InitializeComponent();

            richTextBox.TextChanged += OnTextChanged;
            richTextBox.KeyDown += OnKeyDown;
        }

        public string Rtf
        {
            get
            {
                return richTextBox.Rtf;
            }
            set
            {
                richTextBox.Rtf = value;
            }
        }

        #region Event handling

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (TextBoxValueChanged != null)
            {
                TextBoxValueChanged(sender, e);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                Font existingFont = richTextBox.SelectionFont;

                switch (e.KeyCode)
                {
                    case Keys.B:
                        e.Handled = true;
                        SetStyle(FontStyle.Bold, !existingFont.Bold);
                        break;
                    case Keys.I:
                        e.SuppressKeyPress = true;
                        e.Handled = true;
                        SetStyle(FontStyle.Italic, !existingFont.Italic);
                        break;
                    case Keys.U:
                        e.Handled = true;
                        SetStyle(FontStyle.Underline, !existingFont.Underline);
                        break;
                }
            }
        }

        #endregion

        #region Fontstyle

        private void SetStyle(FontStyle fontStyle, bool setStyle = false)
        {
            int txtStartPosition = richTextBox.SelectionStart;
            int selectionLength = richTextBox.SelectionLength;
            if (selectionLength > 0)
            {
                using (RichTextBox txtTemp = new RichTextBox())
                {
                    txtTemp.Rtf = richTextBox.SelectedRtf;
                    for (int i = 0; i < selectionLength; ++i)
                    {
                        txtTemp.Select(i, 1);
                        txtTemp.SelectionFont = RenderFont(txtTemp.SelectionFont, fontStyle, setStyle);
                    }

                    txtTemp.Select(0, selectionLength);
                    richTextBox.SelectedRtf = txtTemp.SelectedRtf;
                    richTextBox.Select(txtStartPosition, selectionLength);
                }
            }
        }

        private Font RenderFont(Font originalFont, FontStyle fontStyle, bool setStyle)
        {
            FontStyle newStyle;

            if (originalFont != null && setStyle)
            {
                newStyle = originalFont.Style | fontStyle;
            }
            else
            {
                newStyle = originalFont.Style & ~fontStyle;
            }

            return new Font(originalFont.FontFamily.Name, originalFont.Size, newStyle);
        }

        #endregion
    }
}