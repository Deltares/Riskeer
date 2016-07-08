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

using System;

using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewManager;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    [TestFixture]
    public class ActiveViewChangeEventArgsTest
    {
        [Test]
        public void ParameteredConstructor_OnlyActiveView_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var activeView = mocks.Stub<IView>();
            mocks.ReplayAll();

            // Call
            var eventArgs = new ActiveViewChangeEventArgs(activeView);

            // Assert
            Assert.IsInstanceOf<EventArgs>(eventArgs);
            Assert.AreSame(activeView, eventArgs.View);
            Assert.IsNull(eventArgs.OldView);

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_SwitchedToOtherView_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var activeView = mocks.Stub<IView>();
            var previouslyActiveView = mocks.Stub<IView>();
            mocks.ReplayAll();

            // Call
            var eventArgs = new ActiveViewChangeEventArgs(activeView, previouslyActiveView);

            // Assert
            Assert.IsInstanceOf<EventArgs>(eventArgs);
            Assert.AreSame(activeView, eventArgs.View);
            Assert.AreSame(previouslyActiveView, eventArgs.OldView);

            mocks.VerifyAll();
        }
    }
}