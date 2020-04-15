using System;
using Components.Persistence.Stability.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.MacroStabilityInwards.IO.Factories;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableStochasticParameterFactoryTest
    {
        [Test]
        public void Create_DistributionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableStochasticParameterFactory.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("distribution", exception.ParamName);
        }

        [Test]
        public void Create_WithData_ReturnsPersistableStochasticParameter()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            var random = new Random(21);
            distribution.Mean = random.NextRoundedDouble();
            distribution.CoefficientOfVariation = random.NextRoundedDouble();

            // Call
            PersistableStochasticParameter stochasticParameter = PersistableStochasticParameterFactory.Create(distribution);

            // Assert
            Assert.AreEqual(distribution.Mean.Value, stochasticParameter.Mean);
            Assert.AreEqual(distribution.Mean * distribution.CoefficientOfVariation, stochasticParameter.StandardDeviation);
            Assert.IsTrue(stochasticParameter.IsProbabilistic);
            mocks.VerifyAll();
        }
    }
}