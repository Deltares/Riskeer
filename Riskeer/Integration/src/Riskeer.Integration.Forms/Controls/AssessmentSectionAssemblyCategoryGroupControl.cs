// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Controls;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Integration.Forms.Controls
{
    /// <summary>
    /// Control to display a <see cref="AssessmentSectionAssemblyCategoryGroup"/>.
    /// </summary>
    public class AssessmentSectionAssemblyCategoryGroupControl : AssemblyResultWithProbabilityControl
    {
        private readonly NoProbabilityValueDoubleConverter converter = new NoProbabilityValueDoubleConverter();
        
        /// <summary>
        /// Sets the value of <paramref name="result"/> on the control.
        /// </summary>
        /// <param name="result">The <see cref="AssessmentSectionAssemblyResult"/> to set on the control.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="result"/>
        /// has an invalid value for <see cref="AssessmentSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="result"/>
        /// is not supported.</exception>
        public void SetAssemblyResult(AssessmentSectionAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            GroupLabel.Text = new EnumDisplayWrapper<AssessmentSectionAssemblyCategoryGroup>(result.AssemblyCategoryGroup).DisplayName;
            GroupLabel.BackColor = AssemblyCategoryGroupColorHelper.GetAssessmentSectionAssemblyCategoryGroupColor(result.AssemblyCategoryGroup);

            ProbabilityLabel.Text = converter.ConvertToString(result.Probability);
        }
    }
}