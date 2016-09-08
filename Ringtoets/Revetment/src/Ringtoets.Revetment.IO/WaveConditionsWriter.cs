// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Text;
using Core.Common.Base.Data;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Ringtoets.Revetment.IO.Properties;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Revetment.IO
{
    /// <summary>
    /// Csv file writer for writing <see cref="ExportableWaveConditions"/> objects to file.
    /// </summary>
    public static class WaveConditionsWriter
    {
        /// <summary>
        /// Writes waveconditions to a csv file.
        /// </summary>
        /// <param name="exportableWaveConditionsCollection">The <see cref="ExportableWaveConditions"/> to be written to file.</param>
        /// <param name="filePath">The path to the csv file.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="exportableWaveConditionsCollection"/> or 
        /// <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when either <see cref="StringBuilder.AppendLine(string)"/> or 
        /// <see cref="File.WriteAllText(string,string)"/> fail.</exception>
        public static void WriteWaveConditions(IEnumerable<ExportableWaveConditions> exportableWaveConditionsCollection, string filePath)
        {
            if (exportableWaveConditionsCollection == null)
            {
                throw new ArgumentNullException("exportableWaveConditionsCollection");
            }
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
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
            catch (ArgumentException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilsResources.Error_General_output_error_0, filePath), e);
            }
            catch (IOException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilsResources.Error_General_output_error_0, filePath), e);
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilsResources.Error_General_output_error_0, filePath), e);
            }
        }

        private static string CreateCsvLine(ExportableWaveConditions exportableWaveConditions)
        {
            string[] stringComponents =
            {
                exportableWaveConditions.CalculationName,
                exportableWaveConditions.LocationName,
                new RoundedDouble(3, exportableWaveConditions.LocationXCoordinate).ToString(),
                new RoundedDouble(3, exportableWaveConditions.LocationYCoordinate).ToString(),
                exportableWaveConditions.ForeshoreName ?? "",
                exportableWaveConditions.HasBreakWater ? "ja" : "nee",
                exportableWaveConditions.UseForeshore ? "ja" : "nee",
                new RoundedDouble(2, exportableWaveConditions.WaterLevel).ToString(),
                new EnumDisplayWrapper<CoverType>(exportableWaveConditions.CoverType).DisplayName,
                new RoundedDouble(2, exportableWaveConditions.WaveHeight).ToString(),
                new RoundedDouble(2, exportableWaveConditions.WavePeriod).ToString(),
                new RoundedDouble(2, exportableWaveConditions.WaveAngle).ToString()
            };

            return string.Join(", ", stringComponents);
        }
    }
}