// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class CalculationGroupEntityReadExtensionsTest
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
        public void ReadAsPipingCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren()
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            CalculationGroup group = entity.ReadAsPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            Assert.AreEqual(entity.Name, group.Name);
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            List<ICalculationBase> rootChildren = rootGroup.Children;
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            List<ICalculationBase> rootChildGroup2Children = rootChildGroup2.Children;
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
                        DampingFactorExitMean = 2
                    },
                    new PipingCalculationEntity
                    {
                        Order = 0,
                        Name = "1",
                        DampingFactorExitMean = 1
                    }
                }
            };

            var collector = new ReadConversionCollector();
            var generalPipingInput = new GeneralPipingInput();

            // Call
            CalculationGroup rootGroup = rootGroupEntity.ReadAsPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(2, rootChildren.Count);

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
                        DampingFactorExitMean = 1
                    },
                    new PipingCalculationEntity
                    {
                        Order = 2,
                        Name = "calculation2",
                        DampingFactorExitMean = 2
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsPipingCalculationGroup(collector, generalPipingInput);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(4, rootChildren.Count);

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

        #region Macro Stability Inwards

        [Test]
        public void ReadAsMacroStabilityInwardsCalculationGroup_ReadConversionCollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();

            // Call
            TestDelegate call = () => entity.ReadAsMacroStabilityInwardsCalculationGroup(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren()
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsMacroStabilityInwardsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(entity.Name, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
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

            // Call
            CalculationGroup rootGroup = rootGroupEntity.ReadAsMacroStabilityInwardsCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            List<ICalculationBase> rootChildren = rootGroup.Children;
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            List<ICalculationBase> rootChildGroup2Children = rootChildGroup2.Children;
            var rootChildGroup1Child1 = (CalculationGroup) rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup) rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsCalculationGroup_EntityWithChildMacroStabilityInwardsCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                MacroStabilityInwardsCalculationEntities =
                {
                    new MacroStabilityInwardsCalculationEntity
                    {
                        Order = 1,
                        Name = "2",
                        TangentLineNumber = 1,
                        LeftGridNrOfHorizontalPoints = 5,
                        LeftGridNrOfVerticalPoints = 5,
                        RightGridNrOfHorizontalPoints = 5,
                        RightGridNrOfVerticalPoints = 5
                    },
                    new MacroStabilityInwardsCalculationEntity
                    {
                        Order = 0,
                        Name = "1",
                        TangentLineNumber = 1,
                        LeftGridNrOfHorizontalPoints = 5,
                        LeftGridNrOfVerticalPoints = 5,
                        RightGridNrOfHorizontalPoints = 5,
                        RightGridNrOfVerticalPoints = 5
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup rootGroup = rootGroupEntity.ReadAsMacroStabilityInwardsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(2, rootChildren.Count);

            var rootChildCalculation1 = (MacroStabilityInwardsCalculationScenario) rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (MacroStabilityInwardsCalculationScenario) rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsCalculationGroup_EntityWithChildMacroStabilityInwardsCalculationsAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                MacroStabilityInwardsCalculationEntities =
                {
                    new MacroStabilityInwardsCalculationEntity
                    {
                        Order = 0,
                        Name = "calculation1",
                        TangentLineNumber = 1,
                        LeftGridNrOfHorizontalPoints = 5,
                        LeftGridNrOfVerticalPoints = 5,
                        RightGridNrOfHorizontalPoints = 5,
                        RightGridNrOfVerticalPoints = 5
                    },
                    new MacroStabilityInwardsCalculationEntity
                    {
                        Order = 2,
                        Name = "calculation2",
                        TangentLineNumber = 2,
                        LeftGridNrOfHorizontalPoints = 5,
                        LeftGridNrOfVerticalPoints = 5,
                        RightGridNrOfHorizontalPoints = 5,
                        RightGridNrOfVerticalPoints = 5
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsMacroStabilityInwardsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(4, rootChildren.Count);

            var rootChildCalculation1 = (MacroStabilityInwardsCalculationScenario) rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (MacroStabilityInwardsCalculationScenario) rootChildren[2];
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
        public void ReadAsGrassCoverErosionInwardsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren()
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(entity.Name, group.Name);
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            List<ICalculationBase> rootChildren = rootGroup.Children;
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            List<ICalculationBase> rootChildGroup2Children = rootChildGroup2.Children;
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(2, rootChildren.Count);

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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(4, rootChildren.Count);

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
        public void ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren()
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(entity.Name, group.Name);
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            List<ICalculationBase> rootChildren = rootGroup.Children;
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            List<ICalculationBase> rootChildGroup2Children = rootChildGroup2.Children;
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(2, rootChildren.Count);

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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(4, rootChildren.Count);

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
        public void ReadAsHeightStructuresCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren()
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsHeightStructuresCalculationGroup(collector);

            // Assert
            Assert.AreEqual(entity.Name, group.Name);
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsHeightStructuresCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            List<ICalculationBase> rootChildren = rootGroup.Children;
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            List<ICalculationBase> rootChildGroup2Children = rootChildGroup2.Children;
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsHeightStructuresCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(2, rootChildren.Count);

            var rootChildCalculation1 = (StructuresCalculation<HeightStructuresInput>) rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (StructuresCalculation<HeightStructuresInput>) rootChildren[1];
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsHeightStructuresCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(4, rootChildren.Count);

            var rootChildCalculation1 = (StructuresCalculation<HeightStructuresInput>) rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (StructuresCalculation<HeightStructuresInput>) rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);

            var rootChildGroup2 = (CalculationGroup) rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
        }

        #endregion

        #region ClosingStructures

        [Test]
        public void ReadAsClosingStructuresCalculationGroup_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();

            // Call
            TestDelegate call = () => entity.ReadAsClosingStructuresCalculationGroup(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void ReadAsClosingStructuresCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren()
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsClosingStructuresCalculationGroup(collector);

            // Assert
            Assert.AreEqual(entity.Name, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsClosingStructuresCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsClosingStructuresCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            List<ICalculationBase> rootChildren = rootGroup.Children;
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            List<ICalculationBase> rootChildGroup2Children = rootChildGroup2.Children;
            var rootChildGroup1Child1 = (CalculationGroup) rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup) rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsClosingStructuresCalculationGroup_EntityWithChildClosingStructuresCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                ClosingStructuresCalculationEntities =
                {
                    new ClosingStructuresCalculationEntity
                    {
                        Order = 0,
                        Name = "1",
                        IdenticalApertures = 1
                    },
                    new ClosingStructuresCalculationEntity
                    {
                        Order = 1,
                        Name = "2",
                        IdenticalApertures = 1
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup rootGroup = rootGroupEntity.ReadAsClosingStructuresCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(2, rootChildren.Count);

            var rootChildCalculation1 = (StructuresCalculation<ClosingStructuresInput>) rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (StructuresCalculation<ClosingStructuresInput>) rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
        }

        [Test]
        public void ReadAsClosingStructuresCalculationGroup_EntityWithChildClosingStructuresCalculationAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                ClosingStructuresCalculationEntities =
                {
                    new ClosingStructuresCalculationEntity
                    {
                        Order = 0,
                        Name = "calculation1",
                        IdenticalApertures = 1
                    },
                    new ClosingStructuresCalculationEntity
                    {
                        Order = 2,
                        Name = "calculation2",
                        IdenticalApertures = 1
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsClosingStructuresCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(4, rootChildren.Count);

            var rootChildCalculation1 = (StructuresCalculation<ClosingStructuresInput>) rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (StructuresCalculation<ClosingStructuresInput>) rootChildren[2];
            Assert.AreEqual("calculation2", rootChildCalculation2.Name);

            var rootChildGroup2 = (CalculationGroup) rootChildren[3];
            Assert.AreEqual("group2", rootChildGroup2.Name);
        }

        #endregion

        #region StabilityPointStructures

        [Test]
        public void ReadAsStabilityPointStructuresCalculationGroup_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new CalculationGroupEntity();

            // Call
            TestDelegate call = () => entity.ReadAsStabilityPointStructuresCalculationGroup(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void ReadAsStabilityPointStructuresCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren()
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsStabilityPointStructuresCalculationGroup(collector);

            // Assert
            Assert.AreEqual(entity.Name, group.Name);
            CollectionAssert.IsEmpty(group.Children);
        }

        [Test]
        public void ReadAsStabilityPointStructuresCalculationGroup_EntityWithChildGroups_CreateCalculationGroupWithChildGroups()
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsStabilityPointStructuresCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            List<ICalculationBase> rootChildren = rootGroup.Children;
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            List<ICalculationBase> rootChildGroup2Children = rootChildGroup2.Children;
            var rootChildGroup1Child1 = (CalculationGroup) rootChildGroup2Children[0];
            Assert.AreEqual("ABA", rootChildGroup1Child1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child1.Children);
            var rootChildGroup1Child2 = (CalculationGroup) rootChildGroup2Children[1];
            Assert.AreEqual("ABB", rootChildGroup1Child2.Name);
            CollectionAssert.IsEmpty(rootChildGroup1Child2.Children);
        }

        [Test]
        public void ReadAsStabilityPointStructuresCalculationGroup_EntityWithChildStabilityPointStructuresCalculations_CreateCalculationGroupWithChildCalculations()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                StabilityPointStructuresCalculationEntities =
                {
                    new StabilityPointStructuresCalculationEntity
                    {
                        Order = 0,
                        Name = "1"
                    },
                    new StabilityPointStructuresCalculationEntity
                    {
                        Order = 1,
                        Name = "2"
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup rootGroup = rootGroupEntity.ReadAsStabilityPointStructuresCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(2, rootChildren.Count);

            var rootChildCalculation1 = (StructuresCalculation<StabilityPointStructuresInput>) rootChildren[0];
            Assert.AreEqual("1", rootChildCalculation1.Name);

            var rootChildCalculation2 = (StructuresCalculation<StabilityPointStructuresInput>) rootChildren[1];
            Assert.AreEqual("2", rootChildCalculation2.Name);
        }

        [Test]
        public void ReadAsStabilityPointStructuresCalculationGroup_EntityWithChildStabilityPointStructuresCalculationAndGroups_CreateCalculationGroupWithChildCalculationsAndGroups()
        {
            // Setup
            var rootGroupEntity = new CalculationGroupEntity
            {
                Name = "A",
                StabilityPointStructuresCalculationEntities =
                {
                    new StabilityPointStructuresCalculationEntity
                    {
                        Order = 0,
                        Name = "calculation1"
                    },
                    new StabilityPointStructuresCalculationEntity
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsStabilityPointStructuresCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(4, rootChildren.Count);

            var rootChildCalculation1 = (StructuresCalculation<StabilityPointStructuresInput>) rootChildren[0];
            Assert.AreEqual("calculation1", rootChildCalculation1.Name);

            var rootChildGroup1 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("group1", rootChildGroup1.Name);

            var rootChildCalculation2 = (StructuresCalculation<StabilityPointStructuresInput>) rootChildren[2];
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
        public void ReadAsStabilityStoneCoverWaveConditionsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren()
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(entity.Name, group.Name);
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            List<ICalculationBase> rootChildren = rootGroup.Children;
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            List<ICalculationBase> rootChildGroup2Children = rootChildGroup2.Children;
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(2, rootChildren.Count);

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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(4, rootChildren.Count);

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
        public void ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup_EntityWithoutChildren_CreateCalculationGroupWithoutChildren()
        {
            // Setup
            var entity = new CalculationGroupEntity
            {
                Name = "A"
            };

            var collector = new ReadConversionCollector();

            // Call
            CalculationGroup group = entity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual(entity.Name, group.Name);
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector);

            // Assert
            Assert.AreEqual("A", rootGroup.Name);

            List<ICalculationBase> rootChildren = rootGroup.Children;
            var rootChildGroup1 = (CalculationGroup) rootChildren[0];
            Assert.AreEqual("AA", rootChildGroup1.Name);
            CollectionAssert.IsEmpty(rootChildGroup1.Children);
            var rootChildGroup2 = (CalculationGroup) rootChildren[1];
            Assert.AreEqual("AB", rootChildGroup2.Name);

            List<ICalculationBase> rootChildGroup2Children = rootChildGroup2.Children;
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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(2, rootChildren.Count);

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
            CalculationGroup rootGroup = rootGroupEntity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector);

            // Assert
            List<ICalculationBase> rootChildren = rootGroup.Children;
            Assert.AreEqual(4, rootChildren.Count);

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