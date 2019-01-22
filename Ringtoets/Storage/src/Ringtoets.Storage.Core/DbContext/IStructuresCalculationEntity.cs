﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Ringtoets.Storage.Core.DbContext
{
    /// <summary>
    /// Interface for a calculation entity that represents a structure calculation.
    /// </summary>
    public interface IStructuresCalculationEntity
    {
        double? StructureNormalOrientation { get; set; }
        double? AllowedLevelIncreaseStorageMean { get; set; }
        double? AllowedLevelIncreaseStorageStandardDeviation { get; set; }
        double? StorageStructureAreaMean { get; set; }
        double? StorageStructureAreaCoefficientOfVariation { get; set; }
        double? FlowWidthAtBottomProtectionMean { get; set; }
        double? FlowWidthAtBottomProtectionStandardDeviation { get; set; }
        double? CriticalOvertoppingDischargeMean { get; set; }
        double? CriticalOvertoppingDischargeCoefficientOfVariation { get; set; }
        double FailureProbabilityStructureWithErosion { get; set; }
        double? WidthFlowAperturesMean { get; set; }
        double? WidthFlowAperturesStandardDeviation { get; set; }
        double? StormDurationMean { get; set; }
        byte UseBreakWater { get; set; }
        byte UseForeshore { get; set; }
        byte BreakWaterType { get; set; }
        double? BreakWaterHeight { get; set; }
        byte ShouldIllustrationPointsBeCalculated { get; set; }

        HydraulicLocationEntity HydraulicLocationEntity { get; set; }
        ForeshoreProfileEntity ForeshoreProfileEntity { get; set; }
    }
}