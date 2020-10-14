﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Core.Common.Data.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class PipingCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(PipingOutput original, PipingOutput clone)
        {
            Assert.AreEqual(original.UpliftFactorOfSafety, clone.UpliftFactorOfSafety);
            Assert.AreEqual(original.HeaveFactorOfSafety, clone.HeaveFactorOfSafety);
            Assert.AreEqual(original.SellmeijerFactorOfSafety, clone.SellmeijerFactorOfSafety);
            Assert.AreEqual(original.UpliftEffectiveStress, clone.UpliftEffectiveStress);
            Assert.AreEqual(original.HeaveGradient, clone.HeaveGradient);
            Assert.AreEqual(original.SellmeijerCreepCoefficient, clone.SellmeijerCreepCoefficient);
            Assert.AreEqual(original.SellmeijerCriticalFall, clone.SellmeijerCriticalFall);
            Assert.AreEqual(original.SellmeijerReducedFall, clone.SellmeijerReducedFall);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(PipingInput original, PipingInput clone)
        {
            Assert.AreSame(TypeUtils.GetField<GeneralPipingInput>(original, "generalInputParameters"), TypeUtils.GetField<GeneralPipingInput>(clone, "generalInputParameters"));
            Assert.AreEqual(original.EntryPointL, clone.EntryPointL);
            Assert.AreEqual(original.ExitPointL, clone.ExitPointL);
            CoreCloneAssert.AreObjectClones(original.PhreaticLevelExit, clone.PhreaticLevelExit, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.DampingFactorExit, clone.DampingFactorExit, DistributionAssert.AreEqual);
            Assert.AreSame(original.SurfaceLine, clone.SurfaceLine);
            Assert.AreSame(original.StochasticSoilModel, clone.StochasticSoilModel);
            Assert.AreSame(original.StochasticSoilProfile, clone.StochasticSoilProfile);
            Assert.AreSame(original.HydraulicBoundaryLocation, clone.HydraulicBoundaryLocation);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(SemiProbabilisticPipingInput original, SemiProbabilisticPipingInput clone)
        {
            AreClones((PipingInput) original, clone);
            Assert.AreEqual(original.AssessmentLevel, clone.AssessmentLevel);
            Assert.AreEqual(original.UseAssessmentLevelManualInput, clone.UseAssessmentLevelManualInput);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(IPipingCalculation<PipingInput, PipingOutput> original, IPipingCalculation<PipingInput, PipingOutput> clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            CoreCloneAssert.AreObjectClones(original.Comments, clone.Comments, CommonCloneAssert.AreClones);
            CoreCloneAssert.AreObjectClones(original.InputParameters, clone.InputParameters, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(SemiProbabilisticPipingCalculationScenario original, SemiProbabilisticPipingCalculationScenario clone)
        {
            AreClones((IPipingCalculation<PipingInput, PipingOutput>) original, clone);
            Assert.AreEqual(original.Contribution, clone.Contribution);
            Assert.AreEqual(original.IsRelevant, clone.IsRelevant);
            CoreCloneAssert.AreObjectClones(original.Output, clone.Output, AreClones);
        }
    }
}