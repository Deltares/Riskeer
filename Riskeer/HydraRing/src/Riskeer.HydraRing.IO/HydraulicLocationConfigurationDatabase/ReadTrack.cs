// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

namespace Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase
{
    /// <summary>
    /// Container for read track information.
    /// </summary>
    public class ReadTrack
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadTrack"/>.
        /// </summary>
        /// <param name="trackId">The track id.</param>
        /// <param name="hrdFileName">The HRD file name.</param>
        /// <param name="usePreprocessorClosure">Indicator whether to use the preprocessor closure.</param>
        internal ReadTrack(long trackId, string hrdFileName, bool usePreprocessorClosure)
        {
            TrackId = trackId;
            HrdFileName = hrdFileName;
            UsePreprocessorClosure = usePreprocessorClosure;
        }

        /// <summary>
        /// Gets the track id.
        /// </summary>
        public long TrackId { get; }

        /// <summary>
        /// Gets the HRD file name.
        /// </summary>
        public string HrdFileName { get; }

        /// <summary>
        /// Gets the indicator whether to use the preprocessor closure.
        /// </summary>
        public bool UsePreprocessorClosure { get; }
    }
}