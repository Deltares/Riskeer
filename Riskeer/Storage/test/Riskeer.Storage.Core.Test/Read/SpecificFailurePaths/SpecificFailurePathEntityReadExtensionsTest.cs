﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.SpecificFailurePaths;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Read.SpecificFailurePaths
{
    [TestFixture]
    public class SpecificFailurePathEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new SpecificFailurePathEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_ValidEntity_ReturnSpecificFailurePath()
        {
            // Setup
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            var probabilityResultType = random.NextEnumValue<FailurePathAssemblyProbabilityResultType>();

            const string filePath = "failureMechanismSections/File/Path";
            var entity = new SpecificFailurePathEntity
            {
                Name = "Specific failure path name",
                N = random.NextDouble(1.0, 20.0),
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
            SpecificFailurePath specificFailurePath = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Name, specificFailurePath.Name);
            Assert.AreEqual(inAssembly, specificFailurePath.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, specificFailurePath.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, specificFailurePath.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, specificFailurePath.NotInAssemblyComments.Body);
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, specificFailurePath.Sections.Count());
            Assert.AreEqual(filePath, specificFailurePath.FailureMechanismSectionSourcePath);

            Assert.AreEqual(entity.N, specificFailurePath.Input.N, specificFailurePath.Input.N.GetAccuracy());

            FailurePathAssemblyResult assemblyResult = specificFailurePath.AssemblyResult;
            Assert.AreEqual(probabilityResultType, assemblyResult.ProbabilityResultType);
            Assert.AreEqual(entity.ManualFailurePathAssemblyProbability, assemblyResult.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnSpecificFailurePath()
        {
            // Setup
            var entity = new SpecificFailurePathEntity
            {
                N = 1.1
            };
            var collector = new ReadConversionCollector();

            // Call
            SpecificFailurePath specificFailurePath = entity.Read(collector);

            // Assert
            FailurePathAssemblyResult assemblyResult = specificFailurePath.AssemblyResult;
            Assert.IsNaN(assemblyResult.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void Read_ValidEntityWithoutSections_ReturnSpecificFailurePath()
        {
            // Setup
            var entity = new SpecificFailurePathEntity
            {
                N = 1.1
            };

            var collector = new ReadConversionCollector();

            // Call
            SpecificFailurePath specificFailurePath = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, specificFailurePath.Sections.Count());
            Assert.IsNull(specificFailurePath.FailureMechanismSectionSourcePath);
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