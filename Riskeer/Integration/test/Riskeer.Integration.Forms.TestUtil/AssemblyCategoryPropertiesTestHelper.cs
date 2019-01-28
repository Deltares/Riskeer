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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to assert property classes related to assembly categories.
    /// </summary>
    public static class AssemblyCategoryPropertiesTestHelper
    {
        /// <summary>
        /// Asserts whether the content of <paramref name="properties"/> equals the given
        /// <paramref name="expectedFailureMechanismCategories"/> and <paramref name="expectedFailureMechanismSectionCategories"/>.
        /// </summary>
        /// <param name="expectedFailureMechanismCategories">The collection of expected <see cref="FailureMechanismAssemblyCategory"/>.</param>
        /// <param name="expectedFailureMechanismSectionCategories">The collection of expected <see cref="FailureMechanismSectionAssemblyCategory"/>.</param>
        /// <param name="properties">The actual <see cref="FailureMechanismAssemblyCategoriesProperties"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when the content of <paramref name="properties"/> does not equal
        /// <paramref name="expectedFailureMechanismCategories"/> and <paramref name="expectedFailureMechanismSectionCategories"/>.</exception>
        public static void AssertFailureMechanismAndFailureMechanismSectionAssemblyCategoryProperties(
            IEnumerable<FailureMechanismAssemblyCategory> expectedFailureMechanismCategories,
            IEnumerable<FailureMechanismSectionAssemblyCategory> expectedFailureMechanismSectionCategories,
            FailureMechanismAssemblyCategoriesProperties properties)
        {
            Assert.AreEqual(expectedFailureMechanismCategories.Count(), properties.FailureMechanismAssemblyCategories.Length);
            for (var i = 0; i < expectedFailureMechanismCategories.Count(); i++)
            {
                FailureMechanismAssemblyCategory category = expectedFailureMechanismCategories.ElementAt(i);
                FailureMechanismAssemblyCategoryProperties property = properties.FailureMechanismAssemblyCategories[i];
                Assert.AreEqual(category.Group, property.Group);
                Assert.AreEqual(category.UpperBoundary, property.UpperBoundary);
                Assert.AreEqual(category.LowerBoundary, property.LowerBoundary);
            }

            AssertFailureMechanismSectionAssemblyCategoryProperties(expectedFailureMechanismSectionCategories, properties);
        }

        /// <summary>
        /// Asserts whether the content of <paramref name="properties"/> equals the given
        /// <paramref name="expectedFailureMechanismCategories"/>.
        /// </summary>
        /// <param name="expectedFailureMechanismCategories">The collection of expected <see cref="FailureMechanismAssemblyCategory"/>.</param>
        /// <param name="properties">The actual <see cref="AssemblyResultCategoriesProperties"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when the content of <paramref name="properties"/> does not equal
        /// <paramref name="expectedFailureMechanismCategories"/>.</exception>
        public static void AssertFailureMechanismAssemblyCategoryProperties(
            IEnumerable<FailureMechanismAssemblyCategory> expectedFailureMechanismCategories,
            AssemblyResultCategoriesProperties properties)
        {
            Assert.AreEqual(expectedFailureMechanismCategories.Count(), properties.AssemblyCategories.Length);
            for (var i = 0; i < expectedFailureMechanismCategories.Count(); i++)
            {
                FailureMechanismAssemblyCategory category = expectedFailureMechanismCategories.ElementAt(i);
                FailureMechanismAssemblyCategoryProperties property = properties.AssemblyCategories[i];
                Assert.AreEqual(category.Group, property.Group);
                Assert.AreEqual(category.UpperBoundary, property.UpperBoundary);
                Assert.AreEqual(category.LowerBoundary, property.LowerBoundary);
            }
        }

        /// <summary>
        /// Asserts whether the content of <paramref name="properties"/> equals the given
        /// <paramref name="expectedFailureMechanismSectionCategories"/>.
        /// </summary>
        /// <param name="expectedFailureMechanismSectionCategories">The collection of expected <see cref="FailureMechanismSectionAssemblyCategory"/>.</param>
        /// <param name="properties">The actual <see cref="FailureMechanismSectionAssemblyCategoriesProperties"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when the content of <paramref name="properties"/> does not equal
        /// <paramref name="expectedFailureMechanismSectionCategories"/>.</exception>
        public static void AssertFailureMechanismSectionAssemblyCategoryProperties(IEnumerable<FailureMechanismSectionAssemblyCategory> expectedFailureMechanismSectionCategories,
                                                                                   FailureMechanismSectionAssemblyCategoriesProperties properties)
        {
            Assert.AreEqual(expectedFailureMechanismSectionCategories.Count(), properties.FailureMechanismSectionAssemblyCategories.Length);
            for (var i = 0; i < expectedFailureMechanismSectionCategories.Count(); i++)
            {
                FailureMechanismSectionAssemblyCategory category = expectedFailureMechanismSectionCategories.ElementAt(i);
                FailureMechanismSectionAssemblyCategoryProperties property = properties.FailureMechanismSectionAssemblyCategories[i];
                Assert.AreEqual(category.Group, property.Group);
                Assert.AreEqual(category.UpperBoundary, property.UpperBoundary);
                Assert.AreEqual(category.LowerBoundary, property.LowerBoundary);
            }
        }
    }
}