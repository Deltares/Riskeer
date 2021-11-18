// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Integration.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Exceptions;
using Riskeer.Storage.Core.Read;

namespace Riskeer.Storage.Core.Test.Read
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
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void Read_EntityWithMultipleAssessmentSections_ThrowsEntityReadException()
        {
            // Setup
            var entity = new ProjectEntity
            {
                AssessmentSectionEntities =
                {
                    CreateAssessmentSectionEntity(1),
                    CreateAssessmentSectionEntity(2)
                }
            };

            var collector = new ReadConversionCollector();
            
            // Call
            void Call() => entity.Read(collector);

            // Assert
            var exception = Assert.Throws<EntityReadException>(Call);
            const string message = "Het project bevat meer dan 1 traject.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void Read_EntityWithNoAssessmentSections_ThrowsEntityReadException()
        {
            // Setup
            var entity = new ProjectEntity();
            var collector = new ReadConversionCollector();
            
            // Call
            void Call() => entity.Read(collector);

            // Assert
            var exception = Assert.Throws<EntityReadException>(Call);
            const string message = "Het project bevat geen traject.";
            Assert.AreEqual(message, exception.Message);
        }
        
        [Test]
        public void Read_WithAssessmentSection_ReturnsNewProjectWithAssessmentSection()
        {
            // Setup
            AssessmentSectionEntity assessmentSectionEntity = CreateAssessmentSectionEntity(1);
            var entity = new ProjectEntity
            {
                Description = "testName",
                AssessmentSectionEntities =
                {
                    assessmentSectionEntity
                }
            };

            // Call
            RiskeerProject project = entity.Read(new ReadConversionCollector());

            // Assert
            Assert.AreEqual(entity.Description, project.Description);
            Assert.AreEqual(assessmentSectionEntity.Name, project.AssessmentSection.Name);
        }

        private static AssessmentSectionEntity CreateAssessmentSectionEntity(int seed)
        {
            var random = new Random(seed);

            return new AssessmentSectionEntity
            {
                SignalingNorm = 0.00001,
                LowerLimitNorm = 0.0001,
                NormativeNormType = Convert.ToByte(random.NextEnumValue<NormType>()),
                Name = "Just a name",
                Composition = Convert.ToByte(random.NextEnumValue<AssessmentSectionComposition>()),
                BackgroundDataEntities = new[]
                {
                    new BackgroundDataEntity
                    {
                        Name = "Background A",
                        Transparency = 0.0,
                        IsVisible = 1,
                        BackgroundDataType = 1,
                        BackgroundDataMetaEntities = new[]
                        {
                            new BackgroundDataMetaEntity
                            {
                                Key = BackgroundDataIdentifiers.IsConfigured,
                                Value = "0"
                            }
                        }
                    }
                }
            };
        }
    }
}