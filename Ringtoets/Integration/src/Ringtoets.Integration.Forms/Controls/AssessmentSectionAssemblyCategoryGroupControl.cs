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
using System.ComponentModel;
using Core.Common.Util;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Integration.Forms.Controls
{
    /// <summary>
    /// Control to display a <see cref="AssessmentSectionAssemblyCategoryGroup"/>.
    /// </summary>
    public class AssessmentSectionAssemblyCategoryGroupControl : AssemblyResultControl
    {
        /// <summary>
        /// Set the value of <paramref name="result"/> on the control.
        /// </summary>
        /// <param name="result">The <see cref="AssessmentSectionAssemblyCategoryGroup"/> to set on the control.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="result"/>
        /// has an invalid value for <see cref="AssessmentSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="result"/>
        /// is not supported.</exception>
        public void SetAssemblyResult(AssessmentSectionAssemblyCategoryGroup result)
        {
            GroupLabel.Text = new EnumDisplayWrapper<AssessmentSectionAssemblyCategoryGroup>(result).DisplayName;
            GroupLabel.BackColor = AssemblyCategoryGroupColorHelper.GetAssessmentSectionAssemblyCategoryGroupColor(result);
        }
    }
}