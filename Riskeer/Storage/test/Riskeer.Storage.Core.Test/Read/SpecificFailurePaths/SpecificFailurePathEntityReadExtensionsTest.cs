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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
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
        public void Read_EntityNotReadBefore_RegisterEntity()
        {
            // Setup
            var random = new Random(21);
            var entity = new SpecificFailurePathEntity
            {
                Name = "name",
                N = random.NextDouble(1.0, 20.0)
            };

            var collector = new ReadConversionCollector();

            // Precondition
            Assert.IsFalse(collector.Contains(entity));

            // Call
            SpecificFailurePath specificFailurePath = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(entity));
            Assert.AreSame(specificFailurePath, collector.Get(entity));
        }

        [Test]
        public void Read_ValidEntity_ReturnSpecificFailurePath()
        {
            // Setup
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            const string filePath = "failureMechanismSections/File/Path";
            var entity = new SpecificFailurePathEntity
            {
                Name = "Specific failure path name",
                N = random.NextDouble(1.0, 20.0),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = "Some input text",
                OutputComments = "Some output text",
                NotRelevantComments = "Some not relevant text",
                FailureMechanismSectionCollectionSourcePath = filePath,
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
            Assert.AreEqual(Convert.ToBoolean(entity.IsRelevant), specificFailurePath.IsRelevant);
            Assert.AreEqual(entity.InputComments, specificFailurePath.InputComments.Body);
            Assert.AreEqual(entity.OutputComments, specificFailurePath.OutputComments.Body);
            Assert.AreEqual(entity.NotRelevantComments, specificFailurePath.NotRelevantComments.Body);
            Assert.AreEqual(entity.FailureMechanismSectionEntities.Count, specificFailurePath.Sections.Count());
            Assert.AreEqual(filePath, specificFailurePath.FailureMechanismSectionSourcePath);
            
            Assert.AreEqual(entity.N, specificFailurePath.Input.N, specificFailurePath.Input.N.GetAccuracy());

            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        public void Read_EntityRegistered_ReturnRegisteredSpecificFailurePath()
        {
            // Setup
            var entity = new SpecificFailurePathEntity();
            var registeredSpecificFailurePath = new SpecificFailurePath();
            var collector = new ReadConversionCollector();
            collector.Read(entity, registeredSpecificFailurePath);

            // Call
            SpecificFailurePath readSpecificFailurePath = entity.Read(collector);

            // Assert
            Assert.AreSame(registeredSpecificFailurePath, readSpecificFailurePath);
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