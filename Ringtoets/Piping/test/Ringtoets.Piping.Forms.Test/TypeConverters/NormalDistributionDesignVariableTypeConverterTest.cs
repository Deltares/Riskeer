using System.ComponentModel;
using System.Linq;

using Core.Common.Base;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;

using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.TypeConverters;

namespace Ringtoets.Piping.Forms.Test.TypeConverters
{
    [TestFixture]
    public class NormalDistributionDesignVariableTypeConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new NormalDistributionDesignVariableTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_DestinationTypeIsString_ReturnTrue()
        {
            // Setup
            var converter = new NormalDistributionDesignVariableTypeConverter();

            // Call
            var canConvert = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void ConvertTo_DestinationTypeIsString_ReturnNormalDistributionSpecs()
        {
            // Setup
            var distribution = new NormalDistribution
            {
                Mean = 1.1,
                StandardDeviation = 2.2
            };
            var designVariable = new NormalDistributionDesignVariable(distribution);
            var converter = new NormalDistributionDesignVariableTypeConverter();

            // Call
            var result = converter.ConvertTo(designVariable, typeof(string));

            // Assert
            var expectedText = string.Format("{0} (Verwachtingswaarde = {1}, Standaardafwijking = {2})",
                                             designVariable.GetDesignValue(), distribution.Mean, distribution.StandardDeviation);
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void GetPropertiesSupported_Always_ReturnTrue()
        {
            // Setup
            var converter = new NormalDistributionDesignVariableTypeConverter();

            // Call
            var hasSubProperties = converter.GetPropertiesSupported();

            // Assert
            Assert.IsTrue(hasSubProperties);
        }

        [Test]
        public void GetProperties_Always_ReturnMeanAndStandardDeviation()
        {
            // Setup
            var distribution = new NormalDistribution();
            var designVariable = new NormalDistributionDesignVariable(distribution);
            var converter = new NormalDistributionDesignVariableTypeConverter();

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
            Assert.AreEqual("Normaal", distributionTypePropertyDescriptor.GetValue(new object()));

            var meanPropertyDescriptor = properties[1];
            Assert.AreEqual(distribution.GetType(), meanPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), meanPropertyDescriptor.PropertyType);
            Assert.IsFalse(meanPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Verwachtingswaarde", meanPropertyDescriptor.DisplayName);
            Assert.AreEqual("De gemiddelde waarde van de normale verdeling.", meanPropertyDescriptor.Description);

            var stdPropertyDescriptor = properties[2];
            Assert.AreEqual(distribution.GetType(), stdPropertyDescriptor.ComponentType);
            Assert.AreEqual(typeof(double), stdPropertyDescriptor.PropertyType);
            Assert.IsFalse(stdPropertyDescriptor.IsReadOnly);
            Assert.AreEqual("Standaardafwijking", stdPropertyDescriptor.DisplayName);
            Assert.AreEqual("De standaardafwijking van de normale verdeling.", stdPropertyDescriptor.Description);

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
        [TestCase(1)]
        [TestCase(2)]
        public void GivenPipingInputParameterContextPropertiesInDynamicPropertyBag_WhenSettingNewValue_ThenPipingInputUpdatesObservers(int propertyIndexToChange)
        {
            // Scenario
            var mocks = new MockRepository();
            var typeDescriptorContextMock = mocks.StrictMock<ITypeDescriptorContext>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var inputParameters = new PipingInput();
            var inputParametersContext = new PipingInputContext(inputParameters,
                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                Enumerable.Empty<PipingSoilProfile>(),
                                                                assessmentSectionMock);
            var inputParameterContextProperties = new PipingInputContextProperties
            {
                Data = inputParametersContext
            };
            var dynamicPropertyBag = new DynamicPropertyBag(inputParameterContextProperties);
            typeDescriptorContextMock.Expect(tdc => tdc.Instance).Return(dynamicPropertyBag);
            mocks.ReplayAll();

            inputParameters.Attach(observer);

            DesignVariable<NormalDistribution> phreaticLevelExitHeave = inputParameterContextProperties.PhreaticLevelExitHeave;
            PropertyDescriptorCollection properties = new NormalDistributionDesignVariableTypeConverter().GetProperties(typeDescriptorContextMock, phreaticLevelExitHeave);

            // Precondition
            Assert.IsNotNull(properties);

            // Event
            const double newValue = 2.3;
            properties[propertyIndexToChange].SetValue(phreaticLevelExitHeave, newValue);

            // Result
            switch (propertyIndexToChange)
            {
                case 1:
                    Assert.AreEqual(newValue, inputParameters.PhreaticLevelExit.Mean);
                    break;
                case 2:
                    Assert.AreEqual(newValue, inputParameters.PhreaticLevelExit.StandardDeviation);
                    break;
            }
            mocks.VerifyAll();
        }

        #endregion
    }
}