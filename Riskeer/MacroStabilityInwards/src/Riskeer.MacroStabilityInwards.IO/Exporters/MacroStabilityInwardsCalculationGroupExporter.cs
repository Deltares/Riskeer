using System;
using Core.Common.Base.IO;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.MacroStabilityInwards.IO.Exporters
{
    /// <summary>
    /// Exports macro stability inwards calculations from a calculation group and stores them as separate stix files.
    /// </summary>
    public class MacroStabilityInwardsCalculationGroupExporter : IFileExporter
    {
        private readonly CalculationGroup calculationGroup;
        private readonly string folderPath;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationGroupExporter"/>.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to export.</param>
        /// <param name="folderPath">The folder path to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsCalculationGroupExporter(CalculationGroup calculationGroup, string folderPath)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (folderPath == null)
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            this.calculationGroup = calculationGroup;
            this.folderPath = folderPath;
        }

        public bool Export()
        {
            return false;
        }
    }
}