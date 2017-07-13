﻿using System;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.IllustrationPoints
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
    }
}