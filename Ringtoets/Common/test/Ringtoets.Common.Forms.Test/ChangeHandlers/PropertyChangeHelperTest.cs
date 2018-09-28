// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class PropertyChangeHelperTest
    {
        [Test]
        public void ChangePropertyAndNotify_WithoutPropertySetDelegate_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => PropertyChangeHelper.ChangePropertyAndNotify(null, handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setPropertyDelegate", exception.ParamName);
            mocks.VerifyAll();
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

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IObservablePropertyChangeHandler>();
            handler.Expect(h => h.SetPropertyValueAfterConfirmation(setAction)).Return(Enumerable.Empty<IObservable>());
            mocks.ReplayAll();

            // Call
            PropertyChangeHelper.ChangePropertyAndNotify(setAction, handler);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ChangePropertyAndNotify_ChangeHasAffectedObjects_AffectedObjectsNotified()
        {
            // Setup
            SetObservablePropertyValueDelegate setAction = () => {};

            var mocks = new MockRepository();
            var observableA = mocks.StrictMock<IObservable>();
            observableA.Expect(o => o.NotifyObservers());
            var observableB = mocks.StrictMock<IObservable>();
            observableB.Expect(o => o.NotifyObservers());

            IObservable[] affectedObjects =
            {
                observableA,
                observableB
            };

            var handler = mocks.StrictMock<IObservablePropertyChangeHandler>();
            handler.Expect(h => h.SetPropertyValueAfterConfirmation(setAction)).Return(affectedObjects);
            mocks.ReplayAll();

            // Call
            PropertyChangeHelper.ChangePropertyAndNotify(setAction, handler);

            // Assert
            mocks.VerifyAll();
        }
    }
}