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
using Riskeer.Revetment.Data.TestUtil;

namespace Riskeer.GrassCoverErosionOutwards.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class GrassCoverErosionOutwardsCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(GrassCoverErosionOutwardsWaveConditionsCalculation original, GrassCoverErosionOutwardsWaveConditionsCalculation clone)
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
        public static void AreClones(GrassCoverErosionOutwardsWaveConditionsOutput original, GrassCoverErosionOutwardsWaveConditionsOutput clone)
        {
            CoreCloneAssert.AreEnumerationClones(original.WaveRunUpOutput, clone.WaveRunUpOutput, WaveConditionsCloneAssert.AreClones);
            CoreCloneAssert.AreEnumerationClones(original.WaveImpactOutput, clone.WaveImpactOutput, WaveConditionsCloneAssert.AreClones);
            CoreCloneAssert.AreEnumerationClones(original.TailorMadeWaveImpactOutput, clone.TailorMadeWaveImpactOutput, WaveConditionsCloneAssert.AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(GrassCoverErosionOutwardsWaveConditionsInput original, GrassCoverErosionOutwardsWaveConditionsInput clone)
        {
            Assert.AreEqual(original.CalculationType, clone.CalculationType);
            WaveConditionsCloneAssert.AreClones(original, clone);
        }
    }
}