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

using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using BaseCloneAssert = Core.Common.Data.TestUtil.CloneAssert;
using CommonCloneAssert = Ringtoets.Common.Data.TestUtil.CloneAssert;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class CloneAssert
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
            BaseCloneAssert.AreClones(original.ProbabilityAssessmentOutput, clone.ProbabilityAssessmentOutput, CommonCloneAssert.AreClones);
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

        public static void AreClones(GrassCoverErosionInwardsInput original, GrassCoverErosionInwardsInput clone)
        {
            Assert.AreSame(original.DikeProfile, clone.DikeProfile);
            Assert.AreEqual(original.Orientation, clone.Orientation);
            Assert.AreEqual(original.DikeHeight, clone.DikeHeight);
            BaseCloneAssert.AreClones(original.CriticalFlowRate, clone.CriticalFlowRate, DistributionAssert.AreEqual);
            Assert.AreSame(original.HydraulicBoundaryLocation, clone.HydraulicBoundaryLocation);
            Assert.AreEqual(original.DikeHeightCalculationType, clone.DikeHeightCalculationType);
            Assert.AreEqual(original.OvertoppingRateCalculationType, clone.OvertoppingRateCalculationType);
            Assert.AreEqual(original.ShouldDikeHeightIllustrationPointsBeCalculated, clone.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.AreEqual(original.ShouldOvertoppingRateIllustrationPointsBeCalculated, clone.ShouldOvertoppingRateIllustrationPointsBeCalculated);
            Assert.AreEqual(original.ShouldOvertoppingOutputIllustrationPointsBeCalculated, clone.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(original.UseBreakWater, clone.UseBreakWater);
            BaseCloneAssert.AreClones(original.BreakWater, clone.BreakWater, CommonCloneAssert.AreClones);
            Assert.AreEqual(original.UseForeshore, clone.UseForeshore);
        }

        public static void AreClones(GrassCoverErosionInwardsOutput o, GrassCoverErosionInwardsOutput c)
        {
            BaseCloneAssert.AreClones(o.OvertoppingOutput, c.OvertoppingOutput, AreClones);
            BaseCloneAssert.AreClones(o.DikeHeightOutput, c.DikeHeightOutput, AreClones);
            BaseCloneAssert.AreClones(o.OvertoppingRateOutput, c.OvertoppingRateOutput, AreClones);
        }
    }
}