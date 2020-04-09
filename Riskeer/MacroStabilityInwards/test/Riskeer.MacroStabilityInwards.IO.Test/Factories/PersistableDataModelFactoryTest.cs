using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Components.Persistence.Stability.Data;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Factories;

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
            AssertPersistableDataModel(calculation, persistableDataModel, filePath);
        }

        private void AssertPersistableDataModel(MacroStabilityInwardsCalculation calculation, PersistableDataModel persistableDataModel, string filePath)
        {
            AssertProjectInfo(calculation, persistableDataModel.Info, filePath);
            AssertCalculationSettings(calculation, persistableDataModel.CalculationSettings);

            Assert.IsNull(persistableDataModel.AssessmentResults);
            Assert.IsNull(persistableDataModel.Decorations);
            Assert.IsNull(persistableDataModel.Geometry);
            Assert.IsNull(persistableDataModel.Loads);
            Assert.IsNull(persistableDataModel.NailPropertiesForSoils);
            Assert.IsNull(persistableDataModel.Reinforcements);
            Assert.IsNull(persistableDataModel.SoilCorrelations);
            Assert.IsNull(persistableDataModel.SoilLayers);
            Assert.IsNull(persistableDataModel.SoilVisualizations);
            Assert.IsNull(persistableDataModel.Soils);
            Assert.IsNull(persistableDataModel.WaternetCreatorSettings);
            Assert.IsNull(persistableDataModel.Waternets);
            Assert.IsNull(persistableDataModel.StateCorrelations);
            Assert.IsNull(persistableDataModel.States);

            AssertStages(persistableDataModel);
        }

        private void AssertProjectInfo(MacroStabilityInwardsCalculation calculation, PersistableProjectInfo persistableProjectInfo, string filePath)
        {
            Assert.AreEqual(filePath, persistableProjectInfo.Path);
            Assert.AreEqual(calculation.Name, persistableProjectInfo.Project);
            Assert.AreEqual(calculation.InputParameters.SurfaceLine.Name, persistableProjectInfo.CrossSection);
            Assert.AreEqual($"Riskeer {AssemblyUtils.GetAssemblyInfo(Assembly.GetAssembly(GetType())).Version}", persistableProjectInfo.ApplicationCreated);
            Assert.AreEqual("Export from Riskeer", persistableProjectInfo.Remarks);
            Assert.IsNotNull(persistableProjectInfo.Created);
            Assert.IsNull(persistableProjectInfo.Date);
            Assert.IsNull(persistableProjectInfo.LastModified);
            Assert.IsNull(persistableProjectInfo.LastModifier);
            Assert.IsNull(persistableProjectInfo.Analyst);
            Assert.IsTrue(persistableProjectInfo.IsDataValidated);
        }

        private void AssertCalculationSettings(MacroStabilityInwardsCalculation calculation, IEnumerable<PersistableCalculationSettings> calculationSettings)
        {
            Assert.AreEqual(2, calculationSettings.Count());
            PersistableCalculationSettings emptyCalculationSettings = calculationSettings.First();

            Assert.AreEqual("0", emptyCalculationSettings.Id);
            Assert.IsNull(emptyCalculationSettings.AnalysisType);
            Assert.IsNull(emptyCalculationSettings.CalculationType);
            Assert.IsNull(emptyCalculationSettings.UpliftVan);

            PersistableCalculationSettings actualCalculationSettings = calculationSettings.Last();
            Assert.AreEqual("1", actualCalculationSettings.Id);
            Assert.AreEqual(PersistableAnalysisType.UpliftVan, actualCalculationSettings.AnalysisType);
            Assert.AreEqual(PersistableCalculationType.Deterministic, actualCalculationSettings.CalculationType);
            Assert.AreEqual(calculation.Output.SlidingCurve.LeftCircle.Center.X, actualCalculationSettings.UpliftVan.SlipPlane.FirstCircleCenter.Value.X);
            Assert.AreEqual(calculation.Output.SlidingCurve.LeftCircle.Center.Y, actualCalculationSettings.UpliftVan.SlipPlane.FirstCircleCenter.Value.Z);
            Assert.AreEqual(calculation.Output.SlidingCurve.LeftCircle.Radius, actualCalculationSettings.UpliftVan.SlipPlane.FirstCircleRadius);
            Assert.AreEqual(calculation.Output.SlidingCurve.RightCircle.Center.X, actualCalculationSettings.UpliftVan.SlipPlane.SecondCircleCenter.Value.X);
            Assert.AreEqual(calculation.Output.SlidingCurve.RightCircle.Center.Y, actualCalculationSettings.UpliftVan.SlipPlane.SecondCircleCenter.Value.Z);
        }

        private void AssertStages(PersistableDataModel persistableDataModel)
        {
            IEnumerable<PersistableStage> stages = persistableDataModel.Stages;
            Assert.AreEqual(2, stages.Count());
            PersistableStage firstStage = stages.First();
            Assert.AreEqual("0", firstStage.Id);
            Assert.AreEqual(persistableDataModel.CalculationSettings.First().Id, firstStage.CalculationSettingsId);

            PersistableStage lastStage = stages.Last();
            Assert.AreEqual("1", lastStage.Id);
            Assert.AreEqual(persistableDataModel.CalculationSettings.Last().Id, lastStage.CalculationSettingsId);
        }
    }
}