// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismAssemblyResultTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var result = new FailureMechanismAssemblyResult();

            // Assert
            Assert.IsInstanceOf<Observable>(result);

            Assert.AreEqual(FailureMechanismAssemblyProbabilityResultType.None, result.ProbabilityResultType);
            Assert.IsNaN(result.ManualFailureMechanismAssemblyProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void ManualInitialFailureMechanismResultProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double newValue)
        {
            // Setup
            var result = new FailureMechanismAssemblyResult();

            // Call
            void Call() => result.ManualFailureMechanismAssemblyProbability = newValue;

            // Assert
            const string message = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1e-6)]
        [TestCase(0.5)]
        [TestCase(1 - 1e-6)]
        [TestCase(1)]
        [TestCase(double.NaN)]
        public void ManualInitialFailureMechanismResultProbability_ValidValue_NewValueSet(double newValue)
        {
            // Setup
            var result = new FailureMechanismAssemblyResult();

            // Call
            result.ManualFailureMechanismAssemblyProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.ManualFailureMechanismAssemblyProbability);
        }
    }
}