// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class ExpandableReadOnlyArrayConverterTest
    {
        [Test]
        public void DefaultConstructor_Always_ExpectedValues()
        {
            // Call
            var converter = new ExpandableReadOnlyArrayConverter();

            // Assert
            Assert.IsInstanceOf<ExpandableArrayConverter>(converter);
            Assert.IsInstanceOf<ArrayConverter>(converter);
        }

        [Test]
        public void GetProperties_FromArray_ValuesAreReadOnly()
        {
            // Setup
            const int elementCount = 12;
            int[] array = Enumerable.Repeat(10, elementCount).ToArray();
            var converter = new ExpandableReadOnlyArrayConverter();

            // Call
            PropertyDescriptorCollection propertyDescriptors = converter.GetProperties(array);

            // Assert
            Assert.IsNotNull(propertyDescriptors);
            for (var i = 0; i < elementCount; i++)
            {
                Assert.IsTrue(propertyDescriptors[i].IsReadOnly);
            }
        }
    }
}