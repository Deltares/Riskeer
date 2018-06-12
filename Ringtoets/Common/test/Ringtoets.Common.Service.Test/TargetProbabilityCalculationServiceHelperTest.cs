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

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class TargetProbabilityCalculationServiceHelperTest
    {
        [Test]
        public void IsValidTargetProbability_ValidTargetProbability_ReturnsTrue()
        {
            // Call
            bool isValid = TargetProbabilityCalculationServiceHelper.IsValidTargetProbability(0.005);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void IsValidTargetProbability_TargetProbabilityInvalid_ReturnsFalse()
        {
            // Call
            bool isValid = TargetProbabilityCalculationServiceHelper.IsValidTargetProbability(double.NaN);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidTargetProbability_TargetProbabilityTooBig_ReturnsFalse()
        {
            // Call
            bool isValid = TargetProbabilityCalculationServiceHelper.IsValidTargetProbability(1.0);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void ValidateTargetProbability_HandleLogMessageActionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(0.005, null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handleLogMessageAction", parameter);
        }

        [Test]
        public void ValidateTargetProbability_ValidTargetProbability_ReturnsTrueAndHandlesNoLogMessage()
        {
            // Setup
            string logMessage = string.Empty;

            // Call
            bool isValid = TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(0.005, lm => logMessage = lm);

            // Assert
            Assert.IsTrue(isValid);
            Assert.IsEmpty(logMessage);
        }

        [Test]
        public void ValidateTargetProbability_TargetProbabilityInvalid_ReturnsFalseAndHandlesExpectedLogMessage()
        {
            // Setup
            string logMessage = string.Empty;

            // Call
            bool isValid = TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(double.NaN, lm => logMessage = lm);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual("Kon geen doelkans bepalen voor deze berekening.", logMessage);
        }

        [Test]
        public void ValidateTargetProbability_TargetProbabilityTooBig_ReturnsFalseAndHandlesExpectedLogMessage()
        {
            // Setup
            string logMessage = string.Empty;

            // Call
            bool isValid = TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(1.0, lm => logMessage = lm);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual("Doelkans is te groot om een berekening uit te kunnen voeren.", logMessage);
        }
    }
}