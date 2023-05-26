// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Data;

namespace Riskeer.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="HydraulicBoundaryData"/>.
    /// </summary>
    public class HydraulicBoundaryDataContext : ObservableWrappedObjectContextBase<HydraulicBoundaryData>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDataContext"/>.
        /// </summary>
        /// <param name="wrappedData">The <see cref="HydraulicBoundaryData"/> that the <see cref="HydraulicBoundaryDataContext"/> belongs to.</param>
        /// <param name="assessmentSection">The assessment section that the <see cref="HydraulicBoundaryDataContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDataContext(HydraulicBoundaryData wrappedData, AssessmentSection assessmentSection)
            : base(wrappedData)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;
        }

        /// <summary>
        /// Gets the assessment section that the context belongs to.
        /// </summary>
        public AssessmentSection AssessmentSection { get; }
    }
}