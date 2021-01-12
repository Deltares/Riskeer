// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Forms.PropertyClasses.Probabilistic
{
    /// <summary>
    /// ViewModel of profile specific <see cref="PartialProbabilisticFaultTreePipingOutput"/> for properties panel.
    /// </summary>
    public class ProbabilisticFaultTreePipingProfileSpecificOutputProperties : ProbabilisticPipingProfileSpecificOutputProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticFaultTreePipingProfileSpecificOutputProperties"/>.
        /// </summary>
        /// <param name="output">The output to show the properties for.</param>
        /// <param name="calculation">The calculation the output belongs to.</param>
        /// <param name="failureMechanism">The failure mechanism the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section the output belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ProbabilisticFaultTreePipingProfileSpecificOutputProperties(
            PartialProbabilisticFaultTreePipingOutput output,
            ProbabilisticPipingCalculationScenario calculation,
            PipingFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
            : base(output, calculation, failureMechanism, assessmentSection) {}
    }
}