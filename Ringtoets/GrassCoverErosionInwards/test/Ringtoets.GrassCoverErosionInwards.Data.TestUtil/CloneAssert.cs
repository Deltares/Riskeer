﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        public static void AreClones(HydraulicLoadsOutput original, HydraulicLoadsOutput clone)
        {
            Assert.AreEqual(original.TargetProbability, clone.TargetProbability);
            Assert.AreEqual(original.TargetReliability, clone.TargetReliability);
            Assert.AreEqual(original.CalculatedProbability, clone.CalculatedProbability);
            Assert.AreEqual(original.CalculatedReliability, clone.CalculatedReliability);
            Assert.AreEqual(original.CalculationConvergence, clone.CalculationConvergence);
        }
    }
}
