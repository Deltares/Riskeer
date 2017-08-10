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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;
using CoreCloneAssert = Core.Common.Data.TestUtil.CloneAssert;

namespace Ringtoets.Common.Data.TestUtil
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
        public static void AreClones(BreakWater original, BreakWater clone)
        {
            Assert.AreEqual(original.Type, clone.Type);
            Assert.AreEqual(original.Height, clone.Height);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(ProbabilityAssessmentOutput original, ProbabilityAssessmentOutput clone)
        {
            Assert.AreEqual(original.RequiredProbability, clone.RequiredProbability);
            Assert.AreEqual(original.RequiredReliability, clone.RequiredReliability);
            Assert.AreEqual(original.Probability, clone.Probability);
            Assert.AreEqual(original.Reliability, clone.Reliability);
            Assert.AreEqual(original.FactorOfSafety, clone.FactorOfSafety);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(Comment original, Comment clone)
        {
            Assert.AreEqual(original.Body, clone.Body);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(WindDirection original, WindDirection clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.Angle, clone.Angle);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(Stochast original, Stochast clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.Alpha, clone.Alpha);
            Assert.AreEqual(original.Duration, clone.Duration);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(SubMechanismIllustrationPointStochast original, SubMechanismIllustrationPointStochast clone)
        {
            AreClones((Stochast) original, clone);
            Assert.AreEqual(original.Realization, clone.Realization);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(TopLevelIllustrationPointBase original, TopLevelIllustrationPointBase clone)
        {
            CoreCloneAssert.AreClones(original.WindDirection, clone.WindDirection, AreClones);
            Assert.AreEqual(original.ClosingSituation, clone.ClosingSituation);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(IllustrationPointResult original, IllustrationPointResult clone)
        {
            Assert.AreEqual(original.Description, clone.Description);
            Assert.AreEqual(original.Value, clone.Value);
        }
    }
}