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

using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class CalculationGroupEntityReadExtentionsTest
    {
        [Test]
        public void ReadPipingCalculationGroup_ReadConversionCollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            TestDelegate call = () => entity.ReadPipingCalculationGroup(null, generalPipingInput);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void ReadPipingCalculationGroup_GeneralPipingInputIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => entity.ReadPipingCalculationGroup(collector, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("generalPipingInput", paramName);
        }

        [Test]
        [TestCase(123, "A", 1)]
        [TestCase(1, "b", 0)]
        public void ReadPipingCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren(
            long id, string name, byte isEditable)
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = id,
                Name = name,
                IsEditable = isEditable
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            CalculationGroup group = entity.ReadPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            Assert.AreEqual(id, group.StorageId);
            Assert.AreEqual(name, group.Name);
            Assert.AreEqual(Convert.ToBoolean(isEditable), group.IsNameEditable);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadPipingCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = 1,
                Name = "A",
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 2,
                        Name = "AA",
                        IsEditable = 1,
                        Order = 0
                    },
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 3,
                        Name = "AB",
                        IsEditable = 0,
                        CalculationGroupEntity1 =
                        {
                            new CalculationGroupEntity
                            {
                                CalculationGroupEntityId = 4,
                                Name = "ABA",
                                IsEditable = 0,
                                Order = 0
                            },
                            new CalculationGroupEntity
                            {
                                CalculationGroupEntityId = 5,
                                Name = "ABB",
                                IsEditable = 1,
                                Order = 1
                            }
                        },
                        Order = 1
                    }
                }
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            var rootGroup = rootGroupEntity.ReadPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            Assert.AreEqual(1, rootGroup.StorageId);
            Assert.AreEqual("A", rootGroup.Name);
            Assert.IsFalse(rootGroup.IsNameEditable);

            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            var rootChildGroup1 = (CalculationGroup)rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            Assert.AreEqual(2, rootChildGroup1.StorageId);
            Assert.IsTrue(rootChildGroup1.IsNameEditable);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup)rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);
            Assert.AreEqual(3, rootChildGroup2.StorageId);
            Assert.IsFalse(rootChildGroup2.IsNameEditable);

            ICalculationBase[] rootChildGroup2Children = rootChildGroup2.Children.ToArray();
            var rootChildGroup1Child1 = (CalculationGroup)rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            Assert.AreEqual(4, rootChildGroup1Child1.StorageId);
            Assert.IsFalse(rootChildGroup1Child1.IsNameEditable);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup)rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            Assert.AreEqual(5, rootChildGroup1Child2.StorageId);
            Assert.IsTrue(rootChildGroup1Child2.IsNameEditable);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadPipingCalculationGroup_EntityWithChildPipingCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = 1,
                Name = "A",
                PipingCalculationEntities =
                {
                    new PipingCalculationEntity
                    {
                        PipingCalculationEntityId = 3,
                        Order = 0,
                        Name = "1",
                        DampingFactorExitMean = 1,
                    },
                    new PipingCalculationEntity
                    {
                        PipingCalculationEntityId = 6,
                        Order = 1,
                        Name = "2",
                        DampingFactorExitMean = 2,
                    }
                }
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            var rootGroup = rootGroupEntity.ReadPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(2, rootChildren.Length);

            var rootChildCalculation1 = (PipingCalculationScenario)rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);
            Assert.AreEqual(3, rootChildCalculation1.StorageId);

            var rootChildCalculation2 = (PipingCalculationScenario)rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
            Assert.AreEqual(6, rootChildCalculation2.StorageId);
        }

        [Test]
        public void ReadPipingCalculationGroup_EntityWithChildPipingCalculationsAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = 1,
                Name = "A",
                PipingCalculationEntities =
                {
                    new PipingCalculationEntity
                    {
                        PipingCalculationEntityId = 3,
                        Order = 0,
                        Name = "calculation1",
                        DampingFactorExitMean = 1,
                    },
                    new PipingCalculationEntity
                    {
                        PipingCalculationEntityId = 6,
                        Order = 2,
                        Name = "calculation2",
                        DampingFactorExitMean = 2,
                    }
                },
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 2,
                        Order = 1,
                        Name = "group1"
                    },
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 4,
                        Order = 3,
                        Name = "group2"
                    }
                }
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            var rootGroup = rootGroupEntity.ReadPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(4, rootChildren.Length);

            var rootChildCalculation1 = (PipingCalculationScenario)rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);
            Assert.AreEqual(3, rootChildCalculation1.StorageId);

            var rootChildGroup1 = (CalculationGroup)rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);
            Assert.AreEqual(2, rootChildGroup1.StorageId);

            var rootChildCalculation2 = (PipingCalculationScenario)rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);
            Assert.AreEqual(6, rootChildCalculation2.StorageId);

            var rootChildGroup2 = (CalculationGroup)rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
            Assert.AreEqual(4, rootChildGroup2.StorageId);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();

            // Call
            TestDelegate call = () => entity.ReadAsGrassCoverErosionInwardsCalculationGroup(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase(345, "HAbba", 1)]
        [TestCase(45, "Dooeis", 0)]
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren(
            long id, string name, byte isEditable)
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = id,
                Name = name,
                IsEditable = isEditable
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(id, group.StorageId);
            Assert.AreEqual(name, group.Name);
            Assert.AreEqual(Convert.ToBoolean(isEditable), group.IsNameEditable);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = 1,
                Name = "A",
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 2,
                        Name = "AA",
                        IsEditable = 1,
                        Order = 0
                    },
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 3,
                        Name = "AB",
                        IsEditable = 0,
                        CalculationGroupEntity1 =
                        {
                            new CalculationGroupEntity
                            {
                                CalculationGroupEntityId = 4,
                                Name = "ABA",
                                IsEditable = 0,
                                Order = 0
                            },
                            new CalculationGroupEntity
                            {
                                CalculationGroupEntityId = 5,
                                Name = "ABB",
                                IsEditable = 1,
                                Order = 1
                            }
                        },
                        Order = 1
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(1, rootGroup.StorageId);
            Assert.AreEqual("A", rootGroup.Name);
            Assert.IsFalse(rootGroup.IsNameEditable);

            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            var rootChildGroup1 = (CalculationGroup)rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            Assert.AreEqual(2, rootChildGroup1.StorageId);
            Assert.IsTrue(rootChildGroup1.IsNameEditable);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup)rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);
            Assert.AreEqual(3, rootChildGroup2.StorageId);
            Assert.IsFalse(rootChildGroup2.IsNameEditable);

            ICalculationBase[] rootChildGroup2Children = rootChildGroup2.Children.ToArray();
            var rootChildGroup1Child1 = (CalculationGroup)rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            Assert.AreEqual(4, rootChildGroup1Child1.StorageId);
            Assert.IsFalse(rootChildGroup1Child1.IsNameEditable);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup)rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            Assert.AreEqual(5, rootChildGroup1Child2.StorageId);
            Assert.IsTrue(rootChildGroup1Child2.IsNameEditable);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_EntityWithChildGrassCoverErosionInwardsCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = 1,
                Name = "A",
                GrassCoverErosionInwardsCalculationEntities = 
                {
                    new GrassCoverErosionInwardsCalculationEntity
                    {
                        GrassCoverErosionInwardsCalculationEntityId = 3,
                        Order = 0,
                        Name = "1"
                    },
                    new GrassCoverErosionInwardsCalculationEntity
                    {
                        GrassCoverErosionInwardsCalculationEntityId = 6,
                        Order = 1,
                        Name = "2"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(2, rootChildren.Length);

            var rootChildCalculation1 = (GrassCoverErosionInwardsCalculation)rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);
            Assert.AreEqual(3, rootChildCalculation1.StorageId);

            var rootChildCalculation2 = (GrassCoverErosionInwardsCalculation)rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
            Assert.AreEqual(6, rootChildCalculation2.StorageId);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_EntityWithChildGrassCoverErosionInwardsCalculationsAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = 1,
                Name = "A",
                GrassCoverErosionInwardsCalculationEntities = 
                {
                    new GrassCoverErosionInwardsCalculationEntity
                    {
                        GrassCoverErosionInwardsCalculationEntityId = 3,
                        Order = 0,
                        Name = "calculation1"
                    },
                    new GrassCoverErosionInwardsCalculationEntity
                    {
                        GrassCoverErosionInwardsCalculationEntityId = 6,
                        Order = 2,
                        Name = "calculation2"
                    }
                },
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 2,
                        Order = 1,
                        Name = "group1"
                    },
                    new CalculationGroupEntity
                    {
                        CalculationGroupEntityId = 4,
                        Order = 3,
                        Name = "group2"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(4, rootChildren.Length);

            var rootChildCalculation1 = (GrassCoverErosionInwardsCalculation)rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);
            Assert.AreEqual(3, rootChildCalculation1.StorageId);

            var rootChildGroup1 = (CalculationGroup)rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);
            Assert.AreEqual(2, rootChildGroup1.StorageId);

            var rootChildCalculation2 = (GrassCoverErosionInwardsCalculation)rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);
            Assert.AreEqual(6, rootChildCalculation2.StorageId);

            var rootChildGroup2 = (CalculationGroup)rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
            Assert.AreEqual(4, rootChildGroup2.StorageId);
        }
    }
}