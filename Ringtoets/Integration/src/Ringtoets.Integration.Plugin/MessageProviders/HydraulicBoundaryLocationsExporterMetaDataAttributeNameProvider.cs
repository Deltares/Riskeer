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

namespace Ringtoets.Integration.Plugin.MessageProviders
{
    /// <summary>
    /// This class provides the meta data attribute names during the export of <see cref="HydraulicBoundaryLocation"/>
    /// that are part of the <see cref="AssessmentSection"/>.
    /// </summary>
    public class HydraulicBoundaryLocationsExporterMetaDataAttributeNameProvider : IHydraulicBoundaryLocationsExporterMetaDataAttributeNameProvider
    {
        public string DesignWaterLevelCalculation1Name { get; } = "h(A+_A)";
        public string DesignWaterLevelCalculation2Name { get; } = "h(A_B)";
        public string DesignWaterLevelCalculation3Name { get; } = "h(B_C)";
        public string DesignWaterLevelCalculation4Name { get; } = "h(C_D)";
        public string WaveHeightCalculation1Name { get; } = "Hs(A+_A)";
        public string WaveHeightCalculation2Name { get; } = "Hs(A_B)";
        public string WaveHeightCalculation3Name { get; } = "Hs(B_C)";
        public string WaveHeightCalculation4Name { get; } = "Hs(C_D)";
    }
}