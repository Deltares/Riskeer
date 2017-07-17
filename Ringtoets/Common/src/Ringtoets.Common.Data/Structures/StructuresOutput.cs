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
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Data.Structures
{
    /// <summary>
    /// This class contains the result of a calculation for a <see cref="StructuresCalculation{T}"/>.
    /// </summary>
    public class StructuresOutput
    {
        /// <summary>
        /// Creates an instance of <see cref="StructuresOutput"/>.
        /// </summary>
        /// <param name="probabilityAssessmentOutput">The results of the probabilistic
        /// assessment calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="probabilityAssessmentOutput"/> is <c>null</c>.</exception>
        public StructuresOutput(ProbabilityAssessmentOutput probabilityAssessmentOutput)
        {
            if (probabilityAssessmentOutput == null)
            {
                throw new ArgumentNullException(nameof(probabilityAssessmentOutput));
            }
            ProbabilityAssessmentOutput = probabilityAssessmentOutput;
        }

        /// <summary>
        /// Gets the probabilistic assessment output.
        /// </summary>
        public ProbabilityAssessmentOutput ProbabilityAssessmentOutput { get; }

        /// <summary>
        /// Gets the value indicating whether the output contains illustration points.
        /// </summary>
        public bool HasIllustrationPoints
        {
            get
            {
                return GeneralFaultTreeIllustrationPoint != null;
            }
        }

        /// <summary>
        /// Gets the general result with the fault tree illustration points.
        /// </summary>
        public GeneralResult<TopLevelFaultTreeIllustrationPoint> GeneralFaultTreeIllustrationPoint { get; private set; }

        /// <summary>
        /// Sets the general result of this output with the fault tree illustration points.
        /// </summary>
        /// <param name="generalResultFaultTreeIllustrationPoint">The general result which belongs
        /// to this output.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalResultFaultTreeIllustrationPoint"/>
        /// is <c>null</c>.</exception>
        public void SetIllustrationPoints(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResultFaultTreeIllustrationPoint)
        {
            if (generalResultFaultTreeIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(generalResultFaultTreeIllustrationPoint));
            }

            GeneralFaultTreeIllustrationPoint = generalResultFaultTreeIllustrationPoint;
        }
    }
}