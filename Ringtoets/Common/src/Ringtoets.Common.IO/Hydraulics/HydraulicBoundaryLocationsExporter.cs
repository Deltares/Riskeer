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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
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
        private readonly IHydraulicBoundaryLocationMetaDataAttributeNameProvider metaDataAttributeNameProvider;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsExporter"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to export.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <param name="metaDataAttributeNameProvider">The <see cref="IHydraulicBoundaryLocationMetaDataAttributeNameProvider"/>
        /// to be used for setting meta data attribute names.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/>, 
        /// <paramref name="metaDataAttributeNameProvider"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public HydraulicBoundaryLocationsExporter(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                  string filePath,
                                                  IHydraulicBoundaryLocationMetaDataAttributeNameProvider metaDataAttributeNameProvider)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            if (metaDataAttributeNameProvider == null)
            {
                throw new ArgumentNullException(nameof(metaDataAttributeNameProvider));
            }

            IOUtils.ValidateFilePath(filePath);

            this.hydraulicBoundaryLocations = hydraulicBoundaryLocations;
            this.filePath = filePath;
            this.metaDataAttributeNameProvider = metaDataAttributeNameProvider;
        }

        public bool Export()
        {
            var hydraulicBoundaryLocationsWriter = new HydraulicBoundaryLocationsWriter(metaDataAttributeNameProvider);

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