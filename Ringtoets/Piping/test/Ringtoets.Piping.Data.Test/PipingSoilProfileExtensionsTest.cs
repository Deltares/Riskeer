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
using System.Linq;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingSoilProfileExtensionsTest
    {
        private PipingSoilLayer[] testCaseTwoAquitard = {
            new PipingSoilLayer(2.1)
            {
                IsAquifer = false
            },
            new PipingSoilLayer(1.1)
            {
                IsAquifer = false
            }
        };

        private PipingSoilLayer[] testCaseTwoAquifer = {
            new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            },
            new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            }
        };

        private PipingSoilLayer[] testCaseOneAquiferOneAquitard = {
            new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            },
            new PipingSoilLayer(1.1)
            {
                IsAquifer = false
            }
        };

        private PipingSoilLayer[] testCaseOneAquitardOneAquifer = {
            new PipingSoilLayer(2.1)
            {
                IsAquifer = false
            },
            new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            }
        };

        private PipingSoilLayer[] testCaseOneAquifer = {
            new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            }
        };

        private PipingSoilLayer[] testCaseOneAquitard = {
            new PipingSoilLayer(2.1)
            {
                IsAquifer = false
            }
        };

        private PipingSoilLayer[] testCaseOneAquiferOneAquitardOneAquifer = {
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

        private PipingSoilLayer[] testCaseTwoAquiferOneAquitardOneAquifer = {
            new PipingSoilLayer(2.1)
            {
                IsAquifer = true
            },
            new PipingSoilLayer(1.1)
            {
                IsAquifer = true
            },
            new PipingSoilLayer(1.0)
            {
                IsAquifer = false
            },
            new PipingSoilLayer(0.5)
            {
                IsAquifer = true
            }
        };

        private PipingSoilLayer[] testCaseTwoAquitardOneAquiferOneAquitard = {
            new PipingSoilLayer(2.1)
            {
                IsAquifer = false
            },
            new PipingSoilLayer(1.1)
            {
                IsAquifer = false
            },
            new PipingSoilLayer(1.0)
            {
                IsAquifer = true
            },
            new PipingSoilLayer(0.5)
            {
                IsAquifer = false
            }
        };

        #region GetTopmostConsecutiveAquiferLayerThicknessBelowLevel

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_NoAquiferLayer_NaN()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquitard);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerAboveLevel_NaN()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquiferOneAquitard);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerCompletelyBelowLevel_ReturnAquiferLayerThickness()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(2.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerPartlyBelowLevel_ReturnAquiferLayerThicknessUpTillLevel()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.6);

            // Assert
            Assert.AreEqual(1.6, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerTopEqualToLevel_ReturnAquiferLayerThickness()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.1);

            // Assert
            Assert.AreEqual(2.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_AquiferLayerBottomEqualToLevel_NaN()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(0.0);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TwoAquiferLayersCompletelyBelowLevel_ReturnConsecutiveAquiferLayerThickness()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.2);

            // Assert
            Assert.AreEqual(2.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerTopEqualToLevel_ReturnConsecutiveAquiferLayerThickness()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.1);

            // Assert
            Assert.AreEqual(2.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerTopPartlyBelowLevel_ReturnConsecutiveAquiferLayerThickness()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(2.0);

            // Assert
            Assert.AreEqual(2.0, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TopmostAquiferLayerCompletelyAboveLevel_ReturnBottomAquiferLayerThickness()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(0.5);

            // Assert
            Assert.AreEqual(0.5, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_BottomAquiferLayerTopEqualToLevel_BottomAquiferLayerThickness()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.1);

            // Assert
            Assert.AreEqual(1.1, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_TwoConsecutiveAquiferLayersAndOneNonConsecutiveAquiferLayer_ReturnConsecutiveAquiferLayerThicknessUpTillLevel()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquiferOneAquitardOneAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(1.5);

            // Assert
            Assert.AreEqual(0.5, result, 1e-6);
        }

        [Test]
        public void GetTopmostConsecutiveAquiferLayerThicknessBelowLevel_LevelBelowProfile_NaN()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            double result = profile.GetTopmostConsecutiveAquiferLayerThicknessBelowLevel(-1.0);

            // Assert
            Assert.IsNaN(result);
        }

        #endregion

        #region GetConsecutiveAquiferLayersBelowLevel

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_NoAquiferLayer_ReturnEmptyCollection()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerAboveLevel_ReturnEmptyCollection()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquiferOneAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerCompletelyBelowLevel_ReturnAquiferLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(new[] { profile.Layers.ElementAt(0) }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerPartlyBelowLevel_ReturnCollectionWithAquiferLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.6);

            // Assert
            CollectionAssert.AreEqual(new[] { profile.Layers.ElementAt(0) }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerTopEqualToLevel_ReturnCollectionWithAquiferLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(new[] { profile.Layers.ElementAt(0) }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_AquiferLayerBottomEqualToLevel_ReturnEmptyCollection()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(0.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TwoAquiferLayersCompletelyBelowLevel_ReturnConsecutiveAquiferLayers()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerTopEqualToLevel_ReturnConsecutiveAquiferLayers()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerTopPartlyBelowLevel_ReturnCollectionWithAquiferLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(2.0);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TopmostAquiferLayerCompletelyAboveLevel_ReturnCollectionWithoutTopmostAquiferLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(0.5);

            // Assert
            CollectionAssert.AreEqual(new[] { profile.Layers.ElementAt(1) }, result);
        }
        
        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_BottomAquiferLayerTopEqualToLevel_ReturnCollectionWithBottomAquiferLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.1);

            // Assert
            CollectionAssert.AreEqual(new[] { profile.Layers.ElementAt(1) }, result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_TwoConsecutiveAquiferLayersAndOneNonConsecutiveAquiferLayer_ReturnConsecutiveAquiferLayers()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquiferOneAquitardOneAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(1.5);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(2), result);
        }

        [Test]
        public void GetConsecutiveAquiferLayersBelowLevel_LevelBelowProfile_ReturnEmptyCollection()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquiferLayersBelowLevel(-1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        #endregion

        #region GetConsecutiveAquitardLayersBelowLevel

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_NoAquitardLayer_ReturnEmptyCollection()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AquitardLayerAboveLevel_ReturnEmptyCollection()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquitardOneAquifer);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.1);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AquitardLayerCompletelyBelowLevel_ReturnAquitardLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AquitardLayerPartlyBelowLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.6);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AquitardLayerTopEqualToLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_AquitardLayerBottomEqualToLevel_ReturnEmptyCollection()
        {
            // Setup
            var profile = CreateTestProfile(testCaseOneAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(0.0);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TwoAquitardLayersCompletelyBelowLevel_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(2.2);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TopmostAquitardLayerTopEqualToLevel_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(2.1);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TopmostAquitardLayerTopPartlyBelowLevel_ReturnCollectionWithAquitardLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(2.0);

            // Assert
            CollectionAssert.AreEqual(profile.Layers, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TopmostAquitardLayerCompletelyAboveLevel_ReturnCollectionWithoutTopmostAquitardLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(0.5);

            // Assert
            CollectionAssert.AreEqual(new[] { profile.Layers.ElementAt(1) }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_BottomAquitardLayerTopEqualToLevel_ReturnCollectionWithBottomAquitardLayer()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.1);

            // Assert
            CollectionAssert.AreEqual(new[] { profile.Layers.ElementAt(1) }, result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_TwoConsecutiveAquitardLayersAndOneNonConsecutiveAquitardLayer_ReturnConsecutiveAquitardLayers()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquitardOneAquiferOneAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(1.5);

            // Assert
            CollectionAssert.AreEqual(profile.Layers.Take(2), result);
        }

        [Test]
        public void GetConsecutiveAquitardLayersBelowLevel_LevelBelowProfile_ReturnEmptyCollection()
        {
            // Setup
            var profile = CreateTestProfile(testCaseTwoAquitard);

            // Call
            IEnumerable<PipingSoilLayer> result = profile.GetConsecutiveAquitardLayersBelowLevel(-1.0);

            // Assert
            Assert.IsEmpty(result);
        }

        #endregion

        private PipingSoilProfile CreateTestProfile(PipingSoilLayer[] layers)
        {
            return new PipingSoilProfile(string.Empty, 0.0, layers, SoilProfileType.SoilProfile1D, 0);
        }
    }
}