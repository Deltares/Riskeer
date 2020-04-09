using System;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableDataModelFactoryTest
    {
        [Test]
        public void Create_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableDataModelFactory.Create(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Create_CalculationWithoutOutput_ThrowsInvalidOperationException()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());

            // Call
            void Call() => PersistableDataModelFactory.Create(calculation, string.Empty);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(Call);
            Assert.AreEqual("Calculation must have output.", exception.Message);
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableDataModel()
        {
            // Setup
            const string filePath = "ValidFilePath";
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            // Call
            PersistableDataModel persistableDataModel = PersistableDataModelFactory.Create(calculation, filePath);

            // Assert
            PersistableDataModelTestHelper.AssertPersistableDataModel(calculation, filePath, persistableDataModel);
        }
    }
}