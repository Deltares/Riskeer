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
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraWindDirection = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;
using HydraWindDirectionClosingSituation = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraIllustrationPointResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using HydraRingTestWindDirection = Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints.TestWindDirection;
using HydraSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class TopLevelSubMechanismIllustrationPointConverterTest
    {
        [Test]
        public void CreateTopLevelSubMechanismIllustrationPoint_HydraWindDirectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraSubMechanismIllustrationPoint =
                new HydraSubMechanismIllustrationPoint("name",
                                                       Enumerable.Empty<HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast>(),
                                                       Enumerable.Empty<HydraIllustrationPointResult>(),
                                                       double.NaN);

            // Call
            TestDelegate call = () =>
                TopLevelSubMechanismIllustrationPointConverter.CreateTopLevelSubMechanismIllustrationPoint(
                    null, hydraSubMechanismIllustrationPoint);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraWindDirectionClosingSituation", paramName);
        }

        [Test]
        public void CreateTopLevelSubMechanismIllustrationPoint_SubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraWindDirection = new HydraRingTestWindDirection();

            // Call
            TestDelegate call = () =>
                TopLevelSubMechanismIllustrationPointConverter.CreateTopLevelSubMechanismIllustrationPoint(
                    new HydraWindDirectionClosingSituation(hydraWindDirection, string.Empty),
                    null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraSubMechanismIllustrationPoint", paramName);
        }

        [Test]
        public void CreateTopLevelSubMechanismIllustrationPoint_ValidArguments_ExpectedProperties()
        {
            // Setup
            const string closingScenario = "closing scenario";

            var random = new Random(21);
            double angle = random.NextDouble();
            var hydraWindDirection = new HydraWindDirection("Name", angle);

            var windDirectionClosingSituation = new HydraWindDirectionClosingSituation(hydraWindDirection, closingScenario);

            var hydraIllustrationPointResult = new HydraIllustrationPointResult("HydraIllustrationPointResult",
                                                                                random.NextDouble());

            const string name = "HydraSubMechanismIllustrationPointStochast";
            double alpha = random.NextDouble();
            double duration = random.NextDouble();
            double realization = random.NextDouble();
            var hydraSubMechanismIllustrationPointStochast =
                new HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast(name, duration, alpha, realization);

            double beta = random.NextDouble();
            var hydraSubMechanismIllustrationPoint = new HydraSubMechanismIllustrationPoint("name", new[]
            {
                hydraSubMechanismIllustrationPointStochast
            }, new[]
            {
                hydraIllustrationPointResult
            }, beta);

            // Call
            TopLevelSubMechanismIllustrationPoint combination =
                TopLevelSubMechanismIllustrationPointConverter.CreateTopLevelSubMechanismIllustrationPoint(
                    windDirectionClosingSituation, hydraSubMechanismIllustrationPoint);

            // Assert
            WindDirection windDirection = combination.WindDirection;
            Assert.AreEqual(hydraWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(hydraWindDirection.Name, windDirection.Name);

            Assert.AreEqual(closingScenario, combination.ClosingSituation);

            SubMechanismIllustrationPoint subMechanismIllustrationPoint = combination.SubMechanismIllustrationPoint;
            Assert.AreEqual(hydraSubMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(hydraSubMechanismIllustrationPoint.Name, subMechanismIllustrationPoint.Name);

            IllustrationPointResult illustrationPointResult = subMechanismIllustrationPoint.IllustrationPointResults.Single();
            Assert.AreEqual(hydraIllustrationPointResult.Description, illustrationPointResult.Description);
            Assert.AreEqual(hydraIllustrationPointResult.Value, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());

            SubMechanismIllustrationPointStochast stochast = subMechanismIllustrationPoint.Stochasts.Single();
            Assert.AreEqual(hydraSubMechanismIllustrationPointStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraSubMechanismIllustrationPointStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraSubMechanismIllustrationPointStochast.Name, stochast.Name);
            Assert.AreEqual(hydraSubMechanismIllustrationPointStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}