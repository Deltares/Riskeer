// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Factory for creating valid configurations for the <see cref="SurfaceLinesCsvImporter{T}"/>.
    /// </summary>
    public static class SurfaceLinesCsvImporterConfigurationFactory
    {
        /// <summary>
        /// Creates a configuration to replace the current surface lines on <see cref="failureMechanism"/> with imported surface lines.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to replace the surface lines for.</param>
        /// <param name="referenceLine">The reference line to use for matching imported surface lines on.</param>
        /// <returns>The configuration for the replace operation.</returns>
        public static SurfaceLinesCsvImporterConfiguration<PipingSurfaceLine> CreateReplaceStrategyConfiguration(PipingFailureMechanism failureMechanism, ReferenceLine referenceLine)
        {
            return new SurfaceLinesCsvImporterConfiguration<PipingSurfaceLine>(
                new PipingSurfaceLineTransformer(referenceLine),
                new PipingSurfaceLineReplaceDataStrategy(failureMechanism));
        }

        /// <summary>
        /// Creates a configuration to update the current surface lines on <see cref="failureMechanism"/> with imported surface lines.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to update the surface lines for.</param>
        /// <param name="referenceLine">The reference line to use for matching imported surface lines on.</param>
        /// <returns>The configuration for the update operation.</returns>
        public static SurfaceLinesCsvImporterConfiguration<PipingSurfaceLine> CreateUpdateStrategyConfiguration(PipingFailureMechanism failureMechanism, ReferenceLine referenceLine)
        {
            return new SurfaceLinesCsvImporterConfiguration<PipingSurfaceLine>(
                new PipingSurfaceLineTransformer(referenceLine),
                new PipingSurfaceLineUpdateDataStrategy(failureMechanism));
        }
    }
}