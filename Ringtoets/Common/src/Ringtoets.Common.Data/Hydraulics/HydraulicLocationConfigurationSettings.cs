using System;

namespace Ringtoets.Common.Data.Hydraulics {
    /// <summary>
    /// Class which holds information about the hydraulic
    /// location configuration settings.
    /// </summary>
    public class HydraulicLocationConfigurationSettings
    {
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
        /// Sets values to the <see cref="HydraulicLocationConfigurationSettings"/>.
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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="scenarioName"/>
        /// or <paramref name="scope"/> are <c>null</c>.</exception>
        public void SetValues(string scenarioName, int year, string scope, 
                              string seaLevel, string riverDischarge, string lakeLevel,
                              string windDirection, string windSpeed, string comment)
        {
            if (scenarioName == null)
            {
                throw new ArgumentNullException(nameof(scenarioName));
            }

            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

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
    }
}