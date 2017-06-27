// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationServiceTest
    {
        private double testSurfaceLineTopLevel;
        private MacroStabilityInwardsCalculationScenario testCalculation;

        [SetUp]
        public void Setup()
        {
            testCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            testSurfaceLineTopLevel = testCalculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
        }

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationService.Validate(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.Name = name;

            // Call
            Action call = () => MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual($"Validatie van '{name}' is gestart.", msgs.First());
                Assert.AreEqual($"Validatie van '{name}' is beëindigd.", msgs.Last());
            });
        }

        [Test]
        public void Validate_InvalidCalculationWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            var output = new TestMacroStabilityInwardsOutput();
            MacroStabilityInwardsCalculation invalidMacroStabilityInwardsCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();
            invalidMacroStabilityInwardsCalculation.Output = output;

            // Call
            bool isValid = MacroStabilityInwardsCalculationService.Validate(invalidMacroStabilityInwardsCalculation);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreSame(output, invalidMacroStabilityInwardsCalculation.Output);
        }

        [Test]
        public void Validate_InvalidCalculationInput_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                Name = name
            };

            // Call
            var isValid = false;
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                Assert.AreEqual($"Validatie van '{name}' is gestart.", msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[2]);
                Assert.AreEqual("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[3]);
                Assert.AreEqual($"Validatie van '{name}' is beëindigd.", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_HydraulicBoundaryLocationNotCalculated_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.Name = name;
            testCalculation.InputParameters.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            testCalculation.InputParameters.UseAssessmentLevelManualInput = false;

            // Call
            var isValid = false;
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                Assert.AreEqual($"Validatie van '{name}' is gestart.", msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                Assert.AreEqual($"Validatie van '{name}' is beëindigd.", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidManualAssessmentLevel_LogsErrorAndReturnsFalse(double assessmentLevel)
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.UseAssessmentLevelManualInput = true;
            testCalculation.InputParameters.AssessmentLevel = (RoundedDouble) assessmentLevel;
            testCalculation.Name = name;

            // Call
            var isValid = false;
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                Assert.AreEqual($"Validatie van '{name}' is gestart.", msgs.First());
                Assert.AreEqual("Validatie mislukt: De waarde voor 'toetspeil' moet een concreet getal zijn.", msgs[1]);
                Assert.AreEqual($"Validatie van '{name}' is beëindigd.", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutSurfaceLine_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.SurfaceLine = null;
            testCalculation.Name = name;

            // Call
            var isValid = false;
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                Assert.AreEqual($"Validatie van '{name}' is gestart.", msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[1]);
                Assert.AreEqual($"Validatie van '{name}' is beëindigd.", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutStochasticSoilProfile_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.StochasticSoilProfile = null;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = MacroStabilityInwardsCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                Assert.AreEqual($"Validatie van '{name}' is gestart.", msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[1]);
                Assert.AreEqual($"Validatie van '{name}' is beëindigd.", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationService.Calculate(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Calculate_ValidCalculation_LogStartAndEndOfValidatingInputsAndCalculation()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.Name = name;

            Action call = () =>
            {
                // Precondition
                Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation));

                // Call
                MacroStabilityInwardsCalculationService.Calculate(testCalculation);
            };

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual($"Validatie van '{name}' is gestart.", msgs.First());
                CalculationServiceTestHelper.AssertValidationEndMessage(name, msgs[1]);

                CalculationServiceTestHelper.AssertCalculationStartMessage(name, msgs[2]);
                Assert.AreEqual($"Berekening van '{name}' is beëindigd.", msgs.Last());
            });
        }

        [Test]
        public void Calculate_ValidCalculationNoOutput_ShouldSetOutput()
        {
            // Precondition
            Assert.IsNull(testCalculation.Output);
            Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation));

            // Call
            MacroStabilityInwardsCalculationService.Calculate(testCalculation);

            // Assert
            MacroStabilityInwardsOutput macroStabilityInwardsOutput = testCalculation.Output;
            Assert.IsNotNull(macroStabilityInwardsOutput);
        }

        [Test]
        public void Calculate_ValidCalculationWithOutput_ShouldChangeOutput()
        {
            // Setup
            var output = new TestMacroStabilityInwardsOutput();

            testCalculation.Output = output;

            // Precondition
            Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(testCalculation));

            // Call
            MacroStabilityInwardsCalculationService.Calculate(testCalculation);

            // Assert
            Assert.AreNotSame(output, testCalculation.Output);
        }
    }
}