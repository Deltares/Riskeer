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

using System.Linq;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints
{
    /// <summary>
    /// Simple <see cref="FaultTreeIllustrationPoint"/> that can be used in tests.
    /// </summary>
    public class TestFaultTreeIllustrationPoint : FaultTreeIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestFaultTreeIllustrationPoint"/>.
        /// </summary>
        public TestFaultTreeIllustrationPoint() : base("Illustration point", 1, Enumerable.Empty<Stochast>(), CombinationType.And) {}
    }
}