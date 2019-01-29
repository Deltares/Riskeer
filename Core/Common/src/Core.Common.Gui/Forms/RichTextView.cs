// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.IO;
using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Forms
{
    /// <summary>
    /// A view to display a <see cref="RichTextFile"/> instance.
    /// </summary>
    public partial class RichTextView : UserControl, IView
    {
        private RichTextFile richTextFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextView"/> class.
        /// </summary>
        public RichTextView()
        {
            InitializeComponent();
        }

        #region IView Members

        public object Data
        {
            get
            {
                return richTextFile;
            }
            set
            {
                richTextFile = value as RichTextFile;
                if (richTextFile != null && File.Exists(richTextFile.FilePath))
                {
                    richTextBox1.LoadFile(richTextFile.FilePath);
                }
                else
                {
                    richTextBox1.Clear();
                }
            }
        }

        #endregion
    }
}