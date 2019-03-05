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
    public class ClearIllustrationPointsChangeHandlerBaseTest
    {
        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestClearIllustrationPointsChangeHandler(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void Constructor_InquiryNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TestClearIllustrationPointsChangeHandler(inquiryHelper, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("inquiry", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_Always_UsesExpectedInquiryAndReturnsExpectedConfirmation(bool expectedConfirmation)
        {
            // Setup
            const string inquiry = "Inquiry";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquireContinuation(inquiry)).Return(expectedConfirmation);
            mocks.ReplayAll();

            var handler = new TestClearIllustrationPointsChangeHandler(inquiryHelper, inquiry);

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
            mocks.VerifyAll();
        }

        private class TestClearIllustrationPointsChangeHandler : ClearIllustrationPointsChangeHandlerBase
        {
            public TestClearIllustrationPointsChangeHandler(IInquiryHelper helper, string inquiry) : base(helper, inquiry) {}

            public override IEnumerable<IObservable> ClearIllustrationPoints()
            {
                throw new NotImplementedException();
            }
        }
    }
}