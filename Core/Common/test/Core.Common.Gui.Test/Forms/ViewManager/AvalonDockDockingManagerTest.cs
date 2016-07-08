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

using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Utils.Reflection;

using NUnit.Framework;

using Rhino.Mocks;

using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    [TestFixture]
    public class AvalonDockDockingManagerTest
    {
        [Test]
        [RequiresSTA]
        public void GivenViewDockedAsDocument_WhenViewTextIsChanged_ThenViewContainersTitleChangedToNewViewText()
        {
            // Setup
            var mocks = new MockRepository();
            var dockManager = mocks.Stub<DockingManager>();
            var view = new TestView();

            var dock = new AvalonDockDockingManager(dockManager, new[]
            {
                ViewLocation.Document
            });
            dock.Add(view, ViewLocation.Document);

            var layout = TypeUtils.CallPrivateMethod<LayoutContent>(dock, "GetLayoutContent", view);

            // Precondition
            Assert.AreEqual("", layout.Title);

            // Call
            view.Text = @"Test";

            // Assert
            Assert.AreEqual("Test", layout.Title);
        }

        [Test]
        [RequiresSTA]
        public void ActivateView_ChangingActiveViewToDifferentDockedView_OldActiveViewActiveControlIsNull()
        {
            // Setup
            var view = new TestView();
            var view2 = new TestView();

            var dock = new AvalonDockDockingManager(new DockingManager(), new[]
            {
                ViewLocation.Document
            });
            dock.Add(view, ViewLocation.Document);
            dock.Add(view2, ViewLocation.Document);
            dock.ActivateView(view);

            view.ActiveControl = view.Controls[0];

            // Call
            dock.ActivateView(view2);

            // Assert
            Assert.IsNull(view.ActiveControl);
        }
    }
}