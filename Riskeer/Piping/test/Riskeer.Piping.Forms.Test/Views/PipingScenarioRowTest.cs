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

using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingScenarioRowTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new TestPipingCalculationScenario();

            // Call
            var row = new TestPipingScenarioRow(calculation);

            // Assert
            Assert.IsInstanceOf<ScenarioRow<TestPipingCalculationScenario>>(row);
            Assert.IsInstanceOf<IPipingScenarioRow>(row);
            Assert.AreSame(calculation, row.CalculationScenario);

            TestHelper.AssertTypeConverter<PipingScenarioRow<IPipingCalculationScenario<PipingInput>>, NoProbabilityValueDoubleConverter>(
                nameof(PipingScenarioRow<IPipingCalculationScenario<PipingInput>>.SectionFailureProbability));
        }

        private class TestPipingScenarioRow : PipingScenarioRow<TestPipingCalculationScenario>
        {
            public TestPipingScenarioRow(TestPipingCalculationScenario calculationScenario)
                : base(calculationScenario) {}

            public override double FailureProbability { get; }
            public override double SectionFailureProbability { get; }

            public override void Update() {}
        }
    }
}