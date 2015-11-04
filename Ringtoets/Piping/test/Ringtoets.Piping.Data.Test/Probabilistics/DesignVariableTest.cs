using System;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class DesignVariableTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var designVariable = new DesignVariable();

            // Assert
            Assert.IsNull(designVariable.Distribution);
            Assert.AreEqual(0.5, designVariable.Percentile);
        }

        [Test]
        [TestCase(-1234.5678)]
        [TestCase(0 - 1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12345.789)]
        public void Percentile_SettingInvalidValue_ThrowArgumentOutOfRangeException(double invalidPercentile)
        {
            // Setup
            var designVariable = new DesignVariable();

            // Call
            TestDelegate call = () => designVariable.Percentile = invalidPercentile;

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            string customMessagePart = exception.Message.Split(new []{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];
            Assert.AreEqual("Percentiel moet in het bereik van [0, 1] vallen.", customMessagePart);
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(0.54638291)]
        [TestCase(1.0)]
        public void Percentile_SettingValidValue_PropertySet(double validPercentile)
        {
            // Setup
            var designVariable = new DesignVariable();

            // Call
            designVariable.Percentile = validPercentile;

            // Assert
            Assert.AreEqual(validPercentile, designVariable.Percentile);
        }

        [Test]
        public void GetDesignValue_DistributionNotSet_ThrowInvalidOperationException()
        {
            // Setup
            var designVariable = new DesignVariable
            {
                Distribution = null
            };

            // Call
            TestDelegate call = () => designVariable.GetDesignValue();

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            Assert.AreEqual("Een kansverdeling moet opgegeven zijn om op basis van die data een rekenwaarde te bepalen.", exception.Message);
        }

        [Test]
        public void GetDesignValue_DistributionSet_ReturnInverseCdfForGivenPercentile()
        {
            // Setup
            const double percentile = 0.5;
            const double expectedValue = 1.1;

            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            distribution.Expect(d => d.InverseCDF(percentile)).Return(expectedValue);
            mocks.ReplayAll();

            var designVariable = new DesignVariable
            {
                Distribution = distribution,
                Percentile = percentile
            };

            // Call
            var designValue = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(expectedValue, designValue);
            mocks.VerifyAll();
        }
    }
}