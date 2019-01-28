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

namespace Ringtoets.Common.Data.Test.IllustrationPoints
{
    public class TopLevelFaultTreeIllustrationPointExtensionsTest
    {
        [Test]
        public void GetStochastNamesRecursively_TopLevelFaultTreeIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => TopLevelFaultTreeIllustrationPointExtensions.GetStochastNamesRecursively(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("topLevelFaultTreeIllustrationPoint", exception.ParamName);
        }

        [Test]
        public void GetStochastNamesRecursively_TopLevelFaultTreeIllustrationPointWithFaultTreeIllustrationPointAndChildrenContainingStochasts_ReturnStochastNames()
        {
            // Setup
            const string stochastNameA = "Stochast A";
            const string stochastNameB = "Stochast B";
            var illustrationPointNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint(new[]
            {
                new Stochast(stochastNameA, 2, 4),
                new Stochast(stochastNameB, 1, 5)
            }));
            var topLevelFaultTreeIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                            "closing situation",
                                                                                            illustrationPointNode);
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointNode(new FaultTreeIllustrationPoint("Point A", 0.0, new[]
                {
                    new Stochast(stochastNameA, 2, 3)
                }, CombinationType.And)),
                new IllustrationPointNode(new FaultTreeIllustrationPoint("Point B", 0.0, new[]
                {
                    new Stochast(stochastNameB, 2, 3)
                }, CombinationType.And))
            });

            // Call
            IEnumerable<string> names = topLevelFaultTreeIllustrationPoint.GetStochastNamesRecursively();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                stochastNameA,
                stochastNameB,
                stochastNameA,
                stochastNameB
            }, names);
        }

        [Test]
        public void GetStochastNamesRecursively_TopLevelFaultTreeIllustrationPointWithFaultTreeIllustrationPointAndNoChildren_ReturnStochastNames()
        {
            // Setup
            const string stochastNameA = "Stochast A";
            const string stochastNameB = "Stochast B";
            var topLevelFaultTreeIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                            "closing situation",
                                                                                            new IllustrationPointNode(new TestFaultTreeIllustrationPoint(new[]
                                                                                            {
                                                                                                new Stochast(stochastNameA, 4, 2),
                                                                                                new Stochast(stochastNameB, 5, 4)
                                                                                            })));

            // Call
            IEnumerable<string> names = topLevelFaultTreeIllustrationPoint.GetStochastNamesRecursively();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                stochastNameA,
                stochastNameB
            }, names);
        }
    }
}