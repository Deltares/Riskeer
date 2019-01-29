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
using System.Collections.Generic;
using Core.Common.Util;
using NUnit.Framework;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class TargetProbabilityCalculationServiceHelperTest
    {
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
        [TestCaseSource(nameof(ValidTargetProbabilities))]
        public void ValidateTargetProbability_ValidTargetProbability_ReturnsTrueAndHandlesNoLogMessage(double targetProbability)
        {
            // Setup
            string logMessage = string.Empty;

            // Call
            bool isValid = TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(targetProbability, lm => logMessage = lm);

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
        [TestCaseSource(nameof(TargetProbabilitiesThatAreTooBig))]
        public void ValidateTargetProbability_TargetProbabilityTooBig_ReturnsFalseAndHandlesExpectedLogMessage(double targetProbability)
        {
            // Setup
            string logMessage = string.Empty;

            // Call
            bool isValid = TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(targetProbability, lm => logMessage = lm);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual("Doelkans is te groot om een berekening uit te kunnen voeren.", logMessage);
        }

        [Test]
        [TestCaseSource(nameof(TargetProbabilitiesThatAreTooSmall))]
        public void ValidateTargetProbability_TargetProbabilityTooSmall_ReturnsFalseAndHandlesExpectedLogMessage(double targetProbability)
        {
            // Setup
            string logMessage = string.Empty;

            // Call
            bool isValid = TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(targetProbability, lm => logMessage = lm);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual("Doelkans is te klein om een berekening uit te kunnen voeren.", logMessage);
        }

        private static IEnumerable<TestCaseData> ValidTargetProbabilities()
        {
            yield return new TestCaseData(0.005);
            yield return new TestCaseData(StatisticsConverter.ReliabilityToProbability(-1));
        }

        private static IEnumerable<TestCaseData> TargetProbabilitiesThatAreTooBig()
        {
            yield return new TestCaseData(StatisticsConverter.ReliabilityToProbability(-1.005));
            yield return new TestCaseData(5.0);
        }

        private static IEnumerable<TestCaseData> TargetProbabilitiesThatAreTooSmall()
        {
            yield return new TestCaseData(StatisticsConverter.ReliabilityToProbability(40.005));
            yield return new TestCaseData(0.0);
        }
    }
}