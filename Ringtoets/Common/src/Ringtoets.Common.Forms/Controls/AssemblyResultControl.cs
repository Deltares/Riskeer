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
    /// Base control to display an assembly result.
    /// </summary>
    public abstract partial class AssemblyResultControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyResultControl"/>.
        /// </summary>
        protected AssemblyResultControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Dock = DockStyle.Left;
        }

        /// <summary>
        /// Sets the error message of the control.
        /// </summary>
        /// <param name="errorMessage">The error message to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorMessage"/>
        /// is <c>null</c>.</exception>
        public void SetError(string errorMessage)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }

            errorProvider.SetError(this, errorMessage);
        }

        /// <summary>
        /// Sets the warning message of the control.
        /// </summary>
        /// <param name="warningMessage">The warning message to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="warningMessage"/>
        /// is <c>null</c>.</exception>
        public void SetWarning(string warningMessage)
        {
            if (warningMessage == null)
            {
                throw new ArgumentNullException(nameof(warningMessage));
            }

            warningProvider.SetIconPadding(this, string.IsNullOrEmpty(errorProvider.GetError(this)) ? 4 : 24);
            warningProvider.SetError(this, warningMessage);
        }

        /// <summary>
        /// Clears the messages of the control.
        /// </summary>
        public void ClearMessages()
        {
            errorProvider.SetError(this, string.Empty);
            warningProvider.SetError(this, string.Empty);
        }

        /// <summary>
        /// Clears the assembly result of the control.
        /// </summary>
        public virtual void ClearAssemblyResult()
        {
            GroupLabel.Text = string.Empty;
            GroupLabel.BackColor = Color.White;
        }
    }
}