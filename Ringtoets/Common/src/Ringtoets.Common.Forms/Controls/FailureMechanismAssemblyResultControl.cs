﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Util;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Common.Forms.Controls
{
    /// <summary>
    /// Custom control to display the assembly result of a failure mechanism.
    /// </summary>
    public partial class FailureMechanismAssemblyResultControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyResultControl"/>.
        /// </summary>
        public FailureMechanismAssemblyResultControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clears the error message of the control.
        /// </summary>
        public void ClearError()
        {
            ErrorProvider.SetError(this, string.Empty);
        }

        /// <summary>
        /// Sets the error message of the control.
        /// </summary>
        /// <param name="error">The error message to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/>
        /// is <c>null</c>.</exception>
        public void SetError(string error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            ErrorProvider.SetError(this, error);
        }

        /// <summary>
        /// Set the values of the <paramref name="assembly"/> to the control.
        /// </summary>
        /// <param name="assembly">The <see cref="FailureMechanismAssembly"/> to set on the control.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        public virtual void SetAssemblyResult(FailureMechanismAssembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            GroupLabel.Text = new EnumDisplayWrapper<FailureMechanismAssemblyCategoryGroup>(assembly.Group).DisplayName;
            GroupLabel.BackColor = AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(assembly.Group);
        }
    }
}