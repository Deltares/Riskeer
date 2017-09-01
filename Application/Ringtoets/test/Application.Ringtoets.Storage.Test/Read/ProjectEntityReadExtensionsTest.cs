﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Integration.Data;

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
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewProjectWithPropertiesSet()
        {
            // Setup
            const string testDescription = "testName";
            var entity = new ProjectEntity
            {
                Description = testDescription
            };

            // Call
            RingtoetsProject project = entity.Read(new ReadConversionCollector());

            // Assert
            Assert.IsNotNull(project);
            Assert.AreEqual(testDescription, project.Description);
        }

        [Test]
        public void Read_WithAssessmentSection_ReturnsNewProjectWithAssessmentSections()
        {
            // Setup
            const double lowerLimitNorm = 0.0001;
            const double signalingNorm = 0.00001;
            
            var entity = new ProjectEntity
            {
                Description = "testName",
                AssessmentSectionEntities =
                {
                    new AssessmentSectionEntity
                    {
                        SignalingNorm = signalingNorm,
                        LowerLimitNorm = lowerLimitNorm,
                        NormativeNorm = Convert.ToByte(NormType.Signaling),
                        Name = "A",
                        Order = 56,
                        Composition = Convert.ToByte(AssessmentSectionComposition.Dike),
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
                    },
                    new AssessmentSectionEntity
                    {
                        SignalingNorm = signalingNorm,
                        LowerLimitNorm = lowerLimitNorm,
                        NormativeNorm = Convert.ToByte(NormType.Signaling),
                        Name = "B",
                        Order = 0,
                        Composition = Convert.ToByte(AssessmentSectionComposition.Dike),
                        BackgroundDataEntities = new[]
                        {
                            new BackgroundDataEntity
                            {
                                Name = "Background B",
                                Transparency = 0.0,
                                IsVisible = 1,
                                BackgroundDataType = 2,
                                BackgroundDataMetaEntities = new[]
                                {
                                    new BackgroundDataMetaEntity
                                    {
                                        Key = BackgroundDataIdentifiers.WellKnownTileSource,
                                        Value = "1"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Call
            RingtoetsProject project = entity.Read(new ReadConversionCollector());

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