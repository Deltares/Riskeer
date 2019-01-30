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

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating different instances of <see cref="PipingInput"/>
    /// for easier testing.
    /// </summary>
    public static class PipingInputFactory
    {
        /// <summary>
        /// Creates piping input with an aquifer layer and coverage layer.
        /// </summary>
        /// <param name="thicknessAquiferLayer">The thickness of the aquifer layer.</param>
        /// <param name="thicknessCoverageLayer">The thickness of the coverage layer.</param>
        /// <returns>A new <see cref="PipingInput"/>.</returns>
        public static PipingInput CreateInputWithAquiferAndCoverageLayer(double thicknessAquiferLayer = 1.0,
                                                                         double thicknessCoverageLayer = 2.0)
        {
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, thicknessCoverageLayer),
                new Point3D(1.0, 0, thicknessCoverageLayer)
            });
            var stochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, -thicknessAquiferLayer, new[]
                {
                    new PipingSoilLayer(thicknessCoverageLayer)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(0.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));

            return new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile,
                ExitPointL = (RoundedDouble) 0.5
            };
        }

        /// <summary>
        /// Creates piping input with an aquifer layer.
        /// </summary>
        /// <param name="thicknessAquiferLayer">The thickness of the aquifer layer.</param>
        /// <returns>A new <see cref="PipingInput"/>.</returns>
        public static PipingInput CreateInputWithAquifer(double thicknessAquiferLayer = 1.0)
        {
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0.0),
                new Point3D(1.0, 0, 0.0)
            });
            var stochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, -thicknessAquiferLayer, new[]
                {
                    new PipingSoilLayer(0.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));

            return new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile,
                ExitPointL = (RoundedDouble) 0.5
            };
        }

        /// <summary>
        /// Creates piping input with a single aquifer layer above the surface line.
        /// </summary>
        /// <param name="deltaAboveSurfaceLine">The distance between the aquifer layer and the surface line.</param>
        /// <returns>A new <see cref="PipingInput"/>.</returns>
        public static PipingInput CreateInputWithSingleAquiferLayerAboveSurfaceLine(double deltaAboveSurfaceLine)
        {
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            const double surfaceLineTopLevel = 2.0;
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, surfaceLineTopLevel),
                new Point3D(1.0, 0, surfaceLineTopLevel)
            });
            var stochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(surfaceLineTopLevel + deltaAboveSurfaceLine + 2)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(surfaceLineTopLevel + deltaAboveSurfaceLine + 1)
                    {
                        IsAquifer = true
                    },
                    new PipingSoilLayer(surfaceLineTopLevel + deltaAboveSurfaceLine)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D));
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile,
                ExitPointL = (RoundedDouble) 0.5
            };
            return input;
        }

        /// <summary>
        /// Creates piping input with multiple aquifer layers under the surface line.
        /// </summary>
        /// <param name="expectedThickness">The expected thickness of the aquifer.</param>
        /// <returns>A new <see cref="PipingInput"/>.</returns>
        public static PipingInput CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out double expectedThickness)
        {
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.3),
                new Point3D(1.0, 0, 3.3)
            });
            var stochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(4.3)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(3.3)
                    {
                        IsAquifer = true
                    },
                    new PipingSoilLayer(1.1)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile,
                ExitPointL = (RoundedDouble) 0.5
            };
            expectedThickness = 3.3;
            return input;
        }
    }
}