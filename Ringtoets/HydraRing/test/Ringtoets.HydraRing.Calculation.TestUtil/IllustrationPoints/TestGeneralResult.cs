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
using Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints
{
    /// <summary>
    /// A simple general result which can be used for testing.
    /// </summary>
    public class TestGeneralResult : GeneralResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestGeneralResult"/>.
        /// </summary>
        public TestGeneralResult()
        {
            Beta = 0;
            GoverningWind = new WindDirection
            {
                Name = "TestWindDirection"
            };
            Stochasts = Enumerable.Empty<Stochast>();
            IllustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>();
        }
    }
}