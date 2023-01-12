﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.DikeProfiles;

namespace Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// Helper methods for dike geometries.
    /// </summary>
    public static class DikeGeometryHelper
    {
        /// <summary>
        /// This method returns the roughnesses of a dike geometry.
        /// </summary>
        /// <param name="roughnessPoints">The roughness points that represent the dike geometry.</param>
        /// <returns>A collection of roughnesses.</returns>
        public static IEnumerable<RoundedDouble> GetRoughnesses(IEnumerable<RoughnessPoint> roughnessPoints)
        {
            return roughnessPoints.Count() > 1
                       ? roughnessPoints.Take(roughnessPoints.Count() - 1)
                                        .Select(p => p.Roughness)
                       : new RoundedDouble[0];
        }
    }
}