// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
    /// Class which holds information about a hydraulic location configuration database.
    /// </summary>
    public class HydraulicLocationConfigurationDatabase : Observable
    {
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the scenario name.
        /// </summary>
        public string ScenarioName { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the sea level.
        /// </summary>
        public string SeaLevel { get; set; }

        /// <summary>
        /// Gets or sets the river discharge.
        /// </summary>
        public string RiverDischarge { get; set; }

        /// <summary>
        /// Gets or sets the lake level.
        /// </summary>
        public string LakeLevel { get; set; }

        /// <summary>
        /// Gets or sets the wind direction.
        /// </summary>
        public string WindDirection { get; set; }

        /// <summary>
        /// Gets or sets the wind speed.
        /// </summary>
        public string WindSpeed { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; set; }
    }
}