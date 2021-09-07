// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsForTargetProbabilityTest
    {
        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.11)]
        public void Constructor_InvalidTargetProbability_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbability(invalidValue);

            // Assert
            const string expectedMessage = "De waarde van de doelkans moet groter zijn dan 0 en kleiner dan of gelijk aan 0,1.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(1e-100)]
        [TestCase(0.05)]
        [TestCase(0.1)]
        public void Constructor_ValidTargetProbability_ExpectedProperties(double validValue)
        {
            // Call
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(validValue);

            // Assert
            Assert.IsInstanceOf<Observable>(calculationsForTargetProbability);
            Assert.AreEqual(validValue, calculationsForTargetProbability.TargetProbability);
            Assert.IsEmpty(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0.0)]
        [TestCase(0.11)]
        public void TargetProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);

            // Call
            void Call() => calculationsForTargetProbability.TargetProbability = invalidValue;

            // Assert
            const string expectedMessage = "De waarde van de doelkans moet groter zijn dan 0 en kleiner dan of gelijk aan 0,1.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(1e-100)]
        [TestCase(0.05)]
        [TestCase(0.1)]
        public void TargetProbability_ValidValue_NewValueSet(double validValue)
        {
            // Setup
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);

            // Call
            calculationsForTargetProbability.TargetProbability = validValue;

            // Assert
            Assert.AreEqual(validValue, calculationsForTargetProbability.TargetProbability);
        }
    }
}