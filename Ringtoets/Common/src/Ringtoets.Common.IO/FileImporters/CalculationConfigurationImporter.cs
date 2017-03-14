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
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.Readers;

namespace Ringtoets.Common.IO.FileImporters
{
    /// <summary>
    /// Base class for importing a calculation configuration from an XML file and
    /// storing it on a <see cref="CalculationGroup"/>.
    /// </summary>
    public abstract class CalculationConfigurationImporter : FileImporterBase<CalculationGroup>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CalculationConfigurationImporter));

        /// <summary>
        /// Creates a new instance of <see cref="CalculationConfigurationImporter"/>.
        /// </summary>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        protected CalculationConfigurationImporter(string filePath, CalculationGroup importTarget)
            : base(filePath, importTarget) {}

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

            var validCalculationItems = new List<ICalculationBase>();

            foreach (IReadConfigurationItem readItem in readResult.Items)
            {
                if (Canceled)
                {
                    return false;
                }

                ICalculationBase processedItem = ProcessReadItem(readItem);
                if (processedItem != null)
                {
                    validCalculationItems.Add(processedItem);
                }
            }

            NotifyProgress(Resources.Importer_ProgressText_Adding_imported_data_to_data_model, 3, 3);
            AddItemsToModel(validCalculationItems);

            return true;
        }

        protected abstract ICalculationBase ProcessReadItem(IReadConfigurationItem readItem);

        protected abstract ICollection<IReadConfigurationItem> ReadConfigurationItems(string filePath);

        private ReadResult<IReadConfigurationItem> ReadConfiguration()
        {
            try
            {
                return new ReadResult<IReadConfigurationItem>(false)
                {
                    Items = ReadConfigurationItems(FilePath)
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

        private void AddItemsToModel(IEnumerable<ICalculationBase> validCalculationItems)
        {
            foreach (ICalculationBase validCalculationItem in validCalculationItems)
            {
                ImportTarget.Children.Add(validCalculationItem);
            }
        }
    }
}