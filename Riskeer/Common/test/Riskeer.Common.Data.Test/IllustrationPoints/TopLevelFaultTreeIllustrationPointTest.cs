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
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class TopLevelFaultTreeIllustrationPointTest
    {
        [Test]
        public void Constructor_WindDirectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var faultTreeNode = new IllustrationPointNode(new TestIllustrationPoint());

            // Call
            TestDelegate call = () => new TopLevelFaultTreeIllustrationPoint(null,
                                                                             "closing situation",
                                                                             faultTreeNode);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("windDirection", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowsArgumentNullException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            var faultTreeNode = new IllustrationPointNode(new TestIllustrationPoint());

            // Call
            TestDelegate call = () => new TopLevelFaultTreeIllustrationPoint(windDirection,
                                                                             null,
                                                                             faultTreeNode);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("closingSituation", exception.ParamName);
        }

        [Test]
        public void Constructor_FaultTreeNodeRootNull_ThrowsArgumentNullException()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();

            // Call
            TestDelegate call = () => new TopLevelFaultTreeIllustrationPoint(windDirection,
                                                                             "closingSituation",
                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("faultTreeNodeRoot", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedProperties()
        {
            // Setup
            WindDirection windDirection = WindDirectionTestFactory.CreateTestWindDirection();
            const string closingSituation = "closing situation";

            var faultTreeNode = new IllustrationPointNode(new TestIllustrationPoint());

            // Call
            var illustrationPoint = new TopLevelFaultTreeIllustrationPoint(windDirection, closingSituation, faultTreeNode);

            // Assert
            Assert.IsInstanceOf<TopLevelIllustrationPointBase>(illustrationPoint);
            Assert.AreSame(windDirection, illustrationPoint.WindDirection);
            Assert.AreEqual(closingSituation, illustrationPoint.ClosingSituation);
            Assert.AreSame(faultTreeNode, illustrationPoint.FaultTreeNodeRoot);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var faultTreeNodeRoot = new IllustrationPointNode(new TestIllustrationPoint());

            faultTreeNodeRoot.SetChildren(new[]
            {
                new IllustrationPointNode(new TestIllustrationPoint()),
                new IllustrationPointNode(new TestIllustrationPoint())
            });

            var original = new TopLevelFaultTreeIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                  "Random closing situation",
                                                                  faultTreeNodeRoot);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}