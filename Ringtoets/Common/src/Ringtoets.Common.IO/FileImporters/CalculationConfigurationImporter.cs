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
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.Readers;

namespace Ringtoets.Common.IO.FileImporters
{
    /// <summary>
    /// Base class for importing a calculation configuration from an XML file and storing it on a <see cref="CalculationGroup"/>.
    /// </summary>
    /// <typeparam name="TCalculationConfigurationReader">The type of the reader to use for reading the XML file.</typeparam>
    /// <typeparam name="TReadCalculation">The type of the data read from the XML file by the reader.</typeparam>
    public abstract class CalculationConfigurationImporter<TCalculationConfigurationReader, TReadCalculation>
        : FileImporterBase<CalculationGroup>
        where TCalculationConfigurationReader : CalculationConfigurationReader<TReadCalculation>
        where TReadCalculation : class, IReadConfigurationItem
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CalculationConfigurationImporter<TCalculationConfigurationReader, TReadCalculation>));

        /// <summary>
        /// Creates a new instance of <see cref="CalculationConfigurationImporter{TCalculationConfigurationReader,TReadCalculation}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected CalculationConfigurationImporter(string xmlFilePath, CalculationGroup importTarget)
            : base(xmlFilePath, importTarget) {}

        protected override void LogImportCanceledMessage()
        {
            log.Info(Resources.CalculationConfigurationImporter_LogImportCanceledMessage_Import_canceled_no_data_read);
        }

        protected override bool OnImport()
        {
            NotifyProgress(Resources.CalculationConfigurationImporter_ProgressText_Reading_configuration, 1, 3);

            ReadResult<IReadConfigurationItem> readResult = ReadConfiguration();
            if (readResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            NotifyProgress(Resources.CalculationConfigurationImporter_ProgressText_Validating_imported_data, 2, 3);

            var parsedCalculationItems = new List<ICalculationBase>();

            foreach (IReadConfigurationItem readItem in readResult.Items)
            {
                if (Canceled)
                {
                    return false;
                }

                ICalculationBase parsedItem = ParseReadConfigurationItem(readItem);
                if (parsedItem != null)
                {
                    parsedCalculationItems.Add(parsedItem);
                }
            }

            NotifyProgress(Resources.Importer_ProgressText_Adding_imported_data_to_data_model, 3, 3);

            AddItemsToModel(parsedCalculationItems);

            return true;
        }

        /// <summary>
        /// Creates the reader used for reading the calculation configuration from the provided <paramref name="xmlFilePath"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <returns>A reader for reading the calculation configuration.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain configuration elements.</item>
        /// </list>
        /// </exception>
        protected abstract TCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath);

        /// <summary>
        /// Parses a calculation from the provided <paramref name="readCalculation"/>.
        /// </summary>
        /// <param name="readCalculation">The calculation read from XML.</param>
        /// <returns>A parsed calculation instance, or <c>null</c> when something goes wrong while parsing.</returns>
        protected abstract ICalculation ParseReadCalculation(TReadCalculation readCalculation);

        protected void LogOutOfRangeException(string errorMessage, string calculationName, ArgumentOutOfRangeException e)
        {
            LogReadCalculationConversionError($"{errorMessage} {e.Message}", calculationName);
        }

        protected void LogReadCalculationConversionError(string message, string calculationName)
        {
            log.ErrorFormat(Resources.CalculationConfigurationImporter_ValidateCalculation_Error_message_0_calculation_1_skipped, message, calculationName);
        }

        private ReadResult<IReadConfigurationItem> ReadConfiguration()
        {
            try
            {
                return new ReadResult<IReadConfigurationItem>(false)
                {
                    Items = CreateCalculationConfigurationReader(FilePath).Read().ToList()
                };
            }
            catch (Exception exception) when (exception is ArgumentException
                                              || exception is CriticalFileReadException)
            {
                string errorMessage = string.Format(Resources.CalculationConfigurationImporter_HandleCriticalFileReadError_Error_0_no_configuration_imported,
                                                    exception.Message);
                log.Error(errorMessage, exception);
                return new ReadResult<IReadConfigurationItem>(true);
            }
        }

        /// <summary>
        /// Parses the read configuration item.
        /// </summary>
        /// <param name="readConfigurationItem">The read item to parse.</param>
        /// <returns>A parsed calculation item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the item to parse is not valid.</exception>
        private ICalculationBase ParseReadConfigurationItem(IReadConfigurationItem readConfigurationItem)
        {
            var readCalculationGroup = readConfigurationItem as ReadCalculationGroup;
            if (readCalculationGroup != null)
            {
                return ParseReadCalculationGroup(readCalculationGroup);
            }

            var readCalculation = readConfigurationItem as TReadCalculation;
            if (readCalculation != null)
            {
                return ParseReadCalculation(readCalculation);
            }

            throw new InvalidOperationException("Can't parse item that is not a calculation or calculation group.");
        }

        /// <summary>
        /// Parses the read calculation group and it's children.
        /// </summary>
        /// <param name="readCalculationGroup">The calculation group to parse.</param>
        /// <returns>A parsed calculation group.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the one of the children
        /// to parse is not valid.</exception>
        private CalculationGroup ParseReadCalculationGroup(ReadCalculationGroup readCalculationGroup)
        {
            var calculationGroup = new CalculationGroup(readCalculationGroup.Name, true);

            foreach (IReadConfigurationItem item in readCalculationGroup.Items)
            {
                ICalculationBase parsedItem = ParseReadConfigurationItem(item);
                if (parsedItem != null)
                {
                    calculationGroup.Children.Add(parsedItem);
                }
            }

            return calculationGroup;
        }

        private void AddItemsToModel(IEnumerable<ICalculationBase> parsedCalculationItems)
        {
            foreach (ICalculationBase parsedCalculationItem in parsedCalculationItems)
            {
                ImportTarget.Children.Add(parsedCalculationItem);
            }
        }
    }
}