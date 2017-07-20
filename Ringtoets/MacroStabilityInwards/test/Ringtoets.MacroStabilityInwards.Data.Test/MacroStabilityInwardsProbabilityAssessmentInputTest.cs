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

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsProbabilityAssessmentInputTest
    {
        [Test]
        public void Constructor_DefaultPropertiesSet()
        {
            // Call
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput();

            // Assert
            Assert.AreEqual(0.033, probabilityAssessmentInput.A);
            Assert.AreEqual(50, probabilityAssessmentInput.B);

            Assert.IsNaN(probabilityAssessmentInput.SectionLength);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-0.1)]
        [TestCase(1.1)]
        [TestCase(8)]
        [TestCase(double.NaN)]
        public void A_InvalidValue_ThrowsArgumentException(double value)
        {
            // Setup
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput();

            // Call
            TestDelegate call = () => probabilityAssessmentInput.A = value;

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("De waarde moet in het bereik [0,0, 1,0] liggen.", exception.Message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.1)]
        [TestCase(1)]
        [TestCase(0.0000001)]
        [TestCase(0.9999999)]
        public void A_ValidValue_SetsValue(double value)
        {
            // Setup
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput();

            // Call
            probabilityAssessmentInput.A = value;

            // Assert
            Assert.AreEqual(value, probabilityAssessmentInput.A);
        }
    }
}