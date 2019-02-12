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

namespace Riskeer.StabilityStoneCover.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class StabilityStoneCoverCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(StabilityStoneCoverWaveConditionsCalculation original, StabilityStoneCoverWaveConditionsCalculation clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            CoreCloneAssert.AreObjectClones(original.Comments, clone.Comments, CommonCloneAssert.AreClones);
            CoreCloneAssert.AreObjectClones(original.InputParameters, clone.InputParameters, WaveConditionsCloneAssert.AreClones);
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
        public static void AreClones(StabilityStoneCoverWaveConditionsOutput original, StabilityStoneCoverWaveConditionsOutput clone)
        {
            CoreCloneAssert.AreEnumerationClones(original.BlocksOutput, clone.BlocksOutput, WaveConditionsCloneAssert.AreClones);
            CoreCloneAssert.AreEnumerationClones(original.ColumnsOutput, clone.ColumnsOutput, WaveConditionsCloneAssert.AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(StabilityStoneCoverWaveConditionsInput original, StabilityStoneCoverWaveConditionsInput clone)
        {
            Assert.AreEqual(original.CalculationType, clone.CalculationType);
            WaveConditionsCloneAssert.AreClones(original, clone);
        }
    }
}