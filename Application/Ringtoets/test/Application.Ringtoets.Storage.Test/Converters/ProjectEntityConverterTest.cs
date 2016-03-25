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

using System;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class ProjectEntityConverterTest
    {
        [Test]
        public void DefaultConstructor_Always_NewProjectEntityConverter()
        {
            // Call
            ProjectEntityConverter converter = new ProjectEntityConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<Project, ProjectEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // SetUp
            ProjectEntityConverter converter = new ProjectEntityConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertEntityToModel_ValidProjectEntity_ReturnsTheProjectEntityAsProject()
        {
            // SetUp
            const long storageId = 1234L;
            const string description = "Description";
            ProjectEntity projectEntity = new ProjectEntity()
            {
                ProjectEntityId = storageId,
                Description = description
            };
            ProjectEntityConverter converter = new ProjectEntityConverter();

            // Call
            Project project = converter.ConvertEntityToModel(projectEntity);

            // Assert
            Assert.AreNotEqual(projectEntity, project);
            Assert.AreEqual(storageId, project.StorageId);
            Assert.AreEqual(description, project.Description);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // SetUp
            ProjectEntityConverter converter = new ProjectEntityConverter();
            Project project = new Project();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(project, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // SetUp
            ProjectEntityConverter converter = new ProjectEntityConverter();
            ProjectEntity projectEntity = new ProjectEntity();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, projectEntity);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertModelToEntity_ValidProject_UpdatesTheProjectAsProjectEntity()
        {
            // SetUp
            const long storageId = 1234L;
            const string description = "Description";
            Project project = new Project
            {
                StorageId = storageId,
                Description = description
            };
            ProjectEntity projectEntity = new ProjectEntity();
            ProjectEntityConverter converter = new ProjectEntityConverter();

            var timeStart = DateTime.Now.ToUniversalTime();
            timeStart = Round(timeStart, false);

            // Call
            converter.ConvertModelToEntity(project, projectEntity);

            var timeEnd = DateTime.Now.ToUniversalTime();
            timeStart = Round(timeStart, true);

            // Assert
            Assert.IsTrue(projectEntity.LastUpdated.HasValue);
            var lastUpdatedDateTime = new DateTime(1970, 1, 1).AddSeconds(projectEntity.LastUpdated.Value);

            Assert.AreNotEqual(projectEntity, project);
            Assert.AreEqual(storageId, projectEntity.ProjectEntityId);
            Assert.AreEqual(description, projectEntity.Description);
            Assert.GreaterOrEqual(timeEnd.Ticks, lastUpdatedDateTime.Ticks);
            Assert.LessOrEqual(timeStart.Ticks, lastUpdatedDateTime.Ticks);
        }

        private static DateTime Round(DateTime timeStart, bool up)
        {
            var tickSecond = 10000000;
            timeStart = new DateTime(((timeStart.Ticks + (up ? tickSecond : -tickSecond) / 2)/tickSecond)*tickSecond);
            return timeStart;
        }
    }
}