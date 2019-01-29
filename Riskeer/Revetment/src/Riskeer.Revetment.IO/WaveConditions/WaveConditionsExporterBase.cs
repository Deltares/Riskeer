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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using log4net;
using Ringtoets.Revetment.IO.Properties;

namespace Riskeer.Revetment.IO.WaveConditions
{
    /// <summary>
    /// Abstract class for wave conditions to csv file exporters.
    /// </summary>
    public abstract class WaveConditionsExporterBase : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveConditionsExporterBase));

        private readonly IEnumerable<ExportableWaveConditions> exportableWaveConditionsCollection;
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsExporterBase"/>. 
        /// </summary>
        /// <param name="exportableWaveConditionsCollection">The <see cref="ExportableWaveConditions"/> objects to export.</param>
        /// <param name="filePath">The file path to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="exportableWaveConditionsCollection"/> or 
        /// <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the file could not be written.</exception>
        protected WaveConditionsExporterBase(IEnumerable<ExportableWaveConditions> exportableWaveConditionsCollection, string filePath)
        {
            if (exportableWaveConditionsCollection == null)
            {
                throw new ArgumentNullException(nameof(exportableWaveConditionsCollection));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            this.exportableWaveConditionsCollection = exportableWaveConditionsCollection;
            this.filePath = filePath;
        }

        public bool Export()
        {
            try
            {
                WaveConditionsWriter.WriteWaveConditions(exportableWaveConditionsCollection, filePath);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(Resources.WaveConditionsExporter_Error_Exception_0_no_WaveConditions_exported, e.Message);
                return false;
            }

            return true;
        }
    }
}