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
using Core.Common.Base;
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfCalculationChangeHandlerBaseTest
    {
        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestClearIllustrationPointsOfCalculationChangeHandler(null, inquiryHelper);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }
        
        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestClearIllustrationPointsOfCalculationChangeHandler(new TestCalculation(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            var handler = new TestClearIllustrationPointsOfCalculationChangeHandler(new TestCalculation(), inquiryHelper);

            // Assert
            Assert.IsInstanceOf<IClearIllustrationPointsOfCalculationChangeHandler>(handler);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_Always_DisplaysInquiryAndReturnsConfirmation(bool expectedConfirmation)
        {
            // Setup
            const string expectedInquiry = "Weet u zeker dat u de illustratiepunten van deze berekening wilt wissen?";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquireContinuation(expectedInquiry)).Return(expectedConfirmation);
            mocks.ReplayAll();

            var handler = new TestClearIllustrationPointsOfCalculationChangeHandler(new TestCalculation(), inquiryHelper);

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
            mocks.ReplayAll();
        }

        [Test]
        public void DoPostUpdateActions_Always_NotifiesCalculationObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            calculation.Attach(observer);

            var handler = new TestClearIllustrationPointsOfCalculationChangeHandler(calculation, inquiryHelper);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
        }

        private class TestClearIllustrationPointsOfCalculationChangeHandler : ClearIllustrationPointsOfCalculationChangeHandlerBase<TestCalculation>
        {
            public TestClearIllustrationPointsOfCalculationChangeHandler(TestCalculation calculation, IInquiryHelper inquiryHelper)
                : base(calculation, inquiryHelper) {}

            public override bool ClearIllustrationPoints()
            {
                throw new NotImplementedException();
            }
        }
    }
}