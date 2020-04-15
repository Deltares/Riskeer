﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Components.Persistence.Stability.Data;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Primitives;

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
            AssertCalculationSettings(calculation.Output.SlidingCurve, persistableDataModel.CalculationSettings);

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

            AssertStages(persistableDataModel.Stages, persistableDataModel.CalculationSettings);
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

        /// <summary>
        /// Asserts whether the <see cref="PersistableCalculationSettings"/> contains the data
        /// that is representative for the <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The sliding curve that contains the original data.</param>
        /// <param name="calculationSettings">The collection of <see cref="PersistableCalculationSettings"/>
        /// that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="calculationSettings"/>
        /// is not correct.</exception>
        public static void AssertCalculationSettings(MacroStabilityInwardsSlidingCurve slidingCurve, IEnumerable<PersistableCalculationSettings> calculationSettings)
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
            Assert.AreEqual(slidingCurve.LeftCircle.Center.X, actualCalculationSettings.UpliftVan.SlipPlane.FirstCircleCenter.Value.X);
            Assert.AreEqual(slidingCurve.LeftCircle.Center.Y, actualCalculationSettings.UpliftVan.SlipPlane.FirstCircleCenter.Value.Z);
            Assert.AreEqual(slidingCurve.LeftCircle.Radius, actualCalculationSettings.UpliftVan.SlipPlane.FirstCircleRadius);
            Assert.AreEqual(slidingCurve.RightCircle.Center.X, actualCalculationSettings.UpliftVan.SlipPlane.SecondCircleCenter.Value.X);
            Assert.AreEqual(slidingCurve.RightCircle.Center.Y, actualCalculationSettings.UpliftVan.SlipPlane.SecondCircleCenter.Value.Z);
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableStage"/> contains the correct data.
        /// </summary>
        /// <param name="stages">The stages that needs to be asserted.</param>
        /// <param name="calculationSettings">The calculation settings that are used.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="stages"/>
        /// is not correct.</exception>
        public static void AssertStages(IEnumerable<PersistableStage> stages, IEnumerable<PersistableCalculationSettings> calculationSettings)
        {
            Assert.AreEqual(2, stages.Count());

            PersistableStage firstStage = stages.First();
            Assert.IsNotNull(firstStage.Id);
            Assert.AreEqual(calculationSettings.First().Id, firstStage.CalculationSettingsId);

            PersistableStage lastStage = stages.Last();
            Assert.IsNotNull(lastStage.Id);
            Assert.AreEqual(calculationSettings.Last().Id, lastStage.CalculationSettingsId);
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableSoil"/> contains the data
        /// that is representative for the <paramref name="originalLayers"/>.
        /// </summary>
        /// <param name="originalLayers">The layers that contain the original data.</param>
        /// <param name="actualSoils">The collection of <see cref="PersistableSoil"/>
        /// that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="actualSoils"/>
        /// is not correct.</exception>
        public static void AssertPersistableSoils(IEnumerable<MacroStabilityInwardsSoilLayer2D> originalLayers, IEnumerable<PersistableSoil> actualSoils)
        {
            Assert.AreEqual(originalLayers.Count(), actualSoils.Count());

            for (var i = 0; i < originalLayers.Count(); i++)
            {
                PersistableSoil soil = actualSoils.ElementAt(i);
                MacroStabilityInwardsSoilLayerData layerData = originalLayers.ElementAt(i).Data;

                Assert.IsNotNull(soil.Id);
                Assert.AreEqual(layerData.MaterialName, soil.Name);
                Assert.AreEqual($"{layerData.MaterialName}-{soil.Id}", soil.Code);
                Assert.IsTrue(soil.IsProbabilistic);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(layerData).GetDesignValue(), soil.Cohesion);
                AssertStochasticParameter(layerData.Cohesion, soil.CohesionStochasticParameter);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(layerData).GetDesignValue(), soil.FrictionAngle);
                AssertStochasticParameter(layerData.FrictionAngle, soil.FrictionAngleStochasticParameter);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(layerData).GetDesignValue(), soil.ShearStrengthRatio);
                AssertStochasticParameter(layerData.ShearStrengthRatio, soil.ShearStrengthRatioStochasticParameter);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(layerData).GetDesignValue(), soil.StrengthIncreaseExponent);
                AssertStochasticParameter(layerData.StrengthIncreaseExponent, soil.StrengthIncreaseExponentStochasticParameter);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(layerData).GetDesignValue(), soil.VolumetricWeightAbovePhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(layerData).GetDesignValue(), soil.VolumetricWeightBelowPhreaticLevel);

                Assert.IsFalse(soil.CohesionAndFrictionAngleCorrelated);
                Assert.IsFalse(soil.ShearStrengthRatioAndShearStrengthExponentCorrelated);

                Assert.AreEqual(GetExpectedShearStrengthModelTypeForAbovePhreaticLevel(layerData.ShearStrengthModel), soil.ShearStrengthModelTypeAbovePhreaticLevel);
                Assert.AreEqual(GetExpectedShearStrengthModelTypeForBelowPhreaticLevel(layerData.ShearStrengthModel), soil.ShearStrengthModelTypeBelowPhreaticLevel);
            }
        }

        /// <summary>
        /// Assert whether the <see cref="PersistableStochasticParameter"/> contains the data
        /// that is representative for the <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution that contains the original data.</param>
        /// <param name="stochasticParameter">the <see cref="PersistableStochasticParameter"/>
        /// that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="stochasticParameter"/>
        /// is not correct.</exception>
        public static void AssertStochasticParameter(IVariationCoefficientDistribution distribution, PersistableStochasticParameter stochasticParameter)
        {
            Assert.AreEqual(distribution.Mean.Value, stochasticParameter.Mean);
            Assert.AreEqual(distribution.Mean * distribution.CoefficientOfVariation, stochasticParameter.StandardDeviation);
            Assert.IsTrue(stochasticParameter.IsProbabilistic);
        }

        private static PersistableShearStrengthModelType GetExpectedShearStrengthModelTypeForAbovePhreaticLevel(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            switch (shearStrengthModel)
            {
                case MacroStabilityInwardsShearStrengthModel.CPhi:
                case MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated:
                    return PersistableShearStrengthModelType.CPhi;
                case MacroStabilityInwardsShearStrengthModel.SuCalculated:
                    return PersistableShearStrengthModelType.Su;
                default:
                    throw new NotSupportedException();
            }
        }

        private static PersistableShearStrengthModelType GetExpectedShearStrengthModelTypeForBelowPhreaticLevel(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            switch (shearStrengthModel)
            {
                case MacroStabilityInwardsShearStrengthModel.CPhi:
                    return PersistableShearStrengthModelType.CPhi;
                case MacroStabilityInwardsShearStrengthModel.SuCalculated:
                case MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated:
                    return PersistableShearStrengthModelType.Su;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}