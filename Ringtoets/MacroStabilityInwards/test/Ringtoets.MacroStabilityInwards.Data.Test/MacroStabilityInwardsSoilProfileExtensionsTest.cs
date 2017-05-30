// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileExtensionsTest
    {
        private readonly MacroStabilityInwardsSoilLayer[] testCaseOneAquiferLayer =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = true
            }
        };

        private readonly MacroStabilityInwardsSoilLayer[] testCaseOneAquitardLayer =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = false
            }
        };

        private readonly MacroStabilityInwardsSoilLayer[] testCaseOneCoverageLayer =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = false
            },
            new MacroStabilityInwardsSoilLayer(0.1)
            {
                IsAquifer = true
            }
        };

        private readonly MacroStabilityInwardsSoilLayer[] testCaseTwoAquitardLayers =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = false
            },
            new MacroStabilityInwardsSoilLayer(1.1)
            {
                IsAquifer = false
            }
        };

        private readonly MacroStabilityInwardsSoilLayer[] testCaseTwoAquiferLayers =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = true
            },
            new MacroStabilityInwardsSoilLayer(1.1)
            {
                IsAquifer = true
            }
        };

        private readonly MacroStabilityInwardsSoilLayer[] testCaseTwoCoverageLayers =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = false
            },
            new MacroStabilityInwardsSoilLayer(1.1)
            {
                IsAquifer = false
            },
            new MacroStabilityInwardsSoilLayer(0.1)
            {
                IsAquifer = true
            }
        };

        private readonly MacroStabilityInwardsSoilLayer[] testCaseOneAquiferLayerOneAquitardLayer =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = true
            },
            new MacroStabilityInwardsSoilLayer(1.1)
            {
                IsAquifer = false
            }
        };

        private readonly MacroStabilityInwardsSoilLayer[] testCaseOneAquitardLayerOneAquiferLayer =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = false
            },
            new MacroStabilityInwardsSoilLayer(1.1)
            {
                IsAquifer = true
            }
        };

        private readonly MacroStabilityInwardsSoilLayer[] testCaseTwoAquiferLayersOneAquitardLayerOneAquiferLayer =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = true
            },
            new MacroStabilityInwardsSoilLayer(1.1)
            {
                IsAquifer = true
            },
            new MacroStabilityInwardsSoilLayer(1.0)
            {
                IsAquifer = false
            },
            new MacroStabilityInwardsSoilLayer(0.5)
            {
                IsAquifer = true
            }
        };

        private readonly MacroStabilityInwardsSoilLayer[] testCaseTwoCoverageLayersOneAquiferLayerOneAquitardLayer =
        {
            new MacroStabilityInwardsSoilLayer(2.1)
            {
                IsAquifer = false
            },
            new MacroStabilityInwardsSoilLayer(1.1)
            {
                IsAquifer = false
            },
            new MacroStabilityInwardsSoilLayer(1.0)
            {
                IsAquifer = true
            },
            new MacroStabilityInwardsSoilLayer(0.5)
            {
                IsAquifer = false
            }
        };

        private MacroStabilityInwardsSoilProfile CreateTestProfile(MacroStabilityInwardsSoilLayer[] layers)
        {
            return new MacroStabilityInwardsSoilProfile(string.Empty, 0.0, layers, SoilProfileType.SoilProfile1D, 0);
        }

        #region GetTopmostConsecutiveAquiferLayerThicknessBelowLevel

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_NoAquiferLayer_ReturnsNaN()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquitardLayers);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerAboveLevel_ReturnsNaN()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayerOneAquitardLayer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerCompletelyBelowLevel_ReturnAquiferLayerThickness()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(2.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerPartlyBelowLevel_ReturnAquiferLayerThicknessUpTillLevel()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.6);

            // Assert
            Assert.AreEqual(1.6, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerTopEqualToLevel_ReturnAquiferLayerThickness()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.1);

            // Assert
            Assert.AreEqual(2.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerBottomEqualToLevel_ReturnsNaN()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(0.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TwoAquiferLayersCompletelyBelowLevel_ReturnConsecutiveAquiferLayerThickness()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(2.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerTopEqualToLevel_ReturnConsecutiveAquiferLayerThickness()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.1);

            // Assert
            Assert.AreEqual(2.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerTopPartlyBelowLevel_ReturnConsecutiveAquiferLayerThickness()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.0);

            // Assert
            Assert.AreEqual(2.0, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerCompletelyAboveLevel_ReturnBottomAquiferLayerThickness()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(0.5);

            // Assert
            Assert.AreEqual(0.5, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_BottomAquiferLayerTopEqualToLevel_BottomAquiferLayerThickness()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.1);

            // Assert
            Assert.AreEqual(1.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TwoConsecutiveAquiferLayersAndOneNonConsecutiveAquiferLayer_ReturnConsecutiveAquiferLayerThicknessUpTillLevel()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayersOneAquitardLayerOneAquiferLayer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.5);

            // Assert
            Assert.AreEqual(0.5, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_LevelBelowProfile_ReturnsNaN()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(-1.0);

            // Assert
            Assert.IsNaN(result);
        }

        #endregion

        #region GetConsecutiveCoverageLayerThicknessBelowLevel

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_NoAquitardLayer_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(1.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_AquitardLayerAboveLevel_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquitardLayerOneAquiferLayer);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(1.1);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_OnlyAquitardLayer_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquitardLayer);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_CoverageLayerCompletelyBelowLevel_ReturnAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneCoverageLayer);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(2.0, result, 1e-6);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_CoverageLayerPartlyBelowLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneCoverageLayer);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(1.6);

            // Assert
            Assert.AreEqual(1.5, result, 1e-6);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_CoverageLayerTopEqualToLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneCoverageLayer);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(2.1);

            // Assert
            Assert.AreEqual(2.0, result, 1e-6);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_CoverageLayerBottomEqualToLevel_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneCoverageLayer);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(0.1);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_TwoCoverageLayersCompletelyBelowLevel_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(2.0, result, 1e-6);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_TopmostCoverageLayerTopEqualToLevel_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(2.1);

            // Assert
            Assert.AreEqual(2.0, result, 1e-6);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_TopmostCoverageLayerTopPartlyBelowLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(2.0);

            // Assert
            Assert.AreEqual(1.9, result, 1e-6);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_TopmostCoverageLayerCompletelyAboveLevel_ReturnCollectionWithoutTopmostAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(0.5);

            // Assert
            Assert.AreEqual(0.4, result, 1e-6);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_BottomCoverageLayerTopEqualToLevel_ReturnCollectionWithBottomAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(1.1);

            // Assert
            Assert.AreEqual(1.0, result, 1e-6);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_TwoConsecutiveCoverageLayersAndOneNonConsecutiveAquitardLayer_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayersOneAquiferLayerOneAquitardLayer);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(1.5);

            // Assert
            Assert.AreEqual(0.5, result, 1e-6);
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(0.8)]
        [TestCase(0.5)]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_NoCoverageLayerAtLevel_ReturnEmptyCollection(double level)
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayersOneAquiferLayerOneAquitardLayer);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(level);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayerThicknessBelowLevel_LevelBelowProfile_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            double result = profile.GetConsecutiveCoverageLayerThicknessBelowLevel(-1.0);

            // Assert
            Assert.IsNaN(result);
        }

        #endregion

        #region GetConsecutiveAquiferLayersBelowLevel

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_NoAquiferLayer_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquitardLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerAboveLevel_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayerOneAquitardLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerCompletelyBelowLevel_ReturnAquiferLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                profile.Layers.ElementAt(0)
            }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerPartlyBelowLevel_ReturnCollectionWithAquiferLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.6);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                profile.Layers.ElementAt(0)
            }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerTopEqualToLevel_ReturnCollectionWithAquiferLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                profile.Layers.ElementAt(0)
            }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerBottomEqualToLevel_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquiferLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(0.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TwoAquiferLayersCompletelyBelowLevel_ReturnConsecutiveAquiferLayers()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerTopEqualToLevel_ReturnConsecutiveAquiferLayers()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerTopPartlyBelowLevel_ReturnCollectionWithAquiferLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.0);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerCompletelyAboveLevel_ReturnCollectionWithoutTopmostAquiferLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(0.5);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                profile.Layers.ElementAt(1)
            }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_BottomAquiferLayerTopEqualToLevel_ReturnCollectionWithBottomAquiferLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.1);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                profile.Layers.ElementAt(1)
            }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TwoConsecutiveAquiferLayersAndOneNonConsecutiveAquiferLayer_ReturnConsecutiveAquiferLayers()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayersOneAquitardLayerOneAquiferLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.5);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(2), result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_LevelBelowProfile_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(-1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        #endregion

        #region GetConsecutiveCoverageLayersBelowLevel

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_NoAquitardLayer_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoAquiferLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_AquitardLayerAboveLevel_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquitardLayerOneAquiferLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(1.1);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_OnlyAquitardLayer_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneAquitardLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(2.2);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_CoverageLayerCompletelyBelowLevel_ReturnAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneCoverageLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(1), result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_CoverageLayerPartlyBelowLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneCoverageLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(1.6);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(1), result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_CoverageLayerTopEqualToLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneCoverageLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(1), result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_CoverageLayerBottomEqualToLevel_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseOneCoverageLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(0.1);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_TwoCoverageLayersCompletelyBelowLevel_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(2), result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_TopmostCoverageLayerTopEqualToLevel_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(2), result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_TopmostCoverageLayerTopPartlyBelowLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(2.0);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(2), result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_TopmostCoverageLayerCompletelyAboveLevel_ReturnCollectionWithoutTopmostAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(0.5);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                profile.Layers.ElementAt(1)
            }, result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_BottomCoverageLayerTopEqualToLevel_ReturnCollectionWithBottomAquitardLayer()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(1.1);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                profile.Layers.ElementAt(1)
            }, result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_TwoConsecutiveCoverageLayersAndOneNonConsecutiveAquitardLayer_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayersOneAquiferLayerOneAquitardLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(1.5);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(2), result);
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(0.8)]
        [TestCase(0.5)]
        public void GetConsecutiveCoverageLayersBelowLevel_NoCoverageLayerAtLevel_ReturnEmptyCollection(double level)
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayersOneAquiferLayerOneAquitardLayer);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(level);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveCoverageLayersBelowLevel_LevelBelowProfile_ReturnEmptyCollection()
        {
            // Setup
            MacroStabilityInwardsSoilProfile profile = CreateTestProfile(testCaseTwoCoverageLayers);

            // Call
            IEnumerable<MacroStabilityInwardsSoilLayer> result = profile.GetConsecutiveCoverageLayersBelowLevel(-1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        #endregion
    }
}