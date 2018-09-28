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

using System.Collections.Generic;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints
{
    public static class GeneralResultTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/>
        /// with duplicate stochasts.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with duplicate stochasts.</returns>
        public static GeneralResult CreateGeneralResultWithDuplicateStochasts()
        {
            var stochast = new Stochast("Stochast A", 0, 0);
            Stochast[] stochasts =
            {
                stochast,
                stochast
            };
            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>();
            return new GeneralResult(0.5, new TestWindDirection(), stochasts, illustrationPoints);
        }
    }
}