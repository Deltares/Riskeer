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
using HydraSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraWindDirectionClosingSituation = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraIllustrationPointResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using HydraRealizedStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.RealizedStochast;
using HydraRingTestWindDirection = Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints.TestWindDirection;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class WindDirectionClosingSituationIllustrationPointConverterTest
    {
        [Test]
        public void CreateWindDirectionClosingScenarioIllustrationPoint_HydraWindDirectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraSubMechanismIllustrationPoint = new HydraSubMechanismIllustrationPoint("name",
                                                                                            Enumerable.Empty<HydraRealizedStochast>(),
                                                                                            Enumerable.Empty<HydraIllustrationPointResult>(),
                                                                                            double.NaN);

            // Call
            TestDelegate call = () =>
                WindDirectionClosingSituationIllustrationPointConverter.CreateWindDirectionClosingScenarioIllustrationPoint(
                    null, hydraSubMechanismIllustrationPoint);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraWindDirectionClosingSituation", paramName);
        }

        [Test]
        public void CreateWindDirectionClosingScenarioIllustrationPoint_SubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraWindDirection = new HydraRingTestWindDirection();

            // Call
            TestDelegate call = () =>
                WindDirectionClosingSituationIllustrationPointConverter.CreateWindDirectionClosingScenarioIllustrationPoint(
                    new HydraWindDirectionClosingSituation(hydraWindDirection, string.Empty),
                    null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraSubMechanismIllustrationPoint", paramName);
        }

        [Test]
        public void CreateWindDirectionClosingScenarioIllustrationPoint_ValidArguments_ExpectedProperties()
        {
            // Setup
            const string closingScenario = "closing scenario";

            var random = new Random(21);
            double angle = random.NextDouble();
            var hydraWindDirection = new HydraWindDirection("Name", angle);

            var windDirectionClosingSituation = new HydraWindDirectionClosingSituation(hydraWindDirection, closingScenario);

            var hydraIllustrationPointResult = new HydraIllustrationPointResult("HydraIllustrationPointResult",
                                                                                random.NextDouble());

            const string name = "HydraRealizedStochast";
            double alpha = random.NextDouble();
            int duration = random.Next();
            double realization = random.NextDouble();
            var hydraRealizedStochast = new HydraRealizedStochast(name, duration, alpha, realization);

            double beta = random.NextDouble();
            var subMechanismIllustrationPoint = new HydraSubMechanismIllustrationPoint("name", new[]
            {
                hydraRealizedStochast
            }, new[]
            {
                hydraIllustrationPointResult
            }, beta);

            // Call
            WindDirectionClosingSituationIllustrationPoint combination =
                WindDirectionClosingSituationIllustrationPointConverter.CreateWindDirectionClosingScenarioIllustrationPoint(
                    windDirectionClosingSituation, subMechanismIllustrationPoint);

            // Assert
            WindDirection windDirection = combination.WindDirection;
            Assert.AreEqual(hydraWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(hydraWindDirection.Name, windDirection.Name);

            Assert.AreEqual(closingScenario, combination.ClosingSituation);

            IllustrationPoint illustrationPoint = combination.IllustrationPoint;
            Assert.AreEqual(subMechanismIllustrationPoint.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(subMechanismIllustrationPoint.Name, illustrationPoint.Name);

            IllustrationPointResult illustrationPointResult = illustrationPoint.IllustrationPointResults.Single();
            Assert.AreEqual(hydraIllustrationPointResult.Description, illustrationPointResult.Description);
            Assert.AreEqual(hydraIllustrationPointResult.Value, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());

            RealizedStochast stochast = illustrationPoint.Stochasts.Single();
            Assert.AreEqual(hydraRealizedStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(duration, stochast.Duration);
            Assert.AreEqual(hydraRealizedStochast.Name, stochast.Name);
            Assert.AreEqual(hydraRealizedStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}