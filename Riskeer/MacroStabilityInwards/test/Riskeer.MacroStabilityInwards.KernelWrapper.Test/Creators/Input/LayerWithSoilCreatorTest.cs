// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using Deltares.MacroStability.CSharpWrapper;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Point2D = Core.Common.Base.Geometry.Point2D;
using SoilLayer = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using SoilProfile = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilProfile;
using WaterPressureInterpolationModel = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.WaterPressureInterpolationModel;
using CSharpWrapperWaterPressureInterpolationModel = Deltares.MacroStability.CSharpWrapper.Input.WaterPressureInterpolationModel;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class LayerWithSoilCreatorTest
    {
        [Test]
        public void Create_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => LayerWithSoilCreator.Create(null, out IDictionary<SoilLayer, LayerWithSoil> _);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Create_SoilProfile_ExpectedLayersWithSoil()
        {
            // Setup
            var outerRing1 = new Point2D[0];
            var outerRing2 = new Point2D[0];
            var outerRing3 = new Point2D[0];
            var outerRing4 = new Point2D[0];
            var outerRing5 = new Point2D[0];
            var outerRing6 = new Point2D[0];
            var soilProfile = new SoilProfile
            (
                new[]
                {
                    new SoilLayer(outerRing1, CreateRandomConstructionProperties(21, "Material 1"), new[]
                    {
                        new SoilLayer(outerRing2, CreateRandomConstructionProperties(22, "Material 2"), new[]
                        {
                            new SoilLayer(outerRing3, CreateRandomConstructionProperties(22, "Material 3"), Enumerable.Empty<SoilLayer>())
                        }),
                        new SoilLayer(outerRing4, CreateRandomConstructionProperties(23, "Material 4"), Enumerable.Empty<SoilLayer>())
                    }),
                    new SoilLayer(outerRing5, CreateRandomConstructionProperties(24, "Material 5"), new[]
                    {
                        new SoilLayer(outerRing6, CreateRandomConstructionProperties(25, "Material 6"), Enumerable.Empty<SoilLayer>())
                    })
                }, Enumerable.Empty<PreconsolidationStress>()
            );

            // Call
            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(soilProfile, out IDictionary<SoilLayer, LayerWithSoil> layerLookup);

            // Assert
            SoilLayer layer1 = soilProfile.Layers.ElementAt(0);
            SoilLayer layer2 = layer1.NestedLayers.ElementAt(0);
            SoilLayer layer3 = layer2.NestedLayers.ElementAt(0);
            SoilLayer layer4 = layer1.NestedLayers.ElementAt(1);
            SoilLayer layer5 = soilProfile.Layers.ElementAt(1);
            SoilLayer layer6 = layer5.NestedLayers.ElementAt(0);

            Assert.AreEqual(6, layersWithSoil.Length);
            Assert.AreEqual(outerRing1, layersWithSoil[0].OuterRing);
            CollectionAssert.AreEqual(new[]
            {
                outerRing2,
                outerRing3,
                outerRing4
            }, layersWithSoil[0].InnerRings);
            AssertSoilLayerProperties(layer1, layersWithSoil[0]);

            Assert.AreEqual(outerRing2, layersWithSoil[1].OuterRing);
            CollectionAssert.AreEqual(new[]
            {
                outerRing3
            }, layersWithSoil[1].InnerRings);
            AssertSoilLayerProperties(layer2, layersWithSoil[1]);

            Assert.AreEqual(outerRing3, layersWithSoil[2].OuterRing);
            CollectionAssert.IsEmpty(layersWithSoil[2].InnerRings);
            AssertSoilLayerProperties(layer3, layersWithSoil[2]);

            Assert.AreEqual(outerRing4, layersWithSoil[3].OuterRing);
            CollectionAssert.IsEmpty(layersWithSoil[3].InnerRings);
            AssertSoilLayerProperties(layer4, layersWithSoil[3]);

            Assert.AreEqual(outerRing5, layersWithSoil[4].OuterRing);
            CollectionAssert.AreEqual(new[]
            {
                outerRing6
            }, layersWithSoil[4].InnerRings);
            AssertSoilLayerProperties(layer5, layersWithSoil[4]);

            Assert.AreEqual(outerRing6, layersWithSoil[5].OuterRing);
            CollectionAssert.IsEmpty(layersWithSoil[5].InnerRings);
            AssertSoilLayerProperties(layer6, layersWithSoil[5]);
            
            SoilLayer[] originalSoilLayers = 
            {
                layer1,
                layer2,
                layer3,
                layer4,
                layer5,
                layer6
            };
            Assert.AreEqual(layersWithSoil.Length, layerLookup.Count);

            for (var i = 0; i < layersWithSoil.Length; i++)
            {
                Assert.AreSame(originalSoilLayers[i], layerLookup.ElementAt(i).Key);
                Assert.AreSame(layersWithSoil.ElementAt(i), layerLookup.ElementAt(i).Value);
            }
        }

        [Test]
        public void Create_InvalidShearStrengthModel_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var profile = new SoilProfile(new[]
            {
                new SoilLayer(new Point2D[0],
                              new SoilLayer.ConstructionProperties
                              {
                                  ShearStrengthModel = (ShearStrengthModel) 99
                              },
                              Enumerable.Empty<SoilLayer>())
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            void Call() => LayerWithSoilCreator.Create(profile, out IDictionary<SoilLayer, LayerWithSoil> _);

            // Assert
            string message = $"The value of argument 'shearStrengthModel' ({99}) is invalid for Enum type '{nameof(ShearStrengthModel)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        public void Create_InvalidWaterPressureInterpolationModel_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var profile = new SoilProfile(new[]
            {
                new SoilLayer(new Point2D[0],
                              new SoilLayer.ConstructionProperties
                              {
                                  WaterPressureInterpolationModel = (WaterPressureInterpolationModel) 99
                              },
                              Enumerable.Empty<SoilLayer>())
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            void Call() => LayerWithSoilCreator.Create(profile, out IDictionary<SoilLayer, LayerWithSoil> _);

            // Assert
            string message = $"The value of argument 'waterPressureInterpolationModel' ({99}) is invalid for Enum type '{nameof(WaterPressureInterpolationModel)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [TestCase(ShearStrengthModel.CPhi, ShearStrengthModelType.MohrCoulomb, ShearStrengthModelType.MohrCoulomb)]
        [TestCase(ShearStrengthModel.SuCalculated, ShearStrengthModelType.Shansep, ShearStrengthModelType.Shansep)]
        [TestCase(ShearStrengthModel.CPhiOrSuCalculated, ShearStrengthModelType.MohrCoulomb, ShearStrengthModelType.Shansep)]
        public void Create_ValidShearStrengthModel_ExpectedShearStrengthModel(
            ShearStrengthModel shearStrengthModel, ShearStrengthModelType expectedShearStrengthAbovePhreaticLevelModel,
                                                                              ShearStrengthModelType expectedShearStrengthBelowPhreaticLevelModel)
        {
            // Setup
            var profile = new SoilProfile(new[]
            {
                new SoilLayer(new Point2D[0],
                              new SoilLayer.ConstructionProperties
                              {
                                  ShearStrengthModel = shearStrengthModel
                              },
                              Enumerable.Empty<SoilLayer>())
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(profile, out IDictionary<SoilLayer, LayerWithSoil> _);

            // Assert
            Assert.AreEqual(expectedShearStrengthAbovePhreaticLevelModel, layersWithSoil[0].Soil.ShearStrengthAbovePhreaticLevelModel);
            Assert.AreEqual(expectedShearStrengthBelowPhreaticLevelModel, layersWithSoil[0].Soil.ShearStrengthBelowPhreaticLevelModel);
        }

        [TestCase(WaterPressureInterpolationModel.Automatic, CSharpWrapperWaterPressureInterpolationModel.Automatic)]
        [TestCase(WaterPressureInterpolationModel.Hydrostatic, CSharpWrapperWaterPressureInterpolationModel.Hydrostatic)]
        public void Create_ValidWaterPressureInterpolationModel_ExpectedWaterPressureInterpolationModel(
            WaterPressureInterpolationModel waterPressureInterpolationModel, CSharpWrapperWaterPressureInterpolationModel expectedWaterPressureInterpolationModel)
        {
            // Setup
            var profile = new SoilProfile(new[]
            {
                new SoilLayer(new Point2D[0],
                              new SoilLayer.ConstructionProperties
                              {
                                  WaterPressureInterpolationModel = waterPressureInterpolationModel
                              },
                              Enumerable.Empty<SoilLayer>())
            }, Enumerable.Empty<PreconsolidationStress>());

            // Call
            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(profile, out IDictionary<SoilLayer, LayerWithSoil> layerLookup);

            // Assert
            Assert.AreEqual(expectedWaterPressureInterpolationModel, layersWithSoil[0].WaterPressureInterpolationModel);
        }

        private static void AssertSoilLayerProperties(SoilLayer soilLayer, LayerWithSoil layerWithSoil)
        {
            Assert.AreEqual(soilLayer.IsAquifer, layerWithSoil.IsAquifer);
            Assert.AreEqual(CSharpWrapperWaterPressureInterpolationModel.Hydrostatic, layerWithSoil.WaterPressureInterpolationModel);

            Assert.IsNotNull(layerWithSoil.Soil);
            Assert.AreEqual(ShearStrengthModelType.Shansep, layerWithSoil.Soil.ShearStrengthAbovePhreaticLevelModel);
            Assert.AreEqual(ShearStrengthModelType.Shansep, layerWithSoil.Soil.ShearStrengthBelowPhreaticLevelModel);
            Assert.AreEqual(soilLayer.MaterialName, layerWithSoil.Soil.Name);
            Assert.AreEqual(soilLayer.AbovePhreaticLevel, layerWithSoil.Soil.AbovePhreaticLevel);
            Assert.AreEqual(soilLayer.BelowPhreaticLevel, layerWithSoil.Soil.BelowPhreaticLevel);
            Assert.AreEqual(soilLayer.Cohesion, layerWithSoil.Soil.Cohesion);
            Assert.AreEqual(soilLayer.FrictionAngle, layerWithSoil.Soil.FrictionAngle);
            Assert.AreEqual(soilLayer.ShearStrengthRatio, layerWithSoil.Soil.RatioCuPc);
            Assert.AreEqual(soilLayer.StrengthIncreaseExponent, layerWithSoil.Soil.StrengthIncreaseExponent);
            Assert.AreEqual(soilLayer.Dilatancy, layerWithSoil.Soil.Dilatancy);
        }

        private static SoilLayer.ConstructionProperties CreateRandomConstructionProperties(int seed, string materialName)
        {
            var random = new Random(seed);

            return new SoilLayer.ConstructionProperties
            {
                UsePop = random.NextBoolean(),
                IsAquifer = random.NextBoolean(),
                ShearStrengthModel = ShearStrengthModel.SuCalculated,
                MaterialName = materialName,
                AbovePhreaticLevel = random.NextDouble(),
                BelowPhreaticLevel = random.NextDouble(),
                Cohesion = random.NextDouble(),
                FrictionAngle = random.NextDouble(),
                ShearStrengthRatio = random.NextDouble(),
                StrengthIncreaseExponent = random.NextDouble(),
                Pop = random.NextDouble(),
                Dilatancy = random.NextDouble(),
                WaterPressureInterpolationModel = WaterPressureInterpolationModel.Hydrostatic
            };
        }
    }
}