using System.ComponentModel;

using Core.Common.BaseDelftTools;
using Core.Common.Utils.PropertyBag.Dynamic;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.TypeConverters;

namespace Ringtoets.Piping.Forms.Test.TypeConverters
{
    public class ShiftedShiftedLognormalDistributionTypeConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new ShiftedLognormalDistributionTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_DestinationTypeIsString_ReturnTrue()
        {
            // Setup
            var converter = new ShiftedLognormalDistributionTypeConverter();

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
            var converter = new ShiftedLognormalDistributionTypeConverter();

            // Call
            var result = converter.ConvertTo(distribution, typeof(string));

            // Assert
            var expectedText = string.Format("Verschoven lognormale verdeling (\u03BC = {0}, \u03C3 = {1}, Verschuiving = {2})",
                                             distribution.Mean, distribution.StandardDeviation, distribution.Shift);
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void GetPropertiesSupported_Always_ReturnTrue()
        {
            // Setup
            var converter = new ShiftedLognormalDistributionTypeConverter();

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
            var converter = new ShiftedLognormalDistributionTypeConverter();

            // Call
            var properties = converter.GetProperties(distribution);

            // Assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(3, properties.Count);
            var meanPropertyDescriptor = properties[0];
            Assert.AreEqual(distribution.GetType().BaseType, meanPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), meanPropertyDescriptor.PropertyType);
            Assert.IsFalse(meanPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("\u03BC", meanPropertyDescriptor.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de lognormale verdeling.", meanPropertyDescriptor.Description);

            var stdPropertyDescriptor = properties[1];
            Assert.AreEqual(distribution.GetType().BaseType, stdPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), stdPropertyDescriptor.PropertyType);
            Assert.IsFalse(stdPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("\u03C3", stdPropertyDescriptor.DisplayName);
            Assert.AreEqual("De standaardafwijking van de lognormale verdeling.", stdPropertyDescriptor.Description);

            var shiftPropertyDescriptor = properties[2];
            Assert.AreEqual(distribution.GetType(), shiftPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), shiftPropertyDescriptor.PropertyType);
            Assert.IsFalse(shiftPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Verschuiving", shiftPropertyDescriptor.DisplayName);
            Assert.AreEqual("De verschuiving van de lognormale verdeling.", shiftPropertyDescriptor.Description);
        }

        #region Integration tests

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenContextOfPipingCalculationInputsPropertiesWrappedInDynamicPropertyBag_WhenSettingNewValue_ThenPipingDataNotifiedObserversOfChange(int propertyIndexToChange)
        {
            // Scenario
            var pipingData = new PipingData();
            var calculationInputs = new PipingCalculationInputs
            {
                PipingData = pipingData
            };
            var calculationInputsProperties = new PipingCalculationInputsProperties
            {
                Data = calculationInputs
            };
            var dynamicPropertyBag = new DynamicPropertyBag(calculationInputsProperties);

            var mocks = new MockRepository();
            var typeDescriptorContextMock = mocks.StrictMock<ITypeDescriptorContext>();
            typeDescriptorContextMock.Expect(tdc => tdc.Instance).Return(dynamicPropertyBag);

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            pipingData.Attach(observer);

            ShiftedLognormalDistribution distribution = calculationInputsProperties.SandParticlesVolumicWeight;

            var properties = new ShiftedLognormalDistributionTypeConverter().GetProperties(typeDescriptorContextMock, distribution);

            // Precondition
            Assert.IsNotNull(properties);

            // Event
            properties[propertyIndexToChange].SetValue(distribution, 2.3);

            // Result
            mocks.VerifyAll();
        }

        #endregion 
    }
}