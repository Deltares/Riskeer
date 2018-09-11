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

using System.Collections.Generic;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO.Configurations
{
    /// <summary>
    /// Imports a wave conditions calculation configuration from an XML file and stores it on a
    /// <see cref="CalculationGroup"/>.
    /// </summary>
    /// <typeparam name="T">The type of the calculation to import.</typeparam>
    public class AssessmentSectionCategoryWaveConditionsCalculationConfigurationImporter<T>
        : WaveConditionsCalculationConfigurationImporter<T, AssessmentSectionCategoryWaveConditionsCalculationConfigurationReader, AssessmentSectionCategoryWaveConditionsCalculationConfiguration>
        where T : ICalculation<AssessmentSectionCategoryWaveConditionsInput>, new()
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionCategoryWaveConditionsCalculationConfigurationImporter{T}"/>.
        /// </summary>
        public AssessmentSectionCategoryWaveConditionsCalculationConfigurationImporter(string xmlFilePath,
                                                                                       CalculationGroup importTarget,
                                                                                       IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                                       IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                                                                       NormType normType)
            : base(xmlFilePath, importTarget, hydraulicBoundaryLocations, foreshoreProfiles, normType) {}

        protected override AssessmentSectionCategoryWaveConditionsCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new AssessmentSectionCategoryWaveConditionsCalculationConfigurationReader(xmlFilePath);
        }

        protected override void SetCategoryType(AssessmentSectionCategoryWaveConditionsCalculationConfiguration calculationConfiguration,
                                                T calculation,
                                                NormType normType)
        {
            if (calculationConfiguration.CategoryType.HasValue)
            {
                calculation.InputParameters.CategoryType = (AssessmentSectionCategoryType) calculationConfiguration.CategoryType;
            }
            else
            {
                WaveConditionsInputHelper.SetCategoryType(calculation.InputParameters, normType);
            }
        }
    }
}