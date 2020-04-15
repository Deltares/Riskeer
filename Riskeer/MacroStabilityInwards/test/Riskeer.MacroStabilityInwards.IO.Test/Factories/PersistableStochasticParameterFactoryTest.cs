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
using Components.Persistence.Stability.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;

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
            PersistableDataModelTestHelper.AssertStochasticParameter(distribution, stochasticParameter);
            mocks.VerifyAll();
        }
    }
}