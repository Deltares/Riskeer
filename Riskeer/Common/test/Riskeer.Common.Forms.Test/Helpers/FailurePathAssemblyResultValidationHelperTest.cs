﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class FailurePathAssemblyResultValidationHelperTest
    {
        [Test]
        public void GetValidationError_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailurePathAssemblyResultValidationHelper.GetValidationError(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void GetValidationError_WithResultManualAndInvalidProbability_ReturnsErrorMessage()
        {
            // Setup
            var result = new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual,
                ManualFailurePathAssemblyProbability = double.NaN
            };

            // Call
            string message = FailurePathAssemblyResultValidationHelper.GetValidationError(result);

            // Assert
            Assert.AreEqual("Er moet een waarde worden ingevuld voor de faalkans.", message);
        }

        [Test]
        public void GetValidationError_WithResultManualAndValidProbability_ReturnsEmptyMessage()
        {
            // Setup
            var random = new Random(21);
            var result = new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Manual,
                ManualFailurePathAssemblyProbability = random.NextDouble()
            };

            // Call
            string message = FailurePathAssemblyResultValidationHelper.GetValidationError(result);

            // Assert
            Assert.IsEmpty(message);
        }

        [Test]
        public void GetValidationError_WithResultAutomatic_ReturnsEmptyMessage()
        {
            // Setup
            var result = new FailurePathAssemblyResult
            {
                ProbabilityResultType = FailurePathAssemblyProbabilityResultType.Automatic
            };

            // Call
            string message = FailurePathAssemblyResultValidationHelper.GetValidationError(result);

            // Assert
            Assert.IsEmpty(message);
        }
    }
}