﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;
using Deltares.MacroStability.CSharpWrapper.Output;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using SoilProfile = Deltares.MacroStability.CSharpWrapper.Input.SoilProfile;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class MacroStabilityInputCreatorTest
    {
        #region CreateUpliftVan

        [Test]
        public void CreateUpliftVan_UpliftVanInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateUpliftVan(
                null, new List<Soil>(), new Dictionary<SoilLayer, LayerWithSoil>(),
                new SurfaceLine(), new SoilProfile(), new Waternet(), new Waternet());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("upliftVanInput", exception.ParamName);
        }

        [Test]
        public void CreateUpliftVan_SoilsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(),
                null, new Dictionary<SoilLayer, LayerWithSoil>(),
                new SurfaceLine(), new SoilProfile(), new Waternet(), new Waternet());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soils", exception.ParamName);
        }

        [Test]
        public void CreateUpliftVan_LayerLookupNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), new List<Soil>(),
                null, new SurfaceLine(), new SoilProfile(), new Waternet(), new Waternet());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("layerLookup", exception.ParamName);
        }

        [Test]
        public void CreateUpliftVan_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), new List<Soil>(),
                new Dictionary<SoilLayer, LayerWithSoil>(), null, new SoilProfile(),
                new Waternet(), new Waternet());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateUpliftVan_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), new List<Soil>(),
                new Dictionary<SoilLayer, LayerWithSoil>(), new SurfaceLine(),
                null, new Waternet(), new Waternet());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void CreateUpliftVan_DailyWaternetNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), new List<Soil>(),
                new Dictionary<SoilLayer, LayerWithSoil>(), new SurfaceLine(),
                new SoilProfile(), null, new Waternet());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dailyWaternet", exception.ParamName);
        }

        [Test]
        public void CreateUpliftVan_ExtremeWaternetNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), new List<Soil>(),
                new Dictionary<SoilLayer, LayerWithSoil>(), new SurfaceLine(),
                new SoilProfile(), new Waternet(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("extremeWaternet", exception.ParamName);
        }

        [Test]
        public void CreateUpliftVan_ValidData_ReturnMacroStabilityInput()
        {
            // Setup
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> layerLookup);
            List<Soil> soils = layersWithSoil.Select(lws => lws.Soil).ToList();

            SurfaceLine surfaceLine = SurfaceLineCreator.Create(input.SurfaceLine);
            SoilProfile soilProfile = SoilProfileCreator.Create(layersWithSoil);

            var dailyWaternet = new Waternet();
            var extremeWaternet = new Waternet();

            // Call
            MacroStabilityInput macroStabilityInput = MacroStabilityInputCreator.CreateUpliftVan(
                input, soils, layerLookup, surfaceLine, soilProfile, dailyWaternet, extremeWaternet);

            // Assert
            StabilityInput stabilityModel = macroStabilityInput.StabilityModel;

            Assert.AreEqual(Orientation.Inwards, stabilityModel.Orientation);
            Assert.AreEqual(SearchAlgorithm.Grid, stabilityModel.SearchAlgorithm);
            Assert.AreEqual(StabilityModelOptionType.UpliftVan, stabilityModel.ModelOption);

            CollectionAssert.AreEqual(soils, stabilityModel.Soils, new SoilComparer());
            Assert.AreEqual(input.MoveGrid, stabilityModel.MoveGrid);
            Assert.AreEqual(input.MaximumSliceWidth, stabilityModel.MaximumSliceWidth);

            UpliftVanKernelInputAssert.AssertUpliftVanCalculationGrid(
                UpliftVanCalculationGridCreator.Create(input.SlipPlane), stabilityModel.UpliftVanCalculationGrid);

            UpliftVanKernelInputAssert.AssertSlipPlaneConstraints(
                SlipPlaneConstraintsCreator.Create(input.SlipPlaneConstraints), stabilityModel.SlipPlaneConstraints);

            AssertConstructionStages(input, stabilityModel, soilProfile, dailyWaternet, extremeWaternet, layerLookup);

            Assert.AreEqual(input.SlipPlane.GridNumberOfRefinements, stabilityModel.NumberOfRefinementsGrid);
            Assert.AreEqual(input.SlipPlane.TangentLineNumberOfRefinements, stabilityModel.NumberOfRefinementsTangentLines);

            SearchAreaConditions searchAreaConditions = macroStabilityInput.PreprocessingInput.SearchAreaConditions;
            Assert.AreEqual(0.8, searchAreaConditions.MaxSpacingBetweenBoundaries);
            Assert.IsTrue(searchAreaConditions.OnlyAbovePleistoceen);
            Assert.AreEqual(input.SlipPlane.GridAutomaticDetermined, searchAreaConditions.AutoSearchArea);
            Assert.AreEqual(input.SlipPlane.TangentLinesAutomaticAtBoundaries, searchAreaConditions.AutoTangentLines);
            Assert.AreEqual(1, searchAreaConditions.TangentLineNumber);
            Assert.AreEqual(0, searchAreaConditions.TangentLineZTop);
            Assert.AreEqual(0, searchAreaConditions.TangentLineZBottom);
            Assert.AreEqual(input.SlipPlaneConstraints.AutomaticForbiddenZones, searchAreaConditions.AutomaticForbiddenZones);

            Assert.AreEqual(2, macroStabilityInput.PreprocessingInput.PreConstructionStages.Count);

            foreach (PreConstructionStage preConstructionStage in macroStabilityInput.PreprocessingInput.PreConstructionStages)
            {
                Assert.IsFalse(preConstructionStage.CreateWaternet);
                Assert.AreSame(surfaceLine, preConstructionStage.SurfaceLine);
                Assert.IsNull(preConstructionStage.WaternetCreatorInput); // Not needed as Waternet is already calculated
            }

            AssertIrrelevantValues(stabilityModel, searchAreaConditions);
        }

        [Test]
        public void CreateUpliftVan_ValidDataWithManualTangentLines_ReturnMacroStabilityInput()
        {
            // Setup
            var random = new Random(21);
            double tangentZTop = random.NextDouble();
            double tangentZBottom = random.NextDouble();
            int tangentLineNumber = random.Next();

            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create(tangentZTop, tangentZBottom, tangentLineNumber);

            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> layerLookup);
            List<Soil> soils = layersWithSoil.Select(lws => lws.Soil).ToList();

            SurfaceLine surfaceLine = SurfaceLineCreator.Create(input.SurfaceLine);
            SoilProfile soilProfile = SoilProfileCreator.Create(layersWithSoil);

            var dailyWaternet = new Waternet();
            var extremeWaternet = new Waternet();

            // Call
            MacroStabilityInput macroStabilityInput = MacroStabilityInputCreator.CreateUpliftVan(
                input, soils, layerLookup, surfaceLine, soilProfile, dailyWaternet, extremeWaternet);

            // Assert
            SearchAreaConditions searchAreaConditions = macroStabilityInput.PreprocessingInput.SearchAreaConditions;
            Assert.AreEqual(input.SlipPlane.TangentLinesAutomaticAtBoundaries, searchAreaConditions.AutoTangentLines);
            Assert.AreEqual(input.SlipPlane.TangentLineNumber, searchAreaConditions.TangentLineNumber);
            Assert.AreEqual(input.SlipPlane.TangentZTop, searchAreaConditions.TangentLineZTop);
            Assert.AreEqual(input.SlipPlane.TangentZBottom, searchAreaConditions.TangentLineZBottom);
        }

        private static void AssertConstructionStages(
            UpliftVanCalculatorInput input, StabilityInput stabilityModel, SoilProfile soilProfile,
            Waternet dailyWaternet, Waternet extremeWaternet, IDictionary<SoilLayer, LayerWithSoil> layerLookup)
        {
            Assert.AreEqual(2, stabilityModel.ConstructionStages.Count);

            ConstructionStage dailyConstructionStage = stabilityModel.ConstructionStages.ElementAt(0);
            Assert.AreSame(soilProfile, dailyConstructionStage.SoilProfile);
            Assert.AreSame(dailyWaternet, dailyConstructionStage.Waternet);
            CollectionAssert.AreEqual(FixedSoilStressCreator.Create(layerLookup),
                                      dailyConstructionStage.FixedSoilStresses, new FixedSoilStressComparer());
            CollectionAssert.AreEqual(PreconsolidationStressCreator.Create(input.SoilProfile.PreconsolidationStresses),
                                      dailyConstructionStage.PreconsolidationStresses, new PreconsolidationStressComparer());
            AssertMultiplicationFactors(dailyConstructionStage.MultiplicationFactorsCPhiForUplift.Single());
            AssertIrrelevantValues(dailyConstructionStage);

            ConstructionStage extremeConstructionStage = stabilityModel.ConstructionStages.ElementAt(1);
            Assert.AreSame(soilProfile, extremeConstructionStage.SoilProfile);
            Assert.AreSame(extremeWaternet, extremeConstructionStage.Waternet);
            CollectionAssert.IsEmpty(extremeConstructionStage.FixedSoilStresses);
            CollectionAssert.IsEmpty(extremeConstructionStage.PreconsolidationStresses);
            AssertMultiplicationFactors(extremeConstructionStage.MultiplicationFactorsCPhiForUplift.Single());
            AssertIrrelevantValues(extremeConstructionStage);
        }

        private static void AssertMultiplicationFactors(MultiplicationFactorsCPhiForUplift multiplicationFactors)
        {
            Assert.AreEqual(0.0, multiplicationFactors.MultiplicationFactor);
            Assert.AreEqual(1.2, multiplicationFactors.UpliftFactor);
        }

        private static void AssertIrrelevantValues(ConstructionStage constructionStage)
        {
            CollectionAssert.IsEmpty(constructionStage.ConsolidationValues); // Irrelevant
            Assert.IsNotNull(constructionStage.Earthquake); // Irrelevant
            CollectionAssert.IsEmpty(constructionStage.ForbiddenLines); // Irrelevant
            CollectionAssert.IsEmpty(constructionStage.Geotextiles); // Irrelevant
            CollectionAssert.IsEmpty(constructionStage.LineLoads); // Irrelevant
            CollectionAssert.IsEmpty(constructionStage.Nails); // Irrelevant
            CollectionAssert.IsEmpty(constructionStage.TreesOnSlope); // Irrelevant
            CollectionAssert.IsEmpty(constructionStage.UniformLoads); // Irrelevant
            Assert.IsNotNull(constructionStage.YieldStressField); // Irrelevant
        }

        private static void AssertIrrelevantValues(StabilityInput stabilityModel, SearchAreaConditions searchAreaConditions)
        {
            Assert.AreEqual(50, stabilityModel.MaxGridMoves); // Irrelevant

            Assert.IsNotNull(stabilityModel.BishopCalculationCircle); // Irrelevant - Only for Bishop

            Assert.IsNotNull(stabilityModel.BeeswarmAlgorithmOptions); // Irrelevant - Only for Beeswarm

            Assert.IsNotNull(stabilityModel.GeneticAlgorithmOptions); // Irrelevant - Only for Genetic Algorithm
            Assert.IsFalse(searchAreaConditions.AutoGeneticAlgorithmOptions); // Irrelevant - Only for Genetic Algorithm

            Assert.IsNull(stabilityModel.LevenbergMarquardtOptions); // Irrelevant - Only for Levenberg Marquardt

            Assert.AreEqual(0, stabilityModel.MaxAllowedAngleBetweenSlices); // Irrelevant - Only for Spencer
            Assert.AreEqual(0, stabilityModel.RequiredForcePointsInSlices); // Irrelevant - Only for Spencer
            Assert.IsNotNull(stabilityModel.SpencerSlipPlanes); // Irrelevant - Only for Spencer
            Assert.AreEqual(2, stabilityModel.TraversalGridPoints); // Irrelevant - Only for Spencer
            Assert.AreEqual(0, stabilityModel.TraversalRefinements); // Irrelevant - Only for Spencer
            Assert.AreEqual(SearchAreaConditionsSlipPlanePosition.High, searchAreaConditions.SlipPlanePosition); // Irrelevant - Only for Spencer
        }

        #endregion

        #region CreateDailyWaternetForUpliftVan

        [Test]
        public void CreateDailyWaternetForUpliftVan_UpliftVanInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateDailyWaternetForUpliftVan(
                null, new List<Soil>(), new SurfaceLine(), new SoilProfile());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("upliftVanInput", exception.ParamName);
        }

        [Test]
        public void CreateDailyWaternetForUpliftVan_SoilsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateDailyWaternetForUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), null, new SurfaceLine(), new SoilProfile());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soils", exception.ParamName);
        }

        [Test]
        public void CreateDailyWaternetForUpliftVan_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateDailyWaternetForUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), new List<Soil>(), null, new SoilProfile());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateDailyWaternetForUpliftVan_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateDailyWaternetForUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), new List<Soil>(), new SurfaceLine(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void CreateDailyWaternetForUpliftVan_ValidData_ReturnMacroStabilityInput()
        {
            // Setup
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> _);
            List<Soil> soils = layersWithSoil.Select(lws => lws.Soil).ToList();

            SurfaceLine surfaceLine = SurfaceLineCreator.Create(input.SurfaceLine);
            SoilProfile soilProfile = SoilProfileCreator.Create(layersWithSoil);

            // Call
            MacroStabilityInput macroStabilityInput = MacroStabilityInputCreator.CreateDailyWaternetForUpliftVan(
                input, soils, surfaceLine, soilProfile);

            // Assert
            CollectionAssert.AreEqual(soils, macroStabilityInput.StabilityModel.Soils, new SoilComparer());
            Assert.AreSame(soilProfile, macroStabilityInput.StabilityModel.ConstructionStages.Single().SoilProfile);

            PreConstructionStage preConstructionStage = macroStabilityInput.PreprocessingInput.PreConstructionStages.Single();
            Assert.AreSame(surfaceLine, preConstructionStage.SurfaceLine);
            Assert.IsTrue(preConstructionStage.CreateWaternet);
            KernelInputAssert.AssertWaternetCreatorInput(UpliftVanWaternetCreatorInputCreator.CreateDaily(input), preConstructionStage.WaternetCreatorInput);
        }

        #endregion

        #region CreateExtremeWaternetForUpliftVan

        [Test]
        public void CreateExtremeWaternetForUpliftVan_UpliftVanInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateExtremeWaternetForUpliftVan(
                null, new List<Soil>(), new SurfaceLine(), new SoilProfile());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("upliftVanInput", exception.ParamName);
        }

        [Test]
        public void CreateExtremeWaternetForUpliftVan_SoilsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateExtremeWaternetForUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), null, new SurfaceLine(), new SoilProfile());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soils", exception.ParamName);
        }

        [Test]
        public void CreateExtremeWaternetForUpliftVan_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateExtremeWaternetForUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), new List<Soil>(), null, new SoilProfile());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateExtremeWaternetForUpliftVan_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateExtremeWaternetForUpliftVan(
                UpliftVanCalculatorInputTestFactory.Create(), new List<Soil>(), new SurfaceLine(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void CreateExtremeWaternetForUpliftVan_ValidData_ReturnMacroStabilityInput()
        {
            // Setup
            UpliftVanCalculatorInput input = UpliftVanCalculatorInputTestFactory.Create();

            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> _);
            List<Soil> soils = layersWithSoil.Select(lws => lws.Soil).ToList();

            SurfaceLine surfaceLine = SurfaceLineCreator.Create(input.SurfaceLine);
            SoilProfile soilProfile = SoilProfileCreator.Create(layersWithSoil);

            // Call
            MacroStabilityInput macroStabilityInput = MacroStabilityInputCreator.CreateExtremeWaternetForUpliftVan(
                input, soils, surfaceLine, soilProfile);

            // Assert
            CollectionAssert.AreEqual(soils, macroStabilityInput.StabilityModel.Soils, new SoilComparer());
            Assert.AreSame(soilProfile, macroStabilityInput.StabilityModel.ConstructionStages.Single().SoilProfile);

            PreConstructionStage preConstructionStage = macroStabilityInput.PreprocessingInput.PreConstructionStages.Single();
            Assert.AreSame(surfaceLine, preConstructionStage.SurfaceLine);
            Assert.IsTrue(preConstructionStage.CreateWaternet);
            KernelInputAssert.AssertWaternetCreatorInput(UpliftVanWaternetCreatorInputCreator.CreateExtreme(input), preConstructionStage.WaternetCreatorInput);
        }

        #endregion

        #region CreateWaternet

        [Test]
        public void CreateWaternet_WaternetInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInputCreator.CreateWaternet(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waternetInput", exception.ParamName);
        }

        [Test]
        public void CreateWaternet_ValidData_ReturnMacroStabilityInput()
        {
            // Setup
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();

            // Call
            MacroStabilityInput macroStabilityInput = MacroStabilityInputCreator.CreateWaternet(input);

            // Assert
            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> _);
            CollectionAssert.AreEqual(layersWithSoil.Select(lws => lws.Soil).ToList(), macroStabilityInput.StabilityModel.Soils, new SoilComparer());
            KernelInputAssert.AssertSoilProfile(SoilProfileCreator.Create(layersWithSoil),
                                                macroStabilityInput.StabilityModel.ConstructionStages.Single().SoilProfile);

            PreConstructionStage preConstructionStage = macroStabilityInput.PreprocessingInput.PreConstructionStages.Single();
            KernelInputAssert.AssertSurfaceLine(SurfaceLineCreator.Create(input.SurfaceLine), preConstructionStage.SurfaceLine);
            Assert.IsTrue(preConstructionStage.CreateWaternet);
            KernelInputAssert.AssertWaternetCreatorInput(WaternetCreatorInputCreator.Create(input), preConstructionStage.WaternetCreatorInput);
        }

        #endregion
    }
}