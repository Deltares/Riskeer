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
using System.Linq;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.IllustrationPoints
{
    /// <summary>
    /// A simple fault tree illustration point that can be used for testing.
    /// </summary>
    public class TestFaultTreeIllustrationPoint : FaultTreeIllustrationPoint
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestFaultTreeIllustrationPoint"/>.
        /// </summary>
        public TestFaultTreeIllustrationPoint()
            : this(3.14) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestFaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="name">The name of the illustration point.</param>
        public TestFaultTreeIllustrationPoint(string name)
            : base(name,
                   3.14,
                   Enumerable.Empty<Stochast>(),
                   CombinationType.And) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestFaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="stochasts">The stochasts of the illustration point.</param>
        public TestFaultTreeIllustrationPoint(IEnumerable<Stochast> stochasts)
            : base("Illustration Point",
                   3.14,
                   stochasts,
                   CombinationType.And) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestFaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="beta">The beta of the illustration point.</param>
        public TestFaultTreeIllustrationPoint(double beta)
            : base("Illustration Point",
                   beta,
                   Enumerable.Empty<Stochast>(),
                   CombinationType.And) {}
    }
}