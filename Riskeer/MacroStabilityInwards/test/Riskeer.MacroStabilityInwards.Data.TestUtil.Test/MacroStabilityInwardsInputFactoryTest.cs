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

using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsInputFactoryTest
    {
        [Test]
        public void CreateInputWithAquiferAndCoverageLayer_WithoutInputs_ExpectedValues()
        {
            // Call
            MacroStabilityInwardsInput input = MacroStabilityInwardsInputFactory.CreateInputWithAquiferAndCoverageLayer();

            // Assert
            Assert.AreEqual(new[]
            {
                new Point2D(0, 2.0),
                new Point2D(1.0, 2.0)
            }, input.SurfaceLine.LocalGeometry);
            var profile = input.StochasticSoilProfile.SoilProfile as MacroStabilityInwardsSoilProfile1D;
            Assert.NotNull(profile);
            Assert.AreEqual(-1.0, profile.Bottom);

            MacroStabilityInwardsSoilLayer1D[] soilLayers = profile.Layers.ToArray();
            Assert.AreEqual(2, soilLayers.Length);
            AssertLayer(false, 2.0, soilLayers[0]);
            AssertLayer(true, 0.0, soilLayers[1]);
        }

        [Test]
        [TestCase(2.0, 3.0)]
        [TestCase(12.2, 13.5)]
        public void CreateInputWithAquiferAndCoverageLayer_DifferentInputs_ExpectedValues(double thicknessAquiferLayer, double thicknessCoverageLayer)
        {
            // Call
            MacroStabilityInwardsInput input = MacroStabilityInwardsInputFactory.CreateInputWithAquiferAndCoverageLayer(thicknessAquiferLayer, thicknessCoverageLayer);

            // Assert
            Assert.AreEqual(new[]
            {
                new Point2D(0, thicknessCoverageLayer),
                new Point2D(1.0, thicknessCoverageLayer)
            }, input.SurfaceLine.LocalGeometry);
            var profile = input.StochasticSoilProfile.SoilProfile as MacroStabilityInwardsSoilProfile1D;
            Assert.NotNull(profile);
            Assert.AreEqual(-thicknessAquiferLayer, profile.Bottom);

            MacroStabilityInwardsSoilLayer1D[] soilLayers = profile.Layers.ToArray();
            Assert.AreEqual(2, soilLayers.Length);
            AssertLayer(false, thicknessCoverageLayer, soilLayers[0]);
            AssertLayer(true, 0.0, soilLayers[1]);
        }

        [Test]
        public void CreateInputWithAquifer_WithoutInputs_ExpectedValues()
        {
            // Call
            MacroStabilityInwardsInput input = MacroStabilityInwardsInputFactory.CreateInputWithAquifer();

            // Assert
            Assert.AreEqual(new[]
            {
                new Point2D(0, 0.0),
                new Point2D(1.0, 0.0)
            }, input.SurfaceLine.LocalGeometry);
            var profile = input.StochasticSoilProfile.SoilProfile as MacroStabilityInwardsSoilProfile1D;
            Assert.NotNull(profile);
            Assert.AreEqual(-1.0, profile.Bottom);

            MacroStabilityInwardsSoilLayer1D[] soilLayers = profile.Layers.ToArray();
            Assert.AreEqual(1, soilLayers.Length);
            AssertLayer(true, 0.0, soilLayers[0]);
        }

        [Test]
        [TestCase(2.0)]
        [TestCase(12.2)]
        public void CreateInputWithAquifer_DifferentInputs_ExpectedValues(double thicknessAquiferLayer)
        {
            // Call
            MacroStabilityInwardsInput input = MacroStabilityInwardsInputFactory.CreateInputWithAquifer(thicknessAquiferLayer);

            // Assert
            Assert.AreEqual(new[]
            {
                new Point2D(0, 0.0),
                new Point2D(1.0, 0.0)
            }, input.SurfaceLine.LocalGeometry);
            var profile = input.StochasticSoilProfile.SoilProfile as MacroStabilityInwardsSoilProfile1D;
            Assert.NotNull(profile);
            Assert.AreEqual(-thicknessAquiferLayer, profile.Bottom);

            MacroStabilityInwardsSoilLayer1D[] soilLayers = profile.Layers.ToArray();
            Assert.AreEqual(1, soilLayers.Length);
            AssertLayer(true, 0.0, soilLayers[0]);
        }

        [Test]
        [TestCase(2.0)]
        [TestCase(12.2)]
        public void CreateInputWithSingleAquiferLayerAboveSurfaceLine_DifferentInputs_ExpectedValues(double deltaAboveSurfaceLine)
        {
            // Call
            MacroStabilityInwardsInput input = MacroStabilityInwardsInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Assert
            Assert.AreEqual(new[]
            {
                new Point2D(0, 2.0),
                new Point2D(1.0, 2.0)
            }, input.SurfaceLine.LocalGeometry);
            var profile = input.StochasticSoilProfile.SoilProfile as MacroStabilityInwardsSoilProfile1D;
            Assert.NotNull(profile);
            Assert.AreEqual(0.0, profile.Bottom);

            MacroStabilityInwardsSoilLayer1D[] soilLayers = profile.Layers.ToArray();
            Assert.AreEqual(3, soilLayers.Length);
            AssertLayer(false, 4.0 + deltaAboveSurfaceLine, soilLayers[0]);
            AssertLayer(true, 3.0 + deltaAboveSurfaceLine, soilLayers[1]);
            AssertLayer(false, 2.0 + deltaAboveSurfaceLine, soilLayers[2]);
        }

        [Test]
        public void CreateInputWithMultipleAquiferLayersUnderSurfaceLine_Alwyas_ReturnsExpectedValues()
        {
            // Setup
            double expectedAquiferThickness;

            // Call
            MacroStabilityInwardsInput input = MacroStabilityInwardsInputFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedAquiferThickness);

            // Assert
            Assert.AreEqual(new[]
            {
                new Point2D(0, 3.3),
                new Point2D(1.0, 3.3)
            }, input.SurfaceLine.LocalGeometry);
            var profile = input.StochasticSoilProfile.SoilProfile as MacroStabilityInwardsSoilProfile1D;
            Assert.NotNull(profile);
            Assert.AreEqual(0.0, profile.Bottom);

            MacroStabilityInwardsSoilLayer1D[] soilLayers = profile.Layers.ToArray();
            Assert.AreEqual(3, soilLayers.Length);
            AssertLayer(false, 4.3, soilLayers[0]);
            AssertLayer(true, 3.3, soilLayers[1]);
            AssertLayer(true, 1.1, soilLayers[2]);

            Assert.AreEqual(3.3, expectedAquiferThickness);
        }

        private static void AssertLayer(bool aquifer, double top, MacroStabilityInwardsSoilLayer1D soilLayer)
        {
            Assert.AreEqual(aquifer, soilLayer.Data.IsAquifer);
            Assert.AreEqual(top, soilLayer.Top);
        }
    }
}