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
    public class VariationCoefficientDesignVariableTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            // Call
            var designVariable = new SimpleVariationCoefficientDesignVariable(distribution);

            // Assert
            Assert.AreSame(distribution, designVariable.Distribution);
            mocks.VerifyAll(); // Expect no calls on mocks
        }

        [Test]
        public void ParameteredConstructor_DistributionIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleVariationCoefficientDesignVariable(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessagePart = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("Een kansverdeling moet opgegeven zijn om op basis van die data een rekenwaarde te bepalen.", customMessagePart);
        }

        [Test]
        public void Distribution_SetToNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.StrictMock<IVariationCoefficientDistribution>();
            mocks.ReplayAll();

            var designVariable = new SimpleVariationCoefficientDesignVariable(distribution);

            // Call
            TestDelegate call = () => designVariable.Distribution = null;

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessagePart = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("Een kansverdeling moet opgegeven zijn om op basis van die data een rekenwaarde te bepalen.", customMessagePart);
            mocks.VerifyAll(); // Expect no calls on mocks
        }

        private class SimpleVariationCoefficientDesignVariable : VariationCoefficientDesignVariable<IVariationCoefficientDistribution>
        {
            public SimpleVariationCoefficientDesignVariable(IVariationCoefficientDistribution distribution) : base(distribution) {}

            public override RoundedDouble GetDesignValue()
            {
                throw new NotImplementedException();
            }
        }
    }
}