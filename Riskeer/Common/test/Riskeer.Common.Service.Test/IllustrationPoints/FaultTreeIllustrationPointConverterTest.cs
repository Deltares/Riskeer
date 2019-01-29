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
using System.ComponentModel;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraRingCombinationType = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;
using HydraRingFaultTreeIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;

namespace Riskeer.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class FaultTreeIllustrationPointConverterTest
    {
        [Test]
        public void Convert_HydraRingFaultTreeIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FaultTreeIllustrationPointConverter.Convert(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingFaultTreeIllustrationPoint", paramName);
        }

        [Test]
        public void Convert_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraRingStochast = new HydraRingStochast("hydraRingStochast",
                                                          random.NextDouble(),
                                                          random.NextDouble());

            var hydraRingFaultTreeIllustrationPoint =
                new HydraRingFaultTreeIllustrationPoint("name", random.NextDouble(), new[]
                {
                    hydraRingStochast
                }, HydraRingCombinationType.And);

            // Call
            FaultTreeIllustrationPoint faultTreeIllustrationPoint =
                FaultTreeIllustrationPointConverter.Convert(hydraRingFaultTreeIllustrationPoint);

            // Assert
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Beta, faultTreeIllustrationPoint.Beta,
                            faultTreeIllustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Name, faultTreeIllustrationPoint.Name);
            Assert.AreEqual(CombinationType.And, faultTreeIllustrationPoint.CombinationType);

            Stochast stochast = faultTreeIllustrationPoint.Stochasts.Single();
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Name, stochast.Name);
        }

        [Test]
        public void Convert_InvalidHydraRingCombinationType_ThrowsIllustrationPointConversionException()
        {
            // Setup
            var hydraRingFaultTreeIllustrationPoint = new HydraRingFaultTreeIllustrationPoint(
                "name",
                new Random(210).NextDouble(),
                Enumerable.Empty<HydraRingStochast>(),
                (HydraRingCombinationType) 999999);

            // Call
            TestDelegate call = () => FaultTreeIllustrationPointConverter.Convert(hydraRingFaultTreeIllustrationPoint);

            // Assert
            var exception = Assert.Throws<IllustrationPointConversionException>(call);
            string expectedMessage = $"Could not convert the {typeof(HydraRingCombinationType)} into a {typeof(CombinationType)}.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
        }
    }
}