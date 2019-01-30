// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Probabilistics;

namespace Riskeer.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class PercentileBasedDesignVariableTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            mocks.ReplayAll();

            // Call
            var designVariable = new SimpleDesignVariable(distribution);

            // Assert
            Assert.IsInstanceOf<DesignVariable<IDistribution>>(designVariable);
            Assert.AreSame(distribution, designVariable.Distribution);
            Assert.AreEqual(0.5, designVariable.Percentile);
            mocks.VerifyAll(); // Expect no calls on mocks
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1234.5678)]
        [TestCase(0 - 1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(12345.789)]
        [TestCase(double.NaN)]
        public void Percentile_SettingInvalidValue_ThrowArgumentOutOfRangeException(double invalidPercentile)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            mocks.ReplayAll();

            var designVariable = new SimpleDesignVariable(distribution);

            // Call
            TestDelegate call = () => designVariable.Percentile = invalidPercentile;

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            string customMessagePart = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries)[0];
            Assert.AreEqual("Percentiel moet in het bereik [0,0, 1,0] liggen.", customMessagePart);
            mocks.VerifyAll(); // Expect no calls on mocks
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(0.54638291)]
        [TestCase(1.0)]
        public void Percentile_SettingValidValue_PropertySet(double validPercentile)
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IDistribution>();
            mocks.ReplayAll();

            var designVariable = new SimpleDesignVariable(distribution);

            // Call
            designVariable.Percentile = validPercentile;

            // Assert
            Assert.AreEqual(validPercentile, designVariable.Percentile);
            mocks.VerifyAll(); // Expect no calls on mocks
        }

        private class SimpleDesignVariable : PercentileBasedDesignVariable<IDistribution>
        {
            public SimpleDesignVariable(IDistribution distribution) : base(distribution) {}

            public override RoundedDouble GetDesignValue()
            {
                throw new NotImplementedException();
            }
        }
    }
}