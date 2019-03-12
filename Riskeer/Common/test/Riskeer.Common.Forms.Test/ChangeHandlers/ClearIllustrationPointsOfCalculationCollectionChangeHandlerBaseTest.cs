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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfCalculationCollectionChangeHandlerBaseTest
    {
        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestClearIllustrationPointsOfCalculationCollectionChangeHandler(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            var handler = new TestClearIllustrationPointsOfCalculationCollectionChangeHandler(inquiryHelper);

            // Assert
            Assert.IsInstanceOf<IClearIllustrationPointsOfCalculationCollectionChangeHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_Always_UsesExpectedInquiryAndReturnsExpectedConfirmation(bool expectedConfirmation)
        {
            // Setup
            const string confirmationMessage = "Inquiry";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquireContinuation(confirmationMessage)).Return(expectedConfirmation);
            mocks.ReplayAll();

            var handler = new TestClearIllustrationPointsOfCalculationCollectionChangeHandler(inquiryHelper)
            {
                ConfirmationMessage = confirmationMessage
            };

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.IsTrue(handler.GetConfirmationMessageCalled);
            Assert.AreEqual(expectedConfirmation, confirmation);
            mocks.VerifyAll();
        }

        private class TestClearIllustrationPointsOfCalculationCollectionChangeHandler
            : ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase
        {
            public TestClearIllustrationPointsOfCalculationCollectionChangeHandler(IInquiryHelper helper) : base(helper) {}
            public string ConfirmationMessage { private get; set; }

            public bool GetConfirmationMessageCalled { get; private set; }

            public override IEnumerable<IObservable> ClearIllustrationPoints()
            {
                throw new NotImplementedException();
            }

            protected override string GetConfirmationMessage()
            {
                GetConfirmationMessageCalled = true;
                return ConfirmationMessage;
            }
        }
    }
}