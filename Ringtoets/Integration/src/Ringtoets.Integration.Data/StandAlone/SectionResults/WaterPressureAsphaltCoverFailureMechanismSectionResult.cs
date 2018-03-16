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

using Core.Common.Base.Data;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Integration.Data.StandAlone.SectionResults
{
    /// <summary>
    /// This class holds information about the result of a calculation on section level for the
    /// Water Pressure on Asphalt failure mechanism.
    /// </summary>
    public class WaterPressureAsphaltCoverFailureMechanismSectionResult : FailureMechanismSectionResult
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="WaterPressureAsphaltCoverFailureMechanismSectionResult"/>.
        /// </summary>
        public WaterPressureAsphaltCoverFailureMechanismSectionResult(FailureMechanismSection section) : base(section)
        {
            SimpleAssessmentResult = SimpleAssessmentResultType.None;
            TailorMadeAssessmentResult = TailorMadeAssessmentResultType.None;
            ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.None;
            AssessmentLayerThree = RoundedDouble.NaN;
        }

        /// <summary>
        /// Gets or sets the simple assessment result.
        /// </summary>
        public SimpleAssessmentResultType SimpleAssessmentResult { get; set; }

        /// <summary>
        /// Gets or sets the tailor made assessment result.
        /// </summary>
        public TailorMadeAssessmentResultType TailorMadeAssessmentResult { get; set; }

        /// <summary>
        /// Gets or sets the indicator whether the combined assembly should be overwritten by <see cref="ManualAssemblyCategoryGroup"/>.
        /// </summary>
        public bool UseManualAssemblyCategoryGroup { get; set; }

        /// <summary>
        /// Gets or sets the manually selected assembly category group.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup ManualAssemblyCategoryGroup { get; set; }

        /// <summary>
        /// Gets or sets the value of the tailored assessment of safety.
        /// </summary>
        public RoundedDouble AssessmentLayerThree { get; set; }
    }
}