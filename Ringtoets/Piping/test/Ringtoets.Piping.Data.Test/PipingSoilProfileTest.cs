﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    public class PipingSoilProfileTest
    {
        [Test]
        public void Constructor_WithNameBottomLayersAndAquifer_ReturnsInstanceWithPropsAndEquivalentLayerCollection()
        {
            // Setup
            var name = "Profile";
            var random = new Random(22);
            var bottom = random.NextDouble();
            var layers = new Collection<PipingSoilLayer>
            {
                new PipingSoilLayer(bottom)
            };

            // Call
            var profile = new PipingSoilProfile(name, bottom, layers);

            // Assert
            Assert.AreNotSame(layers, profile.Layers);
            Assert.AreEqual(name, profile.Name);
            Assert.AreEqual(bottom, profile.Bottom);
        }

        [Test]
        public void Constructor_WithNameBottomLayersEmpty_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new PipingSoilProfile(String.Empty, Double.NaN, new Collection<PipingSoilLayer>());

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, Properties.Resources.Error_Cannot_Construct_PipingSoilProfile_Without_Layers);
        }

        [Test]
        public void Constructor_WithNameBottomLayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingSoilProfile(String.Empty, Double.NaN, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, Properties.Resources.Error_Cannot_Construct_PipingSoilProfile_Without_Layers);
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

            var profile = new PipingSoilProfile(string.Empty, bottom, equivalentLayers);

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
            TestDelegate test = () => new PipingSoilProfile(String.Empty, bottom, pipingSoilLayers);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "Eén of meerdere lagen hebben een top onder de bodem van het profiel.");
        }

        [Test]
        [TestCase(0,0)]
        [TestCase(1,1.1)]
        public void GetLayerThickness_LayerInProfile_ReturnsThicknessOfLayer(int layerIndex, double expectedThickness)
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

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
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
           TestDelegate test = () => profile.GetLayerThickness(new PipingSoilLayer(1.1));

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "Layer not found in profile.");
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_NoAquiferLayer_NaN()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1),
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(1.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_AquiferLayerAboveLevel_NaN()
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
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(1.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_AquiferLayerCompletelyBelowLevel_ReturnAquiferLayerThickness()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(2.1, result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_AquiferLayerPartlyBelowLevel_ReturnAquiferLayerThicknessUpTillLevel()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(2.1)
                {
                    IsAquifer = true
                },
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(1.6);

            // Assert
            Assert.AreEqual(1.6, result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_AquiferLayerTopEqualToLevel_ReturnAquiferLayerThickness()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(1.6)
                {
                    IsAquifer = true
                },
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(1.6);

            // Assert
            Assert.AreEqual(1.6, result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_TwoAquiferLayersCompletelyBelowLevel_ReturnTopAquiferLayerThickness()
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
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(1.0, result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_TopmostAquiferLayerTopEqualToLevel_ReturnTopAquiferLayerThickness()
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
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(2.1);

            // Assert
            Assert.AreEqual(1.0, result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_TopmostAquiferLayerCompletelyAboveLevel_ReturnBottomAquiferLayerThickness()
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
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(1.3);

            // Assert
            Assert.AreEqual(1.1, result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_TopmostAquiferLayerPartlyAboveLevel_ReturnTopAquiferLayerThicknessUpTillLevel()
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
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(1.5);

            // Assert
            Assert.AreEqual(0.4, result, 1e-8);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_AllAquiferLayersAboveLevel_NaN()
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
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(0.5);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_BottomAquiferLayerTopEqualToLevel_BottomAquiferLayerThickness()
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
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers);

            // Call
            var result = profile.GetTopAquiferLayerThicknessBelowLevel(1.1);

            // Assert
            Assert.AreEqual(1.1, result);
        }

        [Test]
        public void GetTopAquiferLayerThicknessBelowLevel_LevelBelowProfile_ArgumentException()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.5, pipingSoilLayers);

            // Call
            TestDelegate call = () => profile.GetTopAquiferLayerThicknessBelowLevel(0.0);

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
            });

            // Call & Assert
            Assert.AreEqual(name, profile.ToString());
        }
    }
}