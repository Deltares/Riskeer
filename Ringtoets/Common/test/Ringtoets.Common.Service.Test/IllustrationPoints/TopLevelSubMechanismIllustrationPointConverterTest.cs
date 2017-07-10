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
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraRingWindDirection = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;
using HydraRingWindDirectionClosingSituation = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraRingIllustrationPointResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using HydraRingTestWindDirection = Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints.TestWindDirection;
using HydraRingSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraRingSubMechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class TopLevelSubMechanismIllustrationPointConverterTest
    {
        [Test]
        public void CreateTopLevelSubMechanismIllustrationPoint_HydraWindDirectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraRingSubMechanismIllustrationPoint =
                new HydraRingSubMechanismIllustrationPoint("name",
                                                           Enumerable.Empty<HydraRingSubMechanismIllustrationPointStochast>(),
                                                           Enumerable.Empty<HydraRingIllustrationPointResult>(),
                                                           double.NaN);

            // Call
            TestDelegate call = () =>
                TopLevelSubMechanismIllustrationPointConverter.CreateTopLevelSubMechanismIllustrationPoint(
                    null, hydraRingSubMechanismIllustrationPoint);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingWindDirectionClosingSituation", paramName);
        }

        [Test]
        public void CreateTopLevelSubMechanismIllustrationPoint_SubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraRingWindDirection = new HydraRingTestWindDirection();

            // Call
            TestDelegate call = () =>
                TopLevelSubMechanismIllustrationPointConverter.CreateTopLevelSubMechanismIllustrationPoint(
                    new HydraRingWindDirectionClosingSituation(hydraRingWindDirection, string.Empty),
                    null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingSubMechanismIllustrationPoint", paramName);
        }

        [Test]
        public void CreateTopLevelSubMechanismIllustrationPoint_ValidArguments_ExpectedProperties()
        {
            // Setup
            const string closingScenario = "closing scenario";

            var random = new Random(21);
            double angle = random.NextDouble();
            var hydraRingWindDirection = new HydraRingWindDirection("Name", angle);

            var hydraRingWindDirectionClosingSituation = new HydraRingWindDirectionClosingSituation(hydraRingWindDirection,
                                                                                                    closingScenario);

            var hydraRingIllustrationPointResult = new HydraRingIllustrationPointResult("HydraIllustrationPointResult",
                                                                                        random.NextDouble());

            const string name = "HydraSubMechanismIllustrationPointStochast";
            double alpha = random.NextDouble();
            double duration = random.NextDouble();
            double realization = random.NextDouble();
            var hydraRingSubMechanismIllustrationPointStochast =
                new HydraRingSubMechanismIllustrationPointStochast(name, duration, alpha, realization);

            double beta = random.NextDouble();
            var hydraRingSubMechanismIllustrationPoint = new HydraRingSubMechanismIllustrationPoint("name", new[]
            {
                hydraRingSubMechanismIllustrationPointStochast
            }, new[]
            {
                hydraRingIllustrationPointResult
            }, beta);

            // Call
            TopLevelSubMechanismIllustrationPoint combination =
                TopLevelSubMechanismIllustrationPointConverter.CreateTopLevelSubMechanismIllustrationPoint(
                    hydraRingWindDirectionClosingSituation, hydraRingSubMechanismIllustrationPoint);

            // Assert
            WindDirection windDirection = combination.WindDirection;
            Assert.AreEqual(hydraRingWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(hydraRingWindDirection.Name, windDirection.Name);

            Assert.AreEqual(closingScenario, combination.ClosingSituation);

            SubMechanismIllustrationPoint subMechanismIllustrationPoint = combination.SubMechanismIllustrationPoint;
            Assert.AreEqual(hydraRingSubMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(hydraRingSubMechanismIllustrationPoint.Name, subMechanismIllustrationPoint.Name);

            IllustrationPointResult illustrationPointResult = subMechanismIllustrationPoint.IllustrationPointResults.Single();
            Assert.AreEqual(hydraRingIllustrationPointResult.Description, illustrationPointResult.Description);
            Assert.AreEqual(hydraRingIllustrationPointResult.Value, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());

            SubMechanismIllustrationPointStochast stochast = subMechanismIllustrationPoint.Stochasts.Single();
            Assert.AreEqual(hydraRingSubMechanismIllustrationPointStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraRingSubMechanismIllustrationPointStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingSubMechanismIllustrationPointStochast.Name, stochast.Name);
            Assert.AreEqual(hydraRingSubMechanismIllustrationPointStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}