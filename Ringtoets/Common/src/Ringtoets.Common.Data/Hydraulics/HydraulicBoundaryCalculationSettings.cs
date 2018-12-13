namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Class which holds all hydraulic boundary calculation settings.
    /// </summary>
    public class HydraulicBoundaryCalculationSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        public HydraulicBoundaryCalculationSettings(string hydraulicBoundaryDatabaseFilePath, string preprocessorDirectory)

        {
            HydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            PreprocessorDirectory = preprocessorDirectory;
        }

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database.</param>
        /// <param name="hlcdFilePath">The file path of the hlcd.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        public HydraulicBoundaryCalculationSettings(string hydraulicBoundaryDatabaseFilePath,
                                                    string hlcdFilePath,
                                                    string preprocessorDirectory)
            : this(hydraulicBoundaryDatabaseFilePath, preprocessorDirectory)
        {
            HlcdFilePath = hlcdFilePath;
        }

        /// <summary>
        /// Gets the hydraulic boundary database filepath.
        /// </summary>
        public string HydraulicBoundaryDatabaseFilePath { get; }

        /// <summary>
        /// Gets the hlcd file path.
        /// </summary>
        public string HlcdFilePath { get; }

        /// <summary>
        ///  Gets the preprocessor directory.
        /// </summary>
        public string PreprocessorDirectory { get; }
    }
}