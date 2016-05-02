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

namespace Application.Ringtoets.Storage.Test.Read
{
    public class FailureMechanismEntityTest
    {
        [Test]
        public void ReadAsPipingFailureMechanism_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => entity.ReadAsPipingFailureMechanism(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsPipingFailureMechanism_WithCollector_ReturnsNewPipingFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                IsRelevant = Convert.ToByte(isRelevant),
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.IsEmpty(failureMechanism.StochasticSoilModels);
            Assert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithStochasticSoilModelsSet_ReturnsNewPipingFailureMechanismWithStochasticSoilModelsSet()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity(),
                    new StochasticSoilModelEntity()
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.StochasticSoilModels.Count);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithSectionsSet_ReturnsNewPipingFailureMechanismWithFailureMechanismSectionsSet()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismSectionEntities =
                {
                    new FailureMechanismSectionEntity
                    {
                        Name = "section",
                        FailureMechanismSectionPointEntities =
                        {
                            new FailureMechanismSectionPointEntity()
                        }
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }   

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithCollector_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                IsRelevant = Convert.ToByte(isRelevant),
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsGrassCoverErosionInwardsFailureMechanism();

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsFailureMechanism_WithSectionsSet_ReturnsNewGrassCoverErosionInwardsFailureMechanismWithFailureMechanismSectionsAdded()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismSectionEntities =
                {
                    new FailureMechanismSectionEntity
                    {
                        Name = "section",
                        FailureMechanismSectionPointEntities =
                        {
                            new FailureMechanismSectionPointEntity()
                        }
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsGrassCoverErosionInwardsFailureMechanism();

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }   

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsFailureMechanismPlaceholder_WithoutSectionsSet_ReturnsNewFailureMechanismPlaceholder(bool isRelevant)
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                IsRelevant = Convert.ToByte(isRelevant),
            };

            // Call
            var failureMechanism = entity.ReadAsFailureMechanismPlaceholder();

            // Assert
            Assert.IsEmpty(failureMechanism.Sections);
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.IsEmpty(failureMechanism.Sections);
        }   

        [Test]
        public void ReadAsFailureMechanismPlaceholder_WithSectionsSet_ReturnsNewFailureMechanismPlaceholderWithFailureMechanismSections()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismSectionEntities =
                {
                    new FailureMechanismSectionEntity
                    {
                        Name = "section",
                        FailureMechanismSectionPointEntities =
                        {
                            new FailureMechanismSectionPointEntity()
                        }
                    }
                }
            };

            // Call
            var failureMechanism = entity.ReadAsFailureMechanismPlaceholder();

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
        }   
    }
}