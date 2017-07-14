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
using HydraRingCombinationType = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;
using HydraRingFaultTreeIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class FaultTreeIllustrationPointConverterTest
    {
        [Test]
        public void CreateIllustrationPoint_HydraRingFaultTreeIllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FaultTreeIllustrationPointConverter.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingFaultTreeIllustrationPoint", paramName);
        }

        [Test]
        public void Create_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);

            const string name = "hydraRingStochast";
            double alpha = random.NextDouble();
            double duration = random.NextDouble();
            var hydraRingStochast = new HydraRingStochast(name, duration, alpha);

            double beta = random.NextDouble();
            var hydraRingFaultTreeIllustrationPoint = new HydraRingFaultTreeIllustrationPoint("name", beta, new[]
            {
                hydraRingStochast
            }, HydraRingCombinationType.And);

            // Call
            FaultTreeIllustrationPoint faultTreeIllustrationPoint =
                FaultTreeIllustrationPointConverter.Create(hydraRingFaultTreeIllustrationPoint);

            // Assert
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Beta, faultTreeIllustrationPoint.Beta, faultTreeIllustrationPoint.Beta.GetAccuracy());
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Name, faultTreeIllustrationPoint.Name);
            Assert.AreEqual((int) hydraRingFaultTreeIllustrationPoint.CombinationType, (int) faultTreeIllustrationPoint.CombinationType);

            Stochast stochast = faultTreeIllustrationPoint.Stochasts.Single();
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Name, stochast.Name);
        }
    }
}