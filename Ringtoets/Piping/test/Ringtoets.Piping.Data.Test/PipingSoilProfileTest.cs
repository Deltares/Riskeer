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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;
using PrimitivesResources = Ringtoets.Piping.Primitives.Properties.Resources;

namespace Ringtoets.Piping.Data.Test
{
    public class PipingSoilProfileTest
    {
        [Test]
        [TestCase(SoilProfileType.SoilProfile1D)]
        [TestCase(SoilProfileType.SoilProfile2D)]
        public void Constructor_WithNameBottomLayersAndAquifer_ReturnsInstanceWithPropsAndEquivalentLayerCollection(SoilProfileType type)
        {
            // Setup
            var name = "Profile";
            var random = new Random(22);
            var bottom = random.NextDouble();
            var layers = new Collection<PipingSoilLayer>
            {
                new PipingSoilLayer(bottom)
            };
            const long pipingSoilProfileId = 1234L;

            // Call
            var profile = new PipingSoilProfile(name, bottom, layers, type, pipingSoilProfileId);

            // Assert
            Assert.AreNotSame(layers, profile.Layers);
            Assert.AreEqual(name, profile.Name);
            Assert.AreEqual(bottom, profile.Bottom);
            Assert.AreEqual(type, profile.SoilProfileType);
            Assert.AreEqual(pipingSoilProfileId, profile.PipingSoilProfileId);
        }

        [Test]
        public void Constructor_WithNameBottomLayersEmpty_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new PipingSoilProfile(String.Empty, Double.NaN, new Collection<PipingSoilLayer>(), SoilProfileType.SoilProfile1D, 0);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, PrimitivesResources.Error_Cannot_Construct_PipingSoilProfile_Without_Layers);
        }

        [Test]
        public void Constructor_WithNameBottomLayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingSoilProfile(String.Empty, Double.NaN, null, SoilProfileType.SoilProfile1D, 0);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, PrimitivesResources.Error_Cannot_Construct_PipingSoilProfile_Without_Layers);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(15)]
        public void Layers_Always_ReturnsDescendingByTopOrderedList(int layerCount)
        {
            // Setup
            var random = new Random(21);
            var bottom = 0.0;
            var equivalentLayers = new List<PipingSoilLayer>(layerCount);
            for (var i = 0; i < layerCount; i++)
            {
                equivalentLayers.Add(new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = i == 0
                });
            }

            var profile = new PipingSoilProfile(string.Empty, bottom, equivalentLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = profile.Layers.ToArray();

            // Assert
            CollectionAssert.AreEquivalent(equivalentLayers, result);
            CollectionAssert.AreEqual(equivalentLayers.OrderByDescending(l => l.Top).Select(l => l.Top), result.Select(l => l.Top));
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(4)]
        public void Constructor_WithNameBottomLayersBelowBottom_ThrowsArgumentException(double deltaBelowBottom)
        {
            // Setup
            var bottom = 0.0;
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(bottom - deltaBelowBottom),
                new PipingSoilLayer(1.1)
            };

            // Call
            TestDelegate test = () => new PipingSoilProfile(String.Empty, bottom, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "Eén of meerdere lagen hebben een top onder de bodem van de ondergrondschematisatie.");
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1.1)]
        public void GetLayerThickness_LayerInProfile_ReturnsThicknessOfLayer(int layerIndex, double expectedThickness)
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            var thickness = profile.GetLayerThickness(pipingSoilLayers[layerIndex]);

            // Assert
            Assert.AreEqual(expectedThickness, thickness);
        }

        [Test]
        public void GetLayerThickness_LayerNotInProfile_ThrowsArgumentException()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            TestDelegate test = () => profile.GetLayerThickness(new PipingSoilLayer(1.1));

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "Layer not found in profile.");
        }

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
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.0);

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
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.0);

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
                },
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.2);

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
                },
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.6);

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
                },
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.6);

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
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.2);

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
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.1);

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
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.0);

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
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.3);

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
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.5);

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
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.5);

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
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(0.5);

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
                },
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.1);

            // Assert
            Assert.AreEqual(1.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_LevelBelowProfile_ArgumentException()
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
            TestDelegate call = () => profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(0.0);

            // Assert
            var message = string.Format("Level {0} is below the bottom of the soil profile {1}.", 0.0, 0.5);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var profile = new PipingSoilProfile(name, 0.0, new[]
            {
                new PipingSoilLayer(0.0)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call & Assert
            Assert.AreEqual(name, profile.ToString());
        }
    }
}