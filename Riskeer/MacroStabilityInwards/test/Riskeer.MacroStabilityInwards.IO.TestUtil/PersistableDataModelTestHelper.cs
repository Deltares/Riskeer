﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Components.Persistence.Stability.Data;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Geometry;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.IO.Factories;
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

            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(calculation.InputParameters.SoilProfileUnderSurfaceLine.Layers);
            AssertPersistableSoils(layers, persistableDataModel.Soils.Soils);
            AssertPersistableGeometry(layers, persistableDataModel.Geometry);
            AssertPersistableSoilLayers(layers, persistableDataModel.SoilLayers, persistableDataModel.Soils.Soils, persistableDataModel.Geometry);
            AssertWaternets(new[]
            {
                DerivedMacroStabilityInwardsInput.GetWaternetDaily(calculation.InputParameters, new GeneralMacroStabilityInwardsInput()),
                DerivedMacroStabilityInwardsInput.GetWaternetExtreme(calculation.InputParameters, new GeneralMacroStabilityInwardsInput(), RoundedDouble.NaN)
            }, persistableDataModel.Waternets);
            AssertWaternetCreatorSettings(calculation.InputParameters, persistableDataModel.WaternetCreatorSettings, AssessmentSectionTestHelper.GetTestAssessmentLevel(), new[]
            {
                MacroStabilityInwardsExportStageType.Daily,
                MacroStabilityInwardsExportStageType.Extreme
            });
            AssertStates(calculation.InputParameters.SoilProfileUnderSurfaceLine, persistableDataModel.States);

            Assert.IsNull(persistableDataModel.AssessmentResults);
            Assert.IsNull(persistableDataModel.Decorations);
            Assert.IsNull(persistableDataModel.Loads);
            Assert.IsNull(persistableDataModel.NailPropertiesForSoils);
            Assert.IsNull(persistableDataModel.Reinforcements);
            Assert.IsNull(persistableDataModel.SoilCorrelations);
            Assert.IsNull(persistableDataModel.SoilVisualizations);
            Assert.IsNull(persistableDataModel.StateCorrelations);

            AssertStages(persistableDataModel.Stages, persistableDataModel.CalculationSettings, persistableDataModel.Geometry, persistableDataModel.SoilLayers,
                         persistableDataModel.Waternets, persistableDataModel.WaternetCreatorSettings, persistableDataModel.States);
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
            PersistableCalculationSettings dailyCalculationSettings = calculationSettings.First();

            Assert.IsNotNull(dailyCalculationSettings.Id);
            Assert.AreEqual(PersistableAnalysisType.UpliftVan, dailyCalculationSettings.AnalysisType);
            Assert.IsNull(dailyCalculationSettings.CalculationType);
            Assert.IsNull(dailyCalculationSettings.UpliftVan);

            PersistableCalculationSettings extremeCalculationSettings = calculationSettings.Last();
            Assert.IsNotNull(extremeCalculationSettings.Id);
            Assert.AreEqual(PersistableAnalysisType.UpliftVan, extremeCalculationSettings.AnalysisType);
            Assert.AreEqual(PersistableCalculationType.Deterministic, extremeCalculationSettings.CalculationType);
            Assert.AreEqual(slidingCurve.LeftCircle.Center.X, extremeCalculationSettings.UpliftVan.SlipPlane.FirstCircleCenter.Value.X);
            Assert.AreEqual(slidingCurve.LeftCircle.Center.Y, extremeCalculationSettings.UpliftVan.SlipPlane.FirstCircleCenter.Value.Z);
            Assert.AreEqual(slidingCurve.LeftCircle.Radius, extremeCalculationSettings.UpliftVan.SlipPlane.FirstCircleRadius);
            Assert.AreEqual(slidingCurve.RightCircle.Center.X, extremeCalculationSettings.UpliftVan.SlipPlane.SecondCircleCenter.Value.X);
            Assert.AreEqual(slidingCurve.RightCircle.Center.Y, extremeCalculationSettings.UpliftVan.SlipPlane.SecondCircleCenter.Value.Z);
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
        public static void AssertPersistableSoils(IEnumerable<IMacroStabilityInwardsSoilLayer> originalLayers, IEnumerable<PersistableSoil> actualSoils)
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

                var dilatancyDistribution = new VariationCoefficientNormalDistribution(2)
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0
                };

                Assert.AreEqual(0, soil.Dilatancy);
                AssertStochasticParameter(dilatancyDistribution, soil.DilatancyStochasticParameter, false);
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableStochasticParameter"/> contains the data
        /// that is representative for the <paramref name="distribution"/>.
        /// </summary>
        /// <param name="distribution">The distribution that contains the original data.</param>
        /// <param name="stochasticParameter">The <see cref="PersistableStochasticParameter"/>
        /// that needs to be asserted.</param>
        /// <param name="expectedIsProbabilistic">The expected value for <see cref="PersistableStochasticParameter.IsProbabilistic"/>.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="stochasticParameter"/>
        /// is not correct.</exception>
        public static void AssertStochasticParameter(IVariationCoefficientDistribution distribution, PersistableStochasticParameter stochasticParameter, bool expectedIsProbabilistic = true)
        {
            Assert.AreEqual(distribution.Mean.Value, stochasticParameter.Mean);
            Assert.AreEqual(distribution.Mean.Value * distribution.CoefficientOfVariation.Value, stochasticParameter.StandardDeviation);
            Assert.AreEqual(expectedIsProbabilistic, stochasticParameter.IsProbabilistic);
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableGeometry"/> contains the data
        /// that is representative for the <paramref name="layers"/>.
        /// </summary>
        /// <param name="layers">The layers that contain the original data.</param>
        /// <param name="geometries">The <see cref="PersistableGeometry"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="geometries"/>
        /// is not correct.</exception>
        public static void AssertPersistableGeometry(IEnumerable<MacroStabilityInwardsSoilLayer2D> layers, IEnumerable<PersistableGeometry> geometries)
        {
            Assert.AreEqual(2, geometries.Count());

            foreach (PersistableGeometry persistableGeometry in geometries)
            {
                Assert.IsNotNull(persistableGeometry.Id);
                IEnumerable<PersistableLayer> persistableGeometryLayers = persistableGeometry.Layers;

                Assert.AreEqual(layers.Count(), persistableGeometryLayers.Count());

                for (int i = 0; i < layers.Count(); i++)
                {
                    MacroStabilityInwardsSoilLayer2D soilLayer = layers.ElementAt(i);
                    PersistableLayer persistableLayer = persistableGeometryLayers.ElementAt(i);

                    Assert.IsNotNull(persistableLayer.Id);
                    Assert.AreEqual(soilLayer.Data.MaterialName, persistableLayer.Label);

                    CollectionAssert.AreEqual(soilLayer.OuterRing.Points.Select(p => new PersistablePoint(p.X, p.Y)), persistableLayer.Points);
                }
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableSoilLayerCollection"/> contains the data
        /// that is representative for the <paramref name="layers"/>.
        /// </summary>
        /// <param name="layers">The layers that contain the original data.</param>
        /// <param name="soilLayerCollections">The <see cref="PersistableSoilLayerCollection"/>
        /// that needs to be asserted.</param>
        /// <param name="soils">The soils that are used.</param>
        /// <param name="geometries">The geometries that are used.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="soilLayerCollections"/>
        /// is not correct.</exception>
        public static void AssertPersistableSoilLayers(IEnumerable<MacroStabilityInwardsSoilLayer2D> layers, IEnumerable<PersistableSoilLayerCollection> soilLayerCollections,
                                                       IEnumerable<PersistableSoil> soils, IEnumerable<PersistableGeometry> geometries)
        {
            Assert.AreEqual(2, soilLayerCollections.Count());

            IEnumerable<MacroStabilityInwardsSoilLayer2D> originalLayers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(layers);

            for (var i = 0; i < soilLayerCollections.Count(); i++)
            {
                PersistableSoilLayerCollection soilLayerCollection = soilLayerCollections.ElementAt(i);

                Assert.IsNotNull(soilLayerCollection.Id);
                Assert.AreEqual(originalLayers.Count(), soilLayerCollection.SoilLayers.Count());

                for (var j = 0; j < originalLayers.Count(); j++)
                {
                    PersistableSoilLayer persistableSoilLayer = soilLayerCollection.SoilLayers.ElementAt(j);

                    Assert.AreEqual(soils.ElementAt(j).Id, persistableSoilLayer.SoilId);
                    Assert.AreEqual(geometries.ElementAt(i).Layers.ElementAt(j).Id, persistableSoilLayer.LayerId);
                }
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="MacroStabilityInwardsWaternet"/> contains the data
        /// that is representative for the <paramref name="originalWaternets"/>.
        /// </summary>
        /// <param name="originalWaternets">The Waternets that contain the original data.</param>
        /// <param name="actualWaternets">The <see cref="PersistableWaternet"/>
        /// that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="actualWaternets"/>
        /// is not correct.</exception>
        public static void AssertWaternets(IEnumerable<MacroStabilityInwardsWaternet> originalWaternets, IEnumerable<PersistableWaternet> actualWaternets)
        {
            Assert.AreEqual(originalWaternets.Count(), actualWaternets.Count());

            for (var i = 0; i < originalWaternets.Count(); i++)
            {
                MacroStabilityInwardsWaternet originalWaternet = originalWaternets.ElementAt(i);
                PersistableWaternet actualWaternet = actualWaternets.ElementAt(i);

                Assert.IsNotNull(actualWaternet.Id);
                Assert.AreEqual(9.81, actualWaternet.UnitWeightWater);

                PersistableHeadLine firstHeadLine = actualWaternet.HeadLines.FirstOrDefault();
                Assert.AreEqual(actualWaternet.PhreaticLineId, firstHeadLine?.Id);

                Assert.AreEqual(originalWaternet.PhreaticLines.Count(), actualWaternet.HeadLines.Count());

                for (var j = 0; j < originalWaternet.PhreaticLines.Count(); j++)
                {
                    MacroStabilityInwardsPhreaticLine phreaticLine = originalWaternet.PhreaticLines.ElementAt(j);
                    PersistableHeadLine headLine = actualWaternet.HeadLines.ElementAt(j);

                    Assert.IsNotNull(headLine.Id);
                    Assert.AreEqual(phreaticLine.Name, headLine.Label);
                    CollectionAssert.AreEqual(phreaticLine.Geometry.Select(p => new PersistablePoint(p.X, p.Y)), headLine.Points);
                }

                Assert.AreEqual(originalWaternet.WaternetLines.Count(), actualWaternet.ReferenceLines.Count());

                for (var j = 0; j < originalWaternet.WaternetLines.Count(); j++)
                {
                    MacroStabilityInwardsWaternetLine waternetLine = originalWaternet.WaternetLines.ElementAt(j);
                    PersistableReferenceLine referenceLine = actualWaternet.ReferenceLines.ElementAt(j);

                    Assert.IsNotNull(referenceLine.Id);
                    Assert.AreEqual(waternetLine.Name, referenceLine.Label);
                    CollectionAssert.AreEqual(waternetLine.Geometry.Select(p => new PersistablePoint(p.X, p.Y)), referenceLine.Points);

                    Assert.AreEqual(firstHeadLine.Id, referenceLine.TopHeadLineId);
                    Assert.AreEqual(firstHeadLine.Id, referenceLine.BottomHeadLineId);
                }
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableWaternetCreatorSettings"/> contains the data
        /// that is representative for the <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input that contains the original data.</param>
        /// <param name="waternetCreatorSettingsCollection">The <see cref="PersistableWaternetCreatorSettings"/>
        /// that needs to be asserted.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use.</param>
        /// <param name="stages">The stages to use.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="waternetCreatorSettingsCollection"/>
        /// is not correct.</exception>
        public static void AssertWaternetCreatorSettings(MacroStabilityInwardsInput input, IEnumerable<PersistableWaternetCreatorSettings> waternetCreatorSettingsCollection,
                                                         RoundedDouble normativeAssessmentLevel, MacroStabilityInwardsExportStageType[] stages)
        {
            Assert.AreEqual(2, waternetCreatorSettingsCollection.Count());

            for (var i = 0; i < waternetCreatorSettingsCollection.Count(); i++)
            {
                PersistableWaternetCreatorSettings waternetCreatorSettings = waternetCreatorSettingsCollection.ElementAt(i);

                Assert.AreEqual(input.MinimumLevelPhreaticLineAtDikeTopPolder, waternetCreatorSettings.InitialLevelEmbankmentTopWaterSide);
                Assert.AreEqual(input.MinimumLevelPhreaticLineAtDikeTopRiver, waternetCreatorSettings.InitialLevelEmbankmentTopLandSide);
                Assert.AreEqual(input.AdjustPhreaticLine3And4ForUplift, waternetCreatorSettings.AdjustForUplift);
                Assert.AreEqual(input.LeakageLengthInwardsPhreaticLine3, waternetCreatorSettings.PleistoceneLeakageLengthInwards);
                Assert.AreEqual(input.LeakageLengthOutwardsPhreaticLine3, waternetCreatorSettings.PleistoceneLeakageLengthInwards);
                Assert.AreEqual(input.LeakageLengthInwardsPhreaticLine4, waternetCreatorSettings.AquiferLayerInsideAquitardLeakageLengthInwards);
                Assert.AreEqual(input.LeakageLengthOutwardsPhreaticLine4, waternetCreatorSettings.AquiferLayerInsideAquitardLeakageLengthOutwards);
                Assert.AreEqual(input.PiezometricHeadPhreaticLine2Inwards, waternetCreatorSettings.AquitardHeadLandSide);
                Assert.AreEqual(input.PiezometricHeadPhreaticLine2Outwards, waternetCreatorSettings.AquitardHeadWaterSide);
                Assert.AreEqual(input.WaterLevelRiverAverage, waternetCreatorSettings.MeanWaterLevel);
                Assert.AreEqual(input.DrainageConstructionPresent, waternetCreatorSettings.IsDrainageConstructionPresent);
                Assert.AreEqual(input.XCoordinateDrainageConstruction, waternetCreatorSettings.DrainageConstruction.X);
                Assert.AreEqual(input.ZCoordinateDrainageConstruction, waternetCreatorSettings.DrainageConstruction.Z);
                Assert.AreEqual(IsDitchPresent(input.SurfaceLine), waternetCreatorSettings.IsDitchPresent);
                AssertDitchCharacteristics(input.SurfaceLine, waternetCreatorSettings.DitchCharacteristics, IsDitchPresent(input.SurfaceLine));
                AssertEmbankmentCharacteristics(input.SurfaceLine, waternetCreatorSettings.EmbankmentCharacteristics);
                Assert.AreEqual(GetEmbankmentSoilScenario(input.DikeSoilScenario), waternetCreatorSettings.EmbankmentSoilScenario);
                Assert.IsFalse(waternetCreatorSettings.IsAquiferLayerInsideAquitard);
                Assert.IsNotNull(waternetCreatorSettings.AquiferLayerId);

                if (stages[i] == MacroStabilityInwardsExportStageType.Daily)
                {
                    Assert.AreEqual(input.WaterLevelRiverAverage, waternetCreatorSettings.NormativeWaterLevel);
                    Assert.AreEqual(input.LocationInputDaily.WaterLevelPolder, waternetCreatorSettings.WaterLevelHinterland);
                    Assert.AreEqual(input.LocationInputDaily.PenetrationLength, waternetCreatorSettings.IntrusionLength);
                    AssertOffsets(input.LocationInputDaily, waternetCreatorSettings);
                }
                else if (stages[i] == MacroStabilityInwardsExportStageType.Extreme)
                {
                    Assert.AreEqual(normativeAssessmentLevel, waternetCreatorSettings.NormativeWaterLevel);
                    Assert.AreEqual(input.LocationInputExtreme.WaterLevelPolder, waternetCreatorSettings.WaterLevelHinterland);
                    Assert.AreEqual(input.LocationInputExtreme.PenetrationLength, waternetCreatorSettings.IntrusionLength);
                    AssertOffsets(input.LocationInputExtreme, waternetCreatorSettings);
                }
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableState"/> contains the data
        /// that is representative for the <paramref name="soilProfile"/>.
        /// </summary>
        /// <param name="soilProfile">The input that contains the original data.</param>
        /// <param name="states">The <see cref="PersistableState"/>
        /// that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="states"/>
        /// is not correct.</exception>
        public static void AssertStates(IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile, IEnumerable<PersistableState> states)
        {
            Assert.AreEqual(1, states.Count());

            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(soilProfile.Layers);

            for (var i = 0; i < states.Count(); i++)
            {
                PersistableState state = states.ElementAt(i);

                Assert.IsNotNull(state.Id);
                CollectionAssert.IsEmpty(state.StateLines);

                AssertPopStatePoints(layers, state.StatePoints.Where(sp => sp.Stress.StateType == PersistableStateType.Pop));
                AssertYieldStressStatePoints(layers, soilProfile.PreconsolidationStresses, state.StatePoints.Where(sp => sp.Stress.StateType == PersistableStateType.YieldStress));
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableStage"/> contains the correct data.
        /// </summary>
        /// <param name="stages">The stages that needs to be asserted.</param>
        /// <param name="calculationSettings">The calculation settings that are used.</param>
        /// <param name="geometries">The geometries that are used.</param>
        /// <param name="soilLayers">The soil layers that are used.</param>
        /// <param name="waternets">The Waternets that are used.</param>
        /// <param name="waternetCreatorSettings">The Waternet creator settings that are used.</param>
        /// <param name="states">The states that are used.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="stages"/>
        /// is not correct.</exception>
        public static void AssertStages(IEnumerable<PersistableStage> stages, IEnumerable<PersistableCalculationSettings> calculationSettings,
                                        IEnumerable<PersistableGeometry> geometries, IEnumerable<PersistableSoilLayerCollection> soilLayers,
                                        IEnumerable<PersistableWaternet> waternets, IEnumerable<PersistableWaternetCreatorSettings> waternetCreatorSettings,
                                        IEnumerable<PersistableState> states)
        {
            Assert.AreEqual(2, stages.Count());

            Assert.AreEqual("Dagelijkse omstandigheden", stages.First().Label);
            Assert.AreEqual("Extreme omstandigheden", stages.Last().Label);

            for (var i = 0; i < stages.Count(); i++)
            {
                PersistableStage stage = stages.ElementAt(i);
                Assert.IsNotNull(stage.Id);
                Assert.AreEqual(calculationSettings.ElementAt(i).Id, stage.CalculationSettingsId);
                Assert.AreEqual(geometries.ElementAt(i).Id, stage.GeometryId);
                Assert.AreEqual(soilLayers.ElementAt(i).Id, stage.SoilLayersId);
                Assert.AreEqual(waternets.ElementAt(i).Id, stage.WaternetId);
                Assert.AreEqual(waternetCreatorSettings.ElementAt(i).Id, stage.WaternetCreatorSettingsId);
            }

            Assert.AreEqual(states.First().Id, stages.First().StateId);
            Assert.IsNull(stages.Last().StateId);
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableDitchCharacteristics"/> contains the correct data.
        /// </summary>
        /// <param name="surfaceLine">The surface line that contains the original data.</param>
        /// <param name="ditchCharacteristics">The <see cref="PersistableDitchCharacteristics"/> to assert.</param>
        /// <param name="isDitchPresent">Indicator whether a ditch is present.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="ditchCharacteristics"/>
        /// is not correct.</exception>
        public static void AssertDitchCharacteristics(MacroStabilityInwardsSurfaceLine surfaceLine, PersistableDitchCharacteristics ditchCharacteristics, bool isDitchPresent)
        {
            if (isDitchPresent)
            {
                AssertCharacteristicXPoint(surfaceLine.BottomDitchDikeSide, ditchCharacteristics.DitchBottomEmbankmentSide, surfaceLine);
                AssertCharacteristicXPoint(surfaceLine.BottomDitchPolderSide, ditchCharacteristics.DitchBottomLandSide, surfaceLine);
                AssertCharacteristicXPoint(surfaceLine.DitchDikeSide, ditchCharacteristics.DitchEmbankmentSide, surfaceLine);
                AssertCharacteristicXPoint(surfaceLine.DitchPolderSide, ditchCharacteristics.DitchLandSide, surfaceLine);
            }
            else
            {
                Assert.IsNaN(ditchCharacteristics.DitchBottomEmbankmentSide);
                Assert.IsNaN(ditchCharacteristics.DitchBottomLandSide);
                Assert.IsNaN(ditchCharacteristics.DitchEmbankmentSide);
                Assert.IsNaN(ditchCharacteristics.DitchLandSide);
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="PersistableEmbankmentCharacteristics"/> contains the correct data.
        /// </summary>
        /// <param name="surfaceLine">The surface line that contains the original data.</param>
        /// <param name="embankmentCharacteristics">The <see cref="PersistableEmbankmentCharacteristics"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when the data in <paramref name="embankmentCharacteristics"/>
        /// is not correct.</exception>
        public static void AssertEmbankmentCharacteristics(MacroStabilityInwardsSurfaceLine surfaceLine, PersistableEmbankmentCharacteristics embankmentCharacteristics)
        {
            AssertCharacteristicXPoint(surfaceLine.DikeToeAtPolder, embankmentCharacteristics.EmbankmentToeLandSide, surfaceLine);
            AssertCharacteristicXPoint(surfaceLine.DikeTopAtPolder, embankmentCharacteristics.EmbankmentTopLandSide, surfaceLine);
            AssertCharacteristicXPoint(surfaceLine.DikeTopAtRiver, embankmentCharacteristics.EmbankmentTopWaterSide, surfaceLine);
            AssertCharacteristicXPoint(surfaceLine.DikeToeAtRiver, embankmentCharacteristics.EmbankmentToeWaterSide, surfaceLine);
            if (surfaceLine.ShoulderBaseInside != null)
            {
                AssertCharacteristicXPoint(surfaceLine.ShoulderBaseInside, embankmentCharacteristics.ShoulderBaseLandSide, surfaceLine);
            }
            else
            {
                Assert.IsNaN(embankmentCharacteristics.ShoulderBaseLandSide);
            }
        }

        /// <summary>
        /// Gets the <see cref="PersistableEmbankmentSoilScenario"/> based on the <paramref name="dikeSoilScenario"/>.
        /// </summary>
        /// <param name="dikeSoilScenario">The <see cref="MacroStabilityInwardsDikeSoilScenario"/>
        /// to get the <see cref="PersistableEmbankmentSoilScenario"/> for.</param>
        /// <returns>The <see cref="PersistableEmbankmentSoilScenario"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="dikeSoilScenario"/>
        /// has an unsupported value.</exception>
        public static PersistableEmbankmentSoilScenario GetEmbankmentSoilScenario(MacroStabilityInwardsDikeSoilScenario dikeSoilScenario)
        {
            switch (dikeSoilScenario)
            {
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay:
                    return PersistableEmbankmentSoilScenario.ClayEmbankmentOnClay;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay:
                    return PersistableEmbankmentSoilScenario.SandEmbankmentOnClay;
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand:
                    return PersistableEmbankmentSoilScenario.ClayEmbankmentOnSand;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand:
                    return PersistableEmbankmentSoilScenario.SandEmbankmentOnSand;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void AssertPopStatePoints(IEnumerable<MacroStabilityInwardsSoilLayer2D> layers, IEnumerable<PersistableStatePoint> popStatePoints)
        {
            IEnumerable<MacroStabilityInwardsSoilLayer2D> layersWithPop = layers.Where(l => l.Data.UsePop
                                                                                            && l.Data.Pop.Mean != RoundedDouble.NaN
                                                                                            && l.Data.Pop.CoefficientOfVariation != RoundedDouble.NaN);
            Assert.AreEqual(layersWithPop.Count(), popStatePoints.Count());

            for (var j = 0; j < layersWithPop.Count(); j++)
            {
                MacroStabilityInwardsSoilLayer2D layerWithPop = layersWithPop.ElementAt(j);
                PersistableStatePoint statePoint = popStatePoints.ElementAt(j);

                Assert.IsNotNull(statePoint.Id);
                Assert.AreEqual($"POP - {layerWithPop.Data.MaterialName}", statePoint.Label);
                Assert.IsNotNull(statePoint.LayerId);
                Assert.IsTrue(statePoint.IsProbabilistic);

                Point2D interiorPoint = AdvancedMath2D.GetPolygonInteriorPoint(layerWithPop.OuterRing.Points, layerWithPop.NestedLayers.Select(nl => nl.OuterRing.Points));
                Assert.AreEqual(interiorPoint.X, statePoint.Point.X);
                Assert.AreEqual(interiorPoint.Y, statePoint.Point.Z);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(layerWithPop.Data).GetDesignValue(), statePoint.Stress.Pop);
                AssertStochasticParameter(layerWithPop.Data.Pop, statePoint.Stress.PopStochasticParameter);
            }
        }

        private static void AssertYieldStressStatePoints(IEnumerable<MacroStabilityInwardsSoilLayer2D> layers, IEnumerable<IMacroStabilityInwardsPreconsolidationStress> preconsolidationStresses,
                                                         IEnumerable<PersistableStatePoint> yieldStressStatePoints)
        {
            Assert.AreEqual(preconsolidationStresses.Count(), yieldStressStatePoints.Count());

            for (var j = 0; j < preconsolidationStresses.Count(); j++)
            {
                IMacroStabilityInwardsPreconsolidationStress preconsolidationStress = preconsolidationStresses.ElementAt(j);

                MacroStabilityInwardsSoilLayer2D layerWithPreconsolidationStress = layers.Single(l => AdvancedMath2D.PointInPolygon(
                                                                                                     preconsolidationStress.Location,
                                                                                                     l.OuterRing.Points,
                                                                                                     l.NestedLayers.Select(nl => nl.OuterRing.Points)));
                PersistableStatePoint yieldStressStatePoint = yieldStressStatePoints.ElementAt(j);

                Assert.IsNotNull(yieldStressStatePoint.Id);
                Assert.AreEqual($"Grensspanning - {layerWithPreconsolidationStress.Data.MaterialName}", yieldStressStatePoint.Label);
                Assert.IsNotNull(yieldStressStatePoint.LayerId);
                Assert.IsFalse(yieldStressStatePoint.IsProbabilistic);

                Assert.AreEqual(preconsolidationStress.Location.X, yieldStressStatePoint.Point.X);
                Assert.AreEqual(preconsolidationStress.Location.Y, yieldStressStatePoint.Point.Z);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(preconsolidationStress).GetDesignValue(), yieldStressStatePoint.Stress.YieldStress);
            }
        }

        private static void AssertCharacteristicXPoint(Point3D originalPoint, double actualX, MechanismSurfaceLineBase surfaceLine)
        {
            Assert.AreEqual(surfaceLine.GetLocalPointFromGeometry(originalPoint).X, actualX);
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

        private static void AssertOffsets(IMacroStabilityInwardsLocationInput locationInput, PersistableWaternetCreatorSettings waternetCreatorSettings)
        {
            Assert.AreEqual(locationInput.UseDefaultOffsets, waternetCreatorSettings.UseDefaultOffsets);
            Assert.AreEqual(locationInput.PhreaticLineOffsetBelowDikeTopAtPolder, waternetCreatorSettings.OffsetEmbankmentTopLandSide);
            Assert.AreEqual(locationInput.PhreaticLineOffsetBelowDikeTopAtRiver, waternetCreatorSettings.OffsetEmbankmentTopWaterSide);
            Assert.AreEqual(locationInput.PhreaticLineOffsetBelowDikeToeAtPolder, waternetCreatorSettings.OffsetEmbankmentToeLandSide);
            Assert.AreEqual(locationInput.PhreaticLineOffsetBelowShoulderBaseInside, waternetCreatorSettings.OffsetShoulderBaseLandSide);
        }

        private static bool IsDitchPresent(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return surfaceLine.DitchDikeSide != null
                   && surfaceLine.DitchPolderSide != null
                   && surfaceLine.BottomDitchDikeSide != null
                   && surfaceLine.BottomDitchPolderSide != null;
        }
    }
}