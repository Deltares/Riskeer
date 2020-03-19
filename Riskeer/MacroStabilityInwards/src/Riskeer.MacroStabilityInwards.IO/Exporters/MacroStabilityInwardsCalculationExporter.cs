using System;
using Core.Common.Base.IO;
using Core.Common.Util;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.Exporters
{
    /// <summary>
    /// Exports a macro stability inwards calculation stores it separate stix file.
    /// </summary>
    public class MacroStabilityInwardsCalculationExporter : IFileExporter
    {
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationExporter"/>.
        /// </summary>
        /// <param name="calculation">The calculation to export.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public MacroStabilityInwardsCalculationExporter(MacroStabilityInwardsCalculation calculation, string filePath)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            IOUtils.ValidateFilePath(filePath);
            
            this.filePath = filePath;
        }

        public bool Export()
        {
            return false;
        }
    }
}