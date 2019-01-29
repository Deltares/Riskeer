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

using System.ComponentModel;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class PropertiesTestHelperTest
    {
        [Test]
        public void GetAllVisiblePropertyDescriptors_NoProperties_ReturnEmpty()
        {
            // Setup
            var propertiesObject = new NoProperties();

            // Call
            PropertyDescriptorCollection visibleProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(propertiesObject);

            // Assert
            CollectionAssert.IsEmpty(visibleProperties);
        }

        [Test]
        public void GetAllVisiblePropertyDescriptors_HiddenProperties_ReturnEmpty()
        {
            // Setup
            var propertiesObject = new HiddenProperties();

            // Call
            PropertyDescriptorCollection visibleProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(propertiesObject);

            // Assert
            CollectionAssert.IsEmpty(visibleProperties);
        }

        [Test]
        public void GetAllVisiblePropertyDescriptors_VisibleProperties_ReturnCollectionWithTwoProperties()
        {
            // Setup
            var propertiesObject = new VisibleProperties();

            // Call
            PropertyDescriptorCollection visibleProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(propertiesObject);

            // Assert
            Assert.AreEqual(2, visibleProperties.Count);
        }

        [Test]
        [TestCase(true, "<category>", "<display name>", "<description>")]
        [TestCase(false, "A", "B", "C")]
        public void AssertRequiredPropertyDescriptorProperties_PropertyDescriptor_AssertParameters(
            bool isReadOnly, string category, string displayName, string description)
        {
            // Setup
            var mocks = new MockRepository();
            var propertyDescriptor = mocks.Stub<PropertyDescriptor>("stub", null);
            propertyDescriptor.Stub(pd => pd.IsReadOnly).Return(isReadOnly);
            propertyDescriptor.Stub(pd => pd.Category).Return(category);
            propertyDescriptor.Stub(pd => pd.DisplayName).Return(displayName);
            propertyDescriptor.Stub(pd => pd.Description).Return(description);
            mocks.ReplayAll();

            // Call
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(propertyDescriptor,
                                                                            category,
                                                                            displayName,
                                                                            description,
                                                                            isReadOnly);

            // Assert
            mocks.VerifyAll();
        }

        private class NoProperties : ObjectProperties<object> {}

        private class HiddenProperties : ObjectProperties<object>
        {
            [Browsable(false)]
            public string HiddenProperty1 { get; set; }

            [Browsable(false)]
            public string HiddenProperty2 { get; set; }
        }

        private class VisibleProperties : ObjectProperties<object>
        {
            public string Property1 { get; set; }

            public string Property2 { get; set; }
        }
    }
}