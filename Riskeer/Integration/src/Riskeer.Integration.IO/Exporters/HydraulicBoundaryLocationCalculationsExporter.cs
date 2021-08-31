// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Hydraulics;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Integration.IO.Exporters
{
    /// <summary>
    /// Exports hydraulic boundary location calculations and stores them as a shapefile.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationsExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationCalculationsExporter));

        private readonly IEnumerable<HydraulicBoundaryLocationCalculation> calculations;
        private readonly string filePath;
        private readonly string outputMetaDataHeader;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationsExporter"/>.
        /// </summary>
        /// <param name="calculations">The calculations to export.</param>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <param name="outputMetaDataHeader"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/>
        /// or <paramref name="outputMetaDataHeader"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public HydraulicBoundaryLocationCalculationsExporter(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                             string filePath, string outputMetaDataHeader)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (outputMetaDataHeader == null)
            {
                throw new ArgumentNullException(nameof(outputMetaDataHeader));
            }

            IOUtils.ValidateFilePath(filePath);

            this.calculations = calculations;
            this.filePath = filePath;
            this.outputMetaDataHeader = outputMetaDataHeader;
        }

        public bool Export()
        {
            try
            {
                HydraulicBoundaryLocationCalculationsWriter.WriteHydraulicBoundaryLocationCalculations(
                    calculations, filePath, outputMetaDataHeader);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(RiskeerCommonIOResources.HydraulicBoundaryLocationsExporter_Error_Exception_0_no_HydraulicBoundaryLocations_exported,
                                e.Message);
                return false;
            }

            return true;
        }
    }
}