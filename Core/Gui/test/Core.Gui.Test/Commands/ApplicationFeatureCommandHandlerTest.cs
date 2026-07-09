// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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

using Core.Gui.Commands;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.PropertyView;
using Core.Gui.PropertyBag;
using NSubstitute;
using NUnit.Framework;

namespace Core.Gui.Test.Commands
{
    [TestFixture]
    public class ApplicationFeatureCommandHandlerTest
    {
        [Test]
        public void ShowPropertiesFor_Always_PropertiesInitializedOrBroughtToFront()
        {
            // Setup
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var mainWindow = Substitute.For<IMainWindow>();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolver, mainWindow);

            // Call
            commandHandler.ShowPropertiesForSelection();

            // Assert
            mainWindow.Received().InitPropertiesWindowOrBringToFront();
        }

        [Test]
        public void CanShowPropertiesFor_ObjectHasProperties_ReturnTrue()
        {
            // Setup
            var target = new object();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(target).Returns(Substitute.For<IObjectProperties>());
            var mainWindow = Substitute.For<IMainWindow>();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolver, mainWindow);

            // Call
            bool result = commandHandler.CanShowPropertiesFor(target);

            // Assert
            Assert.IsTrue(result);
            propertyResolver.Received().GetObjectProperties(target);
        }

        [Test]
        public void CanShowPropertiesFor_ObjectDoesNotHaveProperties_ReturnFalse()
        {
            // Setup
            var target = new object();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            propertyResolver.GetObjectProperties(target).Returns((IObjectProperties) null);
            var mainWindow = Substitute.For<IMainWindow>();

            var commandHandler = new ApplicationFeatureCommandHandler(propertyResolver, mainWindow);

            // Call
            bool result = commandHandler.CanShowPropertiesFor(target);

            // Assert
            Assert.IsFalse(result);
            propertyResolver.Received().GetObjectProperties(target);
        }
    }
}