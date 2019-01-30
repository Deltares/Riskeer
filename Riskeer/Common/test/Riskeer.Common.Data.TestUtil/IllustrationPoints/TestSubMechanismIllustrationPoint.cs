// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.IllustrationPoints;

namespace Riskeer.Common.Data.TestUtil.IllustrationPoints
{
    /// <summary>
    /// A simple sub mechanism illustration point that can be used for testing.
    /// </summary>
    public class TestSubMechanismIllustrationPoint : SubMechanismIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestSubMechanismIllustrationPoint"/>.
        /// </summary>
        public TestSubMechanismIllustrationPoint()
            : this(3.14) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="name">The name of the illustration point.</param>
        public TestSubMechanismIllustrationPoint(string name)
            : base(name,
                   3.14,
                   Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                   Enumerable.Empty<IllustrationPointResult>()) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="stochasts">The stochasts of the illustration point.</param>
        public TestSubMechanismIllustrationPoint(IEnumerable<SubMechanismIllustrationPointStochast> stochasts)
            : base("Illustration Point",
                   3.14,
                   stochasts,
                   Enumerable.Empty<IllustrationPointResult>()) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="beta">The beta of the illustration point.</param>
        public TestSubMechanismIllustrationPoint(double beta)
            : base("Illustration Point",
                   beta,
                   Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                   Enumerable.Empty<IllustrationPointResult>()) {}
    }
}