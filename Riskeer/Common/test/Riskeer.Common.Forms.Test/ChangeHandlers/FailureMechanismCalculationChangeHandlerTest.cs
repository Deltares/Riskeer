// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using Core.Gui.Helpers;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.IO;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class FailureMechanismCalculationChangeHandlerTest
    {
        [Test]
        public void Constructor_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var inquiryHandler = Substitute.For<IInquiryHelper>();

            // Call
            void Call() => new FailureMechanismCalculationChangeHandler(null, string.Empty, inquiryHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithoutQuery_ThrowsArgumentNullException()
        {
            // Setup
            var inquiryHandler = Substitute.For<IInquiryHelper>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();

            // Call
            void Call() => new FailureMechanismCalculationChangeHandler(failureMechanism, null, inquiryHandler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("query", paramName);
        }

        [Test]
        public void Constructor_WithoutInquiryHandler_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();

            // Call
            void Call() => new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("inquiryHandler", paramName);
        }

        [Test]
        public void Constructor_WithParameters_ImplementsExpectedInterface()
        {
            // Setup
            var inquiryHandler = Substitute.For<IInquiryHelper>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            // Call
            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHandler);

            // Assert
            Assert.IsInstanceOf<IConfirmDataChangeHandler>(handler);
        }

        [Test]
        public void RequireConfirmation_FailureMechanismWithoutCalculations_ReturnsFalse()
        {
            // Setup
            var inquiryHandler = Substitute.For<IInquiryHelper>();
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());

            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHandler);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsFalse(requireConfirmation);
        }

        [Test]
        public void RequireConfirmation_FailureMechanismWithCalculationWithoutOutput_ReturnFalse()
        {
            // Setup
            var inquiryHandler = Substitute.For<IInquiryHelper>();

            var calculation = Substitute.For<ICalculation>();
            calculation.HasOutput.Returns(false);
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });

            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHandler);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsFalse(requireConfirmation);
        }

        [Test]
        public void RequireConfirmation_FailureMechanismWithCalculationWithOutput_ReturnTrue()
        {
            // Setup
            var inquiryHandler = Substitute.For<IInquiryHelper>();

            var calculation = Substitute.For<ICalculation>();
            calculation.HasOutput.Returns(true);
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });

            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHandler);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsTrue(requireConfirmation);
        }

        [Test]
        [TestCase("I am a query", true)]
        [TestCase("I am a query", false)]
        [TestCase("", true)]
        [TestCase("", false)]
        [TestCase("     ", true)]
        [TestCase("     ", false)]
        public void InquireConfirmation_Always_ShowsConfirmationDialogReturnResultOfInquiry(string message, bool expectedResult)
        {
            // Setup
            var inquiryHandler = Substitute.For<IInquiryHelper>();
            inquiryHandler.InquireContinuation(message).Returns(expectedResult);
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, message, inquiryHandler);

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}