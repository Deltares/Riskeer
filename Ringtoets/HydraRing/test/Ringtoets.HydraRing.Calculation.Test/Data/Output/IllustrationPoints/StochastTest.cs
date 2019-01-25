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
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    [TestFixture]
    public class StochastTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new Stochast(null, double.NaN, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ReturnsNewInstance()
        {
            // Setup
            const string name = "stochast name";

            var random = new Random(21);
            double duration = random.NextDouble();
            double alpha = random.NextDouble();

            // Call
            var stochast = new Stochast(name, duration, alpha);

            // Assert
            Assert.AreEqual(name, stochast.Name);
            Assert.AreEqual(duration, stochast.Duration);
            Assert.AreEqual(alpha, stochast.Alpha);
        }
    }
}