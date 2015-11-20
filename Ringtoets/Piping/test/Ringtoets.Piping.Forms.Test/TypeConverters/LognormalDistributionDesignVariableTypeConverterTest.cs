using System.ComponentModel;
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
    public class LognormalDistributionDesignVariableTypeConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new LognormalDistributionDesignVariableTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_DestinationTypeIsString_ReturnTrue()
        {
            // Setup
            var converter = new LognormalDistributionDesignVariableTypeConverter();

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
            var designVariable = new LognormalDistributionDesignVariable(distribution);

            var converter = new LognormalDistributionDesignVariableTypeConverter();

            // Call
            var result = converter.ConvertTo(designVariable, typeof(string));

            // Assert
            var expectedText = string.Format("{0} (\u03BC = {1}, \u03C3 = {2})",
                                             designVariable.GetDesignValue(), distribution.Mean, distribution.StandardDeviation);
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void GetPropertiesSupported_Always_ReturnTrue()
        {
            // Setup
            var converter = new LognormalDistributionDesignVariableTypeConverter();

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
            var designVariable = new LognormalDistributionDesignVariable(distribution);
            var converter = new LognormalDistributionDesignVariableTypeConverter();

            // Call
            var properties = converter.GetProperties(designVariable);

            // Assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(4, properties.Count);

            var distributionTypePropertyDescriptor = properties[0];
            Assert.AreEqual(typeof(string), distributionTypePropertyDescriptor.PropertyType);
            Assert.IsTrue(distributionTypePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Type verdeling", distributionTypePropertyDescriptor.DisplayName);
            Assert.AreEqual("De soort kansverdeling waarin deze parameter in gedefiniëerd wordt.", distributionTypePropertyDescriptor.Description);
            Assert.AreEqual("DistributionType", distributionTypePropertyDescriptor.Name);
            Assert.AreEqual("Lognormaal", distributionTypePropertyDescriptor.GetValue(new object()));

            var meanPropertyDescriptor = properties[1];
            Assert.AreEqual(distribution.GetType(), meanPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), meanPropertyDescriptor.PropertyType);
            Assert.IsFalse(meanPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("\u03BC", meanPropertyDescriptor.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de lognormale verdeling.", meanPropertyDescriptor.Description);

            var stdPropertyDescriptor = properties[2];
            Assert.AreEqual(distribution.GetType(), stdPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), stdPropertyDescriptor.PropertyType);
            Assert.IsFalse(stdPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("\u03C3", stdPropertyDescriptor.DisplayName);
            Assert.AreEqual("De standaardafwijking van de lognormale verdeling.", stdPropertyDescriptor.Description);

            var designValuePropertyDescriptor = properties[3];
            Assert.AreEqual(typeof(double), designValuePropertyDescriptor.PropertyType);
            Assert.IsTrue(designValuePropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Rekenwaarde", designValuePropertyDescriptor.DisplayName);
            Assert.AreEqual("De representatieve waarde die gebruikt wordt door de berekening.", designValuePropertyDescriptor.Description);
            Assert.AreEqual("DesignValue", designValuePropertyDescriptor.Name);
            Assert.AreEqual(designVariable.GetDesignValue(), designValuePropertyDescriptor.GetValue(new object()));
        }

        #region Integration tests

        [Test]
        [TestCase(1, TestName = "PipingCalculationInputsPropertiesWrappedInDynamicPropertyBag_WhenSettingNewValue_ThenPipingDataNotifiedObserversOfChange(1)")]
        [TestCase(2, TestName = "PipingCalculationInputsPropertiesWrappedInDynamicPropertyBag_WhenSettingNewValue_ThenPipingDataNotifiedObserversOfChange(2)")]
        public void GivenContextOfPipingCalculationInputsPropertiesWrappedInDynamicPropertyBag_WhenSettingNewValue_ThenPipingDataNotifiedObserversOfChange(int propertyIndexToChange)
        {
            // Scenario
            var pipingData = new PipingCalculationData();
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

            DesignVariable<LognormalDistribution> dampingFactorExit = calculationInputsProperties.DampingFactorExit;
            var properties = new LognormalDistributionDesignVariableTypeConverter().GetProperties(typeDescriptorContextMock, dampingFactorExit);

            // Precondition
            Assert.IsNotNull(properties);

            // Event
            const double newValue = 2.3;
            properties[propertyIndexToChange].SetValue(dampingFactorExit, newValue);

            // Result
            switch (propertyIndexToChange)
            {
                case 1:
                    Assert.AreEqual(newValue, pipingData.DampingFactorExit.Mean);
                    break;
                case 2:
                    Assert.AreEqual(newValue, pipingData.DampingFactorExit.StandardDeviation);
                    break;
            }
            mocks.VerifyAll();
        }

        #endregion
    }
}