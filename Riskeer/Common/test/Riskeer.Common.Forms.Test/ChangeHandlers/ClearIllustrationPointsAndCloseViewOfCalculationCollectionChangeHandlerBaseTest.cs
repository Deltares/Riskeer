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
using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsAndCloseViewOfCalculationCollectionChangeHandlerBaseTest
    {
        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();
            
            // Call
            void Call() => new TestClearIllustrationPointsAndCloseViewOfCalculationCollectionChangeHandler(inquiryHelper, null);

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
            var changeHandler = new TestClearIllustrationPointsAndCloseViewOfCalculationCollectionChangeHandler(
                inquiryHelper, viewCommands);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase>(changeHandler);
            mocks.VerifyAll();
        }
        
        private class TestClearIllustrationPointsAndCloseViewOfCalculationCollectionChangeHandler : ClearIllustrationPointsAndCloseViewOfCalculationCollectionChangeHandlerBase
        {
            public TestClearIllustrationPointsAndCloseViewOfCalculationCollectionChangeHandler(
                IInquiryHelper inquiryHelper, IViewCommands viewCommands) 
                : base(inquiryHelper, viewCommands) {}
            
            protected override string GetConfirmationMessage()
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<IObservable> PerformClearIllustrationPoints()
            {
                throw new NotImplementedException();
            }

            protected override void CloseView(IViewCommands viewCommands)
            {
                throw new NotImplementedException();
            }
        }
    }
}