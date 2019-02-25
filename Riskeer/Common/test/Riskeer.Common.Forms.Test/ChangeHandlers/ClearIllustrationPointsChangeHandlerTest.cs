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
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsChangeHandlerTest
    {
        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ClearIllustrationPointsChangeHandler(null, string.Empty,
                                                                               () => Enumerable.Empty<IObservable>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void Constructor_ItemDescriptionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ClearIllustrationPointsChangeHandler(inquiryHelper, null,
                                                                               () => Enumerable.Empty<IObservable>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("itemDescription", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ClearIllustrationPointsFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ClearIllustrationPointsChangeHandler(inquiryHelper, string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("clearIllustrationPointsFunc", exception.ParamName);
            mocks.ReplayAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ClearIllustrationPoints_ContinuationTrueOrFalse_ExpectedObserversNotified(bool continuation)
        {
            // Setup
            const string itemDescription = "A";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquireContinuation($"Weet u zeker dat u alle berekende illustratiepunten bij {itemDescription} wilt wissen?"))
                         .Return(continuation);
            var observable = mocks.StrictMock<IObservable>();
            if (continuation)
            {
                observable.Expect(o => o.NotifyObservers());
            }

            mocks.ReplayAll();

            Func<IEnumerable<IObservable>> clearIllustrationPointsFunc = () => new[]
            {
                observable
            };

            var handler = new ClearIllustrationPointsChangeHandler(inquiryHelper, itemDescription, clearIllustrationPointsFunc);

            // Call
            handler.ClearIllustrationPoints();

            // Assert
            mocks.VerifyAll();
        }
    }
}