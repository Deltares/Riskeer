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
using System.Linq;
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Factory to create instances of <see cref="HydraulicBoundaryCalculationSettings"/>.
    /// </summary>
    public static class HydraulicBoundaryCalculationSettingsFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/> from
        /// <paramref name="assessmentSection"/> for the provided <paramref name="hydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to create the
        /// <see cref="HydraulicBoundaryCalculationSettings"/> from.</param>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> to create the
        /// <see cref="HydraulicBoundaryCalculationSettings"/> for.</param>
        /// <returns>A <see cref="HydraulicBoundaryCalculationSettings"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> or
        /// <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the related hydraulic boundary database file path
        /// or the hlcd file path is <c>null</c>, is empty or consists of whitespace.</exception>
        public static HydraulicBoundaryCalculationSettings CreateSettings(IAssessmentSection assessmentSection,
                                                                          HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabases.First(hbd => hbd.Locations.Contains(hydraulicBoundaryLocation));

            return new HydraulicBoundaryCalculationSettings(hydraulicBoundaryDatabase.FilePath,
                                                            hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.FilePath,
                                                            hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessorClosure,
                                                            hydraulicBoundaryDatabase.EffectivePreprocessorDirectory());
        }
    }
}