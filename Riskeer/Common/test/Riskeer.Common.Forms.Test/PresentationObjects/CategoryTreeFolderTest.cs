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

using System.Collections;
using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class CategoryTreeFolderTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var contents = new[]
            {
                new object(),
                new object()
            };
            const TreeFolderCategory category = TreeFolderCategory.Output;

            // Call
            var treeFolder = new CategoryTreeFolder("<name>", contents, category);

            // Assert
            Assert.AreEqual("<name>", treeFolder.Name);
            Assert.AreEqual(category, treeFolder.Category);
            Assert.AreNotSame(contents, treeFolder.Contents);
            CollectionAssert.AreEqual(contents, treeFolder.Contents);
        }

        [Test]
        public void ParameteredConstructor_NotSpecifyingCategory_ExpectedValues()
        {
            // Setup
            var contents = new[]
            {
                new object(),
                new object()
            };

            // Call
            var treeFolder = new CategoryTreeFolder("<name>", contents);

            // Assert
            Assert.AreEqual("<name>", treeFolder.Name);
            Assert.AreEqual(TreeFolderCategory.General, treeFolder.Category);
            Assert.AreNotSame(contents, treeFolder.Contents);
            CollectionAssert.AreEqual(contents, treeFolder.Contents);
        }

        [TestFixture]
        private class CategoryTreeFolderEqualsTest : EqualsTestFixture<CategoryTreeFolder, TestCategoryTreeFolder>
        {
            protected override CategoryTreeFolder CreateObject()
            {
                return CreateConfiguredCategoryTreeFolder();
            }

            protected override TestCategoryTreeFolder CreateDerivedObject()
            {
                CategoryTreeFolder baseFolder = CreateConfiguredCategoryTreeFolder();
                return new TestCategoryTreeFolder(baseFolder.Name,
                                                  baseFolder.Contents,
                                                  baseFolder.Category);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                CategoryTreeFolder baseFolder = CreateConfiguredCategoryTreeFolder();
                yield return new TestCaseData(new CategoryTreeFolder(baseFolder.Name, new[]
                    {
                        1,
                        3
                    }))
                    .SetName("Content");

                yield return new TestCaseData(new CategoryTreeFolder(baseFolder.Name, new object[0]))
                    .SetName("Content Count");

                yield return new TestCaseData(new CategoryTreeFolder("Different name", baseFolder.Contents))
                    .SetName("Name");
            }

            private static CategoryTreeFolder CreateConfiguredCategoryTreeFolder()
            {
                return new CategoryTreeFolder("name", new[]
                {
                    1,
                    2
                });
            }
        }

        private class TestCategoryTreeFolder : CategoryTreeFolder
        {
            public TestCategoryTreeFolder(string name, IEnumerable contents, TreeFolderCategory category = TreeFolderCategory.General) : base(name, contents, category) {}
        }
    }
}