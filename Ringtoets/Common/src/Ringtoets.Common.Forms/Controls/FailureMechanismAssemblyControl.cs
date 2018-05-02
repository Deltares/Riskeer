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
using Core.Common.Util;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.Controls
{
    /// <summary>
    /// Custom control to display a <see cref="FailureMechanismAssembly"/>.
    /// </summary>
    public partial class FailureMechanismAssemblyControl : AssemblyResultControl
    {
        public FailureMechanismAssemblyControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the values of <paramref name="assembly"/> to the control.
        /// </summary>
        /// <param name="assembly">The <see cref="FailureMechanismAssembly"/> to set on the control.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is <c>null</c>.</exception>
        public void SetAssemblyResult(FailureMechanismAssembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            GroupLabel.Text = new EnumDisplayWrapper<FailureMechanismAssemblyCategoryGroup>(assembly.Group).DisplayName;
            GroupLabel.BackColor = AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(assembly.Group);

            probabilityLabel.Text = new NoProbabilityValueDoubleConverter().ConvertToString(assembly.Probability);
        }

        public override void SetError(string error)
        {
            base.SetError(error);
            probabilityLabel.Text = Resources.RoundedDouble_No_result_dash;
        }
    }
}