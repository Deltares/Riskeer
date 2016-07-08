// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewManager;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    [TestFixture]
    public class DockTabClosingEventArgsTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var eventArgs = new DockTabClosingEventArgs();

            // Assert
            Assert.IsNull(eventArgs.View);
            Assert.IsFalse(eventArgs.Cancel);
        }

        [Test]
        public void SimpleProperties_SetNewValue_GetNewValue()
        {
            // Setup
            var mocks = new MockRepository();
            var view = mocks.Stub<IView>();
            mocks.ReplayAll();

            var eventArgs = new DockTabClosingEventArgs();

            // Call
            eventArgs.View = view;
            eventArgs.Cancel = true;

            // Assert
            Assert.AreEqual(view, eventArgs.View);
            Assert.IsTrue(eventArgs.Cancel);

            mocks.VerifyAll();
        }
    }
}