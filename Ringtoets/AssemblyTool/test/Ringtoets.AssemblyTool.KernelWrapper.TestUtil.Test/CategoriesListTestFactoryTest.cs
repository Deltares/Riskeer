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

using Assembly.Kernel.Model.CategoryLimits;
using NUnit.Framework;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class CategoriesListTestFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSectionCategories_Always_ReturnsValidCategoriesList()
        {
            // Call
            CategoriesList<FmSectionCategory> categories = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

            // Assert
            Assert.IsNotNull(categories);

            FmSectionCategory[] sectionCategories = categories.Categories;
            Assert.AreEqual(4, sectionCategories.Length);

            Assert.AreEqual(0, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.25, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.25, sectionCategories[1].LowerLimit);
            Assert.AreEqual(0.5, sectionCategories[1].UpperLimit);

            Assert.AreEqual(0.5, sectionCategories[2].LowerLimit);
            Assert.AreEqual(0.75, sectionCategories[2].UpperLimit);

            Assert.AreEqual(0.75, sectionCategories[3].LowerLimit);
            Assert.AreEqual(1, sectionCategories[3].UpperLimit);
        }

        [Test]
        public void CreateFailureMechanismCategories_Always_ReturnsValidCategoriesList()
        {
            // Call
            CategoriesList<FailureMechanismCategory> categories = CategoriesListTestFactory.CreateFailureMechanismCategories();

            // Assert
            Assert.IsNotNull(categories);

            FailureMechanismCategory[] sectionCategories = categories.Categories;
            Assert.AreEqual(4, sectionCategories.Length);

            Assert.AreEqual(0, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.25, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.25, sectionCategories[1].LowerLimit);
            Assert.AreEqual(0.5, sectionCategories[1].UpperLimit);

            Assert.AreEqual(0.5, sectionCategories[2].LowerLimit);
            Assert.AreEqual(0.75, sectionCategories[2].UpperLimit);

            Assert.AreEqual(0.75, sectionCategories[3].LowerLimit);
            Assert.AreEqual(1, sectionCategories[3].UpperLimit);
        }

        [Test]
        public void CreateAssessmentSectionCategories_Always_ReturnsValidCategoriesList()
        {
            // Call
            CategoriesList<AssessmentSectionCategory> categories = CategoriesListTestFactory.CreateAssessmentSectionCategories();

            // Assert
            Assert.IsNotNull(categories);

            AssessmentSectionCategory[] sectionCategories = categories.Categories;
            Assert.AreEqual(4, sectionCategories.Length);

            Assert.AreEqual(0, sectionCategories[0].LowerLimit);
            Assert.AreEqual(0.25, sectionCategories[0].UpperLimit);

            Assert.AreEqual(0.25, sectionCategories[1].LowerLimit);
            Assert.AreEqual(0.5, sectionCategories[1].UpperLimit);

            Assert.AreEqual(0.5, sectionCategories[2].LowerLimit);
            Assert.AreEqual(0.75, sectionCategories[2].UpperLimit);

            Assert.AreEqual(0.75, sectionCategories[3].LowerLimit);
            Assert.AreEqual(1, sectionCategories[3].UpperLimit);
        }
    }
}