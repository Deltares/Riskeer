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
    public class PersistableProjectInfoFactoryTest
    {
        [Test]
        public void Create_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableProjectInfoFactory.Create(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableProjectInfo()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            const string filePath = "SomeFilePath";

            // Call
            PersistableProjectInfo persistableProjectInfo = PersistableProjectInfoFactory.Create(calculation, filePath);

            // Assert
            PersistableDataModelTestHelper.AssertProjectInfo(calculation, filePath, persistableProjectInfo);
        }
    }
}