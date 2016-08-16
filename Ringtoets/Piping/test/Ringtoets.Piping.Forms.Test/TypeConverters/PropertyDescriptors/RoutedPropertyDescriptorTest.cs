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

using System.ComponentModel;

using NUnit.Framework;

using Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors;

namespace Ringtoets.Piping.Forms.Test.TypeConverters.PropertyDescriptors
{
    [TestFixture]
    public class RoutedPropertyDescriptorTest
    {
        [Test]
        public void ParameteredConstructpr_ExpecedValues()
        {
            // Setup
            var a = new TestA();
            var propertyDescription = TypeDescriptor.GetProperties(a.Child)[0];

            // Call
            var routedPropertyDescriptor = new RoutedPropertyDescriptor(propertyDescription, o => ((TestA)o).Child);

            // Assert
            Assert.AreEqual(propertyDescription.ComponentType, routedPropertyDescriptor.ComponentType);
            Assert.AreEqual(propertyDescription.IsReadOnly, routedPropertyDescriptor.IsReadOnly);
            Assert.AreEqual(propertyDescription.PropertyType, routedPropertyDescriptor.PropertyType);

            Assert.AreEqual(propertyDescription.Name, routedPropertyDescriptor.Name);
            Assert.AreEqual(propertyDescription.Attributes, routedPropertyDescriptor.Attributes);
            Assert.AreEqual(propertyDescription.Category, routedPropertyDescriptor.Category);
            Assert.AreEqual(propertyDescription.Converter, routedPropertyDescriptor.Converter);
            Assert.AreEqual(propertyDescription.Description, routedPropertyDescriptor.Description);
            Assert.AreEqual(propertyDescription.DesignTimeOnly, routedPropertyDescriptor.DesignTimeOnly);
            Assert.AreEqual(propertyDescription.DisplayName, routedPropertyDescriptor.DisplayName);
            Assert.AreEqual(propertyDescription.IsBrowsable, routedPropertyDescriptor.IsBrowsable);
            Assert.AreEqual(propertyDescription.IsLocalizable, routedPropertyDescriptor.IsLocalizable);
            Assert.AreEqual(propertyDescription.SerializationVisibility, routedPropertyDescriptor.SerializationVisibility);
            Assert.AreEqual(propertyDescription.SupportsChangeEvents, routedPropertyDescriptor.SupportsChangeEvents);
        }

        [Test]
        public void CanResetValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
        {
            // Setup
            var a = new TestA();
            var propertyDescription = TypeDescriptor.GetProperties(a.Child)[0];

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(propertyDescription, o => ((TestA)o).Child);

            // Call
            var result = routedPropertyDescriptor.CanResetValue(a);

            // Assert
            Assert.AreEqual(propertyDescription.CanResetValue(a.Child), result);
        }

        [Test]
        public void GetValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
        {
            // Setup
            var a = new TestA();
            var propertyDescription = TypeDescriptor.GetProperties(a.Child)[0];

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(propertyDescription, o => ((TestA)o).Child);

            // Call
            var result = routedPropertyDescriptor.GetValue(a);

            // Assert
            Assert.AreEqual(propertyDescription.GetValue(a.Child), result);
        }

        [Test]
        public void ResetValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            var a = new TestA
            {
                Child =
                {
                    Value = originalPropertyValue
                }
            };

            PropertyDescriptor getSetProperty = TypeDescriptor.GetProperties(a.Child)[0];

            getSetProperty.ResetValue(a.Child);
            var expectedPropertyValueAfterReset = a.Child.Value;

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(getSetProperty, o => ((TestA)o).Child);
            a.Child.Value = originalPropertyValue;

            // Call
            routedPropertyDescriptor.ResetValue(a);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterReset, a.Child.Value);
        }

        [Test]
        public void SetValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
        {
            // Setup
            const double originalPropertyValue = 1.1;
            const double newValue = 2.2;
            var a = new TestA
            {
                Child =
                {
                    Value = originalPropertyValue
                }
            };

            PropertyDescriptor getSetProperty = TypeDescriptor.GetProperties(a.Child)[0];

            getSetProperty.SetValue(a.Child, newValue);
            var expectedPropertyValueAfterReset = a.Child.Value;

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(getSetProperty, o => ((TestA)o).Child);
            a.Child.Value = originalPropertyValue;

            // Call
            routedPropertyDescriptor.SetValue(a, newValue);

            // Assert
            Assert.AreEqual(expectedPropertyValueAfterReset, a.Child.Value);
        }

        [Test]
        public void ShouldSerializeValue_Always_DeferToWrappedPropertyDescriptorButReroutingObjectArgument()
        {
            // Setup
            var a = new TestA();
            var propertyDescription = TypeDescriptor.GetProperties(a.Child)[0];

            var routedPropertyDescriptor = new RoutedPropertyDescriptor(propertyDescription, o => ((TestA)o).Child);

            // Call
            var result = routedPropertyDescriptor.ShouldSerializeValue(a);

            // Assert
            Assert.AreEqual(propertyDescription.ShouldSerializeValue(a.Child), result);
        }

        private class TestA
        {
            public TestA()
            {
                Child = new TestB();
            }

            public TestB Child { get; set; }
        }

        private class TestB
        {
            public double Value { get; set; }
        }
    }
}