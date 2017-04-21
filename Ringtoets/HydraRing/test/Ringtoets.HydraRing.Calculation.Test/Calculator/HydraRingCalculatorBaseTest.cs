// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Test.Calculator
{
    [TestFixture]
    public class HydraRingCalculatorBaseTest
    {
        [Test]
        public void Constructor_WithoutHlcdDirectory_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraRingCalculator(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hlcdDirectory", paramName);
        }

        [Test]
        public void Calculate_WithCustomParser_ParsersExecutedAndOutputSet()
        {
            // Setup
            var calculator = new TestHydraRingCalculator("");

            // Call
            calculator.PublicCalculate();

            // Assert
            Assert.That(!string.IsNullOrEmpty(calculator.OutputDirectory));
            Assert.IsTrue(calculator.Output);
        }
    }

    internal class TestHydraRingCalculator : HydraRingCalculatorBase
    {
        private readonly TestParser parser;

        public TestHydraRingCalculator(string hlcdDirectory) : base(hlcdDirectory)
        {
            parser = new TestParser();
        }

        public bool Output { get; private set; }

        public void PublicCalculate()
        {
            Calculate(HydraRingUncertaintiesType.All, new TestHydraRingCalculationInput());
        }

        protected override void SetOutputs()
        {
            Output = parser.Parsed;
        }

        protected override IEnumerable<IHydraRingFileParser> GetParsers()
        {
            yield return parser;
        }
    }

    internal class TestHydraRingCalculationInput : HydraRingCalculationInput
    {
        public TestHydraRingCalculationInput() : base(12)
        {
            DesignTablesSetting = new DesignTablesSetting(0, 0);
            NumericsSettings = new Dictionary<int, NumericsSetting>
            {
                {
                    1, new NumericsSetting(11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6.0, 6.0, 25)
                }
            };
            TimeIntegrationSetting = new TimeIntegrationSetting(1);
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.AssessmentLevel;
            }
        }

        public override int CalculationTypeId
        {
            get
            {
                return 0;
            }
        }

        public override int VariableId
        {
            get
            {
                return 0;
            }
        }

        public override HydraRingSection Section { get; } = new HydraRingSection(12, 12, 12);
    }

    internal class TestParser : IHydraRingFileParser
    {
        public bool Parsed { get; private set; }

        public void Parse(string workingDirectory, int sectionId)
        {
            Parsed = true;
        }
    }
}