// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
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

        [Test]
        public void Equals_ObjectToEqualToIsNull_ResultShouldBeNotEqual()
        {
            // Setup
            var treeFolder = new CategoryTreeFolder("<name>", new object[0]);

            // Call & Assert
            Assert.AreNotEqual(treeFolder, null);
        }

        [Test]
        public void Equals_ObjectToEqualToIsOfDifferentType_ResultShouldBeNotEqual()
        {
            // Setup
            var enumerable = new object[0];
            var treeFolder = new CategoryTreeFolder("<name>", enumerable);

            // Call & Assert
            Assert.AreNotEqual(treeFolder, new TestCategoryTreeFolder("<name>", enumerable));
        }

        [Test]
        public void Equals_ObjectToEqualToIsSameObject_ResultShouldBeEqual()
        {
            // Setup
            var treeFolder = new CategoryTreeFolder("<name>", new object[0]);

            // Call & Assert
            Assert.AreEqual(treeFolder, treeFolder);
        }

        [Test]
        public void Equals_ObjectToEqualToIsCategoryTreeFolderWithDifferentName_ResultShouldNotBeEqual()
        {
            // Setup
            var enumerable = new object[0];
            var treeFolder1 = new CategoryTreeFolder("<name 1>", enumerable);
            var treeFolder2 = new CategoryTreeFolder("<name 2>", enumerable);

            // Call & Assert
            Assert.AreNotEqual(treeFolder1, treeFolder2);
        }

        [Test]
        public void Equals_ObjectToEqualToIsCategoryTreeFolderWithDifferentAmountOfContents_ResultShouldNotBeEqual()
        {
            // Setup
            var treeFolder1 = new CategoryTreeFolder("<name>", new object[0]);
            var treeFolder2 = new CategoryTreeFolder("<name>", new[]
            {
                new object()
            });

            // Call & Assert
            Assert.AreNotEqual(treeFolder1, treeFolder2);
        }

        [Test]
        public void Equals_ObjectToEqualToIsCategoryTreeFolderWithDifferentContents_ResultShouldNotBeEqual()
        {
            // Setup
            var treeFolder1 = new CategoryTreeFolder("<name>", new[]
            {
                1,
                2
            });
            var treeFolder2 = new CategoryTreeFolder("<name>", new[]
            {
                1,
                3
            });

            // Call & Assert
            Assert.AreNotEqual(treeFolder1, treeFolder2);
        }

        [Test]
        public void Equals_ObjectToEqualToIsCategoryTreeFolderWithSameNameAndSameContents_ResultShouldBeEqual()
        {
            // Setup
            var enumerable = new object[0];
            var treeFolder1 = new CategoryTreeFolder("<name>", enumerable);
            var treeFolder2 = new CategoryTreeFolder("<name>", enumerable);

            // Call & Assert
            Assert.AreEqual(treeFolder1, treeFolder2);
        }

        [Test]
        public void GetHashCode_EqualCategoryTreeFolders_AreEqual()
        {
            // Setup
            var enumerable = new[]
            {
                1,
                2,
                new object()
            };
            var treeFolder1 = new CategoryTreeFolder("<name>", enumerable);
            var treeFolder2 = new CategoryTreeFolder("<name>", enumerable);

            // Precondition
            Assert.AreEqual(treeFolder1, treeFolder1);
            Assert.AreEqual(treeFolder1, treeFolder2);

            // Call & Assert
            Assert.AreEqual(treeFolder1.GetHashCode(), treeFolder1.GetHashCode());
            Assert.AreEqual(treeFolder1.GetHashCode(), treeFolder2.GetHashCode());
        }

        private class TestCategoryTreeFolder : CategoryTreeFolder
        {
            public TestCategoryTreeFolder(string name, IList contents, TreeFolderCategory category = TreeFolderCategory.General) : base(name, contents, category) {}
        }
    }
}