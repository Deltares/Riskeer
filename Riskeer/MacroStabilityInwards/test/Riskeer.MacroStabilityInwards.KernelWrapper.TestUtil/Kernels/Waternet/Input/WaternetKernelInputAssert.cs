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

using System.Linq;
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet.Input
{
    /// <summary>
    /// Class for asserting Waternet kernel input.
    /// </summary>
    public static class WaternetKernelInputAssert
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
            KernelInputAssert.AssertSoilProfile(expected.StabilityModel.ConstructionStages.Single().SoilProfile,
                                                actual.StabilityModel.ConstructionStages.Single().SoilProfile);

            CollectionAssert.AreEqual(expected.StabilityModel.Soils, actual.StabilityModel.Soils, new SoilComparer());

            PreConstructionStage expectedPreConstructionStage = expected.PreprocessingInput.PreConstructionStages.Single();
            PreConstructionStage actualPreConstructionStage = actual.PreprocessingInput.PreConstructionStages.Single();

            KernelInputAssert.AssertSurfaceLine(expectedPreConstructionStage.SurfaceLine, actualPreConstructionStage.SurfaceLine);
            Assert.AreEqual(expectedPreConstructionStage.CreateWaternet, actualPreConstructionStage.CreateWaternet);

            KernelInputAssert.AssertWaternetCreatorInput(expectedPreConstructionStage.WaternetCreatorInput, actualPreConstructionStage.WaternetCreatorInput);
        }
    }
}