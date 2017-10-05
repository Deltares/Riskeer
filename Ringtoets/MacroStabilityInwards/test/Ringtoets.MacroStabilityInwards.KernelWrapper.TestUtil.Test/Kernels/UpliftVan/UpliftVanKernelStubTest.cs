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
using System.Collections.Generic;
using Deltares.WTIStability.Data.Standard;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.UpliftVan
{
    [TestFixture]
    public class UpliftVanKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new UpliftVanKernelStub();

            // Assert
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateFalse_SetCalculatedTrue()
        {
            // Setup
            var kernel = new UpliftVanKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.Calculate();

            // Assert
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateTrue_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            var kernel = new UpliftVanKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void Validate_ThrowExceptionOnValidateAndReturnValidationResultsFalse_SetValidatedTrue()
        {
            // Setup
            var kernel = new UpliftVanKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Validated);

            // Call
            kernel.Validate();

            // Assert
            Assert.IsTrue(kernel.Validated);
        }

        [Test]
        public void Validate_ReturnValidationResultsTrue_ReturnsValidationResults()
        {
            // Setup
            var calculator = new UpliftVanKernelStub
            {
                ReturnValidationResults = true
            };

            // Call
            List<Tuple<ValidationResultType, string>> results = calculator.Validate();

            // Assert
            Assert.IsFalse(calculator.Validated);
            CollectionAssert.AreEqual(new List<Tuple<ValidationResultType, string>>
            {
                new Tuple<ValidationResultType, string>(ValidationResultType.Warning, "Validation Warning"),
                new Tuple<ValidationResultType, string>(ValidationResultType.Error, "Validation Error"),
                new Tuple<ValidationResultType, string>(ValidationResultType.Info, "Validation Info"),
                new Tuple<ValidationResultType, string>(ValidationResultType.Debug, "Validation Debug")
            }, results);
        }

        [Test]
        public void Validate_ThrowExceptionOnValidateTrue_ThrowsUpliftVanKernelWrapperException()
        {
            // Setup
            var kernel = new UpliftVanKernelStub
            {
                ThrowExceptionOnValidate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Validated);

            // Call
            TestDelegate test = () => kernel.Validate();

            // Assert
            var exception = Assert.Throws<UpliftVanKernelWrapperException>(test);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Validated);
        }
    }
}