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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Core.Common.Gui.PropertyBag;
using Core.Common.Gui.UITypeEditors;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
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
            var editor = new StructureEditor<TestStructure>();

            // Assert
            Assert.IsInstanceOf<SelectionEditor<IHasStructureProperty<TestStructure>, TestStructure>>(editor);
        }

        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var simpleStructure = new TestStructure();
            var properties = new ObjectPropertiesWithStructure(simpleStructure, new TestStructure[0]);
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new StructureEditor<TestStructure>();
            var someValue = new object();
            var serviceProvider = mockRepository.Stub<IServiceProvider>();
            var service = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContext = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProvider.Stub(p => p.GetService(null)).IgnoreArguments().Return(service);
            descriptorContext.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(descriptorContext, serviceProvider, someValue);

            // Assert
            Assert.AreSame(someValue, result);
            mockRepository.VerifyAll();
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            var simpleStructure = new TestStructure();
            var properties = new ObjectPropertiesWithStructure(simpleStructure, new[]
            {
                simpleStructure
            });
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new StructureEditor<TestStructure>();
            var someValue = new object();
            var serviceProvider = mockRepository.Stub<IServiceProvider>();
            var service = mockRepository.Stub<IWindowsFormsEditorService>();
            var descriptorContext = mockRepository.Stub<ITypeDescriptorContext>();
            serviceProvider.Stub(p => p.GetService(null)).IgnoreArguments().Return(service);
            descriptorContext.Stub(c => c.Instance).Return(propertyBag);
            mockRepository.ReplayAll();

            // Call
            object result = editor.EditValue(descriptorContext, serviceProvider, someValue);

            // Assert
            Assert.AreSame(simpleStructure, result);
            mockRepository.VerifyAll();
        }

        private class ObjectPropertiesWithStructure : ObjectProperties<object>, IHasStructureProperty<TestStructure>
        {
            private readonly IEnumerable<TestStructure> availableStructures;

            public ObjectPropertiesWithStructure(TestStructure structure, IEnumerable<TestStructure> availableStructures)
            {
                Structure = structure;
                this.availableStructures = availableStructures;
            }

            public TestStructure Structure { get; }

            public IEnumerable<TestStructure> GetAvailableStructures()
            {
                return availableStructures;
            }
        }
    }
}