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
using Core.Gui.PropertyClasses.Map;
using Core.Gui.UITypeEditors;
using NSubstitute;
using NUnit.Framework;

namespace Core.Gui.Test.UITypeEditors
{
    [TestFixture]
    public class MetaDataAttributeEditorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var editor = new MetaDataAttributeEditor();

            // Assert
            Assert.IsInstanceOf<SelectionEditor<IHasMetaData, SelectableMetaDataAttribute>>(editor);
        }

        [Test]
        public void EditValue_WithCurrentItemNotInAvailableItems_ReturnsOriginalValue()
        {
            // Setup
            var properties = new ObjectPropertiesWithSelectableMetaDataAttribute(CreateSelectableMetaDataAttribute(), new SelectableMetaDataAttribute[0]);
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new MetaDataAttributeEditor();
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
            serviceProvider.Received().GetService(Arg.Any<Type>());
        }

        [Test]
        public void EditValue_WithCurrentItemInAvailableItems_ReturnsCurrentItem()
        {
            // Setup
            SelectableMetaDataAttribute selectableMetaDataAttribute = CreateSelectableMetaDataAttribute();
            var properties = new ObjectPropertiesWithSelectableMetaDataAttribute(selectableMetaDataAttribute, new[]
            {
                selectableMetaDataAttribute
            });
            var propertyBag = new DynamicPropertyBag(properties);
            var editor = new MetaDataAttributeEditor();
            var someValue = new object();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var service = Substitute.For<IWindowsFormsEditorService>();
            var descriptorContext = Substitute.For<ITypeDescriptorContext>();
            serviceProvider.GetService(Arg.Any<Type>()).Returns(service);
            descriptorContext.Instance.Returns(propertyBag);

            // Call
            object result = editor.EditValue(descriptorContext, serviceProvider, someValue);

            // Assert
            Assert.AreSame(selectableMetaDataAttribute, result);
            serviceProvider.Received().GetService(Arg.Any<Type>());
        }

        private static SelectableMetaDataAttribute CreateSelectableMetaDataAttribute()
        {
            return new SelectableMetaDataAttribute(string.Empty);
        }

        private class ObjectPropertiesWithSelectableMetaDataAttribute : ObjectProperties<object>, IHasMetaData
        {
            private readonly IEnumerable<SelectableMetaDataAttribute> selectableMetaDataAttributes;

            public ObjectPropertiesWithSelectableMetaDataAttribute(SelectableMetaDataAttribute selectableMetaDataAttribute,
                                                                   IEnumerable<SelectableMetaDataAttribute> selectableMetaDataAttributes)
            {
                SelectedMetaDataAttribute = selectableMetaDataAttribute;
                this.selectableMetaDataAttributes = selectableMetaDataAttributes;
            }

            public SelectableMetaDataAttribute SelectedMetaDataAttribute { get; }

            public IEnumerable<SelectableMetaDataAttribute> GetAvailableMetaDataAttributes()
            {
                return selectableMetaDataAttributes;
            }
        }
    }
}