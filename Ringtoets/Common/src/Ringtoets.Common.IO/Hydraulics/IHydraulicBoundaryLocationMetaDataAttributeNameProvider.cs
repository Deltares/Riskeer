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

namespace Ringtoets.Common.IO.Hydraulics
{
    /// <summary>
    /// Interface for providing meta data attribute names during exports of hydraulic boundary locations.
    /// </summary>
    public interface IHydraulicBoundaryLocationMetaDataAttributeNameProvider
    {
        /// <summary>
        /// Gets the meta data attribute name of the first design water level calculation.
        /// </summary>
        string DesignWaterLevelCalculation1Name { get; }

        /// <summary>
        /// Gets the meta data attribute name of the second design water level calculation.
        /// </summary>
        string DesignWaterLevelCalculation2Name { get; }

        /// <summary>
        /// Gets the meta data attribute name of the third design water level calculation.
        /// </summary>
        string DesignWaterLevelCalculation3Name { get; }

        /// <summary>
        /// Gets the meta data attribute name of the fourth design water level calculation.
        /// </summary>
        string DesignWaterLevelCalculation4Name { get; }

        /// <summary>
        /// Gets the meta data attribute name of the first design wave height calculation.
        /// </summary>
        string WaveHeightCalculation1Name { get; }

        /// <summary>
        /// Gets the meta data attribute name of the second design wave height calculation.
        /// </summary>
        string WaveHeightCalculation2Name { get; }

        /// <summary>
        /// Gets the meta data attribute name of the third design wave height calculation.
        /// </summary>
        string WaveHeightCalculation3Name { get; }

        /// <summary>
        /// Gets the meta data attribute name of the fourth design wave height calculation.
        /// </summary>
        string WaveHeightCalculation4Name { get; }
    }
}