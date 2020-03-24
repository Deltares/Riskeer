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
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandlerTest
    {
        [Test]
        public void Constructor_CollectionDescriptionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper, null, Enumerable.Empty<IObservable>);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("collectionDescription", exception.ParamName);
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
            TestDelegate call = () => new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper, string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("clearIllustrationPointsFunc", exception.ParamName);
            mocks.ReplayAll();
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Cal
            var handler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper, string.Empty, Enumerable.Empty<IObservable>);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase>(handler);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_Always_UsesExpectedInquiryAndReturnsExpectedConfirmation(bool expectedConfirmation)
        {
            // Setup
            const string collectionDescription = "Verzameling";
            string inquiry = $"Weet u zeker dat u alle berekende illustratiepunten bij '{collectionDescription}' wilt wissen?";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(h => h.InquireContinuation(inquiry))
                         .Return(expectedConfirmation);
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper, collectionDescription, Enumerable.Empty<IObservable>);

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
            mocks.VerifyAll();
        }

        [Test]
        public void ClearIllustrationPoints_Always_ExecutesClearIllustrationPointsFunc()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            IObservable[] observables =
            {
                observable
            };

            var handler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper, string.Empty, () => observables);

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ClearIllustrationPoints();

            // Assert
            Assert.AreSame(observables, affectedObjects);
            mocks.VerifyAll();
        }
    }
}