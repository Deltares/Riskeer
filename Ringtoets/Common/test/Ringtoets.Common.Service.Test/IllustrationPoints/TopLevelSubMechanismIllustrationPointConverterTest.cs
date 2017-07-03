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
using HydraSubmechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubmechanismIllustrationPointStochast;
using HydraSubmechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubmechanismIllustrationPoint;

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
                new HydraSubmechanismIllustrationPoint("name",
                                                       Enumerable.Empty<HydraSubmechanismIllustrationPointStochast>(),
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
            Assert.AreEqual("hydraSubmechanismIllustrationPoint", paramName);
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

            const string name = "HydraSubmechanismIllustrationPointStochast";
            double alpha = random.NextDouble();
            double duration = random.NextDouble();
            double realization = random.NextDouble();
            var hydraSubmechanismIllustrationPointStochast =
                new HydraSubmechanismIllustrationPointStochast(name, duration, alpha, realization);

            double beta = random.NextDouble();
            var hydraSubmechanismIllustrationPoint = new HydraSubmechanismIllustrationPoint("name", new[]
            {
                hydraSubmechanismIllustrationPointStochast
            }, new[]
            {
                hydraIllustrationPointResult
            }, beta);

            // Call
            TopLevelSubMechanismIllustrationPoint combination =
                TopLevelSubMechanismIllustrationPointConverter.CreateTopLevelSubMechanismIllustrationPoint(
                    windDirectionClosingSituation, hydraSubmechanismIllustrationPoint);

            // Assert
            WindDirection windDirection = combination.WindDirection;
            Assert.AreEqual(hydraWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(hydraWindDirection.Name, windDirection.Name);

            Assert.AreEqual(closingScenario, combination.ClosingSituation);

            SubMechanismIllustrationPoint subMechanismIllustrationPoint = combination.SubMechanismIllustrationPoint;
            Assert.AreEqual(hydraSubmechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(hydraSubmechanismIllustrationPoint.Name, subMechanismIllustrationPoint.Name);

            IllustrationPointResult illustrationPointResult = subMechanismIllustrationPoint.IllustrationPointResults.Single();
            Assert.AreEqual(hydraIllustrationPointResult.Description, illustrationPointResult.Description);
            Assert.AreEqual(hydraIllustrationPointResult.Value, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());

            SubMechanismIllustrationPointStochast stochast = subMechanismIllustrationPoint.Stochasts.Single();
            Assert.AreEqual(hydraSubmechanismIllustrationPointStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraSubmechanismIllustrationPointStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraSubmechanismIllustrationPointStochast.Name, stochast.Name);
            Assert.AreEqual(hydraSubmechanismIllustrationPointStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}