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

using System;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FaultTreeIllustrationPointBasePropertyTest
    {
        [Test]
        public void Constructor_Null_ThrowsException()
        {
            // Setup
            const string expectedMessage = "Value cannot be null.";

            // Call
            TestDelegate test = () => new FaultTreeIllustrationPointBaseProperty(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPoint_CorrectValues()
        {
            // Setup
            var topLevel = new TopLevelFaultTreeIllustrationPoint(
                new WindDirection("N", 5.0),
                "closing situation",
                new IllustrationPointNode(new FaultTreeIllustrationPoint("N", 7.2, new Stochast[0], CombinationType.And)));
            topLevel.FaultTreeNodeRoot.SetChildren(new[]
            {
                new IllustrationPointNode(new FaultTreeIllustrationPoint("N", 7.2, new Stochast[0], CombinationType.And)),
                new IllustrationPointNode(new SubMechanismIllustrationPoint("N", 7.2, new SubMechanismIllustrationPointStochast[0], new IllustrationPointResult[0]))
            });

            // Call
            var faultTree = new FaultTreeIllustrationPointBaseProperty(topLevel);

            // Assert
            Assert.AreEqual(faultTree.WindDirection, "N");
            Assert.AreEqual(faultTree.Reliability.Value, 7.2);
            Assert.AreEqual(faultTree.Reliability.NumberOfDecimalPlaces, 5);
            Assert.AreEqual(faultTree.CalculatedProbability, StatisticsConverter.ReliabilityToProbability(7.2));
            Assert.AreEqual(faultTree.ClosingSituation, "closing situation");

            Assert.IsNotNull(faultTree.AlphaValues);
            Assert.AreEqual(faultTree.AlphaValues.Length, 0);

            Assert.IsNotNull(faultTree.Durations);
            Assert.AreEqual(faultTree.Durations.Length, 0);

            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(faultTree.IllustrationPoints.Length, 2);
        }

        [Test]
        public void ToString_CorrectValue_ReturnsCorrectString()
        {
            // Setup
            var faultTree = new FaultTreeIllustrationPointBaseProperty(
                new TopLevelFaultTreeIllustrationPoint(
                    new WindDirection("SSE", 5.0),
                    "closing situation",
                    new IllustrationPointNode(new SubMechanismIllustrationPoint("N", 1.5, new SubMechanismIllustrationPointStochast[0], new IllustrationPointResult[0]))));

            // Call
            string toString = faultTree.ToString();

            // Assert
            Assert.AreEqual(toString, "SSE");
        }
    }
}