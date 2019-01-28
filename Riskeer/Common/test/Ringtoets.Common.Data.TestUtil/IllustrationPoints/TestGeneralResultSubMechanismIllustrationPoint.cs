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

using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.IllustrationPoints
{
    /// <summary>
    /// A simple general result with top level sub mechanism illustration points
    ///  which can be used for testing.
    /// </summary>
    public class TestGeneralResultSubMechanismIllustrationPoint : GeneralResult<TopLevelSubMechanismIllustrationPoint>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestGeneralResultSubMechanismIllustrationPoint"/>.
        /// </summary>
        public TestGeneralResultSubMechanismIllustrationPoint()
            : base(WindDirectionTestFactory.CreateTestWindDirection(),
                   Enumerable.Empty<Stochast>(),
                   Enumerable.Empty<TopLevelSubMechanismIllustrationPoint>()) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestGeneralResultSubMechanismIllustrationPoint"/>
        /// with specified <see cref="TopLevelSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="topLevelIllustrationPoints">The top level illustration 
        /// points that are associated with this general result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="topLevelIllustrationPoints"/>
        /// is <c>null</c>.</exception>
        public TestGeneralResultSubMechanismIllustrationPoint(IEnumerable<TopLevelSubMechanismIllustrationPoint> topLevelIllustrationPoints)
            : base(WindDirectionTestFactory.CreateTestWindDirection(),
                   Enumerable.Empty<Stochast>(),
                   topLevelIllustrationPoints) {}
    }
}