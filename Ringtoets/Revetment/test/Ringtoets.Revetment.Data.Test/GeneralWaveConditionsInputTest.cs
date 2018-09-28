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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Revetment.Data.Test
{
    [TestFixture]
    public class GeneralWaveConditionsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double a = random.NextDouble();
            double b = random.NextDouble();
            double c = random.NextDouble();

            // Call
            var generalInput = new GeneralWaveConditionsInput(a, b, c);

            // Assert
            Assert.AreEqual(a, generalInput.A, generalInput.A.GetAccuracy());
            Assert.AreEqual(2, generalInput.A.NumberOfDecimalPlaces);
            Assert.AreEqual(b, generalInput.B, generalInput.B.GetAccuracy());
            Assert.AreEqual(2, generalInput.B.NumberOfDecimalPlaces);
            Assert.AreEqual(c, generalInput.C, generalInput.C.GetAccuracy());
            Assert.AreEqual(2, generalInput.C.NumberOfDecimalPlaces);
        }
    }
}