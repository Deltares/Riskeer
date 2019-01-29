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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.IO.Properties;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;
using DuneErosionDataResources = Ringtoets.DuneErosion.Data.Properties.Resources;

namespace Riskeer.DuneErosion.IO
{
    /// <summary>
    /// Csv file writer for writing <see cref="DuneLocationCalculation"/> objects to *.bnd file.
    /// </summary>
    internal static class DuneLocationCalculationsWriter
    {
        private const string separator = "\t";

        /// <summary>
        /// Writes dune location calculations to a bnd file.
        /// </summary>
        /// <param name="exportableDuneLocationCalculations">The dune location calculations to be written to the file.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="probabilityConverter">The <see cref="TypeConverter"/> to use when exporting the category boundaries.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write to <paramref name="filePath"/>.</exception>
        public static void WriteDuneLocationCalculations(IEnumerable<ExportableDuneLocationCalculation> exportableDuneLocationCalculations,
                                                         string filePath,
                                                         TypeConverter probabilityConverter)
        {
            if (exportableDuneLocationCalculations == null)
            {
                throw new ArgumentNullException(nameof(exportableDuneLocationCalculations));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (probabilityConverter == null)
            {
                throw new ArgumentNullException(nameof(probabilityConverter));
            }

            var stringBuilder = new StringBuilder(Resources.DuneLocationCalculationsWriter_WriteDuneLocationCalculations_HeaderLine + Environment.NewLine);
            stringBuilder.AppendLine(Resources.DuneLocationCalculationsWriter_WriteDuneLocationCalculations_DisplayNameLine);
            stringBuilder.AppendLine(Resources.DuneLocationCalculationsWriter_WriteDuneLocationCalculations_UnitsLine);

            try
            {
                foreach (ExportableDuneLocationCalculation calculation in exportableDuneLocationCalculations)
                {
                    stringBuilder.AppendLine(CreateCsvLine(calculation, probabilityConverter));
                }

                File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), e);
            }
        }

        private static string CreateCsvLine(ExportableDuneLocationCalculation calculation, TypeConverter probabilityConverter)
        {
            DuneLocation duneLocation = calculation.Calculation.DuneLocation;
            var stringComponents = new List<string>
            {
                duneLocation.CoastalAreaId.ToString(CultureInfo.InvariantCulture),
                duneLocation.Offset.ToString(DuneErosionDataResources.DuneLocation_Offset_format, CultureInfo.InvariantCulture),
                Resources.DuneLocationCalculationsWriter_CreateCsvLine_Parameter_without_value,
                duneLocation.D50.ToString(null, CultureInfo.InvariantCulture),
                string.Format(Resources.DuneLocationCalculationsWriter_WBI2017_ID_Format, calculation.CategoryBoundaryName,
                              probabilityConverter.ConvertToInvariantString(calculation.Norm)),
                calculation.CategoryBoundaryName,
                calculation.Norm.ToString(CultureInfo.InvariantCulture)
            };

            stringComponents.InsertRange(2, GetOutputValues(calculation.Calculation.Output));

            return string.Join(separator, stringComponents);
        }

        private static IEnumerable<string> GetOutputValues(DuneLocationCalculationOutput calculationOutput)
        {
            return calculationOutput == null
                       ? new[]
                       {
                           Resources.DuneLocationCalculationsWriter_CreateCsvLine_Parameter_without_value,
                           Resources.DuneLocationCalculationsWriter_CreateCsvLine_Parameter_without_value,
                           Resources.DuneLocationCalculationsWriter_CreateCsvLine_Parameter_without_value
                       }
                       : new[]
                       {
                           GetOutputValue(calculationOutput.WaterLevel),
                           GetOutputValue(calculationOutput.WaveHeight),
                           GetOutputValue(calculationOutput.WavePeriod)
                       };
        }

        private static string GetOutputValue(RoundedDouble value)
        {
            return !double.IsNaN(value)
                       ? value.ToString(null, CultureInfo.InvariantCulture)
                       : Resources.DuneLocationCalculationsWriter_CreateCsvLine_Parameter_without_value;
        }
    }
}