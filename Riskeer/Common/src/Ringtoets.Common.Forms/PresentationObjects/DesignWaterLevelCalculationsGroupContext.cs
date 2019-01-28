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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure multiple enumerations of <see cref="HydraulicBoundaryLocation"/> 
    /// with a design water level calculation result.
    /// </summary>
    public class DesignWaterLevelCalculationsGroupContext : ObservableWrappedObjectContextBase<ObservableList<HydraulicBoundaryLocation>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationsGroupContext"/>.
        /// </summary>
        /// <param name="wrappedData">The locations that the <see cref="DesignWaterLevelCalculationsGroupContext"/> belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that the <see cref="DesignWaterLevelCalculationsGroupContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DesignWaterLevelCalculationsGroupContext(ObservableList<HydraulicBoundaryLocation> wrappedData,
                                                        IAssessmentSection assessmentSection)
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
        public IAssessmentSection AssessmentSection { get; }
    }
}