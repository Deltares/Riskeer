// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Commands
{
    [TestFixture]
    public class ApplicationFeatureCommandHandlerTest
    {
        [Test]
        public void ShowPropertiesFor_InitializeAndShowPropertyGrid()
        {
            // Setup
            var mocks = new MockRepository();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            var mainWindow = mocks.Stub<IMainWindow>();
            mainWindow.Expect(w => w.InitPropertiesWindowAndActivate());
            mocks.ReplayAll();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolver, mainWindow);

            // Call
            commandHandler.ShowPropertiesForSelection();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CanShowPropertiesFor_ObjectHasProperties_ReturnTrue()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(target))
                            .Return(mocks.Stub<IObjectProperties>());
            var mainWindow = mocks.Stub<IMainWindow>();
            mocks.ReplayAll();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolver, mainWindow);

            // Call
            bool result = commandHandler.CanShowPropertiesFor(target);

            // Assert
            Assert.IsTrue(result);
            mocks.VerifyAll();
        }

        [Test]
        public void CanShowPropertiesFor_ObjectDoesNotHaveProperties_ReturnFalse()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var propertyResolver = mocks.Stub<IPropertyResolver>();
            propertyResolver.Expect(r => r.GetObjectProperties(target))
                            .Return(null);
            var mainWindow = mocks.Stub<IMainWindow>();
            mocks.ReplayAll();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolver, mainWindow);

            // Call
            bool result = commandHandler.CanShowPropertiesFor(target);

            // Assert
            Assert.IsFalse(result);
            mocks.VerifyAll();
        }
    }
}