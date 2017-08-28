﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Base.Geometry;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating different instances of <see cref="MacroStabilityInwardsInput"/>
    /// for easier testing.
    /// </summary>
    public static class MacroStabilityInwardsInputFactory
    {
        /// <summary>
        /// Creates macro stability inwards input with an aquifer layer and coverage layer.
        /// </summary>
        /// <param name="thicknessAquiferLayer">The thickness of the aquifer layer.</param>
        /// <param name="thicknessCoverageLayer">The thickness of the coverage layer.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsInput"/>.</returns>
        public static MacroStabilityInwardsInput CreateInputWithAquiferAndCoverageLayer(
            double thicknessAquiferLayer = 1.0,
            double thicknessCoverageLayer = 2.0)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, thicknessCoverageLayer),
                new Point3D(1.0, 0, thicknessCoverageLayer)
            });
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(
                0.0, new MacroStabilityInwardsSoilProfile1D(string.Empty, -thicknessAquiferLayer, new[]
                {
                    new MacroStabilityInwardsSoilLayer1D(thicknessCoverageLayer)
                    {
                        Properties =
                        {
                            IsAquifer = false
                        }
                    },
                    new MacroStabilityInwardsSoilLayer1D(0.0)
                    {
                        Properties =
                        {
                            IsAquifer = true
                        }
                    }
                }, SoilProfileType.SoilProfile1D, 0));

            return new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile
            };
        }

        /// <summary>
        /// Creates macro stability inwards input with an aquifer layer.
        /// </summary>
        /// <param name="thicknessAquiferLayer">The thickness of the aquifer layer.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsInput"/>.</returns>
        public static MacroStabilityInwardsInput CreateInputWithAquifer(double thicknessAquiferLayer = 1.0)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0.0),
                new Point3D(1.0, 0, 0.0)
            });
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, new MacroStabilityInwardsSoilProfile1D(string.Empty, -thicknessAquiferLayer, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(0.0)
                {
                    Properties =
                    {
                        IsAquifer = true
                    }
                }
            }, SoilProfileType.SoilProfile1D, 0));

            return new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile
            };
        }

        /// <summary>
        /// Creates macro stability inwards input with a single aquifer layer above the surface line.
        /// </summary>
        /// <param name="deltaAboveSurfaceLine">The distance between the aquifer layer and the surface line.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsInput"/>.</returns>
        public static MacroStabilityInwardsInput CreateInputWithSingleAquiferLayerAboveSurfaceLine(double deltaAboveSurfaceLine)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            const double surfaceLineTopLevel = 2.0;
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, surfaceLineTopLevel),
                new Point3D(1.0, 0, surfaceLineTopLevel)
            });
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, new MacroStabilityInwardsSoilProfile1D(string.Empty, 0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(surfaceLineTopLevel + deltaAboveSurfaceLine + 2)
                {
                    Properties =
                    {
                        IsAquifer = false
                    }
                },
                new MacroStabilityInwardsSoilLayer1D(surfaceLineTopLevel + deltaAboveSurfaceLine + 1)
                {
                    Properties =
                    {
                        IsAquifer = true
                    }
                },
                new MacroStabilityInwardsSoilLayer1D(surfaceLineTopLevel + deltaAboveSurfaceLine)
                {
                    Properties =
                    {
                        IsAquifer = false
                    }
                }
            }, SoilProfileType.SoilProfile1D, 0));

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile
            };
            return input;
        }

        /// <summary>
        /// Creates macro stability inwards input with multiple aquifer layers under the surface line.
        /// </summary>
        /// <param name="expectedThickness">The expected thickness of the aquifer.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsInput"/>.</returns>
        public static MacroStabilityInwardsInput CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out double expectedThickness)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.3),
                new Point3D(1.0, 0, 3.3)
            });
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, new MacroStabilityInwardsSoilProfile1D(string.Empty, 0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(4.3)
                {
                    Properties =
                    {
                        IsAquifer = false
                    }
                },
                new MacroStabilityInwardsSoilLayer1D(3.3)
                {
                    Properties =
                    {
                        IsAquifer = true
                    }
                },
                new MacroStabilityInwardsSoilLayer1D(1.1)
                {
                    Properties =
                    {
                        IsAquifer = true
                    }
                }
            }, SoilProfileType.SoilProfile1D, 0));

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                SurfaceLine = surfaceLine,
                StochasticSoilProfile = stochasticSoilProfile
            };
            expectedThickness = 3.3;
            return input;
        }
    }
}