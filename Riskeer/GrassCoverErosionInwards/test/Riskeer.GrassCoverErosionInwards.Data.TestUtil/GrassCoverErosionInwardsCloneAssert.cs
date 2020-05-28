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

using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class GrassCoverErosionInwardsCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(OvertoppingOutput original, OvertoppingOutput clone)
        {
            Assert.AreEqual(original.WaveHeight, clone.WaveHeight);
            Assert.AreEqual(original.IsOvertoppingDominant, clone.IsOvertoppingDominant);
            Assert.AreEqual(original.Reliability, clone.Reliability);
            CoreCloneAssert.AreObjectClones(original.GeneralResult, clone.GeneralResult, CommonCloneAssert.AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(HydraulicLoadsOutput original, HydraulicLoadsOutput clone)
        {
            Assert.AreEqual(original.TargetProbability, clone.TargetProbability);
            Assert.AreEqual(original.TargetReliability, clone.TargetReliability);
            Assert.AreEqual(original.CalculatedProbability, clone.CalculatedProbability);
            Assert.AreEqual(original.CalculatedReliability, clone.CalculatedReliability);
            Assert.AreEqual(original.CalculationConvergence, clone.CalculationConvergence);
            CoreCloneAssert.AreObjectClones(original.GeneralResult, clone.GeneralResult, CommonCloneAssert.AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(DikeHeightOutput original, DikeHeightOutput clone)
        {
            AreClones((HydraulicLoadsOutput) original, clone);
            Assert.AreEqual(original.DikeHeight, clone.DikeHeight);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(OvertoppingRateOutput original, OvertoppingRateOutput clone)
        {
            AreClones((HydraulicLoadsOutput) original, clone);
            Assert.AreEqual(original.OvertoppingRate, clone.OvertoppingRate);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(GrassCoverErosionInwardsInput original, GrassCoverErosionInwardsInput clone)
        {
            Assert.AreSame(original.DikeProfile, clone.DikeProfile);
            Assert.AreEqual(original.Orientation, clone.Orientation);
            Assert.AreEqual(original.DikeHeight, clone.DikeHeight);
            CoreCloneAssert.AreObjectClones(original.CriticalFlowRate, clone.CriticalFlowRate, DistributionAssert.AreEqual);
            Assert.AreSame(original.HydraulicBoundaryLocation, clone.HydraulicBoundaryLocation);
            Assert.AreEqual(original.DikeHeightCalculationType, clone.DikeHeightCalculationType);
            Assert.AreEqual(original.OvertoppingRateCalculationType, clone.OvertoppingRateCalculationType);
            Assert.AreEqual(original.ShouldDikeHeightIllustrationPointsBeCalculated, clone.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.AreEqual(original.ShouldOvertoppingRateIllustrationPointsBeCalculated, clone.ShouldOvertoppingRateIllustrationPointsBeCalculated);
            Assert.AreEqual(original.ShouldOvertoppingOutputIllustrationPointsBeCalculated, clone.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(original.UseBreakWater, clone.UseBreakWater);
            CoreCloneAssert.AreObjectClones(original.BreakWater, clone.BreakWater, CommonCloneAssert.AreClones);
            Assert.AreEqual(original.UseForeshore, clone.UseForeshore);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(GrassCoverErosionInwardsOutput original, GrassCoverErosionInwardsOutput clone)
        {
            CoreCloneAssert.AreObjectClones(original.OvertoppingOutput, clone.OvertoppingOutput, AreClones);
            CoreCloneAssert.AreObjectClones(original.DikeHeightOutput, clone.DikeHeightOutput, AreClones);
            CoreCloneAssert.AreObjectClones(original.OvertoppingRateOutput, clone.OvertoppingRateOutput, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(GrassCoverErosionInwardsCalculation original, GrassCoverErosionInwardsCalculation clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            CoreCloneAssert.AreObjectClones(original.Comments, clone.Comments, CommonCloneAssert.AreClones);
            CoreCloneAssert.AreObjectClones(original.InputParameters, clone.InputParameters, AreClones);
            CoreCloneAssert.AreObjectClones(original.Output, clone.Output, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(GrassCoverErosionInwardsCalculationScenario original,
                                     GrassCoverErosionInwardsCalculationScenario clone)
        {
            Assert.AreEqual(original.Contribution, clone.Contribution);
            Assert.AreEqual(original.IsRelevant, clone.IsRelevant);
            AreClones((GrassCoverErosionInwardsCalculation) original, clone);
        }
    }
}