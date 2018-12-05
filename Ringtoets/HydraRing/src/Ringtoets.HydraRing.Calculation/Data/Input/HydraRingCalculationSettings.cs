using System;
using Core.Common.Util;

namespace Ringtoets.HydraRing.Calculation.Data.Input
{
    /// <summary>
    /// Class which holds all the general information to run a Hydra-Ring calculation
    /// </summary>
    public class HydraRingCalculationSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraRingCalculationSettings"/>.
        /// </summary>
        /// <param name="hlcdFilePath">The path which points to the HLCD file.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory to be used for the calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null
        /// is <c>null</c></exception>
        public HydraRingCalculationSettings(string hlcdFilePath,
                                            string preprocessorDirectory)
        {
            if (hlcdFilePath == null)
            {
                throw new ArgumentNullException(nameof(hlcdFilePath));
            }

            if (preprocessorDirectory == null)
            {
                throw new ArgumentNullException(nameof(preprocessorDirectory));
            }

            HlcdFilePath = hlcdFilePath;
            PreprocessorDirectory = preprocessorDirectory;
        }

        /// <summary>
        /// Gets the hydraulic locations configurations filepath.
        /// </summary>
        public string HlcdFilePath { get; }

        /// <summary>
        /// Gets the preprocessor director.
        /// </summary>
        public string PreprocessorDirectory { get; }
    }
}