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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailurePath;

namespace Riskeer.Common.Data.Test.FailurePath
{
    [TestFixture]
    public class FailurePathAssemblyResultTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var result = new FailurePathAssemblyResult();

            // Assert
            Assert.IsInstanceOf<Observable>(result);
            
            Assert.AreEqual(FailurePathAssemblyProbabilityResultType.Automatic, result.ProbabilityResultType);
            Assert.IsNaN(result.ManualFailurePathAssemblyProbability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-20)]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12)]
        public void ManualInitialFailureMechanismResultProfileProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double newValue)
        {
            // Setup
            var result = new FailurePathAssemblyResult();

            // Call
            void Call() => result.ManualFailurePathAssemblyProbability = newValue;

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
        public void ManualInitialFailureMechanismResultProfileProbability_ValidValue_NewValueSet(double newValue)
        {
            // Setup
            var result = new FailurePathAssemblyResult();

            // Call
            result.ManualFailurePathAssemblyProbability = newValue;

            // Assert
            Assert.AreEqual(newValue, result.ManualFailurePathAssemblyProbability);
        }
    }
}