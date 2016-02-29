// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Common.Base.Data;
using NUnit.Framework;

namespace Core.Common.Base.Test.Data
{
    [TestFixture]
    public class ProjectComparerTest
    {
        [Test]
        public void EqualsToNew_NewProject_ReturnsTrue()
        {
            // Setup
            Project newProject = new Project();

            // Call
            bool result = ProjectComparer.EqualsToNew(newProject);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void EqualsToNew_ProjectNameChanged_ReturnsFalse()
        {
            // Setup
            Project newProject = new Project
            {
                Name = "<some name>"
            };

            // Call
            bool result = ProjectComparer.EqualsToNew(newProject);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void EqualsToNew_ProjectDescriptionChanged_ReturnsFalse()
        {
            // Setup
            Project newProject = new Project
            {
                Description = "<some description>"
            };

            // Call
            bool result = ProjectComparer.EqualsToNew(newProject);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void EqualsToNew_ProjectStorageIdChanged_ReturnsFalse()
        {
            // Setup
            Project newProject = new Project
            {
                StorageId = 1L
            };

            // Call
            bool result = ProjectComparer.EqualsToNew(newProject);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void EqualsToNew_ProjectItemsChanged_ReturnsFalse()
        {
            // Setup
            Project newProject = new Project();
            newProject.Items.Add(new object());

            // Call
            bool result = ProjectComparer.EqualsToNew(newProject);

            // Assert
            Assert.IsFalse(result);
        }
    }
}