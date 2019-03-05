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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfStructuresCalculationHandlerTest
    {
        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ClearIllustrationPointsOfStructuresCalculationHandler<TestStructuresInput>(null,
                                                                                                                     new TestStructuresCalculation());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ClearIllustrationPointsOfStructuresCalculationHandler<TestStructuresInput>(inquiryHelper,
                                                                                                                     null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation();

            // Call
            var handler = new ClearIllustrationPointsOfStructuresCalculationHandler<TestStructuresInput>(inquiryHelper, calculation);

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

            var calculation = new TestStructuresCalculation();

            var handler = new ClearIllustrationPointsOfStructuresCalculationHandler<TestStructuresInput>(inquiryHelper, calculation);

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
            mocks.ReplayAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationConfigurations))]
        public void ClearIllustrationPoints_WithVariousCalculationConfigurations_ClearsIllustrationPointsAndReturnsExpectedResult(
            TestStructuresCalculation calculation,
            bool expectedResult)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfStructuresCalculationHandler<TestStructuresInput>(inquiryHelper, calculation);

            bool hasOutput = calculation.HasOutput;

            // Call
            bool isCalculationAffected = handler.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(expectedResult, isCalculationAffected);
            Assert.AreEqual(hasOutput, calculation.HasOutput);

            if (hasOutput)
            {
                Assert.IsFalse(calculation.Output.HasGeneralResult);
            }
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

            var calculation = new TestStructuresCalculation();
            calculation.Attach(observer);

            var handler = new ClearIllustrationPointsOfStructuresCalculationHandler<TestStructuresInput>(inquiryHelper, calculation);

            // Call
            handler.DoPostUpdateActions();

            // Assert
            mocks.VerifyAll();
        }

        private static IEnumerable<TestCaseData> GetCalculationConfigurations()
        {
            yield return new TestCaseData(new TestStructuresCalculation(), false);
            yield return new TestCaseData(new TestStructuresCalculation
            {
                Output = new TestStructuresOutput()
            }, false);
            yield return new TestCaseData(new TestStructuresCalculation
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            }, true);
        }
    }
}