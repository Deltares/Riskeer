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
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraSubmechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubmechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class StochastConverterTest
    {
        [Test]
        public void CreateStochast_HydraStochastNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StochastConverter.CreateStochast(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraStochast", paramName);
        }

        [Test]
        public void CreateStochast_ValidArguments_ExpectedProperties()
        {
            // Setup
            const string name = "name";

            var random = new Random(21);
            double duration = random.NextDouble();
            double alpha = random.NextDouble();
            var hydraStochast = new HydraStochast(name, duration, alpha);

            // Call
            Stochast stochast = StochastConverter.CreateStochast(hydraStochast);

            // Assert
            Assert.AreEqual(hydraStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraStochast.Name, stochast.Name);
        }

        [Test]
        public void CreateSubMechanismIllustrationStochast_HydraSubMechanismIllustrationPointStochastNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StochastConverter.CreateSubMechanismIllustrationStochast(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraSubmechanismIllustrationPointStochast", paramName);
        }

        [Test]
        public void CreateSubMechanismIllustrationStochast_ValidArguments_ExpectedProperties()
        {
            // Setup
            const string name = "name";

            var random = new Random(21);
            double duration = random.Next();
            double alpha = random.NextDouble();
            double realization = random.NextDouble();

            var hydraStochast = new HydraSubmechanismIllustrationPointStochast(name, duration, alpha, realization);

            // Call
            SubMechanismIllustrationPointStochast stochast = StochastConverter.CreateSubMechanismIllustrationStochast(hydraStochast);

            // Assert
            Assert.AreEqual(hydraStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraStochast.Name, stochast.Name);
            Assert.AreEqual(hydraStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}