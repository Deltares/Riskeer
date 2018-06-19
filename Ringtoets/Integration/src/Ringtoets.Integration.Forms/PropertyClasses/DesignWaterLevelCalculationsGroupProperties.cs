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
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of an enumeration of <see cref="HydraulicBoundaryLocation"/> for properties panel.
    /// </summary>
    public class DesignWaterLevelCalculationsGroupProperties : ObjectProperties<ObservableList<HydraulicBoundaryLocation>>
    {
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationsGroupProperties"/>.
        /// </summary>
        /// <param name="locations">The locations to set as data.</param>
        /// <param name="assessmentSection">The assessment section the data belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DesignWaterLevelCalculationsGroupProperties(ObservableList<HydraulicBoundaryLocation> locations,
                                                           IAssessmentSection assessmentSection)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Data = locations;
            this.assessmentSection = assessmentSection;
        }
    }
}