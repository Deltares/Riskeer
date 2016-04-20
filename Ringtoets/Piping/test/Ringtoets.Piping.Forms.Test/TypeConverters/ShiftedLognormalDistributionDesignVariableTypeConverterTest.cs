using System.ComponentModel;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Piping.Forms.TypeConverters;

namespace Ringtoets.Piping.Forms.Test.TypeConverters
{
    [TestFixture]
    public class ShiftedLognormalDistributionDesignVariableTypeConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new ShiftedLognormalDistributionDesignVariableTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_DestinationTypeIsString_ReturnTrue()
        {
            // Setup
            var converter = new ShiftedLognormalDistributionDesignVariableTypeConverter();

            // Call
            var canConvert = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void ConvertTo_DestinationTypeIsString_ReturnNormalDistributionSpecs()
        {
            // Setup
            var distribution = new ShiftedLognormalDistribution(5)
            {
                Mean = (RoundedDouble) 1.1,
                StandardDeviation = (RoundedDouble) 2.2,
                Shift = (RoundedDouble) 3.3
            };
            var designVariable = new ShiftedLognormalDistributionDesignVariable(distribution);
            var converter = new ShiftedLognormalDistributionDesignVariableTypeConverter();

            // Call
            var result = converter.ConvertTo(designVariable, typeof(string));

            // Assert
            var expectedText = string.Format("{0} (Verwachtingswaarde = {1}, Standaardafwijking = {2}, Verschuiving = {3})",
                                             designVariable.GetDesignValue(), distribution.Mean, distribution.StandardDeviation, distribution.Shift);
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void GetPropertiesSupported_Always_ReturnTrue()
        {
            // Setup
            var converter = new ShiftedLognormalDistributionDesignVariableTypeConverter();

            // Call
            var hasSubProperties = converter.GetPropertiesSupported();

            // Assert
            Assert.IsTrue(hasSubProperties);
        }

        [Test]
        public void GetProperties_Always_ReturnMeanAndStandardDeviation()
        {
            // Setup
            var distribution = new ShiftedLognormalDistribution(3);
            var designVariable = new ShiftedLognormalDistributionDesignVariable(distribution);
            var converter = new ShiftedLognormalDistributionDesignVariableTypeConverter();

            // Call
            var properties = converter.GetProperties(designVariable);

            // Assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(5, properties.Count);

            var distributionTypePropertyDescriptor = properties[0];
            Assert.AreEqual(typeof(string), distributionTypePropertyDescriptor.PropertyType);
            Assert.IsTrue(distributionTypePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Type verdeling", distributionTypePropertyDescriptor.DisplayName);
            Assert.AreEqual("Het soort kansverdeling waarin deze parameter gedefinieerd wordt.", distributionTypePropertyDescriptor.Description);
            Assert.AreEqual("DistributionType", distributionTypePropertyDescriptor.Name);
            Assert.AreEqual("Verschoven lognormaal", distributionTypePropertyDescriptor.GetValue(new object()));

            var meanPropertyDescriptor = properties[1];
            Assert.AreEqual(distribution.GetType().BaseType, meanPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), meanPropertyDescriptor.PropertyType);
            Assert.IsFalse(meanPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Verwachtingswaarde", meanPropertyDescriptor.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de verschoven lognormale verdeling.", meanPropertyDescriptor.Description);

            var stdPropertyDescriptor = properties[2];
            Assert.AreEqual(distribution.GetType().BaseType, stdPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), stdPropertyDescriptor.PropertyType);
            Assert.IsFalse(stdPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Standaardafwijking", stdPropertyDescriptor.DisplayName);
            Assert.AreEqual("De standaardafwijking van de verschoven lognormale verdeling.", stdPropertyDescriptor.Description);

            var shiftPropertyDescriptor = properties[3];
            Assert.AreEqual(distribution.GetType(), shiftPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), shiftPropertyDescriptor.PropertyType);
            Assert.IsFalse(shiftPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Verschuiving", shiftPropertyDescriptor.DisplayName);
            Assert.AreEqual("De hoeveelheid waarmee de kansverdeling naar rechts (richting van positieve X-as) verschoven is.", shiftPropertyDescriptor.Description);

            var designValuePropertyDescriptor = properties[4];
            Assert.AreEqual(typeof(RoundedDouble), designValuePropertyDescriptor.PropertyType);
            Assert.IsTrue(designValuePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Rekenwaarde", designValuePropertyDescriptor.DisplayName);
            Assert.AreEqual("De representatieve waarde die gebruikt wordt door de berekening.", designValuePropertyDescriptor.Description);
            Assert.AreEqual("DesignValue", designValuePropertyDescriptor.Name);
            Assert.AreEqual(designVariable.GetDesignValue(), designValuePropertyDescriptor.GetValue(new object()));
        }

        [Test]
        public void GetProperties_TypeConverterPropertyDecoratedWithReadOnlyAttribute_ReturnMeanAndStandardDeviationAndShiftAsReadOnly()
        {
            // Setup
            var distribution = new ShiftedLognormalDistribution(3);
            var designVariable = new ShiftedLognormalDistributionDesignVariable(distribution);
            var converter = new ShiftedLognormalDistributionDesignVariableTypeConverter();

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
            Assert.AreEqual(5, properties.Count);

            var distributionTypePropertyDescriptor = properties[0];
            Assert.AreEqual(typeof(string), distributionTypePropertyDescriptor.PropertyType);
            Assert.IsTrue(distributionTypePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Type verdeling", distributionTypePropertyDescriptor.DisplayName);
            Assert.AreEqual("Het soort kansverdeling waarin deze parameter gedefinieerd wordt.", distributionTypePropertyDescriptor.Description);
            Assert.AreEqual("DistributionType", distributionTypePropertyDescriptor.Name);
            Assert.AreEqual("Verschoven lognormaal", distributionTypePropertyDescriptor.GetValue(new object()));

            var meanPropertyDescriptor = properties[1];
            Assert.AreEqual(distribution.GetType().BaseType, meanPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), meanPropertyDescriptor.PropertyType);
            Assert.IsTrue(meanPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Verwachtingswaarde", meanPropertyDescriptor.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de verschoven lognormale verdeling.", meanPropertyDescriptor.Description);

            var stdPropertyDescriptor = properties[2];
            Assert.AreEqual(distribution.GetType().BaseType, stdPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), stdPropertyDescriptor.PropertyType);
            Assert.IsTrue(stdPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Standaardafwijking", stdPropertyDescriptor.DisplayName);
            Assert.AreEqual("De standaardafwijking van de verschoven lognormale verdeling.", stdPropertyDescriptor.Description);

            var shiftPropertyDescriptor = properties[3];
            Assert.AreEqual(distribution.GetType(), shiftPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(RoundedDouble), shiftPropertyDescriptor.PropertyType);
            Assert.IsTrue(shiftPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Verschuiving", shiftPropertyDescriptor.DisplayName);
            Assert.AreEqual("De hoeveelheid waarmee de kansverdeling naar rechts (richting van positieve X-as) verschoven is.", shiftPropertyDescriptor.Description);

            var designValuePropertyDescriptor = properties[4];
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
                Property = new ShiftedLognormalDistributionDesignVariable(new ShiftedLognormalDistribution(3));
            }

            [ReadOnly(true)]
            public DesignVariable<ShiftedLognormalDistribution> Property { get; set; }
        }
    }
}