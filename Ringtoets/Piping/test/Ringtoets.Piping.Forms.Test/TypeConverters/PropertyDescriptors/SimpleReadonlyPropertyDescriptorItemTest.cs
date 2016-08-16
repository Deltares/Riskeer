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
    public class SimpleReadonlyPropertyDescriptorItemTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            const string displayName = "A";
            const string description = "B";
            const string id = "C";
            const double value = 1.2345;

            // Call
            var propertyItem = new SimpleReadonlyPropertyDescriptorItem(displayName, description, id, value);

            // Assert
            Assert.IsInstanceOf<PropertyDescriptor>(propertyItem);
            Assert.IsTrue(propertyItem.IsReadOnly);
            Assert.IsTrue(propertyItem.IsBrowsable);
            Assert.AreEqual(id, propertyItem.Name);
            Assert.AreEqual(displayName, propertyItem.DisplayName);
            Assert.AreEqual(description, propertyItem.Description);
            Assert.AreEqual(value.GetType(), propertyItem.PropertyType);
        }

        [Test]
        public void CanResetValue_Always_ReturnFalse()
        {
            // Setup
            var propertyItem = new SimpleReadonlyPropertyDescriptorItem("A", "B", "C", "D");

            // Call
            var resetAllowed = propertyItem.CanResetValue(new object());

            // Assert
            Assert.IsFalse(resetAllowed);
        }

        [Test]
        public void GetValue_Always_ReturnValueArgument()
        {
            // Setup
            var valueArgument = new object();
            var propertyItem = new SimpleReadonlyPropertyDescriptorItem("A", "B", "C", valueArgument);

            // Call
            var value = propertyItem.GetValue(new object());

            // Assert
            Assert.AreSame(valueArgument, value);
        }

        [Test]
        public void ShouldSerializeValue_Always_ReturnFalse()
        {
            // Setup
            var propertyItem = new SimpleReadonlyPropertyDescriptorItem("A", "B", "C", "D");

            // Call
            var serializationRequired = propertyItem.ShouldSerializeValue(new object());

            // Assert
            Assert.IsFalse(serializationRequired);
        }
    }
}