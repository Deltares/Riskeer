using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Components.Persistence.Stability.Data;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="PersistableDataModel"/>.
    /// </summary>
    public static class PersistableDataModelTestHelper
    {
        /// <summary>
        /// Asserts whether the <see cref="PersistableDataModel"/> contains the data
        /// that is representative for the <paramref name="calculation"/>
        /// and the <paramref name="filePath"/>.
        /// </summary>
        /// <param name="calculation">The calculation that contains the original data.</param>
        /// <param name="filePath">The file path that is used.</param>
        /// <param name="persistableDataModel">The <see cref="PersistableDataModel"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="persistableDataModel"/>
        /// is not correct.</exception>
        public static void AssertPersistableDataModel(MacroStabilityInwardsCalculation calculation, string filePath, PersistableDataModel persistableDataModel)
        {
            AssertProjectInfo(calculation, filePath, persistableDataModel.Info);
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

        /// <summary>
        /// Asserts whether the <see cref="PersistableProjectInfo"/> contains the data
        /// that is representative for the <paramref name="calculation"/>
        /// and the <paramref name="filePath"/>.
        /// </summary>
        /// <param name="calculation">The calculation that contains the original data.</param>
        /// <param name="filePath">The file path that is used.</param>
        /// <param name="persistableProjectInfo">The <see cref="PersistableProjectInfo"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="persistableProjectInfo"/>
        /// is not correct.</exception>
        public static void AssertProjectInfo(MacroStabilityInwardsCalculation calculation, string filePath, PersistableProjectInfo persistableProjectInfo)
        {
            Assert.AreEqual(filePath, persistableProjectInfo.Path);
            Assert.AreEqual(calculation.Name, persistableProjectInfo.Project);
            Assert.AreEqual(calculation.InputParameters.SurfaceLine.Name, persistableProjectInfo.CrossSection);
            Assert.AreEqual($"Riskeer {AssemblyUtils.GetAssemblyInfo(Assembly.GetAssembly(typeof(PersistableDataModelTestHelper))).Version}", persistableProjectInfo.ApplicationCreated);
            Assert.AreEqual("Export from Riskeer", persistableProjectInfo.Remarks);
            Assert.IsNotNull(persistableProjectInfo.Created);
            Assert.IsNull(persistableProjectInfo.Date);
            Assert.IsNull(persistableProjectInfo.LastModified);
            Assert.IsNull(persistableProjectInfo.LastModifier);
            Assert.IsNull(persistableProjectInfo.Analyst);
            Assert.IsTrue(persistableProjectInfo.IsDataValidated);
        }

        private static void AssertCalculationSettings(MacroStabilityInwardsCalculation calculation, IEnumerable<PersistableCalculationSettings> calculationSettings)
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

        private static void AssertStages(PersistableDataModel persistableDataModel)
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