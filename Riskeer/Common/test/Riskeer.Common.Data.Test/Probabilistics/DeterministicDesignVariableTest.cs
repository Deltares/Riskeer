// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class DeterministicDesignVariableTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            distribution.Mean = new RoundedDouble();
            mocks.ReplayAll();

            // Call
            var designVariable = new DeterministicDesignVariable<IDistribution>(distribution);

            // Assert
            Assert.IsInstanceOf<DesignVariable<IDistribution>>(designVariable);
            Assert.AreSame(distribution, designVariable.Distribution);
            Assert.AreEqual(0, designVariable.GetDesignValue().Value);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDesignValue_Always_ReturnsDeterministicValueWithNumberOfDecimalsFromDistributionMean()
        {
            // Setup
            double testValue = new Random(21).NextDouble();
            const int numberOfDecimalPlaces = 2;

            var mocks = new MockRepository();
            var distribution = mocks.Stub<IDistribution>();
            distribution.Mean = new RoundedDouble(numberOfDecimalPlaces);
            mocks.ReplayAll();

            var designVariable = new DeterministicDesignVariable<IDistribution>(distribution, testValue);

            // Call
            RoundedDouble designValue = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(testValue, designValue.Value, designValue.GetAccuracy());
            Assert.AreEqual(numberOfDecimalPlaces, designValue.NumberOfDecimalPlaces);
            mocks.VerifyAll();
        }
    }
}