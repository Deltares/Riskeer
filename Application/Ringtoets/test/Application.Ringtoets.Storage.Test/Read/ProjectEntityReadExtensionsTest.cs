﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class ProjectEntityReadExtensionsTest
    {
        [Test]
        public void Read_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new ProjectEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewProjectWithPropertiesSet()
        {
            // Setup
            var testDescription = "testName";
            var entityId = new Random(21).Next(1, 502);
            var entity = new ProjectEntity
            {
                ProjectEntityId = entityId,
                Description = testDescription
            };

            // Call
            var project = entity.Read(new ReadConversionCollector());

            // Assert
            Assert.IsNotNull(project);
            Assert.AreEqual(entityId, project.StorageId);
            Assert.AreEqual(testDescription, project.Description);
        }

        [Test]
        public void Read_WithAssessmentSection_ReturnsNewProjectWithAssessmentSections()
        {
            // Setup
            const int norm = 10000;
            var entity = new ProjectEntity
            {
                Description = "testName",
                AssessmentSectionEntities =
                {
                    new AssessmentSectionEntity
                    {
                        Norm = norm
                    },
                    new AssessmentSectionEntity
                    {
                        Norm = norm
                    }
                }
            };

            // Call
            var project = entity.Read(new ReadConversionCollector());

            // Assert
            Assert.AreEqual(2, project.AssessmentSections.Count);
        }
    }
}