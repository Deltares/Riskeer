// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ringtoets.Common.Forms.Controls
{
    /// <summary>
    /// Custom base control to display an assembly result.
    /// </summary>
    public abstract partial class AssemblyResultControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultControl"/>.
        /// </summary>
        protected AssemblyResultControl()
        {
            InitializeComponent();
            Dock = DockStyle.Left;
        }

        /// <summary>
        /// Clears the error message of the control.
        /// </summary>
        public void ClearError()
        {
            ErrorProvider.SetError(this, string.Empty);
        }

        /// <summary>
        /// Sets the error message of the control and clears its values.
        /// </summary>
        /// <param name="error">The error message to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/>
        /// is <c>null</c>.</exception>
        public virtual void SetError(string error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            ErrorProvider.SetError(this, error);
            GroupLabel.Text = string.Empty;
            GroupLabel.BackColor = Color.White;
        }
    }
}