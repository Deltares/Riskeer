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
using System.Windows.Forms.Design;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.Gui.UITypeEditors;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.UITypeEditors;

namespace Ringtoets.Common.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class StructureEditorTest
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
            var editor = new StructureEditor<SimpleStructure>();

            // Assert
            Assert.IsInstanceOf<SelectionEditor<IHasStructureProperty<SimpleStructure>, SimpleStructure>>(editor);
        }

        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var simpleStructure = new SimpleStructure();
            var properties = new ObjectPropertiesWithStructure(simpleStructure, new SimpleStructure[0]);
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new StructureEditor<SimpleStructure>();
            var someValue = new object();
            var serviceProviderStub = mockRepository.Stub<IServiceProvider>();
            var serviceStub = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextStub = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderStub.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceStub);
            descriptorContextStub.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(descriptorContextStub, serviceProviderStub, someValue);

            // Assert
            Assert.AreSame(someValue, result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var simpleStructure = new SimpleStructure();
            var properties = new ObjectPropertiesWithStructure(simpleStructure, new[]
            {
                simpleStructure
            });
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new StructureEditor<SimpleStructure>();
            var someValue = new object();
            var serviceProviderStub = mockRepository.Stub<IServiceProvider>();
            var serviceStub = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContextStub = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProviderStub.Stub(p => p.GetService(null)).IgnoreArguments().Return(serviceStub);
            descriptorContextStub.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(descriptorContextStub, serviceProviderStub, someValue);

            // Assert
            Assert.AreSame(simpleStructure, result);
            mockRepository.VerifyAll();
        }

        private class SimpleStructure : StructureBase
        {
            public SimpleStructure() : base(new ConstructionProperties
            {
                Name = "Name",
                Id = "Id",
                Location = new Point2D(0, 0),
                StructureNormalOrientation = (RoundedDouble) 0
            }) {}
        }

        private class ObjectPropertiesWithStructure : IHasStructureProperty<SimpleStructure>
        {
            private readonly IEnumerable<SimpleStructure> availableStructures;

            public ObjectPropertiesWithStructure(SimpleStructure structure, IEnumerable<SimpleStructure> availableStructures)
            {
                Structure = structure;
                this.availableStructures = availableStructures;
            }

            public object Data { get; set; }

            public SimpleStructure Structure { get; }

            public IEnumerable<SimpleStructure> GetAvailableStructures()
            {
                return availableStructures;
            }
        }
    }
}