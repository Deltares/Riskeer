﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.Exporters
{
    /// <summary>
    /// Exports a piping configuration and stores it as an XML file.
    /// </summary>
    public class PipingConfigurationExporter : IFileExporter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PipingConfigurationExporter));

        private readonly CalculationGroup calculationGroup;
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="PipingConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationGroup"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public PipingConfigurationExporter(CalculationGroup calculationGroup, string filePath)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            IOUtils.ValidateFilePath(filePath);

            this.calculationGroup = calculationGroup;
            this.filePath = filePath;
        }

        public bool Export()
        {
            try
            {
                PipingConfigurationWriter.Write(calculationGroup, filePath);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(Resources.PipingConfigurationExporter_Export_Error_exception_0_no_configuration_exported, e.Message);
                return false;
            }

            return true;
        }
    }
}