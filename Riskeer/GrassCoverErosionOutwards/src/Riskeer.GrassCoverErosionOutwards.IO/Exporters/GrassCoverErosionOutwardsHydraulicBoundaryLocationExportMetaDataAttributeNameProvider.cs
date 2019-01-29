﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Ringtoets.GrassCoverErosionOutwards.IO.Properties;
using Ringtoets.GrassCoverErosionOutwards.Util;

namespace Riskeer.GrassCoverErosionOutwards.IO.Exporters
{
    /// <summary>
    /// Class for providing meta data attribute names during exports of hydraulic boundary locations.
    /// </summary>
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationExportMetaDataAttributeNameProvider
        : IGrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProvider
    {
        public string WaterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm;
            }
        }

        public string WaterLevelCalculationForMechanismSpecificSignalingNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaterLevelCalculationForMechanismSpecificSignalingNorm;
            }
        }

        public string WaterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaterLevelCalculationForMechanismSpecificLowerLimitNorm;
            }
        }

        public string WaterLevelCalculationForLowerLimitNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaterLevelCalculationForLowerLimitNorm;
            }
        }

        public string WaterLevelCalculationForFactorizedLowerLimitNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaterLevelCalculationForFactorizedLowerLimitNorm;
            }
        }

        public string WaveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm;
            }
        }

        public string WaveHeightCalculationForMechanismSpecificSignalingNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaveHeightCalculationForMechanismSpecificSignalingNorm;
            }
        }

        public string WaveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaveHeightCalculationForMechanismSpecificLowerLimitNorm;
            }
        }

        public string WaveHeightCalculationForLowerLimitNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaveHeightCalculationForLowerLimitNorm;
            }
        }

        public string WaveHeightCalculationForFactorizedLowerLimitNormAttributeName
        {
            get
            {
                return Resources.MetaData_WaveHeightCalculationForFactorizedLowerLimitNorm;
            }
        }
    }
}