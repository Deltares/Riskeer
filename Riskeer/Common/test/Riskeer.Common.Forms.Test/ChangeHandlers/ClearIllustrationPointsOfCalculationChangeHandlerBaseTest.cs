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
using Core.Common.Base;
using Core.Gui.Helpers;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfCalculationChangeHandlerBaseTest
    {
        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestClearIllustrationPointsOfCalculationChangeHandler(null, new TestCalculation());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Call
            TestDelegate call = () => new TestClearIllustrationPointsOfCalculationChangeHandler(inquiryHelper, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Call
            var handler = new TestClearIllustrationPointsOfCalculationChangeHandler(inquiryHelper, new TestCalculation());

            // Assert
            Assert.IsInstanceOf<IClearIllustrationPointsOfCalculationChangeHandler>(handler);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_Always_DisplaysInquiryAndReturnsConfirmation(bool expectedConfirmation)
        {
            // Setup
            const string expectedInquiry = "Weet u zeker dat u de illustratiepunten van deze berekening wilt wissen?";
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.InquireContinuation(expectedInquiry).Returns(expectedConfirmation);
            var handler = new TestClearIllustrationPointsOfCalculationChangeHandler(inquiryHelper, new TestCalculation());

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
        }

        [Test]
        public void DoPostUpdateActions_Always_NotifiesCalculationObservers()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var observer = Substitute.For<IObserver>();
            var calculation = new TestCalculation();
            calculation.Attach(observer);

            var handler = new TestClearIllustrationPointsOfCalculationChangeHandler(inquiryHelper, calculation);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            observer.Received().UpdateObserver();
        }

        private class TestClearIllustrationPointsOfCalculationChangeHandler : ClearIllustrationPointsOfCalculationChangeHandlerBase<TestCalculation>
        {
            public TestClearIllustrationPointsOfCalculationChangeHandler(IInquiryHelper inquiryHelper, TestCalculation calculation)
                : base(inquiryHelper, calculation) {}

            public override bool ClearIllustrationPoints()
            {
                throw new NotImplementedException();
            }
        }
    }
}