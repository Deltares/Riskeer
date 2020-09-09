// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;
using Deltares.MacroStability.Data;
using NUnit.Framework;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input
{
    /// <summary>
    /// Class for asserting Uplift Van kernel input.
    /// </summary>
    public static class UpliftVanKernelInputAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="MacroStabilityInput"/>.</param>
        /// <param name="actual">The actual <see cref="MacroStabilityInput"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertMacroStabilityInput(MacroStabilityInput expected, MacroStabilityInput actual)
        {
            AssertStabilityInput(expected.StabilityModel, actual.StabilityModel);
            AssertPreprocessingInput(expected.PreprocessingInput, actual.PreprocessingInput);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SlipPlaneUpliftVan"/>.</param>
        /// <param name="actual">The actual <see cref="SlipPlaneUpliftVan"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertUpliftVanCalculationGrid(UpliftVanCalculationGrid expected, UpliftVanCalculationGrid actual)
        {
            AssertCalculationGrid(expected.LeftGrid, actual.LeftGrid);
            AssertCalculationGrid(expected.RightGrid, actual.RightGrid);
            CollectionAssert.AreEqual(expected.TangentLines, actual.TangentLines);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SlipPlaneConstraints"/>.</param>
        /// <param name="actual">The actual <see cref="SlipPlaneConstraints"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertSlipPlaneConstraints(SlipPlaneConstraints expected, SlipPlaneConstraints actual)
        {
            Assert.AreEqual(expected.SlipPlaneMinDepth, actual.SlipPlaneMinDepth);
            Assert.AreEqual(expected.SlipPlaneMinLength, actual.SlipPlaneMinLength);
            Assert.AreEqual(expected.XEntryMin, actual.XEntryMin);
            Assert.AreEqual(expected.XEntryMax, actual.XEntryMax);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="StabilityInput"/>.</param>
        /// <param name="actual">The actual <see cref="StabilityInput"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertStabilityInput(StabilityInput expected, StabilityInput actual)
        {
            Assert.AreEqual(expected.Orientation, actual.Orientation);
            Assert.AreEqual(expected.SearchAlgorithm, actual.SearchAlgorithm);
            Assert.AreEqual(expected.ModelOption, actual.ModelOption);

            AssertConstructionStages(expected.ConstructionStages, actual.ConstructionStages);

            CollectionAssert.AreEqual(expected.Soils, actual.Soils, new SoilComparer());
            Assert.AreEqual(expected.MoveGrid, actual.MoveGrid);
            Assert.AreEqual(expected.MaximumSliceWidth, actual.MaximumSliceWidth);

            AssertUpliftVanCalculationGrid(expected.UpliftVanCalculationGrid, actual.UpliftVanCalculationGrid);
            AssertSlipPlaneConstraints(expected.SlipPlaneConstraints, actual.SlipPlaneConstraints);
        }

        /// <summary>   
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected collection of <see cref="ConstructionStage"/>.</param>
        /// <param name="actual">The actual collection of <see cref="ConstructionStage"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertConstructionStages(ICollection<ConstructionStage> expected, ICollection<ConstructionStage> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                ConstructionStage expectedConstructionStage = expected.ElementAt(i);
                ConstructionStage actualConstructionStage = actual.ElementAt(i);

                KernelInputAssert.AssertSoilProfile(expectedConstructionStage.SoilProfile, actualConstructionStage.SoilProfile);
                Assert.AreEqual(expectedConstructionStage.Waternet, actualConstructionStage.Waternet);
                CollectionAssert.AreEqual(expectedConstructionStage.FixedSoilStresses, actualConstructionStage.FixedSoilStresses, new FixedSoilStressComparer());
                CollectionAssert.AreEqual(expectedConstructionStage.PreconsolidationStresses, actualConstructionStage.PreconsolidationStresses, new PreconsolidationStressComparer());

                AssertMultiplicationFactorsCPhiForUplift(expectedConstructionStage.MultiplicationFactorsCPhiForUplift.Single(),
                                                         actualConstructionStage.MultiplicationFactorsCPhiForUplift.Single());
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="MultiplicationFactorsCPhiForUplift"/>.</param>
        /// <param name="actual">The actual <see cref="MultiplicationFactorsCPhiForUplift"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertMultiplicationFactorsCPhiForUplift(MultiplicationFactorsCPhiForUplift expected,
                                                                     MultiplicationFactorsCPhiForUplift actual)
        {
            Assert.AreEqual(expected.MultiplicationFactor, actual.MultiplicationFactor);
            Assert.AreEqual(expected.UpliftFactor, actual.UpliftFactor);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SlipCircleGrid"/>.</param>
        /// <param name="actual">The actual <see cref="SlipCircleGrid"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertCalculationGrid(CalculationGrid expected, CalculationGrid actual)
        {
            Assert.AreEqual(expected.GridXLeft, actual.GridXLeft);
            Assert.AreEqual(expected.GridXRight, actual.GridXRight);
            Assert.AreEqual(expected.GridZTop, actual.GridZTop);
            Assert.AreEqual(expected.GridZBottom, actual.GridZBottom);
            Assert.AreEqual(expected.GridXNumber, actual.GridXNumber);
            Assert.AreEqual(expected.GridZNumber, actual.GridZNumber);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="PreprocessingInput"/>.</param>
        /// <param name="actual">The actual <see cref="PreprocessingInput"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertPreprocessingInput(PreprocessingInput expected, PreprocessingInput actual)
        {
            SearchAreaConditions expectedSearchAreaConditions = expected.SearchAreaConditions;
            SearchAreaConditions actualSearchAreaConditions = actual.SearchAreaConditions;

            Assert.AreEqual(expectedSearchAreaConditions.MaxSpacingBetweenBoundaries, actualSearchAreaConditions.MaxSpacingBetweenBoundaries);
            Assert.AreEqual(expectedSearchAreaConditions.OnlyAbovePleistoceen, actualSearchAreaConditions.OnlyAbovePleistoceen);
            Assert.AreEqual(expectedSearchAreaConditions.AutoSearchArea, actualSearchAreaConditions.AutoSearchArea);
            Assert.AreEqual(expectedSearchAreaConditions.AutoTangentLines, actualSearchAreaConditions.AutoTangentLines);
            Assert.AreEqual(expectedSearchAreaConditions.AutomaticForbiddenZones, actualSearchAreaConditions.AutomaticForbiddenZones);
            Assert.AreEqual(expectedSearchAreaConditions.TangentLineNumber, actualSearchAreaConditions.TangentLineNumber);
            Assert.AreEqual(expectedSearchAreaConditions.TangentLineZTop, actualSearchAreaConditions.TangentLineZTop);
            Assert.AreEqual(expectedSearchAreaConditions.TangentLineZBottom, actualSearchAreaConditions.TangentLineZBottom);

            AssertPreConstructionStages(expected.PreConstructionStages, actual.PreConstructionStages);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected collection of <see cref="PreConstructionStage"/>.</param>
        /// <param name="actual">The actual collection of <see cref="PreConstructionStage"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertPreConstructionStages(ICollection<PreConstructionStage> expected, ICollection<PreConstructionStage> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                PreConstructionStage expectedPreConstructionStage = expected.ElementAt(i);
                PreConstructionStage actualPreConstructionStage = actual.ElementAt(i);

                Assert.AreEqual(expectedPreConstructionStage.WaternetCreationMode, actualPreConstructionStage.WaternetCreationMode);
                KernelInputAssert.AssertSurfaceLine(expectedPreConstructionStage.SurfaceLine, actualPreConstructionStage.SurfaceLine);
            }
        }
    }
}