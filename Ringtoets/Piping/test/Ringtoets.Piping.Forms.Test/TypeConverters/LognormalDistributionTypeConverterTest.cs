using System.ComponentModel;
using System.Linq.Expressions;
using Core.Common.Base;
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
    [TestFixture]
    public class LognormalDistributionTypeConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new LognormalDistributionTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_DestinationTypeIsString_ReturnTrue()
        {
            // Setup
            var converter = new LognormalDistributionTypeConverter();

            // Call
            var canConvert = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void ConvertTo_DestinationTypeIsString_ReturnLognormalDistributionSpecs()
        {
            // Setup
            var distribution = new LognormalDistribution
            {
                Mean = 1.1,
                StandardDeviation = 2.2
            };
            var converter = new LognormalDistributionTypeConverter();

            // Call
            var result = converter.ConvertTo(distribution, typeof(string));

            // Assert
            var expectedText = string.Format("Lognormale verdeling (\u03BC = {0}, \u03C3 = {1})",
                                             distribution.Mean, distribution.StandardDeviation);
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void GetPropertiesSupported_Always_ReturnTrue()
        {
            // Setup
            var converter = new LognormalDistributionTypeConverter();

            // Call
            var hasSubProperties = converter.GetPropertiesSupported();

            // Assert
            Assert.IsTrue(hasSubProperties);
        }

        [Test]
        public void GetProperties_Always_ReturnMeanAndStandardDeviation()
        {
            // Setup
            var distribution = new LognormalDistribution();
            var converter = new LognormalDistributionTypeConverter();

            // Call
            var properties = converter.GetProperties(distribution);

            // Assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(2, properties.Count);
            var meanPropertyDescriptor = properties[0];
            Assert.AreEqual(distribution.GetType(), meanPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), meanPropertyDescriptor.PropertyType);
            Assert.IsFalse(meanPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("\u03BC", meanPropertyDescriptor.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de lognormale verdeling.", meanPropertyDescriptor.Description);

            var stdPropertyDescriptor = properties[1];
            Assert.AreEqual(distribution.GetType(), stdPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), stdPropertyDescriptor.PropertyType);
            Assert.IsFalse(stdPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("\u03C3", stdPropertyDescriptor.DisplayName);
            Assert.AreEqual("De standaardafwijking van de lognormale verdeling.", stdPropertyDescriptor.Description);
        }

        #region Integration tests

        [Test]
        [TestCase(0)]
        [TestCase(1)]
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

            LognormalDistribution distribution = calculationInputsProperties.DampingFactorExit;

            var properties = new LognormalDistributionTypeConverter().GetProperties(typeDescriptorContextMock, distribution);

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