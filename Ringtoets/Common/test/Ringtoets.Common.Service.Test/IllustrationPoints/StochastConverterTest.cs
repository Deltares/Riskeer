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
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraRingStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraRingSubMechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class StochastConverterTest
    {
        [Test]
        public void Convert_HydraRingStochastNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StochastConverter.Convert((HydraRingStochast) null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingStochast", paramName);
        }

        [Test]
        public void Convert_ValidHydraRingStochastArgument_ExpectedProperties()
        {
            // Setup
            const string name = "name";

            var random = new Random(21);
            double duration = random.NextDouble();
            double alpha = random.NextDouble();
            var hydraRingStochast = new HydraRingStochast(name, duration, alpha);

            // Call
            Stochast stochast = StochastConverter.Convert(hydraRingStochast);

            // Assert
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Name, stochast.Name);
        }

        [Test]
        public void Convert_HydraRingSubMechanismIllustrationPointStochastNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StochastConverter.Convert(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingSubMechanismIllustrationPointStochast", paramName);
        }

        [Test]
        public void Convert_ValidHydraRingSubMechanismIllustrationPointStochast_ExpectedProperties()
        {
            // Setup
            const string name = "name";

            var random = new Random(21);
            double duration = random.Next();
            double alpha = random.NextDouble();
            double realization = random.NextDouble();

            var hydraRingStochast = new HydraRingSubMechanismIllustrationPointStochast(name, duration, alpha, realization);

            // Call
            SubMechanismIllustrationPointStochast stochast = StochastConverter.Convert(hydraRingStochast);

            // Assert
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Name, stochast.Name);
            Assert.AreEqual(hydraRingStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}