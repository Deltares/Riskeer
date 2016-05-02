using System.ComponentModel;
using System.Linq;

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;

using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.Primitives;

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
            var distribution = new LognormalDistribution(1)
            {
                Mean = (RoundedDouble)1.1,
                StandardDeviation = (RoundedDouble)2.2
            };
            var designVariable = new LognormalDistributionDesignVariable(distribution);

            var converter = new LognormalDistributionDesignVariableTypeConverter();

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
            var distribution = new LognormalDistribution(2);
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
            var distribution = new LognormalDistribution(2);
            var designVariable = new LognormalDistributionDesignVariable(distribution);
            var converter = new LognormalDistributionDesignVariableTypeConverter();

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

        #region Integration tests

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenPipingInputParameterContextPropertiesInDynamicPropertyBag_WhenSettingNewValue_ThenPipingInputUpdatesObservers(int propertyIndexToChange)
        {
            // Scenario
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var typeDescriptorContextMock = mocks.StrictMock<ITypeDescriptorContext>();

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculationItem = new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput());
            var failureMechanism = new PipingFailureMechanism();

            var inputParameters = new PipingInput(new GeneralPipingInput());
            var inputParametersContext = new PipingInputContext(inputParameters,
                                                                calculationItem,
                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                failureMechanism,
                                                                assessmentSectionMock);

            var inputParameterContextProperties = new PipingInputContextProperties
            {
                Data = inputParametersContext
            };
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(inputParameterContextProperties).Find("DampingFactorExit", false);
            var dynamicPropertyBag = new DynamicPropertyBag(inputParameterContextProperties);

            typeDescriptorContextMock.Expect(tdc => tdc.Instance).Return(dynamicPropertyBag).Repeat.Twice();
            typeDescriptorContextMock.Stub(tdc => tdc.PropertyDescriptor).Return(propertyDescriptor);
            mocks.ReplayAll();

            inputParameters.Attach(observer);

            DesignVariable<LognormalDistribution> dampingFactorExit = inputParameterContextProperties.DampingFactorExit;
            var properties = new LognormalDistributionDesignVariableTypeConverter().GetProperties(typeDescriptorContextMock, dampingFactorExit);

            // Precondition
            Assert.IsNotNull(properties);

            // Event
            const double newDoubleValue = 2.3;
            properties[propertyIndexToChange].SetValue(dampingFactorExit, (RoundedDouble)newDoubleValue);
            
            // Result
            switch (propertyIndexToChange)
            {
                case 1:
                    Assert.AreEqual(newDoubleValue, inputParameters.DampingFactorExit.Mean.Value);
                    break;
                case 2:
                    Assert.AreEqual(newDoubleValue, inputParameters.DampingFactorExit.StandardDeviation.Value);
                    break;
            }
            mocks.VerifyAll();
        }

        #endregion

        private class ClassWithReadOnlyDesignVariable
        {
            public ClassWithReadOnlyDesignVariable()
            {
                Property = new LognormalDistributionDesignVariable(new LognormalDistribution(3));
            }

            [ReadOnly(true)]
            public DesignVariable<LognormalDistribution> Property { get; set; }
        }
    }
}