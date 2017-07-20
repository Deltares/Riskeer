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

using System;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.MacroStabilityInwards.Primitives;

using ReferenceLineIntersectionsResult = Ringtoets.Common.IO.SurfaceLines.SurfaceLineExtensions.ReferenceLineIntersectionsResult;
using ReferenceLineIntersectionResult = Ringtoets.Common.IO.SurfaceLines.SurfaceLineExtensions.ReferenceLineIntersectionResult;

namespace Ringtoets.MacroStabilityInwards.IO.Importers
{
    /// <summary>
    /// Transforms generic <see cref="SurfaceLine"/> into piping specific <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>.
    /// </summary>
    public class MacroStabilityInwardsSurfaceLineTransformer : ISurfaceLineTransformer<RingtoetsMacroStabilityInwardsSurfaceLine>
    {
        private readonly ReferenceLine referenceLine;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSurfaceLineTransformer"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line to determine locations for the surface
        /// lines for.</param>
        public MacroStabilityInwardsSurfaceLineTransformer(ReferenceLine referenceLine)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }
            this.referenceLine = referenceLine;
        }

        public RingtoetsMacroStabilityInwardsSurfaceLine Transform(SurfaceLine surfaceLine, CharacteristicPoints characteristicPoints)
        {
            ReferenceLineIntersectionResult result = surfaceLine.CheckReferenceLineInterSections(referenceLine);

            if (result.TypeOfIntersection != ReferenceLineIntersectionsResult.OneIntersection)
            {
                return null;
            }

            var macroStabilityInwardsSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = surfaceLine.Name
            };
            macroStabilityInwardsSurfaceLine.SetGeometry(surfaceLine.Points);

            if (!macroStabilityInwardsSurfaceLine.SetCharacteristicPoints(characteristicPoints))
            {
                return null;
            }

            macroStabilityInwardsSurfaceLine.ReferenceLineIntersectionWorldPoint = result.IntersectionPoint;

            return macroStabilityInwardsSurfaceLine;
        }
    }
}