// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using log4net;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.IO.Properties;

namespace Riskeer.Common.IO.Configurations.Export
{
    /// <summary>
    /// Base class for exporting a calculation configuration and storing it as an XML file.
    /// </summary>
    /// <typeparam name="TWriter">The <see cref="CalculationConfigurationWriter{T}"/> 
    /// to use for exporting <typeparamref name="TCalculation"/>.</typeparam>
    /// <typeparam name="TCalculation">The <see cref="ICalculation"/> type to export.</typeparam>
    /// <typeparam name="TConfiguration">The <see cref="IConfigurationItem"/> type used to convert 
    /// <typeparamref name="TCalculation"/> into before writing to XML using a <typeparamref name="TWriter"/>.
    /// </typeparam>
    public abstract class CalculationConfigurationExporter<TWriter, TCalculation, TConfiguration>
        : IFileExporter
        where TWriter : CalculationConfigurationWriter<TConfiguration>
        where TCalculation : class, ICalculation
        where TConfiguration : class, IConfigurationItem
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CalculationConfigurationExporter<TWriter, TCalculation, TConfiguration>));
        private readonly IEnumerable<ICalculationBase> calculations;
        private readonly TWriter writer;
        private IEnumerable<IConfigurationItem> configuration;

        /// <summary>
        /// Creates a new instance of <see cref="CalculationConfigurationExporter{TWriter,TCalculation,TConfiguration}"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="writer">The writer to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected CalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, TWriter writer)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            this.calculations = calculations;
            this.writer = writer;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Thrown when any element of <see cref="calculations"/>
        /// is not a <see cref="CalculationGroup"/> nor a <typeparamref name="TCalculation"/>.</exception>
        public bool Export()
        {
            configuration = ToConfiguration(calculations).ToArray();

            try
            {
                writer.Write(configuration);
            }
            catch (CriticalFileWriteException e)
            {
                log.ErrorFormat(Resources.CalculationConfigurationExporter_Export_ExceptionMessage_0_no_configuration_exported, e.Message);
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

        /// <summary>
        /// Converts a sequence of <see cref="ICalculationBase"/> into a sequence of <see cref="IConfigurationItem"/>.
        /// </summary>
        /// <param name="calculationsToConvert">The sequence to be converted into a sequence of
        /// <see cref="IConfigurationItem"/>.</param>
        /// <returns>The converted <see cref="IEnumerable{T}"/> of <see cref="IConfigurationItem"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when an element of <paramref name="calculationsToConvert"/>
        /// isn't a <see cref="CalculationGroup"/> nor a <typeparamref name="TCalculation"/>.</exception>
        private IEnumerable<IConfigurationItem> ToConfiguration(IEnumerable<ICalculationBase> calculationsToConvert)
        {
            foreach (ICalculationBase child in calculationsToConvert)
            {
                switch (child)
                {
                    case CalculationGroup innerGroup:
                        yield return ToConfiguration(innerGroup);
                        break;
                    case TCalculation calculation:
                        yield return ToConfiguration(calculation);
                        break;
                    default:
                        throw new ArgumentException($"Cannot export calculation of type '{child.GetType()}' using this exporter.");
                }
            }
        }

        private CalculationGroupConfiguration ToConfiguration(CalculationGroup group)
        {
            return new CalculationGroupConfiguration(group.Name, ToConfiguration(group.Children).ToArray());
        }
    }
}