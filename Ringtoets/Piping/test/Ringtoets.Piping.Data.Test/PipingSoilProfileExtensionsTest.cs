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

using System.Collections.Generic;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingSoilProfileExtensionsTest
    {
        #region GetTopmostConsecutiveAquiferLayerThicknessBelowLevel

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_NoAquiferLayer_NaN()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1),
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerAboveLevel_NaN()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerCompletelyBelowLevel_ReturnAquiferLayerThickness()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(2.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerPartlyBelowLevel_ReturnAquiferLayerThicknessUpTillLevel()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.6);

            // Assert
            Assert.AreEqual(1.6, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerTopEqualToLevel_ReturnAquiferLayerThickness()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(1.6)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.6);

            // Assert
            Assert.AreEqual(1.6, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TwoAquiferLayersCompletelyBelowLevel_ReturnConsecutiveAquiferLayerThickness()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(0.5)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(1.6, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerTopEqualToLevel_ReturnConsecutiveAquiferLayerThickness()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = false
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.1);

            // Assert
            Assert.AreEqual(1.6, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerTopPartlyBelowLevel_ReturnConsecutiveAquiferLayerThickness()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = false
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.0);

            // Assert
            Assert.AreEqual(1.5, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerCompletelyAboveLevel_ReturnBottomAquiferLayerThickness()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.5)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.3);

            // Assert
            Assert.AreEqual(1.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerPartlyAboveLevel_ReturnConsecutiveAquiferLayerThicknessUpTillLevel()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.5);

            // Assert
            Assert.AreEqual(1.5, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TwoConsecutiveAquiferLayersAndOneNonConsecutiveAquiferLayer_ReturnConsecutiveAquiferLayerThicknessUpTillLevel()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.0),
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.5);

            // Assert
            Assert.AreEqual(0.5, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AllAquiferLayersAboveLevel_NaN()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(0.6)
                {
                    IsAquifer = false
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(0.5);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_BottomAquiferLayerTopEqualToLevel_BottomAquiferLayerThickness()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.1);

            // Assert
            Assert.AreEqual(1.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_LevelBelowProfile_NaN()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.5, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(0.0);

            // Assert
            Assert.IsNaN(result);
        }

        #endregion

        #region GetConsecutiveAquiferLayersBelowLevel

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_NoAquiferLayer_ReturnsEmptyCollection()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1),
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerAboveLevel_ReturnsEmptyCollection()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerCompletelyBelowLevel_ReturnAquiferLayer()
        {
            // Setup
            var aquiferLayer = new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayer
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayer }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerPartlyBelowLevel_ReturnCollectionWithAquiferLayer()
        {
            // Setup
            var aquiferLayer = new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayer
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.6);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayer }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerTopEqualToLevel_ReturnCollectionWithAquiferLayer()
        {
            // Setup
            var aquiferLayer = new PipingSoilLayer(1.6)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayer
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.6);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayer }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TwoAquiferLayersCompletelyBelowLevel_ReturnConsecutiveAquiferLayers()
        {
            // Setup
            var aquiferLayerA = new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            };
            var aquiferLayerB = new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayerA,
                aquiferLayerB,
                new PipingSoilLayer(0.5)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayerA, aquiferLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerTopEqualToLevel_ReturnConsecutiveAquiferLayers()
        {
            // Setup
            var aquiferLayerA = new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            };
            var aquiferLayerB = new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayerA,
                aquiferLayerB,
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = false
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayerA, aquiferLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerTopPartlyBelowLevel_ReturnCollectionWithAquiferLayer()
        {
            // Setup
            var aquiferLayerA = new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            };
            var aquiferLayerB = new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayerA,
                aquiferLayerB,
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = false
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.0);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayerA, aquiferLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerCompletelyAboveLevel_ReturnCollectionWithoutTopmostAquiferLayer()
        {
            // Setup
            var aquiferLayerA = new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            };
            var aquiferLayerB = new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayerA,
                new PipingSoilLayer(1.5)
                {
                    IsAquifer = false
                },
                aquiferLayerB
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.3);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerPartlyAboveLevel_ReturnCollectionWithTopmostAquiferLayer()
        {
            // Setup
            var aquiferLayerA = new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            };
            var aquiferLayerB = new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayerA,
                aquiferLayerB
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.5);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayerA, aquiferLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TwoConsecutiveAquiferLayersAndOneNonConsecutiveAquiferLayer_ReturnConsecutiveAquiferLayers()
        {
            // Setup
            var aquiferLayerA = new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            };
            var aquiferLayerB = new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayerA,
                aquiferLayerB,
                new PipingSoilLayer(1.0),
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.5);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayerA, aquiferLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AllAquiferLayersAboveLevel_ReturnsEmptyCollection()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(0.6)
                {
                    IsAquifer = false
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(0.5);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_BottomAquiferLayerTopEqualToLevel_ReturnsCollectionWithBottomAquiferLayer()
        {
            // Setup
            var aquiferLayerA = new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            };
            var aquiferLayerB = new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            };
            var pipingSoilLayers = new[]
            {
                aquiferLayerA,
                aquiferLayerB
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.1);

            // Assert
            CollectionAssert.AreEqual(new[] { aquiferLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_LevelBelowProfile_ReturnsEmptyCollection()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.5, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(0.0);

            // Assert
            Assert.IsEmpty(result);
        }

        #endregion

        #region GetConsecutiveAquitardLayersBelowLevel

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_NoAquitardLayer_ReturnsEmptyCollection()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AquitardLayerAboveLevel_ReturnsEmptyCollection()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1),
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AquitardLayerCompletelyBelowLevel_ReturnAquitardLayer()
        {
            // Setup
            var aquitardLayer = new PipingSoilLayer(2.1);
            var pipingSoilLayers = new[]
            {
                aquitardLayer
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayer }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AquitardLayerPartlyBelowLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            var aquitardLayer = new PipingSoilLayer(2.1);
            var pipingSoilLayers = new[]
            {
                aquitardLayer
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.6);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayer }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AquitardLayerTopEqualToLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            var aquitardLayer = new PipingSoilLayer(1.6);
            var pipingSoilLayers = new[]
            {
                aquitardLayer
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.6);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayer }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TwoAquitardLayersCompletelyBelowLevel_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            var aquitardLayerA = new PipingSoilLayer(2.1);
            var aquitardLayerB = new PipingSoilLayer(1.1);
            var pipingSoilLayers = new[]
            {
                aquitardLayerA,
                aquitardLayerB,
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayerA, aquitardLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TopmostAquitardLayerTopEqualToLevel_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            var aquitardLayerA = new PipingSoilLayer(2.1);
            var aquitardLayerB = new PipingSoilLayer(1.1);
            var pipingSoilLayers = new[]
            {
                aquitardLayerA,
                aquitardLayerB,
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayerA, aquitardLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TopmostAquitardLayerTopPartlyBelowLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            var aquitardLayerA = new PipingSoilLayer(2.1);
            var aquitardLayerB = new PipingSoilLayer(1.1);
            var pipingSoilLayers = new[]
            {
                aquitardLayerA,
                aquitardLayerB,
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(2.0);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayerA, aquitardLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TopmostAquitardLayerCompletelyAboveLevel_ReturnCollectionWithoutTopmostAquitardLayer()
        {
            // Setup
            var aquitardLayerA = new PipingSoilLayer(2.1);
            var aquitardLayerB = new PipingSoilLayer(1.1);
            var pipingSoilLayers = new[]
            {
                aquitardLayerA,
                new PipingSoilLayer(1.5)
                {
                    IsAquifer = true
                },
                aquitardLayerB
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.3);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TopmostAquitardLayerPartlyAboveLevel_ReturnCollectionWithTopmostAquitardLayer()
        {
            // Setup
            var aquitardLayerA = new PipingSoilLayer(2.1);
            var aquitardLayerB = new PipingSoilLayer(1.1);
            var pipingSoilLayers = new[]
            {
                aquitardLayerA,
                aquitardLayerB
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.5);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayerA, aquitardLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TwoConsecutiveAquitardLayersAndOneNonConsecutiveAquitardLayer_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            var aquitardLayerA = new PipingSoilLayer(2.1);
            var aquitardLayerB = new PipingSoilLayer(1.1);
            var pipingSoilLayers = new[]
            {
                aquitardLayerA,
                aquitardLayerB,
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(0.5)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.5);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayerA, aquitardLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AllAquitardLayersAboveLevel_ReturnsEmptyCollection()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(0.6)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(0.5);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_BottomAquitardLayerTopEqualToLevel_ReturnsCollectionWithBottomAquitardLayer()
        {
            // Setup
            var aquitardLayerA = new PipingSoilLayer(2.1)
            {
                IsAquifer = false
            };
            var aquitardLayerB = new PipingSoilLayer(1.1)
            {
                IsAquifer = false
            };
            var pipingSoilLayers = new[]
            {
                aquitardLayerA,
                aquitardLayerB
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.1);

            // Assert
            CollectionAssert.AreEqual(new[] { aquitardLayerB }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_LevelBelowProfile_ReturnsEmptyCollection()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.5, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(0.0);

            // Assert
            Assert.IsEmpty(result);
        }

        #endregion

    }
}