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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Gui.Helpers;
using NSubstitute;
using NUnit.Framework;
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
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Call
            TestDelegate call = () => new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper, null, Enumerable.Empty<IObservable>);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("collectionDescription", exception.ParamName);
        }

        [Test]
        public void Constructor_ClearIllustrationPointsFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Call
            TestDelegate call = () => new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper, string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("clearIllustrationPointsFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Cal
            var handler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper, string.Empty, Enumerable.Empty<IObservable>);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase>(handler);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void InquireConfirmation_Always_UsesExpectedInquiryAndReturnsExpectedConfirmation(bool expectedConfirmation)
        {
            // Setup
            const string collectionDescription = "Verzameling";
            string inquiry = $"Weet u zeker dat u alle berekende illustratiepunten bij '{collectionDescription}' wilt wissen?";
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.InquireContinuation(inquiry).Returns(expectedConfirmation);
            var handler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper, collectionDescription, Enumerable.Empty<IObservable>);

            // Call
            bool confirmation = handler.InquireConfirmation();

            // Assert
            Assert.AreEqual(expectedConfirmation, confirmation);
        }

        [Test]
        public void ClearIllustrationPoints_Always_ExecutesClearIllustrationPointsFunc()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var observable = Substitute.For<IObservable>();
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
        }
    }
}