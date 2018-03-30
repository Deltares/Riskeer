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

using System;
using Core.Common.Base.IO;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Ringtoets.GrassCoverErosionOutwards.IO.Exporters
{
    /// <summary>
    /// Exports grass cover erosion outwards hydraulic boundary locations and stores them as a shapefile.
    /// </summary>
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter : IFileExporter
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to get calculations from.</param>
        /// <param name="assessmentSection">The assessment section to get locations and calculation from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> or
        /// <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                           IAssessmentSection assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }
        }

        public bool Export()
        {
            throw new NotImplementedException();
        }
    }
}