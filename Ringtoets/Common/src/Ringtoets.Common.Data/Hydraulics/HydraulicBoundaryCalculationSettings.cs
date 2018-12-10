using System;

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Class which holds all hydraulic boundary calculations settings.
    /// </summary>
    public class HydraulicBoundaryCalculationSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database.</param>
        /// <param name="hlcdFilePath">The file path to the hlcd.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        public HydraulicBoundaryCalculationSettings(string hydraulicBoundaryDatabaseFilePath,
                                                    string hlcdFilePath,
                                                    string preprocessorDirectory)
        {
            HydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            HlcdFilePath = hlcdFilePath;
            PreprocessorDirectory = preprocessorDirectory;
        }

        /// <summary>
        /// Gets the hydraulic boundary database filepath.
        /// </summary>
        public string HydraulicBoundaryDatabaseFilePath { get; }

        /// <summary>
        /// Gets the HLCD file path.
        /// </summary>
        public string HlcdFilePath { get; }

        /// <summary>
        ///  Gets the preprocessor directory.
        /// </summary>
        public string PreprocessorDirectory { get; }
    }
}