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
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Gui.PropertyBag;
using Core.Gui.UITypeEditors;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.UITypeEditors;

namespace Riskeer.Common.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class HydraulicBoundaryLocationEditorTest
    {
        [SetUp]
        public void SetUp() {}

        [Test]
        public void DefaultConstructor_ReturnsNewInstance()
        {
            // Call
            var editor = new HydraulicBoundaryLocationEditor();

            // Assert
            Assert.IsInstanceOf<SelectionEditor<IHasHydraulicBoundaryLocationProperty, SelectableHydraulicBoundaryLocation>>(editor);
        }

        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            SelectableHydraulicBoundaryLocation selectableHydraulicBoundaryLocation =
                CreateSelectableHydraulicBoundaryLocation();
            var properties = new ObjectPropertiesWithSelectableHydraulicBoundaryLocation(
                selectableHydraulicBoundaryLocation, new SelectableHydraulicBoundaryLocation[0]);
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new HydraulicBoundaryLocationEditor();
            var someValue = new object();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var descriptorContext = Substitute.For<ITypeDescriptorContext>();
            serviceProvider.GetService(Arg.Any<Type>()).Returns(service);
            descriptorContext.Instance.Returns(propertyBag);
            // Call
            object result = editor.EditValue(descriptorContext, serviceProvider, someValue);

            // Assert
            Assert.AreSame(someValue, result);
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var selectedHydraulicBoundaryLocation = new HydraulicBoundaryLocation(23, "name", 0, 0);
            var properties = new ObjectPropertiesWithSelectableHydraulicBoundaryLocation(
                new SelectableHydraulicBoundaryLocation(selectedHydraulicBoundaryLocation, null),
                new[]
                {
                    new SelectableHydraulicBoundaryLocation(selectedHydraulicBoundaryLocation, null)
                });
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new HydraulicBoundaryLocationEditor();
            var someValue = new object();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var descriptorContext = Substitute.For<ITypeDescriptorContext>();
            serviceProvider.GetService(Arg.Any<Type>()).Returns(service);
            descriptorContext.Instance.Returns(propertyBag);
            // Call
            object result = editor.EditValue(descriptorContext, serviceProvider, someValue);

            // Assert
            Assert.AreEqual(new SelectableHydraulicBoundaryLocation(selectedHydraulicBoundaryLocation, null), result);
        }

        private static SelectableHydraulicBoundaryLocation CreateSelectableHydraulicBoundaryLocation()
        {
            return new SelectableHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "", 0, 0), null);
        }

        private class ObjectPropertiesWithSelectableHydraulicBoundaryLocation : ObjectProperties<object>, IHasHydraulicBoundaryLocationProperty
        {
            private readonly IEnumerable<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations;

            public ObjectPropertiesWithSelectableHydraulicBoundaryLocation(SelectableHydraulicBoundaryLocation selectedHydraulicBoundaryLocation,
                                                                           IEnumerable<SelectableHydraulicBoundaryLocation> selectableHydraulicBoundaryLocations)
            {
                SelectedHydraulicBoundaryLocation = selectedHydraulicBoundaryLocation;
                this.selectableHydraulicBoundaryLocations = selectableHydraulicBoundaryLocations;
            }

            public SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation { get; }

            public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
            {
                return selectableHydraulicBoundaryLocations;
            }
        }
    }
}