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
using Application.Ringtoets.Storage.Create;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class ProjectCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var project = new Project();

            // Call
            TestDelegate test = () => project.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsProjectEntityWithDescription()
        {
            // Setup
            var testdescription = "testDescription";
            var project = new Project
            {
                Description = testdescription
            };
            var collector = new PersistenceRegistry();

            // Call
            var entity = project.Create(collector);

            // Assert
            Assert.NotNull(entity);
            Assert.AreEqual(testdescription, entity.Description);
        }

        [Test]
        public void Create_WithAssessmentSections_AddsSectionsToEntity()
        {
            // Setup
            var project = new Project
            {
                Items =
                {
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                }
            };
            var collector = new PersistenceRegistry();

            // Call
            var entity = project.Create(collector);

            // Assert
            Assert.AreEqual(1, entity.AssessmentSectionEntities.Count);
        }
    }
}