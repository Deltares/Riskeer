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
using System.Globalization;
using System.IO;
using System.Text;
using Core.Common.IO.Exceptions;
using Ringtoets.DuneErosion.Data;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

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
                throw new ArgumentNullException("duneLocations");
            }
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            var stringBuilder = new StringBuilder("Kv\tNr\tRp\tHs\tTp\tD50" + Environment.NewLine);

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
            string[] stringComponents =
            {
                location.CoastalAreaId.ToString(null, CultureInfo.InvariantCulture),
                location.Offset.ToString(null, CultureInfo.InvariantCulture),
                location.Output.WaterLevel.ToString(null, CultureInfo.InvariantCulture),
                location.Output.WaveHeight.ToString(null, CultureInfo.InvariantCulture),
                location.Output.WavePeriod.ToString(null, CultureInfo.InvariantCulture),
                location.D50.ToString(null, CultureInfo.InvariantCulture)
            };

            return string.Join(separator, stringComponents);
        }
    }
}