using System.ComponentModel;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;
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
        public void ConvertTo_DestinationTypeIsString_ReturnShiftedLognormalDistributionSpecs()
        {
            // Setup
            var distribution = new ShiftedLognormalDistribution
            {
                Mean = 1.1,
                StandardDeviation = 2.2,
                Shift = 3.3
            };
            var designVariable = new DesignVariable { Distribution = distribution };
            var converter = new ShiftedLognormalDistributionDesignVariableTypeConverter();

            // Call
            var result = converter.ConvertTo(designVariable, typeof(string));

            // Assert
            var expectedText = string.Format("{0} (\u03BC = {1}, \u03C3 = {2}, Verschuiving = {3})",
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
            var distribution = new ShiftedLognormalDistribution();
            var designVariable = new DesignVariable { Distribution = distribution };
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
            Assert.AreEqual("De soort kansverdeling waarin deze parameter in gedefiniëerd wordt.", distributionTypePropertyDescriptor.Description);
            Assert.AreEqual("DistributionType", distributionTypePropertyDescriptor.Name);
            Assert.AreEqual("Verschoven lognormale verdeling", distributionTypePropertyDescriptor.GetValue(new object()));

            var meanPropertyDescriptor = properties[1];
            Assert.AreEqual(distribution.GetType().BaseType, meanPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), meanPropertyDescriptor.PropertyType);
            Assert.IsFalse(meanPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("\u03BC", meanPropertyDescriptor.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de verschoven lognormale verdeling.", meanPropertyDescriptor.Description);

            var stdPropertyDescriptor = properties[2];
            Assert.AreEqual(distribution.GetType().BaseType, stdPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), stdPropertyDescriptor.PropertyType);
            Assert.IsFalse(stdPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("\u03C3", stdPropertyDescriptor.DisplayName);
            Assert.AreEqual("De standaardafwijking van de verschoven lognormale verdeling.", stdPropertyDescriptor.Description);

            var shiftPropertyDescriptor = properties[3];
            Assert.AreEqual(distribution.GetType(), shiftPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), shiftPropertyDescriptor.PropertyType);
            Assert.IsFalse(shiftPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Verschuiving", shiftPropertyDescriptor.DisplayName);
            Assert.AreEqual("De verschuiving van de lognormale verdeling.", shiftPropertyDescriptor.Description);

            var designValuePropertyDescriptor = properties[4];
            Assert.AreEqual(typeof(double), designValuePropertyDescriptor.PropertyType);
            Assert.IsTrue(designValuePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Rekenwaarde", designValuePropertyDescriptor.DisplayName);
            Assert.AreEqual("De representatieve waarde die gebruikt wordt door de berekening.", designValuePropertyDescriptor.Description);
            Assert.AreEqual("DesignValue", designValuePropertyDescriptor.Name);
            Assert.AreEqual(designVariable.GetDesignValue(), designValuePropertyDescriptor.GetValue(new object()));
        }

        // TODO: Add integration test if parameter 'Saturated volumetric weight of the cover layer' can be found somewhere in the application. Probably should be a soil layer property (SoilLayer.BelowPhreaticLevel).
    }
}