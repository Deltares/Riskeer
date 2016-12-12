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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

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
            var entity = new ProjectEntity
            {
                Description = testDescription
            };

            // Call
            var project = entity.Read(new ReadConversionCollector());

            // Assert
            Assert.IsNotNull(project);
            Assert.AreEqual(testDescription, project.Description);
        }

        [Test]
        public void Read_WithAssessmentSection_ReturnsNewProjectWithAssessmentSections()
        {
            // Setup
            const double norm = 0.0001;
            var entity = new ProjectEntity
            {
                Description = "testName",
                AssessmentSectionEntities =
                {
                    new AssessmentSectionEntity
                    {
                        Norm = norm,
                        Name = "A",
                        Order = 56,
                        Composition = Convert.ToInt16(AssessmentSectionComposition.Dike)
                    },
                    new AssessmentSectionEntity
                    {
                        Norm = norm,
                        Name = "B",
                        Order = 0,
                        Composition = Convert.ToInt16(AssessmentSectionComposition.Dike)
                    }
                }
            };

            // Call
            var project = entity.Read(new ReadConversionCollector());

            // Assert
            Assert.AreEqual(2, project.AssessmentSections.Count);
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, project.AssessmentSections.Select(a => a.Name));
        }
    }
}