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
using Ringtoets.HydraRing.IO;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Exports hydraulic boundary locations and stores them as a shapefile.
    /// </summary>
    public class HydraulicBoundaryLocationsExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationsExporter));

        private readonly IEnumerable<IHydraulicBoundaryLocation> hydraulicBoundaryLocations;
        private readonly string filePath;
        private readonly string designWaterLevelName;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsExporter"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to export.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <param name="designWaterLevelName">The Dutch name of the content of the 
        /// <see cref="IHydraulicBoundaryLocation.DesignWaterLevel"/> property.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/> 
        /// or <paramref name="designWaterLevelName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public HydraulicBoundaryLocationsExporter(IEnumerable<IHydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                  string filePath, string designWaterLevelName)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocations");
            }

            if (designWaterLevelName == null)
            {
                throw new ArgumentNullException("designWaterLevelName");
            }

            FileUtils.ValidateFilePath(filePath);

            this.hydraulicBoundaryLocations = hydraulicBoundaryLocations;
            this.filePath = filePath;
            this.designWaterLevelName = designWaterLevelName;
        }

        public bool Export()
        {
            var hydraulicBoundaryLocationsWriter = new HydraulicBoundaryLocationsWriter(designWaterLevelName);

            try
            {
                hydraulicBoundaryLocationsWriter.WriteHydraulicBoundaryLocations(hydraulicBoundaryLocations, filePath);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(RingtoetsCommonFormsResources.HydraulicBoundaryLocationsExporter_Error_Exception_0_no_HydraulicBoundaryLocations_exported, e.Message);
                return false;
            }

            return true;
        }
    }
}