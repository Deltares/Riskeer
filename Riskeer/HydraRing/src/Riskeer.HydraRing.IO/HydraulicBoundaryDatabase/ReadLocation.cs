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

namespace Riskeer.HydraRing.IO.HydraulicBoundaryDatabase
{
    /// <summary>
    /// Container for location data that is read from a locations file.
    /// </summary>
    public class ReadLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadLocation"/>.
        /// </summary>
        /// <param name="locationId">The database id of the read location.</param>
        /// <param name="hrdLocationId">The HRD location id of the read location.</param>
        /// <param name="assessmentSectionName">The name of the assessment section the read location belongs to.</param>
        /// <param name="hrdFileName">The name of the HRD file the read location is represented by.</param>
        public ReadLocation(long locationId, long hrdLocationId, string assessmentSectionName, string hrdFileName)
        {
            LocationId = locationId;
            HrdLocationId = hrdLocationId;
            AssessmentSectionName = assessmentSectionName;
            HrdFileName = hrdFileName;
        }

        /// <summary>
        /// Gets the database id of the read location.
        /// </summary>
        public long LocationId { get; }

        /// <summary>
        /// Gets the HRD location id of the read location.
        /// </summary>
        public long HrdLocationId { get; }

        /// <summary>
        /// Gets the name of the assessment section the read location belongs to.
        /// </summary>
        public string AssessmentSectionName { get; }

        /// <summary>
        /// Gets the name of the HRD file the read location is represented by.
        /// </summary>
        public string HrdFileName { get; }
    }
}