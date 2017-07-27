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
    public class SubMechanismIllustrationPointChildPropertyTest
    {
        [Test]
        public void Constructor_InvalidIllustrationPointType_ThrowsException()
        {
            // Setup
            const string expectedMessage = "IllustrationPoint data type has to be SubMechanismIllustrationPoint";

            // Call
            TestDelegate test = () => new SubMechanismIllustrationPointChildProperty(new IllustrationPointNode(
                                                                                         new FaultTreeIllustrationPoint("N", 1.5, new Stochast[0], CombinationType.And)),
                                                                                     "N");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FaultTreeIllustrationPoint_CorrectValues()
        {
            // Setup

            // Call
            var faultTree = new SubMechanismIllustrationPointChildProperty(new IllustrationPointNode(
                                                                               new SubMechanismIllustrationPoint("N", 1.5, new SubMechanismIllustrationPointStochast[0],
                                                                                                                 new IllustrationPointResult[0])),
                                                                           "N");

            // Assert
            Assert.AreEqual(faultTree.WindDirection, "N");
            Assert.AreEqual(faultTree.Reliability.Value, 1.5);
            Assert.AreEqual(faultTree.Reliability.NumberOfDecimalPlaces, 5);
            Assert.AreEqual(faultTree.CalculatedProbability, StatisticsConverter.ReliabilityToProbability(1.5));
            Assert.AreEqual(faultTree.Name, "N");

            Assert.IsNotNull(faultTree.SubMechanismStochasts);
            Assert.AreEqual(faultTree.SubMechanismStochasts.Length, 0);

            Assert.IsNotNull(faultTree.IllustrationPoints);
            Assert.AreEqual(faultTree.IllustrationPoints.Length, 0);
        }

        [Test]
        public void ToString_CorrectValue_ReturnsCorrectString()
        {
            // Setup
            var faultTree = new SubMechanismIllustrationPointChildProperty(new IllustrationPointNode(
                                                                               new SubMechanismIllustrationPoint("N", 1.5, new SubMechanismIllustrationPointStochast[0],
                                                                                                                 new IllustrationPointResult[0])),
                                                                           "N");

            // Call
            string toString = faultTree.ToString();

            // Assert
            Assert.AreEqual(toString, "N");
        }
    }
}