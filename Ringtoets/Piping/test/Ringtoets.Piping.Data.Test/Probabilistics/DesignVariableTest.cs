﻿using System;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class DesignVariableTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distributionMock = mocks.StrictMock<IDistribution>();
            mocks.ReplayAll();

            // Call
            var designVariable = new SimpleDesignVariable(distributionMock);

            // Assert
            Assert.AreSame(distributionMock, designVariable.Distribution);
            Assert.AreEqual(0.5, designVariable.Percentile);
            mocks.VerifyAll(); // Expect no cals on mocks
        }

        [Test]
        public void ParameteredConstructor_DistributionIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleDesignVariable(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessagePart = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
            Assert.AreEqual("Een kansverdeling moet opgegeven zijn om op basis van die data een rekenwaarde te bepalen.", customMessagePart);
        }

        [Test]
        [TestCase(-1234.5678)]
        [TestCase(0 - 1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12345.789)]
        public void Percentile_SettingInvalidValue_ThrowArgumentOutOfRangeException(double invalidPercentile)
        {
            // Setup
            var mocks = new MockRepository();
            var distributionMock = mocks.StrictMock<IDistribution>();
            mocks.ReplayAll();

            var designVariable = new SimpleDesignVariable(distributionMock);

            // Call
            TestDelegate call = () => designVariable.Percentile = invalidPercentile;

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            string customMessagePart = exception.Message.Split(new []{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];
            Assert.AreEqual("Percentiel moet in het bereik van [0, 1] vallen.", customMessagePart);
            mocks.VerifyAll(); // Expect no cals on mocks
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(0.54638291)]
        [TestCase(1.0)]
        public void Percentile_SettingValidValue_PropertySet(double validPercentile)
        {
            // Setup
            var mocks = new MockRepository();
            var distributionMock = mocks.StrictMock<IDistribution>();
            mocks.ReplayAll();

            var designVariable = new SimpleDesignVariable(distributionMock);

            // Call
            designVariable.Percentile = validPercentile;

            // Assert
            Assert.AreEqual(validPercentile, designVariable.Percentile);
            mocks.VerifyAll(); // Expect no cals on mocks
        }

        [Test]
        public void Distribution_SetToNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var distributionMock = mocks.StrictMock<IDistribution>();
            mocks.ReplayAll();

            var designVariable = new SimpleDesignVariable(distributionMock);

            // Call
            TestDelegate call = () => designVariable.Distribution = null;

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessagePart = exception.Message.Split(new []{Environment.NewLine}, StringSplitOptions.None)[0];
            Assert.AreEqual("Een kansverdeling moet opgegeven zijn om op basis van die data een rekenwaarde te bepalen.", customMessagePart);
        }

        private class SimpleDesignVariable : DesignVariable<IDistribution>
        {
            public SimpleDesignVariable(IDistribution distribution) : base(distribution) {}

            public override double GetDesignValue()
            {
                throw new NotImplementedException();
            }
        }
    }
}