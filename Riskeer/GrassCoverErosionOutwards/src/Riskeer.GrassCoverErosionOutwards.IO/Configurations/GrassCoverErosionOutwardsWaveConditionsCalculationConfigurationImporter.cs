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

using System.Collections.Generic;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.IO.Configurations.Converters;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.Configurations;

namespace Riskeer.GrassCoverErosionOutwards.IO.Configurations
{
    /// <summary>
    /// Imports a grass cover erosion outwards wave conditions calculation configuration
    /// from an XML file and stores it on a <see cref="CalculationGroup"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationImporter
        : WaveConditionsCalculationConfigurationImporter<GrassCoverErosionOutwardsWaveConditionsCalculation,
            GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationReader, GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration>
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationImporter"/>.
        /// </summary>
        public GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationImporter(string xmlFilePath,
                                                                                       CalculationGroup importTarget,
                                                                                       IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                                       IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                                                                       NormType normType)
            : base(xmlFilePath, importTarget, hydraulicBoundaryLocations, foreshoreProfiles, normType) {}

        protected override GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationReader(xmlFilePath);
        }

        protected override void SetCalculationSpecificParameters(GrassCoverErosionOutwardsWaveConditionsCalculationConfiguration calculationConfiguration,
                                                                 GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                 NormType normType)
        {
            if (calculationConfiguration.CategoryType.HasValue)
            {
                calculation.InputParameters.CategoryType = (FailureMechanismCategoryType) new ConfigurationGrassCoverErosionOutwardsCategoryTypeConverter()
                    .ConvertTo(calculationConfiguration.CategoryType.Value, typeof(FailureMechanismCategoryType));
            }
            else
            {
                WaveConditionsInputHelper.SetCategoryType(calculation.InputParameters, normType);
            }

            if (calculationConfiguration.CalculationType.HasValue)
            {
                calculation.InputParameters.CalculationType = (GrassCoverErosionOutwardsWaveConditionsCalculationType) new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter()
                    .ConvertTo(calculationConfiguration.CalculationType.Value, typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType));
            }
        }
    }
}