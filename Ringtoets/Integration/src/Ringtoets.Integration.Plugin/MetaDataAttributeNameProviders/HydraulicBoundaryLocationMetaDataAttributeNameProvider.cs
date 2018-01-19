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

using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Hydraulics;
using Ringtoets.Integration.Data;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Integration.Plugin.MetaDataAttributeNameProviders
{
    /// <summary>
    /// This class provides the meta data attribute names during the export of <see cref="HydraulicBoundaryLocation"/>
    /// that are part of the <see cref="AssessmentSection"/>.
    /// </summary>
    public class HydraulicBoundaryLocationMetaDataAttributeNameProvider : IHydraulicBoundaryLocationMetaDataAttributeNameProvider
    {
        public string DesignWaterLevelCalculation1AttributeName { get; } = RingtoetsCommonIOResources.MetaData_DesignWaterLevelCalculation1AttributeName_Description;
        public string DesignWaterLevelCalculation2AttributeName { get; } = RingtoetsCommonIOResources.MetaData_DesignWaterLevelCalculation2AttributeName_Description;
        public string DesignWaterLevelCalculation3AttributeName { get; } = RingtoetsCommonIOResources.MetaData_DesignWaterLevelCalculation3AttributeName_Description;
        public string DesignWaterLevelCalculation4AttributeName { get; } = RingtoetsCommonIOResources.MetaData_DesignWaterLevelCalculation4AttributeName_Description;
        public string WaveHeightCalculation1AttributeName { get; } = RingtoetsCommonIOResources.MetaData_WaveHeightCalculation1AttributeName_Description;
        public string WaveHeightCalculation2AttributeName { get; } = RingtoetsCommonIOResources.MetaData_WaveHeightCalculation2AttributeName_Description;
        public string WaveHeightCalculation3AttributeName { get; } = RingtoetsCommonIOResources.MetaData_WaveHeightCalculation3AttributeName_Description;
        public string WaveHeightCalculation4AttributeName { get; } = RingtoetsCommonIOResources.MetaData_WaveHeightCalculation4AttributeName_Description;
    }
}