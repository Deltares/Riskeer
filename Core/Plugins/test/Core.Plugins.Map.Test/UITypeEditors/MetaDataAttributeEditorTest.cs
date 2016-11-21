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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Design;
using Core.Common.Gui.PropertyBag;
using Core.Common.Gui.UITypeEditors;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Plugins.Map.PropertyClasses;
using Core.Plugins.Map.UITypeEditors;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.UITypeEditors
{
    [TestFixture]
    public class MetaDataAttributeEditorTest
    {
        [Test]
        public void DefaultConstructor_ReturnsNewInstance()
        {
            // Call
            var editor = new MetaDataAttributeEditor();

            // Assert
            Assert.IsInstanceOf<SelectionEditor<MapPointDataProperties, string>>(editor);
        }

        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var mapData = new MapPointData("Name");

            var properties = new MapPointDataProperties
            {
                Data = mapData
            };
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new MetaDataAttributeEditor();
            var someValue = new object();

            var mockRepository = new MockRepository();
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
            const string newValue = "Test 2";

            var feature = new MapFeature(Enumerable.Empty<MapGeometry>());
            feature.MetaData["Test"] = "test";
            feature.MetaData[newValue] = "test 2";

            var mapData = new MapPointData("Name")
            {
                Features = new[]
                {
                    feature
                }
            };

            var properties = new MapPointDataProperties
            {
                Data = mapData
            };
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new MetaDataAttributeEditor();

            var mockRepository = new MockRepository();
            var serviceProviderStub = mockRepository.Stub<IServiceProvider>();
            var serviceStub = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextStub = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderStub.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceStub);
            descriptorContextStub.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            var result = editor.EditValue(descriptorContextStub, serviceProviderStub, newValue);

            // Assert
            Assert.AreSame(newValue, result);
            mockRepository.VerifyAll();
        }
    }
}