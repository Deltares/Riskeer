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
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class FailurePathEntityReadExtensionsTest
    {
        [Test]
        public void ReadCommonFailureMechanismProperties_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            void Call() => ((FailureMechanismEntity) null).ReadCommonFailureMechanismProperties(failureMechanism, new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ReadCommonFailureMechanismProperties_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            void Call() => entity.ReadCommonFailureMechanismProperties(null, new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ReadCommonFailureMechanismProperties_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var entity = new FailureMechanismEntity();

            // Call
            void Call() => entity.ReadCommonFailureMechanismProperties(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("collector", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadCommonFailureMechanismProperties_WithoutSectionsSet_ReturnsNewStandAloneFailureMechanism(bool inAssembly)
        {
            // Setup
            var random = new Random(21);
            var assemblyResultType = random.NextEnumValue<FailurePathAssemblyProbabilityResultType>();

            var entity = new FailureMechanismEntity
            {
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                CalculationsInputComments = "Some calculation text",
                NotInAssemblyComments = "Really not in assembly",
                FailurePathAssemblyProbabilityResultType = Convert.ToByte(assemblyResultType),
                ManualFailurePathAssemblyProbability = random.NextDouble()
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new TestFailureMechanism();

            // Call
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

            // Assert
            AssertCommonFailurePathProperties(entity, failureMechanism);
            Assert.AreEqual(entity.CalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            Assert.IsNull(failureMechanism.FailureMechanismSectionSourcePath);
        }

        [Test]
        public void ReadCommonFailureMechanismProperties_WithNullValues_ReturnsNewStandAloneFailureMechanism()
        {
            // Setup
            var entity = new FailureMechanismEntity();
            var collector = new ReadConversionCollector();
            var failureMechanism = new TestFailureMechanism();

            // Call
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

            // Assert
            Assert.IsNull(failureMechanism.InAssemblyInputComments.Body);
            Assert.IsNull(failureMechanism.InAssemblyOutputComments.Body);
            Assert.IsNull(failureMechanism.NotInAssemblyComments.Body);
            Assert.IsNull(failureMechanism.CalculationsInputComments.Body);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            Assert.IsNull(failureMechanism.FailureMechanismSectionSourcePath);

            Assert.IsNaN(failureMechanism.AssemblyResult.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void ReadCommonFailureMechanismProperties_WithSectionsSet_ReturnsNewStandAloneFailureMechanismWithFailureMechanismSections()
        {
            // Setup
            const string filePath = "failureMechanismSections/File/Path";
            var entity = new FailureMechanismEntity
            {
                FailureMechanismSectionCollectionSourcePath = filePath,
                FailureMechanismSectionEntities =
                {
                    CreateSimpleFailureMechanismSectionEntity()
                }
            };
            var collector = new ReadConversionCollector();
            var failureMechanism = new TestFailureMechanism();

            // Call
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, failureMechanism.Sections.Count());
            Assert.AreEqual(filePath, failureMechanism.FailureMechanismSectionSourcePath);
        }

        [Test]
        public void ReadSpecificFailurePath_EntityNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => ((SpecificFailurePathEntity) null).ReadSpecificFailurePath(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("entity", paramName);
        }

        [Test]
        public void ReadSpecificFailurePath_CollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new SpecificFailurePathEntity();

            // Call
            void Call() => entity.ReadSpecificFailurePath(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void ReadSpecificFailurePath_ValidEntity_ReturnSpecificFailurePath()
        {
            // Setup
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            var probabilityResultType = random.NextEnumValue<FailurePathAssemblyProbabilityResultType>();

            const string filePath = "failureMechanismSections/File/Path";
            var entity = new SpecificFailurePathEntity
            {
                Name = "Specific failure path name",
                Code = "FAALPAD",
                N = random.NextDouble(1.0, 20.0),
                ApplyLengthEffectInSection = Convert.ToByte(random.NextBoolean()),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = "Some input text",
                InAssemblyOutputComments = "Some output text",
                NotInAssemblyComments = "Some not in assembly text",
                FailureMechanismSectionCollectionSourcePath = filePath,
                FailurePathAssemblyProbabilityResultType = Convert.ToByte(probabilityResultType),
                ManualFailurePathAssemblyProbability = random.NextDouble(),
                FailureMechanismSectionEntities =
                {
                    CreateSimpleFailureMechanismSectionEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            SpecificFailurePath specificFailurePath = entity.ReadSpecificFailurePath(collector);

            // Assert
            AssertCommonFailurePathProperties(entity, specificFailurePath);

            Assert.AreEqual(entity.Name, specificFailurePath.Name);
            Assert.AreEqual(entity.Code, specificFailurePath.Code);
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, specificFailurePath.Sections.Count());
            Assert.AreEqual(filePath, specificFailurePath.FailureMechanismSectionSourcePath);

            Assert.AreEqual(entity.N, specificFailurePath.GeneralInput.N, specificFailurePath.GeneralInput.N.GetAccuracy());
            Assert.AreEqual(Convert.ToBoolean(entity.ApplyLengthEffectInSection), specificFailurePath.GeneralInput.ApplyLengthEffectInSection);
        }

        [Test]
        public void ReadSpecificFailurePath_EntityWithNullValues_ReturnSpecificFailurePath()
        {
            // Setup
            var entity = new SpecificFailurePathEntity
            {
                N = 1.1
            };
            var collector = new ReadConversionCollector();

            // Call
            SpecificFailurePath specificFailurePath = entity.ReadSpecificFailurePath(collector);

            // Assert
            FailurePathAssemblyResult assemblyResult = specificFailurePath.AssemblyResult;
            Assert.IsNaN(assemblyResult.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void ReadSpecificFailurePath_ValidEntityWithoutSections_ReturnSpecificFailurePath()
        {
            // Setup
            var entity = new SpecificFailurePathEntity
            {
                N = 1.1
            };

            var collector = new ReadConversionCollector();

            // Call
            SpecificFailurePath specificFailurePath = entity.ReadSpecificFailurePath(collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, specificFailurePath.Sections.Count());
            Assert.IsNull(specificFailurePath.FailureMechanismSectionSourcePath);
        }

        [Test]
        public void ReadSpecificFailurePath_ValidEntityWithSections_ReturnSpecificFailurePath()
        {
            // Setup
            const string filePath = "failurePathSections/File/Path";
            FailureMechanismSectionEntity failureMechanismSectionEntity = CreateSimpleFailureMechanismSectionEntity();
            var entity = new SpecificFailurePathEntity
            {
                N = 1.1,
                FailureMechanismSectionEntities = new List<FailureMechanismSectionEntity>
                {
                    failureMechanismSectionEntity
                },
                FailureMechanismSectionCollectionSourcePath = filePath
            };
            var sectionResultEntity = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity
            {
                FailureMechanismSectionEntity = failureMechanismSectionEntity
            };
            entity.FailureMechanismSectionEntities.First().NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntities =
                new List<NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity>
            {
                sectionResultEntity
            };
            SectionResultHelper.SetSectionResult(sectionResultEntity);

            var collector = new ReadConversionCollector();

            // Call
            SpecificFailurePath specificFailurePath = entity.ReadSpecificFailurePath(collector);

            // Assert
            SectionResultHelper.AssertSectionResults(entity.FailureMechanismSectionEntities
                                                           .SelectMany(fms => fms.NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntities).Single(),
                                                     specificFailurePath.SectionResults.Single());
        }

        private static void AssertCommonFailurePathProperties(IFailurePathEntity entity, IFailurePath failurePath)
        {
            var inAssembly = Convert.ToBoolean(entity.InAssembly);

            Assert.AreEqual(inAssembly, failurePath.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failurePath.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failurePath.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failurePath.NotInAssemblyComments.Body);

            var probabilityResultType = (FailurePathAssemblyProbabilityResultType) entity.FailurePathAssemblyProbabilityResultType;
            FailurePathAssemblyResult assemblyResult = failurePath.AssemblyResult;
            Assert.AreEqual(probabilityResultType, assemblyResult.ProbabilityResultType);
            Assert.AreEqual(entity.ManualFailurePathAssemblyProbability, assemblyResult.ManualFailurePathAssemblyProbability);
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