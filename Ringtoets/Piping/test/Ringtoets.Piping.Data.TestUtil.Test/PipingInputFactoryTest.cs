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
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class PipingInputFactoryTest
    {
        [Test]
        public void CreateInputWithAquiferAndCoverageLayer_WithoutInputs_ExpectedValues()
        {
            // Call
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();

            // Assert
            Assert.AreEqual(0.5, input.ExitPointL);
            Assert.AreEqual(new[]
            {
                new Point2D(0, 2.0),
                new Point2D(1.0, 2.0)
            }, input.SurfaceLine.LocalGeometry);
            PipingSoilProfile profile = input.StochasticSoilProfile.SoilProfile;
            Assert.AreEqual(-1.0, profile.Bottom);

            PipingSoilLayer[] pipingSoilLayers = profile.Layers.ToArray();
            Assert.AreEqual(2, pipingSoilLayers.Length);
            AssertLayer(false, 2.0, pipingSoilLayers[0]);
            AssertLayer(true, 0.0, pipingSoilLayers[1]);
        }

        [Test]
        [TestCase(2.0, 3.0)]
        [TestCase(12.2, 13.5)]
        public void CreateInputWithAquiferAndCoverageLayer_DifferentInputs_ExpectedValues(double thicknessAquiferLayer, double thicknessCoverageLayer)
        {
            // Call
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer(thicknessAquiferLayer, thicknessCoverageLayer);

            // Assert
            Assert.AreEqual(0.5, input.ExitPointL);
            Assert.AreEqual(new[]
            {
                new Point2D(0, thicknessCoverageLayer),
                new Point2D(1.0, thicknessCoverageLayer)
            }, input.SurfaceLine.LocalGeometry);
            PipingSoilProfile profile = input.StochasticSoilProfile.SoilProfile;
            Assert.AreEqual(-thicknessAquiferLayer, profile.Bottom);

            PipingSoilLayer[] pipingSoilLayers = profile.Layers.ToArray();
            Assert.AreEqual(2, pipingSoilLayers.Length);
            AssertLayer(false, thicknessCoverageLayer, pipingSoilLayers[0]);
            AssertLayer(true, 0.0, pipingSoilLayers[1]);
        }

        [Test]
        public void CreateInputWithAquifer_WithoutInputs_ExpectedValues()
        {
            // Call
            PipingInput input = PipingInputFactory.CreateInputWithAquifer();

            // Assert
            Assert.AreEqual(0.5, input.ExitPointL);
            Assert.AreEqual(new[]
            {
                new Point2D(0, 0.0),
                new Point2D(1.0, 0.0)
            }, input.SurfaceLine.LocalGeometry);
            PipingSoilProfile profile = input.StochasticSoilProfile.SoilProfile;
            Assert.AreEqual(-1.0, profile.Bottom);

            PipingSoilLayer[] pipingSoilLayers = profile.Layers.ToArray();
            Assert.AreEqual(1, pipingSoilLayers.Length);
            AssertLayer(true, 0.0, pipingSoilLayers[0]);
        }

        [Test]
        [TestCase(2.0)]
        [TestCase(12.2)]
        public void CreateInputWithAquifer_DifferentInputs_ExpectedValues(double thicknessAquiferLayer)
        {
            // Call
            PipingInput input = PipingInputFactory.CreateInputWithAquifer(thicknessAquiferLayer);

            // Assert
            Assert.AreEqual(0.5, input.ExitPointL);
            Assert.AreEqual(new[]
            {
                new Point2D(0, 0.0),
                new Point2D(1.0, 0.0)
            }, input.SurfaceLine.LocalGeometry);
            PipingSoilProfile profile = input.StochasticSoilProfile.SoilProfile;
            Assert.AreEqual(-thicknessAquiferLayer, profile.Bottom);

            PipingSoilLayer[] pipingSoilLayers = profile.Layers.ToArray();
            Assert.AreEqual(1, pipingSoilLayers.Length);
            AssertLayer(true, 0.0, pipingSoilLayers[0]);
        }

        [Test]
        [TestCase(2.0)]
        [TestCase(12.2)]
        public void CreateInputWithSingleAquiferLayerAboveSurfaceLine_DifferentInputs_ExpectedValues(double deltaAboveSurfaceLine)
        {
            // Call
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Assert
            Assert.AreEqual(0.5, input.ExitPointL);
            Assert.AreEqual(new[]
            {
                new Point2D(0, 2.0),
                new Point2D(1.0, 2.0)
            }, input.SurfaceLine.LocalGeometry);
            PipingSoilProfile profile = input.StochasticSoilProfile.SoilProfile;
            Assert.AreEqual(0.0, profile.Bottom);

            PipingSoilLayer[] pipingSoilLayers = profile.Layers.ToArray();
            Assert.AreEqual(3, pipingSoilLayers.Length);
            AssertLayer(false, 4.0 + deltaAboveSurfaceLine, pipingSoilLayers[0]);
            AssertLayer(true, 3.0 + deltaAboveSurfaceLine, pipingSoilLayers[1]);
            AssertLayer(false, 2.0 + deltaAboveSurfaceLine, pipingSoilLayers[2]);
        }

        [Test]
        public void CreateInputWithMultipleAquiferLayersUnderSurfaceLine_Alwyas_ReturnsExpectedValues()
        {
            // Setup
            double expectedAquiferThickness;

            // Call
            PipingInput input = PipingInputFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedAquiferThickness);

            // Assert
            Assert.AreEqual(0.5, input.ExitPointL);
            Assert.AreEqual(new[]
            {
                new Point2D(0, 3.3),
                new Point2D(1.0, 3.3)
            }, input.SurfaceLine.LocalGeometry);
            PipingSoilProfile profile = input.StochasticSoilProfile.SoilProfile;
            Assert.AreEqual(0.0, profile.Bottom);

            PipingSoilLayer[] pipingSoilLayers = profile.Layers.ToArray();
            Assert.AreEqual(3, pipingSoilLayers.Length);
            AssertLayer(false, 4.3, pipingSoilLayers[0]);
            AssertLayer(true, 3.3, pipingSoilLayers[1]);
            AssertLayer(true, 1.1, pipingSoilLayers[2]);

            Assert.AreEqual(3.3, expectedAquiferThickness);
        }

        private static void AssertLayer(bool aquifer, double top, PipingSoilLayer pipingSoilLayer)
        {
            Assert.AreEqual(aquifer, pipingSoilLayer.IsAquifer);
            Assert.AreEqual(top, pipingSoilLayer.Top);
        }
    }
}