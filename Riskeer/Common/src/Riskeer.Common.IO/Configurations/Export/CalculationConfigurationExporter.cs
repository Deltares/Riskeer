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
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Configurations.Export
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
        private readonly IEnumerable<IConfigurationItem> configuration;
        private readonly TWriter writer;

        /// <summary>
        /// Creates a new instance of <see cref="CalculationConfigurationExporter{TWriter,TCalculation,TConfiguration}"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid or when any element
        /// of <paramref name="calculations"/> is not a <see cref="CalculationGroup"/> nor a <typeparamref name="TCalculation"/>.</exception>
        protected CalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            writer = CreateWriter(filePath);
            configuration = ToConfiguration(calculations).ToArray();
        }

        public bool Export()
        {
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
        /// Creates the writer that will be used in the <see cref="CalculationConfigurationExporter{TWriter,TCalculation,TConfiguration}"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to export to.</param>
        /// <returns>A new <typeparamref name="TWriter"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        protected abstract TWriter CreateWriter(string filePath);

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
        /// <param name="calculations">The sequence to be converted into a sequence of
        /// <see cref="IConfigurationItem"/>.</param>
        /// <returns>The converted <see cref="IEnumerable{T}"/> of <see cref="IConfigurationItem"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when an element of <paramref name="calculations"/>
        /// isn't a <see cref="CalculationGroup"/> nor a <typeparamref name="TCalculation"/>.</exception>
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
            return new CalculationGroupConfiguration(group.Name, ToConfiguration(group.Children).ToArray());
        }
    }
}