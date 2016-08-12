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
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO.Properties;

namespace Ringtoets.HydraRing.IO
{
    /// <summary>
    /// Exports the locations of a <see cref="HydraulicBoundaryDatabase"/> and stores them as a shapefile.
    /// </summary>
    public class HydraulicBoundaryLocationsExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationsExporter));

        private readonly ICollection<HydraulicBoundaryLocation> hydraulicBoundaryLocations;
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsExporter"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to export.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public HydraulicBoundaryLocationsExporter(ICollection<HydraulicBoundaryLocation> hydraulicBoundaryLocations, string filePath)
        {
            FileUtils.ValidateFilePath(filePath);

            this.hydraulicBoundaryLocations = hydraulicBoundaryLocations;
            this.filePath = filePath;
        }

        public bool Export()
        {
            var hydraulicBoundaryLocationsWriter = new HydraulicBoundaryLocationsWriter();

            try
            {
                hydraulicBoundaryLocationsWriter.WriteHydraulicBoundaryLocations(hydraulicBoundaryLocations, filePath);
            }
            catch (CriticalFileWriteException e)
            {
                log.Error(string.Format(Resources.HydraulicBoundaryLocationsExporter_Error_0_no_HydraulicBoundaryLocations_exported, e.Message));
                return false;
            }

            return true;
        }
    }
}