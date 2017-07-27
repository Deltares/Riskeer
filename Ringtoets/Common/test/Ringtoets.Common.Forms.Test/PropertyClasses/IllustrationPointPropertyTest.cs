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
using System.Collections.Generic;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class IllustrationPointPropertyTest
    {
        [Test]
        public void Constructor_Null_ThrowsException()
        {
            // Setup
            const string expectedMessage = "Value cannot be null.";

            // Call
            TestDelegate test = () => new IllustrationPointProperty(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_GeneralResultFaultTreeIllustrationPoint_CorrectValues()
        {
            // Setup

            // Call
            var topLevelFaultTreeIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(new WindDirection("SSE", 5.0), "closing situation", new IllustrationPointNode(new TestIllustrationPoint()));

            var expectedFaultTreeIllustrationPointBaseProperty = new[]
            {
                new FaultTreeIllustrationPointBaseProperty(topLevelFaultTreeIllustrationPoint)
            };
            var generalResult = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(new WindDirection("SSE", 5.0), new Stochast[0], new List<TopLevelFaultTreeIllustrationPoint> {topLevelFaultTreeIllustrationPoint});
            var property = new IllustrationPointProperty(generalResult);

            // Assert
            Assert.AreEqual(new Stochast[0], property.AlphaValues);
            Assert.AreEqual(new Stochast[0], property.Durations);
            Assert.AreEqual("SSE", property.WindDirection);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].WindDirection, property.IllustrationPoints[0].WindDirection);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].Reliability, property.IllustrationPoints[0].Reliability);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].CalculatedProbability, property.IllustrationPoints[0].CalculatedProbability);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].ClosingSituation, property.IllustrationPoints[0].ClosingSituation);
            Assert.AreEqual(expectedFaultTreeIllustrationPointBaseProperty[0].IllustrationPoints, property.IllustrationPoints[0].IllustrationPoints);


        }

    }
}