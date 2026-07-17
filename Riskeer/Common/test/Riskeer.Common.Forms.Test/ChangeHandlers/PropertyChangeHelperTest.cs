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
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class PropertyChangeHelperTest
    {
        [Test]
        public void ChangePropertyAndNotify_WithoutPropertySetDelegate_ThrowsArgumentNullException()
        {
            // Setup
            var handler = Substitute.For<IObservablePropertyChangeHandler>();
            // Call
            TestDelegate test = () => PropertyChangeHelper.ChangePropertyAndNotify(null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setPropertyDelegate", exception.ParamName);
        }

        [Test]
        public void ChangePropertyAndNotify_WithoutChangeHandler_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PropertyChangeHelper.ChangePropertyAndNotify(() => {}, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("changeHandler", exception.ParamName);
        }

        [Test]
        public void ChangePropertyAndNotify_ChangeHasNoAffectedObjects_HandlerUsedForSetAction()
        {
            // Setup
            SetObservablePropertyValueDelegate setAction = () => {};
            var handler = Substitute.For<IObservablePropertyChangeHandler>();
            handler.SetPropertyValueAfterConfirmation(setAction).Returns(Enumerable.Empty<IObservable>());
            // Call
            PropertyChangeHelper.ChangePropertyAndNotify(setAction, handler);
            // Assert
            handler.Received().SetPropertyValueAfterConfirmation(setAction);
        }

        [Test]
        public void ChangePropertyAndNotify_ChangeHasAffectedObjects_AffectedObjectsNotified()
        {
            // Setup
            SetObservablePropertyValueDelegate setAction = () => {};
            var observableA = Substitute.For<IObservable>();
            var observableB = Substitute.For<IObservable>();

            IObservable[] affectedObjects =
            {
                observableA,
                observableB
            };

            var handler = Substitute.For<IObservablePropertyChangeHandler>();
            handler.SetPropertyValueAfterConfirmation(setAction).Returns(affectedObjects);
            // Call
            PropertyChangeHelper.ChangePropertyAndNotify(setAction, handler);

            // Assert
            observableA.Received(1).NotifyObservers();
            observableB.Received(1).NotifyObservers();
        }
    }
}