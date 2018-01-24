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

using System;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;

namespace Application.Ringtoets.Storage.TestUtil.Hydraulics
{
    /// <summary>
    /// Factory for creating <see cref="HydraulicLocationEntity"/>
    /// which cam be used for testing.
    /// </summary>
    public static class HydraulicLocationEntityTestFactory
    {
        /// <summary>
        /// Creates a minimal <see cref="HydraulicLocationEntity"/> with a configured
        /// name and its hydraulic calculation entities set.
        /// </summary>
        /// <returns>A valid <see cref="HydraulicLocationEntity"/>.</returns>
        public static HydraulicLocationEntity CreateHydraulicLocationEntity()
        {
            var random = new Random(21);
            return new HydraulicLocationEntity
            {
                Name = "A",
                HydraulicLocationCalculationEntity = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity1 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity2 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity3 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity4 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity5 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity6 = CreateCalculationEntity(random.Next()),
                HydraulicLocationCalculationEntity7 = CreateCalculationEntity(random.Next())
            };
        }

        private static HydraulicLocationCalculationEntity CreateCalculationEntity(int seed)
        {
            var random = new Random(seed);

            return new HydraulicLocationCalculationEntity
            {
                ShouldIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean())
            };
        }
    }
}