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
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
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
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new FailureMechanismCalculationChangeHandler(null, string.Empty, inquiryHelper);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutQuery_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new FailureMechanismCalculationChangeHandler(failureMechanism, null, inquiryHelper);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("query", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutInquiryHelper_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("inquiryHelper", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithParameters_ImplementsExpectedInterface()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHelper);

            // Assert
            Assert.IsInstanceOf<IConfirmDataChangeHandler>(handler);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RequireConfirmation_FailureMechanismWithoutCalculations_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.StrictMock<IInquiryHelper>();
            mockRepository.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHelper);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsFalse(requireConfirmation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RequireConfirmation_FailureMechanismWithCalculationWithoutOutput_ReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.StrictMock<IInquiryHelper>();

            var calculation = mockRepository.StrictMock<ICalculation>();
            calculation.Expect(calc => calc.HasOutput).Return(false);
            mockRepository.ReplayAll();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHelper);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsFalse(requireConfirmation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RequireConfirmation_FailureMechanismWithCalculationWithOutput_ReturnTrue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.StrictMock<IInquiryHelper>();

            var calculation = mockRepository.StrictMock<ICalculation>();
            calculation.Expect(calc => calc.HasOutput).Return(true);
            mockRepository.ReplayAll();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, string.Empty, inquiryHelper);

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsTrue(requireConfirmation);
            mockRepository.VerifyAll();
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
            var mockRepository = new MockRepository();
            var inquiryHelper = mockRepository.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.InquireContinuation(message)).Return(expectedResult);
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var handler = new FailureMechanismCalculationChangeHandler(failureMechanism, message, inquiryHelper);

            // Call
            bool result = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedResult, result);
            mockRepository.VerifyAll();
        }
    }
}