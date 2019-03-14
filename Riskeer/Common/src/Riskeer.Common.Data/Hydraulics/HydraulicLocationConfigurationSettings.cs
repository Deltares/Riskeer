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

using System;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Class which holds information about the hydraulic
    /// location configuration settings.
    /// </summary>
    public class HydraulicLocationConfigurationSettings
    {
        private bool canUsePreprocessor;
        private bool usePreprocessor;
        private string preprocessorDirectory;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationSettings"/>.
        /// </summary>
        /// <remarks><see cref="CanUsePreprocessor"/> is set to <c>false</c>.</remarks>
        public HydraulicLocationConfigurationSettings()
        {
            CanUsePreprocessor = false;
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the scenario name.
        /// </summary>
        public string ScenarioName { get; private set; }

        /// <summary>
        /// Gets the year.
        /// </summary>
        public int Year { get; private set; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public string Scope { get; private set; }

        /// <summary>
        /// Gets the sea level.
        /// </summary>
        public string SeaLevel { get; private set; }

        /// <summary>
        /// Gets the river discharge.
        /// </summary>
        public string RiverDischarge { get; private set; }

        /// <summary>
        /// Gets the lake level.
        /// </summary>
        public string LakeLevel { get; private set; }

        /// <summary>
        /// Gets the wind direction.
        /// </summary>
        public string WindDirection { get; private set; }

        /// <summary>
        /// Gets the wind speed.
        /// </summary>
        public string WindSpeed { get; private set; }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Gets the indicator whether to use the preprocessor closure.
        /// </summary>
        public bool UsePreprocessorClosure { get; private set; }


        /// <summary>
        /// Gets or sets a value indicating whether the Hydra-Ring preprocessor can be used.
        /// </summary>
        /// <remarks>When setting this property to <c>false</c>, both <see cref="UsePreprocessor"/>
        /// and <see cref="PreprocessorDirectory"/> are reset.</remarks>
        public bool CanUsePreprocessor
        {
            get
            {
                return canUsePreprocessor;
            }
            set
            {
                canUsePreprocessor = value;

                if (!canUsePreprocessor)
                {
                    usePreprocessor = false;
                    preprocessorDirectory = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Hydra-Ring preprocessor must be used.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when set while <see cref="CanUsePreprocessor"/> is <c>false</c>.</exception>
        public bool UsePreprocessor
        {
            get
            {
                return usePreprocessor;
            }
            set
            {
                if (!CanUsePreprocessor)
                {
                    throw new InvalidOperationException($"{nameof(CanUsePreprocessor)} is false.");
                }

                usePreprocessor = value;
            }
        }

        /// <summary>
        /// Gets or sets the Hydra-Ring preprocessor directory.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when set while <see cref="CanUsePreprocessor"/> is <c>false</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when setting a value that matches <see cref="string.IsNullOrWhiteSpace"/>.</exception>
        public string PreprocessorDirectory
        {
            get
            {
                return preprocessorDirectory;
            }
            set
            {
                if (!CanUsePreprocessor)
                {
                    throw new InvalidOperationException($"{nameof(CanUsePreprocessor)} is false.");
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_Path_must_have_a_value);
                }

                preprocessorDirectory = value;
            }
        }

        /// <summary>
        /// Sets values to the <see cref="HydraulicLocationConfigurationSettings"/>.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="scenarioName">The name of the scenario.</param>
        /// <param name="year">The year.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="usePreprocessorClosure">The use preprocessor closure indicator.</param>
        /// <param name="seaLevel">The sea level.</param>
        /// <param name="riverDischarge">The river discharge.</param>
        /// <param name="lakeLevel">The lake level.</param>
        /// <param name="windDirection">The wind direction.</param>
        /// <param name="windSpeed">The wind speed.</param>
        /// <param name="comment">The comment.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="scenarioName"/>
        /// or <paramref name="scope"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/>
        /// is <c>null</c>, empty or consists of whitespace.</exception>
        public void SetValues(string filePath, string scenarioName, int year, string scope,
                              bool usePreprocessorClosure, string seaLevel, string riverDischarge,
                              string lakeLevel, string windDirection, string windSpeed, string comment)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($@"'{nameof(filePath)}' is null, empty or consists of whitespace.");
            }

            if (scenarioName == null)
            {
                throw new ArgumentNullException(nameof(scenarioName));
            }

            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            FilePath = filePath;
            ScenarioName = scenarioName;
            Year = year;
            Scope = scope;
            UsePreprocessorClosure = usePreprocessorClosure;
            SeaLevel = seaLevel;
            RiverDischarge = riverDischarge;
            LakeLevel = lakeLevel;
            WindDirection = windDirection;
            WindSpeed = windSpeed;
            Comment = comment;
        }
    }
}