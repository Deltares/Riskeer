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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using log4net;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Riskeer.DuneErosion.IO
{
    /// <summary>
    /// Exports dune location calculations and stores them as a bnd file.
    /// </summary>
    public class DuneLocationCalculationsExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DuneLocationCalculationsExporter));

        private readonly IEnumerable<ExportableDuneLocationCalculation> exportableDuneLocationCalculations;
        private readonly string filePath;
        private readonly TypeConverter probabilityConverter;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsExporter"/>.
        /// </summary>
        /// <param name="exportableDuneLocationCalculations">The dune location calculations to export.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <param name="probabilityConverter">The <see cref="TypeConverter"/> to use when exporting the category boundaries.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="exportableDuneLocationCalculations"/>
        /// or <paramref name="probabilityConverter"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public DuneLocationCalculationsExporter(IEnumerable<ExportableDuneLocationCalculation> exportableDuneLocationCalculations,
                                                string filePath,
                                                TypeConverter probabilityConverter)
        {
            if (exportableDuneLocationCalculations == null)
            {
                throw new ArgumentNullException(nameof(exportableDuneLocationCalculations));
            }

            if (probabilityConverter == null)
            {
                throw new ArgumentNullException(nameof(probabilityConverter));
            }

            IOUtils.ValidateFilePath(filePath);

            this.exportableDuneLocationCalculations = exportableDuneLocationCalculations;
            this.filePath = filePath;
            this.probabilityConverter = probabilityConverter;
        }

        public bool Export()
        {
            try
            {
                DuneLocationCalculationsWriter.WriteDuneLocationCalculations(exportableDuneLocationCalculations, filePath, probabilityConverter);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(RingtoetsCommonIOResources.HydraulicBoundaryLocationsExporter_Error_Exception_0_no_HydraulicBoundaryLocations_exported, e.Message);
                return false;
            }

            return true;
        }
    }
}