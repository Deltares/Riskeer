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
using HydraIllustrationPointResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using HydraSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraSubmechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubmechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointConverterTest
    {
        [Test]
        public void CreateCreateIllustrationPoint_SubMechanismIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => IllustrationPointConverter.CreateIllustrationPoint(null);

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

            const string name = "HydraRealizedStochast";
            double alpha = random.NextDouble();
            double duration = random.NextDouble();
            double realization = random.NextDouble();
            var hydraRealizedStochast = new HydraSubmechanismIllustrationPointStochast(name, duration, alpha, realization);

            double beta = random.NextDouble();
            var subMechanismIllustrationPoint = new HydraSubMechanismIllustrationPoint("name", new[]
            {
                hydraRealizedStochast
            }, new[]
            {
                hydraIllustrationPointResult
            }, beta);

            // Call
            IllustrationPoint illustrationPoint = IllustrationPointConverter.CreateIllustrationPoint(subMechanismIllustrationPoint);

            // Assert
            Assert.AreEqual(subMechanismIllustrationPoint.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(subMechanismIllustrationPoint.Name, illustrationPoint.Name);

            IllustrationPointResult illustrationPointResult = illustrationPoint.IllustrationPointResults.Single();
            Assert.AreEqual(hydraIllustrationPointResult.Description, illustrationPointResult.Description);
            Assert.AreEqual(hydraIllustrationPointResult.Value, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());

            Data.Hydraulics.IllustrationPoints.SubmechanismIllustrationPointStochast stochast = illustrationPoint.Stochasts.Single();
            Assert.AreEqual(hydraRealizedStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRealizedStochast.Name, stochast.Name);
            Assert.AreEqual(hydraRealizedStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}