// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Piping.Data.TestUtil
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
            Assert.AreEqual(original.UpliftZValue, clone.UpliftZValue);
            Assert.AreEqual(original.UpliftFactorOfSafety, clone.UpliftFactorOfSafety);
            Assert.AreEqual(original.HeaveZValue, clone.HeaveZValue);
            Assert.AreEqual(original.HeaveFactorOfSafety, clone.HeaveFactorOfSafety);
            Assert.AreEqual(original.SellmeijerZValue, clone.SellmeijerZValue);
            Assert.AreEqual(original.UpliftEffectiveStress, clone.UpliftEffectiveStress);
            Assert.AreEqual(original.HeaveGradient, clone.HeaveGradient);
            Assert.AreEqual(original.SellmeijerCreepCoefficient, clone.SellmeijerCreepCoefficient);
            Assert.AreEqual(original.SellmeijerCriticalFall, clone.SellmeijerCriticalFall);
            Assert.AreEqual(original.SellmeijerReducedFall, clone.SellmeijerReducedFall);
            Assert.AreEqual(original.SellmeijerFactorOfSafety, clone.SellmeijerFactorOfSafety);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(PipingSemiProbabilisticOutput original, PipingSemiProbabilisticOutput clone)
        {
            Assert.AreEqual(original.UpliftProbability, clone.UpliftProbability);
            Assert.AreEqual(original.UpliftReliability, clone.UpliftReliability);
            Assert.AreEqual(original.UpliftFactorOfSafety, clone.UpliftFactorOfSafety);
            Assert.AreEqual(original.HeaveProbability, clone.HeaveProbability);
            Assert.AreEqual(original.HeaveReliability, clone.HeaveReliability);
            Assert.AreEqual(original.HeaveFactorOfSafety, clone.HeaveFactorOfSafety);
            Assert.AreEqual(original.SellmeijerFactorOfSafety, clone.SellmeijerFactorOfSafety);
            Assert.AreEqual(original.SellmeijerProbability, clone.SellmeijerProbability);
            Assert.AreEqual(original.SellmeijerReliability, clone.SellmeijerReliability);
            Assert.AreEqual(original.RequiredProbability, clone.RequiredProbability);
            Assert.AreEqual(original.RequiredReliability, clone.RequiredReliability);
            Assert.AreEqual(original.PipingProbability, clone.PipingProbability);
            Assert.AreEqual(original.PipingReliability, clone.PipingReliability);
            Assert.AreEqual(original.PipingFactorOfSafety, clone.PipingFactorOfSafety);
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
            Assert.AreEqual(original.AssessmentLevel, clone.AssessmentLevel);
            Assert.AreEqual(original.UseAssessmentLevelManualInput, clone.UseAssessmentLevelManualInput);
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
        public static void AreClones(PipingCalculation original, PipingCalculation clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            CoreCloneAssert.AreObjectClones(original.Comments, clone.Comments, CommonCloneAssert.AreClones);
            CoreCloneAssert.AreObjectClones(original.InputParameters, clone.InputParameters, AreClones);
            CoreCloneAssert.AreObjectClones(original.Output, clone.Output, AreClones);
            CoreCloneAssert.AreObjectClones(original.SemiProbabilisticOutput, clone.SemiProbabilisticOutput, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(PipingCalculationScenario original, PipingCalculationScenario clone)
        {
            AreClones((PipingCalculation)original, clone);
            Assert.AreEqual(original.Contribution, clone.Contribution);
            Assert.AreEqual(original.IsRelevant, clone.IsRelevant);
        }
    }
}