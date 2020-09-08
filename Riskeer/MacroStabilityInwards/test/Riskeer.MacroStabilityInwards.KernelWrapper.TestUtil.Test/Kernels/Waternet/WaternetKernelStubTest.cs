// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Deltares.MacroStability.CSharpWrapper.Input;
using Deltares.MacroStability.CSharpWrapper.Output;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.Waternet
{
    [TestFixture]
    public class WaternetKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new WaternetKernelStub();

            // Assert
            Assert.IsInstanceOf<IWaternetKernel>(kernel);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsFalse(kernel.Validated);
            Assert.IsFalse(kernel.ThrowExceptionOnCalculate);
            Assert.IsFalse(kernel.ThrowExceptionOnValidate);
            Assert.IsFalse(kernel.ReturnValidationResults);

            Assert.IsNull(kernel.KernelInput);
            Assert.IsNull(kernel.Waternet);
        }

        [Test]
        public void SetInput_Always_SetsKernelInputOnKernel()
        {
            // Setup
            var input = new MacroStabilityInput();
            var kernel = new WaternetKernelStub();

            // Call
            kernel.SetInput(input);

            // Assert
            Assert.AreSame(input, kernel.KernelInput);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateFalse_SetCalculatedTrue()
        {
            // Setup
            var kernel = new WaternetKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.Calculate();

            // Assert
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void Calculate_ThrowExceptionOnCalculateTrue_ThrowsWaternetKernelWrapperException()
        {
            // Setup
            var kernel = new WaternetKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            void Call() => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(Call);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void Validate_ThrowExceptionOnValidateFalse_SetValidatedTrue()
        {
            // Setup
            var kernel = new WaternetKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Validated);

            // Call
            kernel.Validate().ToArray();

            // Assert
            Assert.IsTrue(kernel.Validated);
        }

        [Test]
        public void Validate_ThrowExceptionOnValidateTrue_ThrowsWaternetKernelWrapperException()
        {
            // Setup
            var kernel = new WaternetKernelStub
            {
                ThrowExceptionOnValidate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Validated);

            // Call
            void Call() => kernel.Validate().ToArray();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(Call);
            Assert.AreEqual($"Message 1{Environment.NewLine}Message 2", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Validated);
        }

        [Test]
        public void Validate_ReturnValidationResultsTrue_ReturnsValidationResults()
        {
            // Setup
            var kernel = new WaternetKernelStub
            {
                ReturnValidationResults = true
            };

            // Call
            Message[] results = kernel.Validate().ToArray();

            // Assert
            Assert.IsTrue(kernel.Validated);
            Assert.AreEqual(3, results.Length);
            MessageHelper.AssertMessage(MessageHelper.CreateMessage(MessageType.Warning, "Validation Warning"), results[0]);
            MessageHelper.AssertMessage(MessageHelper.CreateMessage(MessageType.Error, "Validation Error"), results[1]);
            MessageHelper.AssertMessage(MessageHelper.CreateMessage(MessageType.Info, "Validation Info"), results[2]);
        }

        [Test]
        public void Validate_ReturnValidationResultsFalse_ReturnsNoValidationResults()
        {
            // Setup
            var kernel = new WaternetKernelStub
            {
                ReturnValidationResults = false
            };

            // Call
            Message[] results = kernel.Validate().ToArray();

            // Assert
            Assert.IsTrue(kernel.Validated);
            CollectionAssert.IsEmpty(results);
        }
    }
}