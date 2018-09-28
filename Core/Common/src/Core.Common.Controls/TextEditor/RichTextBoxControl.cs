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
using System.Windows.Forms;

namespace Core.Common.Controls.TextEditor
{
    /// <summary>
    /// Wrapper for the <see cref="RichTextBox"/>.
    /// </summary>
    public partial class RichTextBoxControl : UserControl
    {
        private bool loaded;
        private string rtfToSetAfterLoad;

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

            Load += OnLoad;
        }

        /// <summary>
        /// Gets or sets the Rtf from the <see cref="RichTextBox"/>.
        /// </summary>
        public string Rtf
        {
            get
            {
                return loaded ? richTextBox.Rtf : rtfToSetAfterLoad;
            }
            set
            {
                if (loaded)
                {
                    richTextBox.Rtf = value;
                }
                else
                {
                    rtfToSetAfterLoad = value;
                }
            }
        }

        /// <summary>
        /// This is needed for the RichTextBox to apply styling.
        /// </summary>
        private void OnLoad(object sender, EventArgs eventArgs)
        {
            loaded = true;

            if (!string.IsNullOrEmpty(rtfToSetAfterLoad))
            {
                richTextBox.Rtf = rtfToSetAfterLoad;
            }
        }

        #region Event handling

        private void OnTextChanged(object sender, EventArgs e)
        {
            OnTextBoxValueChanged(e);
        }

        private void OnTextBoxValueChanged(EventArgs e)
        {
            TextBoxValueChanged?.Invoke(this, e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.B:
                        e.Handled = true;
                        SetStyle(FontStyle.Bold);
                        break;
                    case Keys.I:
                        e.SuppressKeyPress = true;
                        e.Handled = true;
                        SetStyle(FontStyle.Italic);
                        break;
                    case Keys.U:
                        e.Handled = true;
                        SetStyle(FontStyle.Underline);
                        break;
                }
            }
        }

        #endregion

        #region Fontstyle

        private void SetStyle(FontStyle newFontStyle)
        {
            richTextBox.SelectionFont = CreateFontWithToggledStyle(richTextBox.SelectionFont, newFontStyle);
        }

        private static Font CreateFontWithToggledStyle(Font originalFont, FontStyle newFontStyle)
        {
            FontStyle newStyle = originalFont.Style ^ newFontStyle;
            return new Font(originalFont, newStyle);
        }

        #endregion
    }
}