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
using HydraRealizedStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.RealizedStochast;

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
        public void CreateStochast_HydraStochastNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraStochast = new HydraStochast
            {
                Alpha = 0,
                Duration = 0
            };

            // Call
            TestDelegate call = () => StochastConverter.CreateStochast(hydraStochast);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void CreateStochast_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            int duration = random.Next();
            var hydraStochast = new HydraStochast
            {
                Alpha = random.NextDouble(),
                Duration = duration,
                Name = "Name"
            };

            // Call
            Stochast stochast = StochastConverter.CreateStochast(hydraStochast);

            // Assert
            Assert.AreEqual(hydraStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(duration, stochast.Duration);
            Assert.AreEqual(hydraStochast.Name, stochast.Name);
        }

        [Test]
        public void CreateRealizedStochast_HydraRealizedStochastNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StochastConverter.CreateRealizedStochast(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRealizedStochast", paramName);
        }

        [Test]
        public void CreateRealizedStochast_HydraRealizedStochastNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraStochast = new HydraRealizedStochast
            {
                Alpha = 0,
                Duration = 0
            };

            // Call
            TestDelegate call = () => StochastConverter.CreateStochast(hydraStochast);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void CreateRealizedStochast_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            int duration = random.Next();
            var hydraStochast = new HydraRealizedStochast
            {
                Alpha = random.NextDouble(),
                Duration = duration,
                Name = "Name",
                Realization = random.NextDouble()
            };

            // Call
            RealizedStochast stochast = StochastConverter.CreateRealizedStochast(hydraStochast);

            // Assert
            Assert.AreEqual(hydraStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(duration, stochast.Duration);
            Assert.AreEqual(hydraStochast.Name, stochast.Name);
            Assert.AreEqual(hydraStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());
        }
    }
}