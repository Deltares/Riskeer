// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Application.Ringtoets.Storage.DbContext
{
    public interface IStructuresCalculationEntity
    {
        Nullable<double> ModelFactorSuperCriticalFlowMean { get; set; }
        Nullable<double> StructureNormalOrientation { get; set; }
        Nullable<double> AllowedLevelIncreaseStorageMean { get; set; }
        Nullable<double> AllowedLevelIncreaseStorageStandardDeviation { get; set; }
        Nullable<double> StorageStructureAreaMean { get; set; }
        Nullable<double> StorageStructureAreaCoefficientOfVariation { get; set; }
        Nullable<double> FlowWidthAtBottomProtectionMean { get; set; }
        Nullable<double> FlowWidthAtBottomProtectionStandardDeviation { get; set; }
        Nullable<double> CriticalOvertoppingDischargeMean { get; set; }
        Nullable<double> CriticalOvertoppingDischargeCoefficientOfVariation { get; set; }
        double FailureProbabilityStructureWithErosion { get; set; }
        Nullable<double> WidthFlowAperturesMean { get; set; }
        Nullable<double> WidthFlowAperturesCoefficientOfVariation { get; set; }
        Nullable<double> StormDurationMean { get; set; }
        byte UseBreakWater { get; set; }
        byte UseForeshore { get; set; }
        short BreakWaterType { get; set; }
        Nullable<double> BreakWaterHeight { get; set; }

        HydraulicLocationEntity HydraulicLocationEntity { get; set; }
        ForeshoreProfileEntity ForeshoreProfileEntity { get; set; }
    }
}