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

using NUnit.Framework;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.Data.TestUtil
{
    /// <summary>
    /// Helper class to test assembly tool data objects.
    /// </summary>
    public static class AssemblyToolTestHelper
    {
        /// <summary>
        /// Asserts that two <see cref="FailureMechanismSectionAssembly"/> are equal.
        /// </summary>
        /// <param name="expectedSectionAssembly">The expected assembly.</param>
        /// <param name="actualSectionAssembly">The actual assembly.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actualSectionAssembly"/> is not
        /// equal to <paramref name="expectedSectionAssembly"/>.</exception>
        public static void AssertAreEqual(FailureMechanismSectionAssembly expectedSectionAssembly,
                                          FailureMechanismSectionAssembly actualSectionAssembly)
        {
            Assert.AreEqual(expectedSectionAssembly.Group, actualSectionAssembly.Group);
            Assert.AreEqual(expectedSectionAssembly.Probability, actualSectionAssembly.Probability);
        }

        /// <summary>
        /// Asserts that two <see cref="FailureMechanismAssembly"/> are equal.
        /// </summary>
        /// <param name="expectedAssembly">The expected assembly.</param>
        /// <param name="actualAssembly">The actual assembly.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actualAssembly"/> is not
        /// equal to <paramref name="expectedAssembly"/>.</exception>
        public static void AssertAreEqual(FailureMechanismAssembly expectedAssembly,
                                          FailureMechanismAssembly actualAssembly)
        {
            Assert.AreEqual(expectedAssembly.Group, actualAssembly.Group);
            Assert.AreEqual(expectedAssembly.Probability, actualAssembly.Probability);
        }
    }
}