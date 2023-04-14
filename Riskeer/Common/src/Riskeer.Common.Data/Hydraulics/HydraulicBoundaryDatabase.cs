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

using Core.Common.Base;

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Class which holds information about a hydraulic boundary database.
    /// </summary>
    public class HydraulicBoundaryDatabase : Observable
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        public HydraulicBoundaryDatabase()
        {
            Locations = new ObservableList<HydraulicBoundaryLocation>();
        }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the indicator whether to use the preprocessor closure.
        /// </summary>
        public bool UsePreprocessorClosure { get; set; }

        /// <summary>
        /// Gets the hydraulic boundary locations.
        /// </summary>
        public ObservableList<HydraulicBoundaryLocation> Locations { get; }
    }
}