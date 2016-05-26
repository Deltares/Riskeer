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
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;

using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ReadOnlyLognormalDistributionPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ReadOnlyLognormalDistributionProperties();

            // Assert
            Assert.IsInstanceOf<LognormalDistributionProperties>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual("Lognormaal", properties.DistributionType);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Call
            var properties = new ReadOnlyLognormalDistributionProperties
            {
                Data = new LognormalDistribution(1)
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(4, dynamicProperties.Count);

            string meanPropertyName = TypeUtils.GetMemberName<LognormalDistribution>(d => d.Mean);
            var meanAttributes = Attribute.GetCustomAttributes(properties.GetType().GetProperty(meanPropertyName));
            Assert.IsNotNull(meanAttributes);
            AssertAttributesOfType<ReadOnlyAttribute, bool>(meanAttributes, true, attribute => attribute.IsReadOnly);
            AssertAttributesOfType<ResourcesDisplayNameAttribute, string>(meanAttributes, "Verwachtingswaarde",
                                                                          attribute => attribute.DisplayName);
            AssertAttributesOfType<ResourcesDescriptionAttribute, string>(meanAttributes,
                                                                          "De gemiddelde waarde van de lognormale verdeling.",
                                                                          attribute => attribute.Description);

            string standardDeviationPropertyName = TypeUtils.GetMemberName<LognormalDistribution>(d => d.StandardDeviation);
            var standardAttributes = Attribute.GetCustomAttributes(properties.GetType().GetProperty(standardDeviationPropertyName));
            Assert.IsNotNull(standardAttributes);
            AssertAttributesOfType<ReadOnlyAttribute, bool>(standardAttributes, true, attribute => attribute.IsReadOnly);
            AssertAttributesOfType<ResourcesDisplayNameAttribute, string>(standardAttributes, "Standaardafwijking",
                                                                          attribute => attribute.DisplayName);
            AssertAttributesOfType<ResourcesDescriptionAttribute, string>(standardAttributes,
                                                                          "De standaardafwijking van de lognormale verdeling.",
                                                                          attribute => attribute.Description);
        }

        private static void AssertAttributesOfType<T, TR>(IEnumerable<Attribute> attributes, TR expectedValue,
                                                          Func<T, TR> action)
        {
            var meanDisplayNameAttribute = attributes.OfType<T>();
            Assert.IsNotNull(meanDisplayNameAttribute);
            var e = meanDisplayNameAttribute.FirstOrDefault();
            Assert.IsNotNull(e);
            Assert.AreEqual(expectedValue, action(e));
        }
    }
}