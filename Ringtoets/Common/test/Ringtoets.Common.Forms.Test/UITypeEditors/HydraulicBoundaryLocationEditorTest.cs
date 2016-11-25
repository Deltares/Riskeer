// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.Gui.UITypeEditors;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class HydraulicBoundaryLocationEditorTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void DefaultConstructor_ReturnsNewInstance()
        {
            // Call
            var editor = new HydraulicBoundaryLocationEditor();

            // Assert
            Assert.IsInstanceOf<SelectionEditor<IHasHydraulicBoundaryLocationProperty, HydraulicBoundaryLocation, SelectableHydraulicBoundaryLocation>>(editor);
        }

        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            HydraulicBoundaryLocation selectableHydraulicBoundaryLocationhydraulicBoundaryLocation =
                CreateSelectableHydraulicBoundaryLocation();
            var properties = new ObjectPropertiesWithHydraulicBoundaryLocation(
                selectableHydraulicBoundaryLocationhydraulicBoundaryLocation, new HydraulicBoundaryLocation[0]);
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new HydraulicBoundaryLocationEditor();
            var someValue = new object();
            var serviceProviderStub = mockRepository.Stub<IServiceProvider>();
            var serviceStub = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextStub = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderStub.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceStub);
            descriptorContextStub.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(descriptorContextStub, serviceProviderStub, someValue);

            // Assert
            Assert.AreSame(someValue, result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = CreateSelectableHydraulicBoundaryLocation();
            var properties = new ObjectPropertiesWithHydraulicBoundaryLocation(hydraulicBoundaryLocation, new[]
            {
                hydraulicBoundaryLocation
            });
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new HydraulicBoundaryLocationEditor();
            var someValue = new object();
            var serviceProviderStub = mockRepository.Stub<IServiceProvider>();
            var serviceStub = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextStub = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderStub.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceStub);
            descriptorContextStub.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(descriptorContextStub, serviceProviderStub, someValue);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, result);
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableOptions_InputWithLocationsAndNoReferencePoint_ReturnsLocationsSortedByName()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = CreateSelectableHydraulicBoundaryLocation();
            HydraulicBoundaryLocation[] locations =
            {
                hydraulicBoundaryLocation,
                new HydraulicBoundaryLocation(0, "A", 0, 1),
                new HydraulicBoundaryLocation(0, "C", 0, 2),
                new HydraulicBoundaryLocation(0, "D", 0, 3),
                new HydraulicBoundaryLocation(0, "B", 0, 4),
            };

            var properties = new ObjectPropertiesWithHydraulicBoundaryLocation(hydraulicBoundaryLocation, locations);
            var propertyBag = new DynamicPropertyBag(properties);

            var editor = new TestHydraulicBoundaryLocationEditor();
            var descriptorContextStub = mockRepository.Stub<ITypeDescriptorContext>();
            descriptorContextStub.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                editor.GetItems(descriptorContextStub);

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                locations.Select(location =>
                                 new SelectableHydraulicBoundaryLocation(location, null))
                         .OrderBy(hbl => hbl.HydraulicBoundaryLocation.Name);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableOptions_InputWithLocationsAndReferencePoint_ReturnsLocationsSortedByDistanceThenByName()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = CreateSelectableHydraulicBoundaryLocation();
            HydraulicBoundaryLocation[] locations =
            {
                hydraulicBoundaryLocation,
                new HydraulicBoundaryLocation(0, "A", 0, 10),
                new HydraulicBoundaryLocation(0, "E", 0, 500),
                new HydraulicBoundaryLocation(0, "F", 0, 100),
                new HydraulicBoundaryLocation(0, "D", 0, 200),
                new HydraulicBoundaryLocation(0, "C", 0, 200),
                new HydraulicBoundaryLocation(0, "B", 0, 200)
            };

            var properties = new ObjectPropertiesWithHydraulicBoundaryLocation(hydraulicBoundaryLocation, locations)
            {
                ReferencePoint = new Point2D(0, 0)
            };
            var propertyBag = new DynamicPropertyBag(properties);

            var editor = new TestHydraulicBoundaryLocationEditor();
            var descriptorContextStub = mockRepository.Stub<ITypeDescriptorContext>();
            descriptorContextStub.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                editor.GetItems(descriptorContextStub);

            // Assert
            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                locations.Select(location =>
                                 new SelectableHydraulicBoundaryLocation(location, properties.ReferencePoint))
                         .OrderBy(hbl => hbl.Distance)
                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Name);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenLocationAndReferencePoint_WhenUpdatingReferencePoint_ThenUpdateSelectableBoundaryLocations()
        {
            // Given
            HydraulicBoundaryLocation hydraulicBoundaryLocation = CreateSelectableHydraulicBoundaryLocation();
            HydraulicBoundaryLocation[] locations =
            {
                hydraulicBoundaryLocation,
                new HydraulicBoundaryLocation(0, "A", 0, 10),
                new HydraulicBoundaryLocation(0, "E", 0, 500),
                new HydraulicBoundaryLocation(0, "F", 0, 100),
                new HydraulicBoundaryLocation(0, "D", 0, 200),
                new HydraulicBoundaryLocation(0, "C", 0, 200),
                new HydraulicBoundaryLocation(0, "B", 0, 200)
            };

            var properties = new ObjectPropertiesWithHydraulicBoundaryLocation(hydraulicBoundaryLocation, locations)
            {
                ReferencePoint = new Point2D(0, 0)
            };
            var propertyBag = new DynamicPropertyBag(properties);

            var editor = new TestHydraulicBoundaryLocationEditor();
            var descriptorContextStub = mockRepository.Stub<ITypeDescriptorContext>();
            descriptorContextStub.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            IEnumerable<SelectableHydraulicBoundaryLocation> originalList =
                editor.GetItems(descriptorContextStub).ToList();

            // When
            properties.ReferencePoint = new Point2D(0, 190);

            // Then
            IEnumerable<SelectableHydraulicBoundaryLocation> availableHydraulicBoundaryLocations =
                editor.GetItems(descriptorContextStub).ToList();
            CollectionAssert.AreNotEqual(originalList, availableHydraulicBoundaryLocations);

            IEnumerable<SelectableHydraulicBoundaryLocation> expectedList =
                locations.Select(hbl =>
                                 new SelectableHydraulicBoundaryLocation(hbl, properties.ReferencePoint))
                         .OrderBy(hbl => hbl.Distance)
                         .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Name);
            CollectionAssert.AreEqual(expectedList, availableHydraulicBoundaryLocations);
            mockRepository.VerifyAll();
        }

        private static HydraulicBoundaryLocation CreateSelectableHydraulicBoundaryLocation()
        {
            return new HydraulicBoundaryLocation(1, "", 0, 0);
        }

        private class ObjectPropertiesWithHydraulicBoundaryLocation : IHasHydraulicBoundaryLocationProperty
        {
            private readonly HydraulicBoundaryLocation selectedHydraulicBoundaryLocation;
            private readonly IEnumerable<HydraulicBoundaryLocation> selectableHydraulicBoundaryLocations;

            public ObjectPropertiesWithHydraulicBoundaryLocation(HydraulicBoundaryLocation selectedHydraulicBoundaryLocation,
                                                                 IEnumerable<HydraulicBoundaryLocation> selectableHydraulicBoundaryLocations)
            {
                this.selectedHydraulicBoundaryLocation = selectedHydraulicBoundaryLocation;
                this.selectableHydraulicBoundaryLocations = selectableHydraulicBoundaryLocations;
            }

            public object Data { get; set; }

            public Point2D ReferencePoint { get; set; }

            public HydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
            {
                get
                {
                    return selectedHydraulicBoundaryLocation;
                }
            }

            public IEnumerable<HydraulicBoundaryLocation> GetHydraulicBoundaryLocations()
            {
                return selectableHydraulicBoundaryLocations;
            }

            public Point2D GetReferenceLocation()
            {
                return ReferencePoint;
            }
        }

        private class TestHydraulicBoundaryLocationEditor : HydraulicBoundaryLocationEditor
        {
            public IEnumerable<SelectableHydraulicBoundaryLocation> GetItems(ITypeDescriptorContext context)
            {
                return GetAvailableOptions(context);
            }
        }
    }
}