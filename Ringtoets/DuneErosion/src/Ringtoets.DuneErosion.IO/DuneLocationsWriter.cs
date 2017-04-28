// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.IO.Properties;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using DuneErosionDataResources = Ringtoets.DuneErosion.Data.Properties.Resources;

namespace Ringtoets.DuneErosion.IO
{
    /// <summary>
    /// Csv file writer for writing <see cref="DuneLocation"/> objects to *.bnd file.
    /// </summary>
    internal static class DuneLocationsWriter
    {
        private const string separator = "\t";

        /// <summary>
        /// Writes dune locations to a bnd file.
        /// </summary>
        /// <param name="duneLocations">The dune locations to be written to the file.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write to <paramref name="filePath"/>.</exception>
        public static void WriteDuneLocations(IEnumerable<DuneLocation> duneLocations,
                                              string filePath)
        {
            if (duneLocations == null)
            {
                throw new ArgumentNullException(nameof(duneLocations));
            }
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var stringBuilder = new StringBuilder(Resources.DuneLocationsWriter_WriteDuneLocations_HeaderLine + Environment.NewLine);
            stringBuilder.AppendLine(Resources.DuneLocationsWriter_WriteDuneLocations_UnitsLine);

            try
            {
                foreach (DuneLocation location in duneLocations)
                {
                    stringBuilder.AppendLine(CreateCsvLine(location));
                }

                File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilsResources.Error_General_output_error_0, filePath), e);
            }
        }

        private static string CreateCsvLine(DuneLocation location)
        {
            var stringComponents = new List<string>
            {
                location.CoastalAreaId.ToString(null, CultureInfo.InvariantCulture),
                location.Offset.ToString(DuneErosionDataResources.DuneLocation_Offset_format, CultureInfo.InvariantCulture),
                Resources.DuneLocationsWriter_CreateCsvLine_Parameter_without_value,
                location.D50.ToString(null, CultureInfo.InvariantCulture)
            };

            stringComponents.InsertRange(2, GetOutputValues(location.Output));

            return string.Join(separator, stringComponents);
        }

        private static IEnumerable<string> GetOutputValues(DuneLocationOutput output)
        {
            return output == null
                       ? new[]
                       {
                           Resources.DuneLocationsWriter_CreateCsvLine_Parameter_without_value,
                           Resources.DuneLocationsWriter_CreateCsvLine_Parameter_without_value,
                           Resources.DuneLocationsWriter_CreateCsvLine_Parameter_without_value
                       }
                       : new[]
                       {
                           GetOutputValue(output.WaterLevel),
                           GetOutputValue(output.WaveHeight),
                           GetOutputValue(output.WavePeriod)
                       };
        }

        private static string GetOutputValue(RoundedDouble value)
        {
            return !double.IsNaN(value)
                       ? value.ToString(null, CultureInfo.InvariantCulture)
                       : Resources.DuneLocationsWriter_CreateCsvLine_Parameter_without_value;
        }
    }
}