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
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Piping.Forms.TypeConverters;

namespace Ringtoets.Piping.Forms.Test.TypeConverters
{
    [TestFixture]
    public class LogNormalDistributionDesignVariableTypeConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new LogNormalDistributionDesignVariableTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_DestinationTypeIsString_ReturnTrue()
        {
            // Setup
            var converter = new LogNormalDistributionDesignVariableTypeConverter();

            // Call
            var canConvert = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void ConvertTo_DestinationTypeIsString_ReturnLogNormalDistributionSpecs()
        {
            // Setup
            var distribution = new LogNormalDistribution(1)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 2.2
            };
            var designVariable = new LogNormalDistributionDesignVariable(distribution);

            var converter = new LogNormalDistributionDesignVariableTypeConverter();

            // Call
            var result = converter.ConvertTo(designVariable, typeof(string));

            // Assert
            var expectedText = $"{designVariable.GetDesignValue()} (Verwachtingswaarde = {distribution.Mean}, Standaardafwijking = {distribution.StandardDeviation})";
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void GetPropertiesSupported_Always_ReturnTrue()
        {
            // Setup
            var converter = new LogNormalDistributionDesignVariableTypeConverter();

            // Call
            var hasSubProperties = converter.GetPropertiesSupported();

            // Assert
            Assert.IsTrue(hasSubProperties);
        }

        [Test]
        public void GetProperties_Always_ReturnMeanAndStandardDeviation()
        {
            // Setup
            var distribution = new LogNormalDistribution(2);
            var designVariable = new LogNormalDistributionDesignVariable(distribution);
            var converter = new LogNormalDistributionDesignVariableTypeConverter();

            // Call
            var properties = converter.GetProperties(designVariable);

            // Assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(4, properties.Count);

            var distributionTypePropertyDescriptor = properties[0];
            Assert.AreEqual(typeof(string), distributionTypePropertyDescriptor.PropertyType);
            Assert.IsTrue(distributionTypePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Type verdeling", distributionTypePropertyDescriptor.DisplayName);
            Assert.AreEqual("Het soort kansverdeling waarin deze parameter gedefinieerd wordt.", distributionTypePropertyDescriptor.Description);
            Assert.AreEqual("DistributionType", distributionTypePropertyDescriptor.Name);
            Assert.AreEqual("Lognormaal", distributionTypePropertyDescriptor.GetValue(new object()));

            var meanPropertyDescriptor = properties[1];
            Assert.AreEqual(distribution.GetType(), meanPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), meanPropertyDescriptor.PropertyType);
            Assert.IsFalse(meanPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Verwachtingswaarde", meanPropertyDescriptor.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de lognormale verdeling.", meanPropertyDescriptor.Description);

            var stdPropertyDescriptor = properties[2];
            Assert.AreEqual(distribution.GetType(), stdPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), stdPropertyDescriptor.PropertyType);
            Assert.IsFalse(stdPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Standaardafwijking", stdPropertyDescriptor.DisplayName);
            Assert.AreEqual("De standaardafwijking van de lognormale verdeling.", stdPropertyDescriptor.Description);

            var designValuePropertyDescriptor = properties[3];
            Assert.AreEqual(typeof(RoundedDouble), designValuePropertyDescriptor.PropertyType);
            Assert.IsTrue(designValuePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Rekenwaarde", designValuePropertyDescriptor.DisplayName);
            Assert.AreEqual("De representatieve waarde die gebruikt wordt door de berekening.", designValuePropertyDescriptor.Description);
            Assert.AreEqual("DesignValue", designValuePropertyDescriptor.Name);
            Assert.AreEqual(designVariable.GetDesignValue(), designValuePropertyDescriptor.GetValue(new object()));
        }

        [Test]
        public void GetProperties_TypeConverterPropertyDecoratedWithReadOnlyAttribute_ReturnMeanAndStandardDeviationAsReadOnly()
        {
            // Setup
            var distribution = new LogNormalDistribution(2);
            var designVariable = new LogNormalDistributionDesignVariable(distribution);
            var converter = new LogNormalDistributionDesignVariableTypeConverter();

            var classWithDecoratedProperty = new ClassWithReadOnlyDesignVariable();
            PropertyDescriptor propertyWithReadonlyAttributeDescriptor = TypeDescriptor.GetProperties(classWithDecoratedProperty)[0];

            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            context.Stub(c => c.PropertyDescriptor).Return(propertyWithReadonlyAttributeDescriptor);
            context.Stub(c => c.Instance).Return(designVariable);
            mocks.ReplayAll();

            // Call
            var properties = converter.GetProperties(context, designVariable);

            // Assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(4, properties.Count);

            var distributionTypePropertyDescriptor = properties[0];
            Assert.AreEqual(typeof(string), distributionTypePropertyDescriptor.PropertyType);
            Assert.IsTrue(distributionTypePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Type verdeling", distributionTypePropertyDescriptor.DisplayName);
            Assert.AreEqual("Het soort kansverdeling waarin deze parameter gedefinieerd wordt.", distributionTypePropertyDescriptor.Description);
            Assert.AreEqual("DistributionType", distributionTypePropertyDescriptor.Name);
            Assert.AreEqual("Lognormaal", distributionTypePropertyDescriptor.GetValue(new object()));

            var meanPropertyDescriptor = properties[1];
            Assert.AreEqual(distribution.GetType(), meanPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), meanPropertyDescriptor.PropertyType);
            Assert.IsTrue(meanPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Verwachtingswaarde", meanPropertyDescriptor.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de lognormale verdeling.", meanPropertyDescriptor.Description);

            var stdPropertyDescriptor = properties[2];
            Assert.AreEqual(distribution.GetType(), stdPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), stdPropertyDescriptor.PropertyType);
            Assert.IsTrue(stdPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Standaardafwijking", stdPropertyDescriptor.DisplayName);
            Assert.AreEqual("De standaardafwijking van de lognormale verdeling.", stdPropertyDescriptor.Description);

            var designValuePropertyDescriptor = properties[3];
            Assert.AreEqual(typeof(RoundedDouble), designValuePropertyDescriptor.PropertyType);
            Assert.IsTrue(designValuePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Rekenwaarde", designValuePropertyDescriptor.DisplayName);
            Assert.AreEqual("De representatieve waarde die gebruikt wordt door de berekening.", designValuePropertyDescriptor.Description);
            Assert.AreEqual("DesignValue", designValuePropertyDescriptor.Name);
            Assert.AreEqual(designVariable.GetDesignValue(), designValuePropertyDescriptor.GetValue(new object()));

            mocks.VerifyAll();
        }

        private class ClassWithReadOnlyDesignVariable
        {
            public ClassWithReadOnlyDesignVariable()
            {
                Property = new LogNormalDistributionDesignVariable(new LogNormalDistribution(3));
            }

            [ReadOnly(true)]
            public DesignVariable<LogNormalDistribution> Property { get; set; }
        }
    }
}