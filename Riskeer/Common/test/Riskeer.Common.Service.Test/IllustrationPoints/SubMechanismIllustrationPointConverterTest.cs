// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.IllustrationPoints;
using HydraRingIllustrationPointResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using HydraRingSubMechanismIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraRingSubMechanismIllustrationPointStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;

namespace Riskeer.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class SubMechanismIllustrationPointConverterTest
    {
        [Test]
        public void Convert_HydraRingSubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SubMechanismIllustrationPointConverter.Convert(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingSubMechanismIllustrationPoint", paramName);
        }

        [Test]
        public void Convert_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraIllustrationPointResult = new HydraRingIllustrationPointResult("HydraIllustrationPointResult",
                                                                                    random.NextDouble());

            const string name = "hydraRingSubMechanismIllustrationPointStochast";
            double alpha = random.NextDouble();
            double duration = random.NextDouble();
            double realization = random.NextDouble();
            var hydraRingSubMechanismIllustrationPointStochast =
                new HydraRingSubMechanismIllustrationPointStochast(name, duration, alpha, realization);

            double beta = random.NextDouble();
            var hydraSubMechanismIllustrationPoint = new HydraRingSubMechanismIllustrationPoint("name", new[]
            {
                hydraRingSubMechanismIllustrationPointStochast
            }, new[]
            {
                hydraIllustrationPointResult
            }, beta);

            // Call
            SubMechanismIllustrationPoint subMechanismIllustrationPoint =
                SubMechanismIllustrationPointConverter.Convert(hydraSubMechanismIllustrationPoint);

            // Assert
            Assert.AreEqual(subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(subMechanismIllustrationPoint.Name, subMechanismIllustrationPoint.Name);

            IllustrationPointResult illustrationPointResult = subMechanismIllustrationPoint.IllustrationPointResults.Single();
            Assert.AreEqual(hydraIllustrationPointResult.Description, illustrationPointResult.Description);
            Assert.AreEqual(hydraIllustrationPointResult.Value, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());

            SubMechanismIllustrationPointStochast stochast = subMechanismIllustrationPoint.Stochasts.Single();
            Assert.AreEqual(hydraRingSubMechanismIllustrationPointStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraRingSubMechanismIllustrationPointStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingSubMechanismIllustrationPointStochast.Name, stochast.Name);
            Assert.AreEqual(hydraRingSubMechanismIllustrationPointStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}