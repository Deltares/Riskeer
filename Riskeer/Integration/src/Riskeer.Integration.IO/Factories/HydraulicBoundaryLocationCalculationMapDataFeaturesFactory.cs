﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Components.Gis.Features;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Util;
using Riskeer.Integration.IO.Properties;
using RiskeerCommonUtilResources = Riskeer.Common.Util.Properties.Resources;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory for creating <see cref="MapFeature"/> for <see cref="HydraulicBoundaryLocationCalculation"/>.
    /// </summary>
    public static class HydraulicBoundaryLocationCalculationMapDataFeaturesFactory
    {
        /// <summary>
        /// Creates a hydraulic boundary location calculation feature based on the given <paramref name="calculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create the feature for.</param>
        /// <param name="hydraulicBoundaryDatabaseFileName">The file name of the hydraulic boundary database the calculation belongs to.</param>
        /// <param name="metaDataHeader">The meta data header to use.</param>
        /// <returns>A feature based on the given <paramref name="calculation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static MapFeature CreateHydraulicBoundaryLocationCalculationFeature(HydraulicBoundaryLocationCalculation calculation,
                                                                                   string hydraulicBoundaryDatabaseFileName,
                                                                                   string metaDataHeader)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (hydraulicBoundaryDatabaseFileName == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabaseFileName));
            }

            if (metaDataHeader == null)
            {
                throw new ArgumentNullException(nameof(metaDataHeader));
            }

            HydraulicBoundaryLocation location = calculation.HydraulicBoundaryLocation;
            MapFeature feature = RiskeerMapDataFeaturesFactoryHelper.CreateSinglePointMapFeature(location.Location);
            feature.MetaData[RiskeerCommonUtilResources.MetaData_ID] = location.Id;
            feature.MetaData[RiskeerCommonUtilResources.MetaData_Name] = location.Name;
            feature.MetaData[Resources.HydraulicBoundaryLocationCalculationMapDataFeaturesFactory_HydraulicBoundaryDatabase_FileName_DisplayName] = hydraulicBoundaryDatabaseFileName;
            feature.MetaData[metaDataHeader] = GetCalculationResult(calculation.Output).ToString();
            return feature;
        }

        private static RoundedDouble GetCalculationResult(HydraulicBoundaryLocationCalculationOutput output)
        {
            return output?.Result ?? RoundedDouble.NaN;
        }
    }
}