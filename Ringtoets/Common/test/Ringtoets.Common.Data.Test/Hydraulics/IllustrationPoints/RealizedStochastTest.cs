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
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Hydraulics.IllustrationPoints
{
    [TestFixture]
    public class RealizedStochastTest
    {
        [Test]
        public void Constructor_ValidArguments_ReturnExpectedValues()
        {
            // Setup
            const string name = "Stochast name";

            var random = new Random(21);
            int duration = random.Next();
            double alpha = random.NextDouble();
            double realization = random.NextDouble();

            // Call
            var stochast = new RealizedStochast(name, duration, alpha, realization);

            // Assert
            Assert.IsInstanceOf<Stochast>(stochast);
            Assert.AreEqual(realization, stochast.Realization,
                            stochast.Realization.GetAccuracy());
            Assert.AreEqual(5, stochast.Realization.NumberOfDecimalPlaces);

            Assert.AreEqual(name, stochast.Name);
            Assert.AreEqual(duration, stochast.Duration);
            Assert.AreEqual(alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
        }
    }
}