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
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Writers;

namespace Ringtoets.Common.IO.Exporters
{
    /// <summary>
    /// Base class for exporting a calculation configuration and storing it as an XML file.
    /// </summary>
    /// <typeparam name="TWriter">The <see cref="SchemaCalculationConfigurationWriter{T}"/> 
    /// to use for exporting <see cref="TCalculation"/>.</typeparam>
    /// <typeparam name="TCalculation">The <see cref="TCalculation"/> type to export.</typeparam>
    /// <typeparam name="TConfiguration">The type of the calculations after conversion for writing.</typeparam>
    public abstract class SchemaCalculationConfigurationExporter<TWriter, TCalculation, TConfiguration> : IFileExporter
        where TWriter : SchemaCalculationConfigurationWriter<TConfiguration>
        where TCalculation : class, ICalculation
        where TConfiguration : class, IConfigurationItem
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SchemaCalculationConfigurationExporter<TWriter, TCalculation, TConfiguration>));
        private readonly IEnumerable<IConfigurationItem> configuration;
        private readonly TWriter writer;

        /// <summary>
        /// Creates a new instance of <see cref="SchemaCalculationConfigurationExporter{TWriter,TCalculation,TConfiguration}"/>.
        /// </summary>
        /// <param name="calculations">The calculation configuration to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        protected SchemaCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            writer = (TWriter) Activator.CreateInstance(typeof(TWriter), filePath);
            configuration = ToConfiguration(calculations);
        }

        public bool Export()
        {
            try
            {
                writer.Write(configuration);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(Resources.CalculationConfigurationExporter_Export_Exception_0_no_configuration_exported, e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converts the <paramref name="calculation"/> to a <typeparamref name="TConfiguration"/>.
        /// </summary>
        /// <param name="calculation">The <typeparamref name="TCalculation"/> to convert.</param>
        /// <returns>A new instance of <typeparamref name="TConfiguration"/> with values set equal
        /// to properties of <paramref name="calculation"/>.</returns>
        protected abstract TConfiguration ToConfiguration(TCalculation calculation);

        private IEnumerable<IConfigurationItem> ToConfiguration(IEnumerable<ICalculationBase> calculations)
        {
            foreach (ICalculationBase child in calculations)
            {
                var innerGroup = child as CalculationGroup;
                if (innerGroup != null)
                {
                    yield return ToConfiguration(innerGroup);
                }

                var calculation = child as TCalculation;
                if (calculation != null)
                {
                    yield return ToConfiguration(calculation);
                }

                if (innerGroup == null && calculation == null)
                {
                    throw new ArgumentException($"Cannot export calculation of type '{child.GetType()}' using this exporter.");
                }
            }
        }

        private CalculationGroupConfiguration ToConfiguration(CalculationGroup group)
        {
            return new CalculationGroupConfiguration(group.Name, ToConfiguration(group.Children));
        }
    }
}