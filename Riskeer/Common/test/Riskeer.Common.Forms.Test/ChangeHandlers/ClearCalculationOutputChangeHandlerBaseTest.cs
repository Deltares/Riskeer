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
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearCalculationOutputChangeHandlerBaseTest
    {
        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestClearCalculationOutputChangeHandler(null, inquiryHelper, viewCommands);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestClearCalculationOutputChangeHandler(Enumerable.Empty<ICalculation>(), null, viewCommands);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestClearCalculationOutputChangeHandler(Enumerable.Empty<ICalculation>(), inquiryHelper, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("viewCommands", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            var changeHandler = new TestClearCalculationOutputChangeHandler(Enumerable.Empty<ICalculation>(), inquiryHelper, viewCommands);

            // Assert
            Assert.IsInstanceOf<IClearCalculationOutputChangeHandler>(changeHandler);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_Always_DisplaysInquiryAndReturnsConfirmation(bool expectedConfirmation)
        {
            // Setup
            const string expectedInquiry = "Weet u zeker dat u alle uitvoer wilt wissen?";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquireContinuation(expectedInquiry)).Return(expectedConfirmation);
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var changeHandler = new TestClearCalculationOutputChangeHandler(Enumerable.Empty<ICalculation>(), inquiryHelper, viewCommands);

            // Call
            bool confirmation = changeHandler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearCalculations_Always_ExecutesPreUpdateActionsAndClearsOutput()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.Stub<IViewCommands>();
            var calculation1 = mocks.StrictMock<ICalculation>();
            calculation1.Expect(c => c.ClearOutput());
            var calculation2 = mocks.StrictMock<ICalculation>();
            calculation2.Expect(c => c.ClearOutput());
            mocks.ReplayAll();

            ICalculation[] calculations =
            {
                calculation1,
                calculation2
            };

            var changeHandler = new TestClearCalculationOutputChangeHandler(calculations, inquiryHelper, viewCommands);

            // Precondition
            Assert.IsFalse(changeHandler.DoPreUpdateActionsExecuted);

            // Call
            IEnumerable<IObservable> affectedCalculations = changeHandler.ClearCalculations();

            // Assert
            Assert.IsTrue(changeHandler.DoPreUpdateActionsExecuted);
            CollectionAssert.AreEqual(calculations, affectedCalculations);
            mocks.VerifyAll();
        }

        private class TestClearCalculationOutputChangeHandler : ClearCalculationOutputChangeHandlerBase<ICalculation>
        {
            public TestClearCalculationOutputChangeHandler(IEnumerable<ICalculation> calculations, IInquiryHelper inquiryHelper, IViewCommands viewCommands)
                : base(calculations, inquiryHelper, viewCommands) {}

            public bool DoPreUpdateActionsExecuted { get; private set; }

            protected override void DoPreUpdateActions()
            {
                DoPreUpdateActionsExecuted = true;
            }
        }
    }
}