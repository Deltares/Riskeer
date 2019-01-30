// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class RiskeerProjectCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var project = new RingtoetsProject();

            // Call
            TestDelegate test = () => project.Create(null);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsProjectEntityWithDescription()
        {
            // Setup
            const string testdescription = "testDescription";
            var project = new RingtoetsProject
            {
                Description = testdescription
            };
            var registry = new PersistenceRegistry();

            // Call
            ProjectEntity entity = project.Create(registry);

            // Assert
            Assert.NotNull(entity);
            Assert.AreEqual(testdescription, entity.Description);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string testdescription = "original description";
            var project = new RingtoetsProject
            {
                Description = testdescription
            };
            var registry = new PersistenceRegistry();

            // Call
            ProjectEntity entity = project.Create(registry);

            // Assert
            Assert.AreNotSame(testdescription, entity.Description);
            Assert.AreEqual(testdescription, entity.Description);
        }

        [Test]
        public void Create_WithAssessmentSections_AddsSectionsToEntity()
        {
            // Setup
            var project = new RingtoetsProject
            {
                AssessmentSections =
                {
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            ProjectEntity entity = project.Create(registry);

            // Assert
            Assert.AreEqual(1, entity.AssessmentSectionEntities.Count);
        }
    }
}