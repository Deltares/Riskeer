﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.ComponentModel;
using Core.Common.Util;
using Riskeer.Common.Forms.Helpers;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Forms.Controls
{
    /// <summary>
    /// Control to display a <see cref="FailureMechanismAssemblyCategoryGroup"/>.
    /// </summary>
    public class FailureMechanismAssemblyCategoryGroupControl : AssemblyResultControl
    {
        /// <summary>
        /// Sets the value of <paramref name="result"/> on the control.
        /// </summary>
        /// <param name="result">The <see cref="FailureMechanismAssemblyCategoryGroup"/> to set on the control.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="result"/>
        /// has an invalid value for <see cref="FailureMechanismAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="result"/>
        /// is not supported.</exception>
        public void SetAssemblyResult(FailureMechanismAssemblyCategoryGroup result)
        {
            GroupLabel.Text = new EnumDisplayWrapper<FailureMechanismAssemblyCategoryGroup>(result).DisplayName;
            GroupLabel.BackColor = AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(result);
        }
    }
}