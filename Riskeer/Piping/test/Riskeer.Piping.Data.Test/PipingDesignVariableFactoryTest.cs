// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingDesignVariableFactoryTest
    {
        [Test]
        public void GetPhreaticLevelExit_PipingInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingDesignVariableFactory.GetPhreaticLevelExit(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("pipingInput", exception.ParamName);
        }

        [Test]
        public void GetPhreaticLevelExit_ValidPipingInput_CreateDesignVariableForPhreaticLevelExit()
        {
            // Setup
            var pipingInput = new TestPipingInput();

            // Call
            DesignVariable<NormalDistribution> phreaticLevelExit = PipingDesignVariableFactory.GetPhreaticLevelExit(pipingInput);

            // Assert
            Assert.AreSame(pipingInput.PhreaticLevelExit, phreaticLevelExit.Distribution);
            AssertPercentile(0.05, phreaticLevelExit);
        }

        private static void AssertPercentile<T>(double percentile, DesignVariable<T> designVariable) where T : IDistribution
        {
            Assert.IsInstanceOf<PercentileBasedDesignVariable<T>>(designVariable);
            var percentileBasedDesignVariable = (PercentileBasedDesignVariable<T>) designVariable;
            Assert.AreEqual(percentile, percentileBasedDesignVariable.Percentile);
        }
    }
}