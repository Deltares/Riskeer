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
using System.Linq;
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
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class CalculationGroupCreateExtensionsTest
    {
        [Test]
        public void Create_RegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var group = new CalculationGroup();

            // Call
            TestDelegate call = () => group.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        [TestCase(0)]
        [TestCase(123)]
        public void Create_GroupWithoutChildren_CreateEntity(int order)
        {
            // Setup
            const string name = "blaballab";
            var group = new CalculationGroup
            {
                Name = name
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, order);

            // Assert
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(order, entity.Order);

            CollectionAssert.IsEmpty(entity.CalculationGroupEntity1);
            CollectionAssert.IsEmpty(entity.FailureMechanismEntities);
            Assert.IsNull(entity.ParentCalculationGroupEntityId);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "original";
            var group = new CalculationGroup
            {
                Name = name
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            Assert.AreNotSame(name, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(name, entity.Name);
        }

        [Test]
        public void Create_GroupWithChildren_CreateEntities()
        {
            // Setup
            const string name = "blaballab";
            var group = new CalculationGroup
            {
                Name = name
            };
            group.Children.Add(new CalculationGroup
            {
                Name = "A",
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "AA"
                    },
                    new CalculationGroup
                    {
                        Name = "AB"
                    }
                }
            });
            group.Children.Add(new CalculationGroup
            {
                Name = "B"
            });

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(0, entity.Order);

            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            Assert.AreEqual(2, childEntity1.CalculationGroupEntity1.Count);
            CalculationGroupEntity childEntity1ChildEntity1 = childEntity1.CalculationGroupEntity1.ElementAt(0);
            Assert.AreEqual("AA", childEntity1ChildEntity1.Name);
            Assert.AreEqual(0, childEntity1ChildEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1ChildEntity1.CalculationGroupEntity1);
            CalculationGroupEntity childEntity1ChildEntity2 = childEntity1.CalculationGroupEntity1.ElementAt(1);
            Assert.AreEqual("AB", childEntity1ChildEntity2.Name);
            Assert.AreEqual(1, childEntity1ChildEntity2.Order);
            CollectionAssert.IsEmpty(childEntity1ChildEntity2.CalculationGroupEntity1);

            CalculationGroupEntity childEntity2 = childGroupEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
            CollectionAssert.IsEmpty(childEntity2.CalculationGroupEntity1);
        }

        [Test]
        public void Create_GroupWithChildPipingCalculations_CreateEntities()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var group = new CalculationGroup
            {
                Children =
                {
                    new PipingCalculationScenario(generalInputParameters)
                    {
                        Name = "A"
                    },
                    new PipingCalculationScenario(generalInputParameters)
                    {
                        Name = "B"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            PipingCalculationEntity[] childCalculationEntities = entity.PipingCalculationEntities.ToArray();
            Assert.AreEqual(2, childCalculationEntities.Length);

            PipingCalculationEntity childEntity1 = childCalculationEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            PipingCalculationEntity childEntity2 = childCalculationEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
        }

        [Test]
        public void Create_GroupWithChildPipingCalculationsAndChildCalculationGroups_CreateEntities()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var group = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "A"
                    },
                    new PipingCalculationScenario(generalInputParameters)
                    {
                        Name = "B"
                    },
                    new CalculationGroup
                    {
                        Name = "C"
                    },
                    new PipingCalculationScenario(generalInputParameters)
                    {
                        Name = "D"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            PipingCalculationEntity[] childCalculationEntities = entity.PipingCalculationEntities.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual(2, childCalculationEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1.CalculationGroupEntity1);

            PipingCalculationEntity childEntity2 = childCalculationEntities[0];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);

            CalculationGroupEntity childEntity3 = childGroupEntities[1];
            Assert.AreEqual("C", childEntity3.Name);
            Assert.AreEqual(2, childEntity3.Order);
            CollectionAssert.IsEmpty(childEntity3.CalculationGroupEntity1);

            PipingCalculationEntity childEntity4 = childCalculationEntities[1];
            Assert.AreEqual("D", childEntity4.Name);
            Assert.AreEqual(3, childEntity4.Order);
        }

        [Test]
        public void Create_GroupWithMacroStabilityInwardsCalculations_CreatesEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new MacroStabilityInwardsCalculationScenario
                    {
                        Name = "A"
                    },
                    new MacroStabilityInwardsCalculationScenario
                    {
                        Name = "B"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            MacroStabilityInwardsCalculationEntity[] childCalculationEntities = entity.MacroStabilityInwardsCalculationEntities.ToArray();
            Assert.AreEqual(2, childCalculationEntities.Length);

            MacroStabilityInwardsCalculationEntity childEntity1 = childCalculationEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);

            MacroStabilityInwardsCalculationEntity childEntity2 = childCalculationEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
        }

        [Test]
        public void Create_GroupWithChildMacroStabilityInwardsCalculationsAndChildCalculationGroups_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "A"
                    },
                    new MacroStabilityInwardsCalculationScenario
                    {
                        Name = "B"
                    },
                    new CalculationGroup
                    {
                        Name = "C"
                    },
                    new MacroStabilityInwardsCalculationScenario
                    {
                        Name = "D"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            MacroStabilityInwardsCalculationEntity[] childCalculationEntities = entity.MacroStabilityInwardsCalculationEntities.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual(2, childCalculationEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1.CalculationGroupEntity1);

            MacroStabilityInwardsCalculationEntity childEntity2 = childCalculationEntities[0];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);

            CalculationGroupEntity childEntity3 = childGroupEntities[1];
            Assert.AreEqual("C", childEntity3.Name);
            Assert.AreEqual(2, childEntity3.Order);
            CollectionAssert.IsEmpty(childEntity3.CalculationGroupEntity1);

            MacroStabilityInwardsCalculationEntity childEntity4 = childCalculationEntities[1];
            Assert.AreEqual("D", childEntity4.Name);
            Assert.AreEqual(3, childEntity4.Order);
        }

        [Test]
        public void Create_GroupWithChildGrassCoverErosionInwardsCalculations_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation
                    {
                        Name = "A"
                    },
                    new GrassCoverErosionInwardsCalculation
                    {
                        Name = "B"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            GrassCoverErosionInwardsCalculationEntity[] childCalculationEntities = entity.GrassCoverErosionInwardsCalculationEntities.ToArray();
            Assert.AreEqual(2, childCalculationEntities.Length);

            GrassCoverErosionInwardsCalculationEntity childEntity1 = childCalculationEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            GrassCoverErosionInwardsCalculationEntity childEntity2 = childCalculationEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
        }

        [Test]
        public void Create_GroupWithChildGrassCoverErosionInwardCalculationsAndChildCalculationGroups_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "A"
                    },
                    new GrassCoverErosionInwardsCalculation
                    {
                        Name = "B"
                    },
                    new CalculationGroup
                    {
                        Name = "C"
                    },
                    new GrassCoverErosionInwardsCalculation
                    {
                        Name = "D"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            GrassCoverErosionInwardsCalculationEntity[] childCalculationEntities = entity.GrassCoverErosionInwardsCalculationEntities.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual(2, childCalculationEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1.CalculationGroupEntity1);

            GrassCoverErosionInwardsCalculationEntity childEntity2 = childCalculationEntities[0];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);

            CalculationGroupEntity childEntity3 = childGroupEntities[1];
            Assert.AreEqual("C", childEntity3.Name);
            Assert.AreEqual(2, childEntity3.Order);
            CollectionAssert.IsEmpty(childEntity3.CalculationGroupEntity1);

            GrassCoverErosionInwardsCalculationEntity childEntity4 = childCalculationEntities[1];
            Assert.AreEqual("D", childEntity4.Name);
            Assert.AreEqual(3, childEntity4.Order);
        }

        [Test]
        public void Create_GroupWithChildGrassCoverErosionOutwardsWaveConditionsCalculations_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
                    {
                        Name = "A"
                    },
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
                    {
                        Name = "B"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            GrassCoverErosionOutwardsWaveConditionsCalculationEntity[] childCalculationEntities = entity.GrassCoverErosionOutwardsWaveConditionsCalculationEntities.ToArray();
            Assert.AreEqual(2, childCalculationEntities.Length);

            GrassCoverErosionOutwardsWaveConditionsCalculationEntity childEntity1 = childCalculationEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            GrassCoverErosionOutwardsWaveConditionsCalculationEntity childEntity2 = childCalculationEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
        }

        [Test]
        public void Create_GroupWithChildGrassCoverErosionOutwardsWaveConditionsCalculationsAndChildCalculationGroups_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "A"
                    },
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
                    {
                        Name = "B"
                    },
                    new CalculationGroup
                    {
                        Name = "C"
                    },
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
                    {
                        Name = "D"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            GrassCoverErosionOutwardsWaveConditionsCalculationEntity[] childCalculationEntities = entity.GrassCoverErosionOutwardsWaveConditionsCalculationEntities.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual(2, childCalculationEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1.CalculationGroupEntity1);

            GrassCoverErosionOutwardsWaveConditionsCalculationEntity childEntity2 = childCalculationEntities[0];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);

            CalculationGroupEntity childEntity3 = childGroupEntities[1];
            Assert.AreEqual("C", childEntity3.Name);
            Assert.AreEqual(2, childEntity3.Order);
            CollectionAssert.IsEmpty(childEntity3.CalculationGroupEntity1);

            GrassCoverErosionOutwardsWaveConditionsCalculationEntity childEntity4 = childCalculationEntities[1];
            Assert.AreEqual("D", childEntity4.Name);
            Assert.AreEqual(3, childEntity4.Order);
        }

        [Test]
        public void Create_GroupWithChildHeightStructuresCalculations_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<HeightStructuresInput>
                    {
                        Name = "A"
                    },
                    new StructuresCalculation<HeightStructuresInput>
                    {
                        Name = "B"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            HeightStructuresCalculationEntity[] childCalculationEntities = entity.HeightStructuresCalculationEntities.ToArray();
            Assert.AreEqual(2, childCalculationEntities.Length);

            HeightStructuresCalculationEntity childEntity1 = childCalculationEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            HeightStructuresCalculationEntity childEntity2 = childCalculationEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
        }

        [Test]
        public void Create_GroupWithChildHeightStructuresCalculationsAndChildCalculationGroups_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "A"
                    },
                    new StructuresCalculation<HeightStructuresInput>
                    {
                        Name = "B"
                    },
                    new CalculationGroup
                    {
                        Name = "C"
                    },
                    new StructuresCalculation<HeightStructuresInput>
                    {
                        Name = "D"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            HeightStructuresCalculationEntity[] childCalculationEntities = entity.HeightStructuresCalculationEntities.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual(2, childCalculationEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1.CalculationGroupEntity1);

            HeightStructuresCalculationEntity childEntity2 = childCalculationEntities[0];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);

            CalculationGroupEntity childEntity3 = childGroupEntities[1];
            Assert.AreEqual("C", childEntity3.Name);
            Assert.AreEqual(2, childEntity3.Order);
            CollectionAssert.IsEmpty(childEntity3.CalculationGroupEntity1);

            HeightStructuresCalculationEntity childEntity4 = childCalculationEntities[1];
            Assert.AreEqual("D", childEntity4.Name);
            Assert.AreEqual(3, childEntity4.Order);
        }

        [Test]
        public void Create_GroupWithChildStabilityStoneCoverWaveConditionsCalculations_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StabilityStoneCoverWaveConditionsCalculation
                    {
                        Name = "A"
                    },
                    new StabilityStoneCoverWaveConditionsCalculation
                    {
                        Name = "B"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            StabilityStoneCoverWaveConditionsCalculationEntity[] childCalculationEntities = entity.StabilityStoneCoverWaveConditionsCalculationEntities.ToArray();
            Assert.AreEqual(2, childCalculationEntities.Length);

            StabilityStoneCoverWaveConditionsCalculationEntity childEntity1 = childCalculationEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            StabilityStoneCoverWaveConditionsCalculationEntity childEntity2 = childCalculationEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
        }

        [Test]
        public void Create_GroupWithChildStabilityStoneCoverWaveConditionsCalculationsAndChildCalculationGroups_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "A"
                    },
                    new StabilityStoneCoverWaveConditionsCalculation
                    {
                        Name = "B"
                    },
                    new CalculationGroup
                    {
                        Name = "C"
                    },
                    new StabilityStoneCoverWaveConditionsCalculation
                    {
                        Name = "D"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            StabilityStoneCoverWaveConditionsCalculationEntity[] childCalculationEntities = entity.StabilityStoneCoverWaveConditionsCalculationEntities.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual(2, childCalculationEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1.CalculationGroupEntity1);

            StabilityStoneCoverWaveConditionsCalculationEntity childEntity2 = childCalculationEntities[0];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);

            CalculationGroupEntity childEntity3 = childGroupEntities[1];
            Assert.AreEqual("C", childEntity3.Name);
            Assert.AreEqual(2, childEntity3.Order);
            CollectionAssert.IsEmpty(childEntity3.CalculationGroupEntity1);

            StabilityStoneCoverWaveConditionsCalculationEntity childEntity4 = childCalculationEntities[1];
            Assert.AreEqual("D", childEntity4.Name);
            Assert.AreEqual(3, childEntity4.Order);
        }

        [Test]
        public void Create_GroupWithChildWaveImpactAsphaltCoverWaveConditionsCalculations_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new WaveImpactAsphaltCoverWaveConditionsCalculation
                    {
                        Name = "A"
                    },
                    new WaveImpactAsphaltCoverWaveConditionsCalculation
                    {
                        Name = "B"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity[] childCalculationEntities = entity.WaveImpactAsphaltCoverWaveConditionsCalculationEntities.ToArray();
            Assert.AreEqual(2, childCalculationEntities.Length);

            WaveImpactAsphaltCoverWaveConditionsCalculationEntity childEntity1 = childCalculationEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity childEntity2 = childCalculationEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
        }

        [Test]
        public void Create_GroupWithChildWaveImpactAsphaltCoverWaveConditionsCalculationsAndChildCalculationGroups_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "A"
                    },
                    new WaveImpactAsphaltCoverWaveConditionsCalculation
                    {
                        Name = "B"
                    },
                    new CalculationGroup
                    {
                        Name = "C"
                    },
                    new WaveImpactAsphaltCoverWaveConditionsCalculation
                    {
                        Name = "D"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity[] childCalculationEntities = entity.WaveImpactAsphaltCoverWaveConditionsCalculationEntities.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual(2, childCalculationEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1.CalculationGroupEntity1);

            WaveImpactAsphaltCoverWaveConditionsCalculationEntity childEntity2 = childCalculationEntities[0];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);

            CalculationGroupEntity childEntity3 = childGroupEntities[1];
            Assert.AreEqual("C", childEntity3.Name);
            Assert.AreEqual(2, childEntity3.Order);
            CollectionAssert.IsEmpty(childEntity3.CalculationGroupEntity1);

            WaveImpactAsphaltCoverWaveConditionsCalculationEntity childEntity4 = childCalculationEntities[1];
            Assert.AreEqual("D", childEntity4.Name);
            Assert.AreEqual(3, childEntity4.Order);
        }

        [Test]
        public void Create_GroupWithChildClosingStructuresCalculations_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<ClosingStructuresInput>
                    {
                        Name = "A"
                    },
                    new StructuresCalculation<ClosingStructuresInput>
                    {
                        Name = "B"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            ClosingStructuresCalculationEntity[] childCalculationEntities = entity.ClosingStructuresCalculationEntities.ToArray();
            Assert.AreEqual(2, childCalculationEntities.Length);

            ClosingStructuresCalculationEntity childEntity1 = childCalculationEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            ClosingStructuresCalculationEntity childEntity2 = childCalculationEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
        }

        [Test]
        public void Create_GroupWithChildClosingStructuresCalculationsAndChildCalculationGroups_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "A"
                    },
                    new StructuresCalculation<ClosingStructuresInput>
                    {
                        Name = "B"
                    },
                    new CalculationGroup
                    {
                        Name = "C"
                    },
                    new StructuresCalculation<ClosingStructuresInput>
                    {
                        Name = "D"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            ClosingStructuresCalculationEntity[] childCalculationEntities = entity.ClosingStructuresCalculationEntities.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual(2, childCalculationEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1.CalculationGroupEntity1);

            ClosingStructuresCalculationEntity childEntity2 = childCalculationEntities[0];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);

            CalculationGroupEntity childEntity3 = childGroupEntities[1];
            Assert.AreEqual("C", childEntity3.Name);
            Assert.AreEqual(2, childEntity3.Order);
            CollectionAssert.IsEmpty(childEntity3.CalculationGroupEntity1);

            ClosingStructuresCalculationEntity childEntity4 = childCalculationEntities[1];
            Assert.AreEqual("D", childEntity4.Name);
            Assert.AreEqual(3, childEntity4.Order);
        }

        [Test]
        public void Create_GroupWithChildStabilityPointStructuresCalculations_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<StabilityPointStructuresInput>
                    {
                        Name = "A"
                    },
                    new StructuresCalculation<StabilityPointStructuresInput>
                    {
                        Name = "B"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            StabilityPointStructuresCalculationEntity[] childCalculationEntities = entity.StabilityPointStructuresCalculationEntities.ToArray();
            Assert.AreEqual(2, childCalculationEntities.Length);

            StabilityPointStructuresCalculationEntity childEntity1 = childCalculationEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            StabilityPointStructuresCalculationEntity childEntity2 = childCalculationEntities[1];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);
        }

        [Test]
        public void Create_GroupWithChildStabilityPointStructuresCalculationsAndChildCalculationGroups_CreateEntities()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup
                    {
                        Name = "A"
                    },
                    new StructuresCalculation<StabilityPointStructuresInput>
                    {
                        Name = "B"
                    },
                    new CalculationGroup
                    {
                        Name = "C"
                    },
                    new StructuresCalculation<StabilityPointStructuresInput>
                    {
                        Name = "D"
                    }
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            CalculationGroupEntity entity = group.Create(registry, 0);

            // Assert
            CalculationGroupEntity[] childGroupEntities = entity.CalculationGroupEntity1.ToArray();
            StabilityPointStructuresCalculationEntity[] childCalculationEntities = entity.StabilityPointStructuresCalculationEntities.ToArray();
            Assert.AreEqual(2, childGroupEntities.Length);
            Assert.AreEqual(2, childCalculationEntities.Length);

            CalculationGroupEntity childEntity1 = childGroupEntities[0];
            Assert.AreEqual("A", childEntity1.Name);
            Assert.AreEqual(0, childEntity1.Order);
            CollectionAssert.IsEmpty(childEntity1.CalculationGroupEntity1);

            StabilityPointStructuresCalculationEntity childEntity2 = childCalculationEntities[0];
            Assert.AreEqual("B", childEntity2.Name);
            Assert.AreEqual(1, childEntity2.Order);

            CalculationGroupEntity childEntity3 = childGroupEntities[1];
            Assert.AreEqual("C", childEntity3.Name);
            Assert.AreEqual(2, childEntity3.Order);
            CollectionAssert.IsEmpty(childEntity3.CalculationGroupEntity1);

            StabilityPointStructuresCalculationEntity childEntity4 = childCalculationEntities[1];
            Assert.AreEqual("D", childEntity4.Name);
            Assert.AreEqual(3, childEntity4.Order);
        }
    }
}