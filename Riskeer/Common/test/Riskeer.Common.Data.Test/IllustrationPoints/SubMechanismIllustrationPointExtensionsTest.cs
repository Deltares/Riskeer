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
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Data.Test.IllustrationPoints
{
    public class SubMechanismIllustrationPointExtensionsTest
    {
        [Test]
        public void GetStochastNames_SubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => SubMechanismIllustrationPointExtensions.GetStochastNames(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("subMechanismIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void GetStochastNames_SubMechanismIllustrationPointWithStochast_ReturnStochastNames()
        {
            // Setup
            var random = new Random(21);
            const string stochastNameA = "Stochast A";
            const string stochastNameB = "Stochast B";
            var subMechanismIllustrationPoint = new TestSubMechanismIllustrationPoint(new[]
            {
                new SubMechanismIllustrationPointStochast(stochastNameA,
                                                          random.NextDouble(),
                                                          random.NextDouble(),
                                                          random.NextDouble()),
                new SubMechanismIllustrationPointStochast(stochastNameB,
                                                          random.NextDouble(),
                                                          random.NextDouble(),
                                                          random.NextDouble())
            });

            // Call
            IEnumerable<string> names = subMechanismIllustrationPoint.GetStochastNames();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                stochastNameA,
                stochastNameB
            }, names);
        }
    }
}