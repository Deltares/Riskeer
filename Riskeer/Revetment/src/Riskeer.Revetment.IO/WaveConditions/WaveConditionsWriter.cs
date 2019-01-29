// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Ringtoets.Revetment.IO.Properties;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.Revetment.IO.WaveConditions
{
    /// <summary>
    /// Csv file writer for writing <see cref="ExportableWaveConditions"/> objects to file.
    /// </summary>
    public static class WaveConditionsWriter
    {
        private const string separator = ", ";

        /// <summary>
        /// Writes wave conditions to a csv file.
        /// </summary>
        /// <param name="exportableWaveConditionsCollection">The <see cref="ExportableWaveConditions"/> to be written to file.</param>
        /// <param name="filePath">The path to the csv file.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="exportableWaveConditionsCollection"/> or 
        /// <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write to <paramref name="filePath"/>.</exception>
        public static void WriteWaveConditions(IEnumerable<ExportableWaveConditions> exportableWaveConditionsCollection, string filePath)
        {
            if (exportableWaveConditionsCollection == null)
            {
                throw new ArgumentNullException(nameof(exportableWaveConditionsCollection));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var stringBuilder = new StringBuilder(Resources.WaveConditionsWriter_HeaderLine + Environment.NewLine);

            try
            {
                foreach (ExportableWaveConditions exportableWaveConditions in exportableWaveConditionsCollection)
                {
                    stringBuilder.AppendLine(CreateCsvLine(exportableWaveConditions));
                }

                File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), e);
            }
        }

        private static string CreateCsvLine(ExportableWaveConditions exportableWaveConditions)
        {
            string[] stringComponents =
            {
                exportableWaveConditions.CalculationName,
                exportableWaveConditions.LocationName,
                new RoundedDouble(3, exportableWaveConditions.LocationXCoordinate).ToString(null, CultureInfo.InvariantCulture),
                new RoundedDouble(3, exportableWaveConditions.LocationYCoordinate).ToString(null, CultureInfo.InvariantCulture),
                exportableWaveConditions.ForeshoreId ?? string.Empty,
                exportableWaveConditions.UseBreakWater ? Resources.Yes : Resources.No,
                exportableWaveConditions.UseForeshore ? Resources.Yes : Resources.No,
                exportableWaveConditions.CoverType.Name,
                exportableWaveConditions.CategoryBoundaryName,
                new RoundedDouble(2, exportableWaveConditions.WaterLevel).ToString(null, CultureInfo.InvariantCulture),
                new RoundedDouble(2, exportableWaveConditions.WaveHeight).ToString(null, CultureInfo.InvariantCulture),
                new RoundedDouble(2, exportableWaveConditions.WavePeriod).ToString(null, CultureInfo.InvariantCulture),
                new RoundedDouble(2, exportableWaveConditions.WaveAngle).ToString(null, CultureInfo.InvariantCulture),
                new RoundedDouble(2, exportableWaveConditions.WaveDirection).ToString(null, CultureInfo.InvariantCulture)
            };

            return string.Join(separator, stringComponents);
        }
    }
}