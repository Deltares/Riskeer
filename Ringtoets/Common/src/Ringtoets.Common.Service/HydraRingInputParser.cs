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

using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Helper class to parse the breakwater and foreshore properties of a calculation input object into required objects 
    /// for performing calculations with Hydra-Ring.
    /// </summary>
    public static class HydraRingInputParser
    {
        /// <summary>
        /// Parses the foreshore geometry of a calculation input object into a <see cref="IEnumerable{T}"/> of 
        /// <see cref="HydraRingForelandPoint"/>.
        /// </summary>
        /// <param name="input">A calculation input object that implements <see cref="IUseForeshore"/>.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="HydraRingForelandPoint"/>.</returns>
        public static IEnumerable<HydraRingForelandPoint> ParseForeshore(IUseForeshore input)
        {
            return input.UseForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0];
        }

        /// <summary>
        /// Parses the breakwater properties of a calculation input object into a <see cref="HydraRingBreakWater"/> object.
        /// </summary>
        /// <param name="input">A calculation input object that implements <see cref="IUseBreakWater"/>.</param>
        /// <returns>A <see cref="HydraRingBreakWater"/> object, <c>null</c> if <see cref="IUseBreakWater.UseBreakWater"/>
        /// is <c>false</c></returns>
        public static HydraRingBreakWater ParseBreakWater(IUseBreakWater input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null;
        }
    }
}