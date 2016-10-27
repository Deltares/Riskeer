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
using Ringtoets.Common.Data.Structures;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class CalculationGroupEntityReadExtentionsTest
    {
        #region Piping

        [Test]
        public void ReadAsPipingCalculationGroup_ReadConversionCollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            TestDelegate call = () => entity.ReadAsPipingCalculationGroup(null, generalPipingInput);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void ReadAsPipingCalculationGroup_GeneralPipingInputIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => entity.ReadAsPipingCalculationGroup(collector, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("generalPipingInput", paramName);
        }

        [Test]
        [TestCase("A")]
        [TestCase("b")]
        public void ReadAsPipingCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren(
            string name)
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = name
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            CalculationGroup group = entity.ReadAsPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            Assert.AreEqual(name, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsPipingCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Name = "AA",
                        Order = 0
                    },
                    new CalculationGroupEntity
                    {
                        Name = "AB",
                        CalculationGroupEntity1 =
                        {
                            new CalculationGroupEntity
                            {
                                Name = "ABB",
                                Order = 1
                            },
                            new CalculationGroupEntity
                            {
                                Name = "ABA",
                                Order = 0
                            }
                        },
                        Order = 1
                    }
                }
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            var rootGroup = rootGroupEntity.ReadAsPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            ICalculationBase[] rootChildGroup2Children = rootChildGroup2.Children.ToArray();
            var rootChildGroup1Child1 = (CalculationGroup) rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup) rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsPipingCalculationGroup_EntityWithChildPipingCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                PipingCalculationEntities =
                {
                    new PipingCalculationEntity
                    {
                        Order = 1,
                        Name = "2",
                        DampingFactorExitMean = 2,
                    },
                    new PipingCalculationEntity
                    {
                        Order = 0,
                        Name = "1",
                        DampingFactorExitMean = 1,
                    }
                }
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            var rootGroup = rootGroupEntity.ReadAsPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(2, rootChildren.Length);

            var rootChildCalculation1 = (PipingCalculationScenario) rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (PipingCalculationScenario) rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
        }

        [Test]
        public void ReadAsPipingCalculationGroup_EntityWithChildPipingCalculationsAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                PipingCalculationEntities =
                {
                    new PipingCalculationEntity
                    {
                        Order = 0,
                        Name = "calculation1",
                        DampingFactorExitMean = 1,
                    },
                    new PipingCalculationEntity
                    {
                        Order = 2,
                        Name = "calculation2",
                        DampingFactorExitMean = 2,
                    }
                },
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Order = 3,
                        Name = "group2"
                    },
                    new CalculationGroupEntity
                    {
                        Order = 1,
                        Name = "group1"
                    }
                }
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            var rootGroup = rootGroupEntity.ReadAsPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(4, rootChildren.Length);

            var rootChildCalculation1 = (PipingCalculationScenario) rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (PipingCalculationScenario) rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);

            var rootChildGroup2 = (CalculationGroup) rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
        }

        #endregion

        #region Grass Cover Erosion Inwards

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
        [TestCase("HAbba")]
        [TestCase("Dooeis")]
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren(
            string name)
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = name
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(name, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Name = "AB",
                        CalculationGroupEntity1 =
                        {
                            new CalculationGroupEntity
                            {
                                Name = "ABB",
                                Order = 1
                            },
                            new CalculationGroupEntity
                            {
                                Name = "ABA",
                                Order = 0
                            }
                        },
                        Order = 1
                    },
                    new CalculationGroupEntity
                    {
                        Name = "AA",
                        Order = 0
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            ICalculationBase[] rootChildGroup2Children = rootChildGroup2.Children.ToArray();
            var rootChildGroup1Child1 = (CalculationGroup) rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup) rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_EntityWithChildGrassCoverErosionInwardsCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                GrassCoverErosionInwardsCalculationEntities =
                {
                    new GrassCoverErosionInwardsCalculationEntity
                    {
                        Order = 0,
                        Name = "1"
                    },
                    new GrassCoverErosionInwardsCalculationEntity
                    {
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

            var rootChildCalculation1 = (GrassCoverErosionInwardsCalculation) rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (GrassCoverErosionInwardsCalculation) rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
        }

        [Test]
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_EntityWithChildGrassCoverErosionInwardsCalculationsAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                GrassCoverErosionInwardsCalculationEntities =
                {
                    new GrassCoverErosionInwardsCalculationEntity
                    {
                        Order = 2,
                        Name = "calculation2"
                    },
                    new GrassCoverErosionInwardsCalculationEntity
                    {
                        Order = 0,
                        Name = "calculation1"
                    }
                },
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Order = 1,
                        Name = "group1"
                    },
                    new CalculationGroupEntity
                    {
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

            var rootChildCalculation1 = (GrassCoverErosionInwardsCalculation) rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (GrassCoverErosionInwardsCalculation) rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);

            var rootChildGroup2 = (CalculationGroup) rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
        }

        #endregion

        #region Grass Cover Erosion Outwards

        [Test]
        public void ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();

            // Call
            TestDelegate call = () => entity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase("HAbba")]
        [TestCase("Dooeis")]
        public void ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren(
            string name)
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = name
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(name, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Name = "AB",
                        CalculationGroupEntity1 =
                        {
                            new CalculationGroupEntity
                            {
                                Name = "ABA",
                                Order = 0
                            },
                            new CalculationGroupEntity
                            {
                                Name = "ABB",
                                Order = 1
                            }
                        },
                        Order = 1
                    },
                    new CalculationGroupEntity
                    {
                        Name = "AA",
                        Order = 0
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            ICalculationBase[] rootChildGroup2Children = rootChildGroup2.Children.ToArray();
            var rootChildGroup1Child1 = (CalculationGroup) rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup) rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup_EntityWithChildGrassCoverErosionOutwardsWaveConditionsCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                GrassCoverErosionOutwardsWaveConditionsCalculationEntities =
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
                    {
                        Order = 0,
                        Name = "1"
                    },
                    new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
                    {
                        Order = 1,
                        Name = "2"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(2, rootChildren.Length);

            var rootChildCalculation1 = (GrassCoverErosionOutwardsWaveConditionsCalculation) rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (GrassCoverErosionOutwardsWaveConditionsCalculation) rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
        }

        [Test]
        public void ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup_EntityWithChildGrassCoverErosionOutwardsWaveConditionsCalculationsAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                GrassCoverErosionOutwardsWaveConditionsCalculationEntities =
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
                    {
                        Order = 0,
                        Name = "calculation1"
                    },
                    new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
                    {
                        Order = 2,
                        Name = "calculation2"
                    }
                },
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Order = 1,
                        Name = "group1"
                    },
                    new CalculationGroupEntity
                    {
                        Order = 3,
                        Name = "group2"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(4, rootChildren.Length);

            var rootChildCalculation1 = (GrassCoverErosionOutwardsWaveConditionsCalculation) rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (GrassCoverErosionOutwardsWaveConditionsCalculation) rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);

            var rootChildGroup2 = (CalculationGroup) rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
        }

        #endregion

        #region HeightStructures

        [Test]
        public void ReadAsHeightStructuresCalculationGroup_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();

            // Call
            TestDelegate call = () => entity.ReadAsHeightStructuresCalculationGroup(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase("HAbba")]
        [TestCase("Dooeis")]
        public void ReadAsHeightStructuresCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren(
            string name)
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = name,
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsHeightStructuresCalculationGroup(collector);

            // Assert
            Assert.AreEqual(name, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsHeightStructuresCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Name = "AB",
                        CalculationGroupEntity1 =
                        {
                            new CalculationGroupEntity
                            {
                                Name = "ABA",
                                Order = 0
                            },
                            new CalculationGroupEntity
                            {
                                Name = "ABB",
                                Order = 1
                            }
                        },
                        Order = 1
                    },
                    new CalculationGroupEntity
                    {
                        Name = "AA",
                        Order = 0
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsHeightStructuresCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            ICalculationBase[] rootChildGroup2Children = rootChildGroup2.Children.ToArray();
            var rootChildGroup1Child1 = (CalculationGroup) rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup) rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsHeightStructuresCalculationGroup_EntityWithChildHeightStructuresCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                HeightStructuresCalculationEntities =
                {
                    new HeightStructuresCalculationEntity
                    {
                        Order = 0,
                        Name = "1"
                    },
                    new HeightStructuresCalculationEntity
                    {
                        Order = 1,
                        Name = "2"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsHeightStructuresCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(2, rootChildren.Length);

            var rootChildCalculation1 = (StructuresCalculation<HeightStructuresInput>)rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (StructuresCalculation<HeightStructuresInput>)rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
        }

        [Test]
        public void ReadAsHeightStructuresCalculationGroup_EntityWithChildHeightStructuresCalculationAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                HeightStructuresCalculationEntities =
                {
                    new HeightStructuresCalculationEntity
                    {
                        Order = 0,
                        Name = "calculation1"
                    },
                    new HeightStructuresCalculationEntity
                    {
                        Order = 2,
                        Name = "calculation2"
                    }
                },
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Order = 1,
                        Name = "group1"
                    },
                    new CalculationGroupEntity
                    {
                        Order = 3,
                        Name = "group2"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsHeightStructuresCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(4, rootChildren.Length);

            var rootChildCalculation1 = (StructuresCalculation<HeightStructuresInput>)rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (StructuresCalculation<HeightStructuresInput>)rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);

            var rootChildGroup2 = (CalculationGroup) rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
        }

        #endregion

        #region Stability Stone Cover

        [Test]
        public void ReadAsStabilityStoneCoverWaveConditionsCalculationGroup_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();

            // Call
            TestDelegate call = () => entity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase("HAbba")]
        [TestCase("Dooeis")]
        public void ReadAsStabilityStoneCoverWaveConditionsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren(
            string name)
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = name,
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(name, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsStabilityStoneCoverWaveConditionsCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Name = "AB",
                        CalculationGroupEntity1 =
                        {
                            new CalculationGroupEntity
                            {
                                Name = "ABA",
                                Order = 0
                            },
                            new CalculationGroupEntity
                            {
                                Name = "ABB",
                                Order = 1
                            }
                        },
                        Order = 1
                    },
                    new CalculationGroupEntity
                    {
                        Name = "AA",
                        Order = 0
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            ICalculationBase[] rootChildGroup2Children = rootChildGroup2.Children.ToArray();
            var rootChildGroup1Child1 = (CalculationGroup) rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup) rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsStabilityStoneCoverWaveConditionsCalculationGroup_EntityWithChildStabilityStoneCoverWaveConditionsCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                StabilityStoneCoverWaveConditionsCalculationEntities =
                {
                    new StabilityStoneCoverWaveConditionsCalculationEntity
                    {
                        Order = 1,
                        Name = "2"
                    },
                    new StabilityStoneCoverWaveConditionsCalculationEntity
                    {
                        Order = 0,
                        Name = "1"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(2, rootChildren.Length);

            var rootChildCalculation1 = (StabilityStoneCoverWaveConditionsCalculation) rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (StabilityStoneCoverWaveConditionsCalculation) rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
        }

        [Test]
        public void ReadAsStabilityStoneCoverWaveConditionsCalculationGroup_EntityWithChildStabilityStoneCoverWaveConditionsCalculationsAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                StabilityStoneCoverWaveConditionsCalculationEntities =
                {
                    new StabilityStoneCoverWaveConditionsCalculationEntity
                    {
                        Order = 0,
                        Name = "calculation1"
                    },
                    new StabilityStoneCoverWaveConditionsCalculationEntity
                    {
                        Order = 2,
                        Name = "calculation2"
                    }
                },
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Order = 3,
                        Name = "group2"
                    },
                    new CalculationGroupEntity
                    {
                        Order = 1,
                        Name = "group1"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(4, rootChildren.Length);

            var rootChildCalculation1 = (StabilityStoneCoverWaveConditionsCalculation) rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (StabilityStoneCoverWaveConditionsCalculation) rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);

            var rootChildGroup2 = (CalculationGroup) rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
        }

        #endregion

        #region Wave Impact Asphalt Cover

        [Test]
        public void ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();

            // Call
            TestDelegate call = () => entity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCase("HAbba")]
        [TestCase("Dooeis")]
        public void ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren(
            string name)
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = name,
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(name, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Name = "AB",
                        CalculationGroupEntity1 =
                        {
                            new CalculationGroupEntity
                            {
                                Name = "ABA",
                                Order = 0
                            },
                            new CalculationGroupEntity
                            {
                                Name = "ABB",
                                Order = 1
                            }
                        },
                        Order = 1
                    },
                    new CalculationGroupEntity
                    {
                        Name = "AA",
                        Order = 0
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            ICalculationBase[] rootChildGroup2Children = rootChildGroup2.Children.ToArray();
            var rootChildGroup1Child1 = (CalculationGroup) rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup) rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup_EntityWithChildWaveImpactAsphaltCoverWaveConditionsCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                WaveImpactAsphaltCoverWaveConditionsCalculationEntities =
                {
                    new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
                    {
                        Order = 1,
                        Name = "2"
                    },
                    new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
                    {
                        Order = 0,
                        Name = "1"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(2, rootChildren.Length);

            var rootChildCalculation1 = (WaveImpactAsphaltCoverWaveConditionsCalculation) rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (WaveImpactAsphaltCoverWaveConditionsCalculation) rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
        }

        [Test]
        public void ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup_EntityWithChildWaveImpactAsphaltCoverWaveConditionsCalculationsAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                WaveImpactAsphaltCoverWaveConditionsCalculationEntities =
                {
                    new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
                    {
                        Order = 0,
                        Name = "calculation1"
                    },
                    new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
                    {
                        Order = 2,
                        Name = "calculation2"
                    }
                },
                CalculationGroupEntity1 =
                {
                    new CalculationGroupEntity
                    {
                        Order = 3,
                        Name = "group2"
                    },
                    new CalculationGroupEntity
                    {
                        Order = 1,
                        Name = "group1"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            var rootGroup = rootGroupEntity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector);

            // Assert
            ICalculationBase[] rootChildren = rootGroup.Children.ToArray();
            Assert.AreEqual(4, rootChildren.Length);

            var rootChildCalculation1 = (WaveImpactAsphaltCoverWaveConditionsCalculation) rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (WaveImpactAsphaltCoverWaveConditionsCalculation) rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);

            var rootChildGroup2 = (CalculationGroup) rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
        }

        #endregion
    }
}