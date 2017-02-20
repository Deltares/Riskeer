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
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.Readers;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Piping.IO.Importers
{
    /// <summary>
    /// Imports a piping configuration and stores it on a <see cref="CalculationGroup"/>.
    /// </summary>
    public class PipingConfigurationImporter : FileImporterBase<CalculationGroup>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PipingConfigurationImporter));

        private readonly IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations;

        private readonly List<ICalculationBase> validCalculationItems;

        /// <summary>
        /// Creates a new instance of <see cref="PipingConfigurationImporter"/>.
        /// </summary>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations used to check if
        ///     the imported objects contain the right location.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingConfigurationImporter(string filePath,
                                           CalculationGroup importTarget,
                                           IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
            : base(filePath, importTarget)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }
            this.hydraulicBoundaryLocations = hydraulicBoundaryLocations;

            validCalculationItems = new List<ICalculationBase>();
        }

        protected override void LogImportCanceledMessage()
        {
            log.Info(Resources.PipingConfigurationImporter_LogImportCanceledMessage_import_canceled_no_data_read);
        }

        protected override bool OnImport()
        {
            NotifyProgress("Inlezen berekening configuratie.", 1, 3);

            ReadResult<IReadPipingCalculationItem> readResult = ReadConfiguration();
            if (readResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            NotifyProgress("Valideren berekening configuratie.", 2, 3);

            foreach (IReadPipingCalculationItem readItem in readResult.ImportedItems)
            {
                if (Canceled)
                {
                    return false;
                }

                ValidateReadItems(readItem);
            }

            NotifyProgress(RingtoetsCommonIOResources.Importer_ProgressText_Adding_imported_data_to_DataModel, 3, 3);
            AddItemsToModel();

            return true;
        }

        private void AddItemsToModel()
        {
            foreach (ICalculationBase validCalculationItem in validCalculationItems)
            {
                ImportTarget.Children.Add(validCalculationItem);
            }
        }

        private void ValidateReadItems(IReadPipingCalculationItem readItem)
        {
            var readCalculation = readItem as ReadPipingCalculation;
            var readCalculationGroup = readItem as ReadPipingCalculationGroup;

            if (readCalculation != null)
            {
                ValidateCalculation(readCalculation);
            }

            if (readCalculationGroup != null)
            {
                ValidateCalculationGroup(readCalculationGroup);
            }
        }

        private void ValidateCalculation(ReadPipingCalculation readCalculation)
        {
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Name = readCalculation.Name
            };

            if (readCalculation.HydraulicBoundaryLocation != null)
            {
                HydraulicBoundaryLocation location = hydraulicBoundaryLocations.FirstOrDefault(l => l.Name == readCalculation.HydraulicBoundaryLocation);

                if (location != null)
                {
                    pipingCalculation.InputParameters.HydraulicBoundaryLocation = location;
                }
                else
                { 
                    log.Warn("Hydraulische randvoorwaarde locatie bestaat niet. Berekening overgeslagen.");
                    return;
                }
            }

            validCalculationItems.Add(pipingCalculation);

            // Validate when set:
            // - HR location
            // - Surface line
            // - Stochastic soil model
            // - Stochastic soil profile
            // Validate the stochastic soil model crosses the surface line when set
            // Validate the stochastic soil profile is part of the soil model
        }

        private static void ValidateCalculationGroup(ReadPipingCalculationGroup readCalculationGroup)
        {
        }

        private ReadResult<IReadPipingCalculationItem> ReadConfiguration()
        {
            try
            {
                return new ReadResult<IReadPipingCalculationItem>(false)
                {
                    ImportedItems = new PipingConfigurationReader(FilePath).Read().ToList()
                };
            }
            catch (ArgumentException e)
            {
                return HandleCriticalFileReadError(e);
            }
            catch (CriticalFileReadException e)
            {
                return HandleCriticalFileReadError(e);
            }
        }

        private static ReadResult<IReadPipingCalculationItem> HandleCriticalFileReadError(Exception e)
        {
            var errorMessage = string.Format(Resources.PipingConfigurationImporter_HandleCriticalFileReadError_Error_0_no_configuration_imported,
                                             e.Message);
            log.Error(errorMessage);
            return new ReadResult<IReadPipingCalculationItem>(true);
        }
    }
}