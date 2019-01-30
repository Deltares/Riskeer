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

namespace Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase
{
    /// <summary>
    /// Class for holding configuration settings that are read from a hydraulic location
    /// configuration database file.
    /// </summary>
    public class ReadHydraulicLocationConfigurationDatabaseSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadHydraulicLocationConfigurationDatabaseSettings"/>.
        /// </summary>
        /// <param name="scenarioName">The name of the scenario.</param>
        /// <param name="year">The year.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="seaLevel">The sea level.</param>
        /// <param name="riverDischarge">The river discharge.</param>
        /// <param name="lakeLevel">The lake level.</param>
        /// <param name="windDirection">The wind direction.</param>
        /// <param name="windSpeed">The wind speed.</param>
        /// <param name="comment">The comment.</param>
        internal ReadHydraulicLocationConfigurationDatabaseSettings(string scenarioName, int year, string scope,
                                                                    string seaLevel, string riverDischarge, string lakeLevel,
                                                                    string windDirection, string windSpeed, string comment)
        {
            ScenarioName = scenarioName;
            Year = year;
            Scope = scope;
            SeaLevel = seaLevel;
            RiverDischarge = riverDischarge;
            LakeLevel = lakeLevel;
            WindDirection = windDirection;
            WindSpeed = windSpeed;
            Comment = comment;
        }

        /// <summary>
        /// Gets the scenario name.
        /// </summary>
        public string ScenarioName { get; }

        /// <summary>
        /// Gets the year.
        /// </summary>
        public int Year { get; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public string Scope { get; }

        /// <summary>
        /// Gets the sea level.
        /// </summary>
        public string SeaLevel { get; }

        /// <summary>
        /// Gets the river discharge.
        /// </summary>
        public string RiverDischarge { get; }

        /// <summary>
        /// Gets the lake level.
        /// </summary>
        public string LakeLevel { get; }

        /// <summary>
        /// Gets the wind direction.
        /// </summary>
        public string WindDirection { get; }

        /// <summary>
        /// Gets the wind speed.
        /// </summary>
        public string WindSpeed { get; }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        public string Comment { get; }
    }
}