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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class SpecificFailureMechanismEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => ((SpecificFailureMechanismEntity) null).Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void Read_CollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new SpecificFailureMechanismEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_ValidEntity_ReturnSpecificFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            var probabilityResultType = random.NextEnumValue<FailureMechanismAssemblyProbabilityResultType>();

            const string filePath = "failureMechanismSections/File/Path";
            var entity = new SpecificFailureMechanismEntity
            {
                Name = "Specific failure mechanism name",
                Code = "FAALMECHANISME",
                N = random.NextDouble(1.0, 20.0),
                ApplyLengthEffectInSection = Convert.ToByte(random.NextBoolean()),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Some not in assembly text",
                FailureMechanismSectionCollectionSourcePath = filePath,
                FailureMechanismAssemblyResultProbabilityResultType = Convert.ToByte(probabilityResultType),
                FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability = random.NextDouble(),
                FailureMechanismSectionEntities =
                {
                    CreateSimpleFailureMechanismSectionEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            SpecificFailureMechanism specificFailureMechanism = entity.Read(collector);

            // Assert
            FailureMechanismEntityTestHelper.AssertIFailureMechanismEntityProperties(entity, specificFailureMechanism);

            Assert.AreEqual(entity.Name, specificFailureMechanism.Name);
            Assert.AreEqual(entity.Code, specificFailureMechanism.Code);
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, specificFailureMechanism.Sections.Count());
            Assert.AreEqual(filePath, specificFailureMechanism.FailureMechanismSectionSourcePath);

            Assert.AreEqual(entity.N, specificFailureMechanism.GeneralInput.N, specificFailureMechanism.GeneralInput.N.GetAccuracy());
            Assert.AreEqual(Convert.ToBoolean(entity.ApplyLengthEffectInSection), specificFailureMechanism.GeneralInput.ApplyLengthEffectInSection);
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnSpecificFailureMechanism()
        {
            // Setup
            var entity = new SpecificFailureMechanismEntity
            {
                N = 1.1
            };
            var collector = new ReadConversionCollector();

            // Call
            SpecificFailureMechanism specificFailureMechanism = entity.Read(collector);

            // Assert
            FailureMechanismAssemblyResult assemblyResult = specificFailureMechanism.AssemblyResult;
            Assert.IsNaN(assemblyResult.ManualFailureMechanismAssemblyProbability);
        }

        [Test]
        public void Read_ValidEntityWithoutSections_ReturnSpecificFailureMechanism()
        {
            // Setup
            var entity = new SpecificFailureMechanismEntity
            {
                N = 1.1
            };

            var collector = new ReadConversionCollector();

            // Call
            SpecificFailureMechanism specificFailureMechanism = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, specificFailureMechanism.Sections.Count());
            Assert.IsNull(specificFailureMechanism.FailureMechanismSectionSourcePath);
        }

        [Test]
        public void Read_ValidEntityWithSections_ReturnSpecificFailureMechanism()
        {
            // Setup
            const string filePath = "sections/File/Path";
            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();

            var sectionResultEntity = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            SectionResultTestHelper.SetSectionResult(sectionResultEntity);

            failureMechanismSectionEntity.NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntities = new List<NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity>
            {
                sectionResultEntity
            };

            var entity = new SpecificFailureMechanismEntity
            {
                N = 1.1,
                FailureMechanismSectionEntities = new List<FailureMechanismSectionEntity>
                {
                    failureMechanismSectionEntity
                },
                FailureMechanismSectionCollectionSourcePath = filePath
            };

            var collector = new ReadConversionCollector();

            // Call
            SpecificFailureMechanism specificFailureMechanism = entity.Read(collector);

            // Assert
            Assert.AreEqual(filePath, specificFailureMechanism.FailureMechanismSectionSourcePath);
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, specificFailureMechanism.Sections.Count());

            SectionResultTestHelper.AssertSectionResult(entity.FailureMechanismSectionEntities
                                                              .SelectMany(fms => fms.NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntities)
                                                              .Single(),
                                                        specificFailureMechanism.SectionResults.Single());
        }

        private static FailureMechanismSectionEntity CreateSimpleFailureMechanismSectionEntity()
        {
            var dummyPoints = new[]
            {
                new Point2D(0, 0)
            };
            string dummyPointXml = new Point2DCollectionXmlSerializer().ToXml(dummyPoints);
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                Name = "section",
                FailureMechanismSectionPointXml = dummyPointXml
            };
            return failureMechanismSectionEntity;
        }
    }
}