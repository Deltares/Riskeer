// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Revetment.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class WaveConditionsCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(WaveConditionsOutput original, WaveConditionsOutput clone)
        {
            Assert.AreEqual(original.CalculatedProbability, clone.CalculatedProbability);
            Assert.AreEqual(original.CalculatedReliability, clone.CalculatedReliability);
            Assert.AreEqual(original.CalculationConvergence, clone.CalculationConvergence);
            Assert.AreEqual(original.TargetProbability, clone.TargetProbability);
            Assert.AreEqual(original.TargetReliability, clone.TargetReliability);
            Assert.AreEqual(original.WaterLevel, clone.WaterLevel);
            Assert.AreEqual(original.WaveAngle, clone.WaveAngle);
            Assert.AreEqual(original.WaveDirection, clone.WaveDirection);
            Assert.AreEqual(original.WaveHeight, clone.WaveHeight);
            Assert.AreEqual(original.WavePeakPeriod, clone.WavePeakPeriod);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(AssessmentSectionCategoryWaveConditionsInput original, AssessmentSectionCategoryWaveConditionsInput clone)
        {
            Assert.AreEqual(original.CategoryType, clone.CategoryType);

            AreClones((WaveConditionsInput) original, clone);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(FailureMechanismCategoryWaveConditionsInput original, FailureMechanismCategoryWaveConditionsInput clone)
        {
            Assert.AreEqual(original.CategoryType, clone.CategoryType);

            AreClones((WaveConditionsInput) original, clone);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(WaveConditionsInput original, WaveConditionsInput clone)
        {
            CoreCloneAssert.AreObjectClones(original.BreakWater, clone.BreakWater, CommonCloneAssert.AreClones);
            Assert.AreSame(original.ForeshoreProfile, clone.ForeshoreProfile);
            Assert.AreSame(original.HydraulicBoundaryLocation, clone.HydraulicBoundaryLocation);
            Assert.AreEqual(original.Orientation, clone.Orientation);
            Assert.AreEqual(original.StepSize, clone.StepSize);
            Assert.AreEqual(original.UpperBoundaryRevetment, clone.UpperBoundaryRevetment);
            Assert.AreEqual(original.LowerBoundaryWaterLevels, clone.LowerBoundaryWaterLevels);
            Assert.AreEqual(original.LowerBoundaryRevetment, clone.LowerBoundaryRevetment);
            Assert.AreEqual(original.UseBreakWater, clone.UseBreakWater);
            Assert.AreEqual(original.UseForeshore, clone.UseForeshore);
        }
    }
}