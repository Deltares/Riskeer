// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Controls
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
        /// Sets a manual assembly warning message on the control.
        /// </summary>
        public void SetManualAssemblyWarning()
        {
            manualAssemblyWarningProvider.SetIconPadding(this, string.IsNullOrEmpty(errorProvider.GetError(this)) ? 4 : 24);
            manualAssemblyWarningProvider.SetError(this, Resources.ManualAssemblyWarning_FailureMechanismAssemblyResult_is_based_on_manual_assemblies);
        }

        /// <summary>
        /// Clears the messages of the control.
        /// </summary>
        public void ClearMessages()
        {
            errorProvider.SetError(this, string.Empty);
            manualAssemblyWarningProvider.SetError(this, string.Empty);
        }

        /// <summary>
        /// Clears the assembly result of the control.
        /// </summary>
        public virtual void ClearAssemblyResult()
        {
            GroupLabel.Text = string.Empty;
            GroupLabel.BackColor = Color.White;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Dock = DockStyle.Left;
        }
    }
}