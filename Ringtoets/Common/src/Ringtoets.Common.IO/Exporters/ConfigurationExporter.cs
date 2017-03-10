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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.Common.IO.Exporters
{
    /// <summary>
    /// Base class for exporting a configuration and storing it as an XML file.
    /// </summary>
    public abstract class ConfigurationExporter<TWriter, TCalculation> : IFileExporter
        where TCalculation : class, ICalculation
        where TWriter : CalculationConfigurationWriter<TCalculation>, new()
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigurationExporter<TWriter, TCalculation>));
        private readonly IEnumerable<ICalculationBase> configuration;
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="ConfigurationExporter{TWriter,TCalculation}"/>.
        /// </summary>
        /// <param name="configuration">The configuration to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        protected ConfigurationExporter(IEnumerable<ICalculationBase> configuration, string filePath)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            IOUtils.ValidateFilePath(filePath);

            this.configuration = configuration;
            this.filePath = filePath;
        }

        public bool Export()
        {
            try
            {
                new TWriter().Write(configuration, filePath);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(Resources.ConfigurationExporter_Export_Error_exception_0_no_configuration_exported, e.Message);
                return false;
            }

            return true;
        }
    }
}