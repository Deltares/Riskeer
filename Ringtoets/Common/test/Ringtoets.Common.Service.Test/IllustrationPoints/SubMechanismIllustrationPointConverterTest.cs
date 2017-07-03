﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using HydraIllustrationPointResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using HydraSubMechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class SubMechanismIllustrationPointConverterTest
    {
        [Test]
        public void CreateCreateIllustrationPoint_SubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SubMechanismIllustrationPointConverter.CreateSubMechanismIllustrationPoint(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("subMechanismIllustrationPoint", paramName);
        }

        [Test]
        public void CreateIllustrationPoint_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraIllustrationPointResult = new HydraIllustrationPointResult("HydraIllustrationPointResult",
                                                                                random.NextDouble());

            const string name = "hydraSubMechanismIllustrationPointStochast";
            double alpha = random.NextDouble();
            double duration = random.NextDouble();
            double realization = random.NextDouble();
            var hydraSubMechanismIllustrationPointStochast =
                new HydraSubMechanismIllustrationPointStochast(name, duration, alpha, realization);

            double beta = random.NextDouble();
            var hydraSubMechanismIllustrationPoint = new HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint("name", new[]
            {
                hydraSubMechanismIllustrationPointStochast
            }, new[]
            {
                hydraIllustrationPointResult
            }, beta);

            // Call
            SubMechanismIllustrationPoint subMechanismIllustrationPoint =
                SubMechanismIllustrationPointConverter.CreateSubMechanismIllustrationPoint(hydraSubMechanismIllustrationPoint);

            // Assert
            Assert.AreEqual(subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(subMechanismIllustrationPoint.Name, subMechanismIllustrationPoint.Name);

            IllustrationPointResult illustrationPointResult = subMechanismIllustrationPoint.IllustrationPointResults.Single();
            Assert.AreEqual(hydraIllustrationPointResult.Description, illustrationPointResult.Description);
            Assert.AreEqual(hydraIllustrationPointResult.Value, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());

            SubMechanismIllustrationPointStochast stochast = subMechanismIllustrationPoint.Stochasts.Single();
            Assert.AreEqual(hydraSubMechanismIllustrationPointStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraSubMechanismIllustrationPointStochast.Name, stochast.Name);
            Assert.AreEqual(hydraSubMechanismIllustrationPointStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}