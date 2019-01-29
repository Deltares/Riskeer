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

using Core.Common.Base.Geometry;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Simple macro stability inwards failure mechanism that can be used for testing.
    /// </summary>
    public class TestMacroStabilityInwardsFailureMechanism : MacroStabilityInwardsFailureMechanism
    {
        /// <summary>
        /// Creates a new instance of a <see cref="TestMacroStabilityInwardsFailureMechanism"/> with a non-zero contribution.
        /// </summary>
        public TestMacroStabilityInwardsFailureMechanism()
        {
            Contribution = 24;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="TestMacroStabilityInwardsFailureMechanism"/> with sections and a surface line.
        /// </summary>
        /// <returns>A new instance of <see cref="TestMacroStabilityInwardsFailureMechanism"/>.</returns>
        public static TestMacroStabilityInwardsFailureMechanism GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty)
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path/to/surfaceLines");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            }, "path/to/stochasticSoilModels");

            failureMechanism.SetSections(new[]
            {
                new FailureMechanismSection("Section", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                })
            }, "path/to/sections");

            return failureMechanism;
        }
    }
}