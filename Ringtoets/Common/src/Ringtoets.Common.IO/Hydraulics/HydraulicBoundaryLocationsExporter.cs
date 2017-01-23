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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using log4net;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Hydraulics
{
    /// <summary>
    /// Exports hydraulic boundary locations and stores them as a shapefile.
    /// </summary>
    public class HydraulicBoundaryLocationsExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationsExporter));

        private readonly IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations;
        private readonly string filePath;
        private readonly string designWaterLevelName;
        private readonly string waveHeightName;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsExporter"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to export.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <param name="designWaterLevelName">The Dutch name of the content of the 
        /// <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/> property.</param>
        /// <param name="waveHeightName">The Dutch name of the content of the 
        /// <see cref="HydraulicBoundaryLocation.WaveHeight"/> property.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/>, 
        /// <paramref name="designWaterLevelName"/> or <see cref="waveHeightName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public HydraulicBoundaryLocationsExporter(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                  string filePath, string designWaterLevelName, string waveHeightName)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }
            if (designWaterLevelName == null)
            {
                throw new ArgumentNullException(nameof(designWaterLevelName));
            }
            if (waveHeightName == null)
            {
                throw new ArgumentNullException(nameof(waveHeightName));
            }

            IOUtils.ValidateFilePath(filePath);

            this.hydraulicBoundaryLocations = hydraulicBoundaryLocations;
            this.filePath = filePath;
            this.designWaterLevelName = designWaterLevelName;
            this.waveHeightName = waveHeightName;
        }

        public bool Export()
        {
            var hydraulicBoundaryLocationsWriter = new HydraulicBoundaryLocationsWriter(designWaterLevelName, waveHeightName);

            try
            {
                hydraulicBoundaryLocationsWriter.WriteHydraulicBoundaryLocations(hydraulicBoundaryLocations, filePath);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(Resources.HydraulicBoundaryLocationsExporter_Error_Exception_0_no_HydraulicBoundaryLocations_exported, e.Message);
                return false;
            }

            return true;
        }
    }
}