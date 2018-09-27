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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;

namespace Ringtoets.Common.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class CalculationScenarioConversionExtensionsTest
    {
        [Test]
        public void ToScenarioConfiguration_CalculationScenarioNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((ICalculationScenario) null).ToScenarioConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationScenario", exception.ParamName);
        }

        [Test]
        public void ToScenarioConfiguration_ValidCalculationScenario_InstanceWithExpectedParametersSet()
        {
            // Setup
            var mocks = new MockRepository();
            var scenario = mocks.Stub<ICalculationScenario>();
            mocks.ReplayAll();

            var random = new Random(21);
            RoundedDouble contribution = random.NextRoundedDouble();
            bool relevant = random.NextBoolean();

            scenario.Contribution = contribution;
            scenario.IsRelevant = relevant;

            // Call
            ScenarioConfiguration configuration = scenario.ToScenarioConfiguration();

            // Assert
            Assert.AreEqual(contribution * 100, configuration.Contribution);
            Assert.AreEqual(relevant, configuration.IsRelevant);
            mocks.VerifyAll();
        }
    }
}