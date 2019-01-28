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

namespace Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints
{
    /// <summary>
    /// Simple <see cref="SubMechanismIllustrationPoint"/> that can be used in tests.
    /// </summary>
    public class TestSubMechanismIllustrationPoint : SubMechanismIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestSubMechanismIllustrationPoint"/>.
        /// </summary>
        public TestSubMechanismIllustrationPoint()
            : base("Illustration point", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                   Enumerable.Empty<IllustrationPointResult>(), 1) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="name">The name of the illustration point</param>
        public TestSubMechanismIllustrationPoint(string name)
            : base(name, Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                   Enumerable.Empty<IllustrationPointResult>(), 1) {}
    }
}