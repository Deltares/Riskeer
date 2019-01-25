// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.HydraRing.Calculation.Data.Input.Overtopping
{
    /// <summary>
    /// Container for Hydra-Ring profile point related data with roughness.
    /// </summary>
    public class HydraRingRoughnessProfilePoint : HydraRingProfilePoint
    {
        /// <summary>
        ///  Creates a new instance of the <see cref="HydraRingRoughnessProfilePoint"/> class.
        /// </summary>
        /// <param name="x">The X-coordinate.</param>
        /// <param name="z">The Z-coordinate.</param>
        /// <param name="roughness">The reduction factor.</param>
        public HydraRingRoughnessProfilePoint(double x, double z, double roughness) : base(x, z)
        {
            Roughness = roughness;
        }

        public override double Roughness { get; }
    }
}