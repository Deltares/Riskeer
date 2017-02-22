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
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.Readers;
using Ringtoets.Piping.Primitives;
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
        private readonly PipingFailureMechanism failureMechanism;

        private readonly List<ICalculationBase> validCalculationItems;

        /// <summary>
        /// Creates a new instance of <see cref="PipingConfigurationImporter"/>.
        /// </summary>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations used to check if 
        /// the imported objects contain the right location.</param>
        /// <param name="failureMechanism">The piping failure mechanism used to check
        /// if the imported objects contain the right data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingConfigurationImporter(string filePath,
                                           CalculationGroup importTarget,
                                           IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                           PipingFailureMechanism failureMechanism)
            : base(filePath, importTarget)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            this.hydraulicBoundaryLocations = hydraulicBoundaryLocations;
            this.failureMechanism = failureMechanism;

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
            var pipingCalculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Name = readCalculation.Name
            };

            try
            {
                ReadHydraulicBoundaryLocation(readCalculation, pipingCalculation);
                ReadSurfaceLine(readCalculation, pipingCalculation);
                ReadEntryExitPoint(readCalculation, pipingCalculation);
                ReadStochasticSoilModel(readCalculation, pipingCalculation);
                ReadStochastics(readCalculation, pipingCalculation);
            }
            catch (CriticalFileValidationException e)
            {
                log.ErrorFormat("{0} Berekening overgeslagen.", e.Message);
                return;
            }

            validCalculationItems.Add(pipingCalculation);

            // Warn user when double name
        }

        /// <summary>
        /// Reads the hydraulic boundary location or the assessment level that is manually set.
        /// </summary>
        /// <param name="readCalculation">The read calculation</param>
        /// <param name="pipingCalculation"></param>
        private void ReadHydraulicBoundaryLocation(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.HydraulicBoundaryLocation != null)
            {
                HydraulicBoundaryLocation location = hydraulicBoundaryLocations.FirstOrDefault(l => l.Name == readCalculation.HydraulicBoundaryLocation);

                if (location != null)
                {
                    pipingCalculation.InputParameters.HydraulicBoundaryLocation = location;
                }
                else
                {
                    throw new CriticalFileValidationException("Hydraulische randvoorwaarde locatie bestaat niet.");
                }
            }
            else if (readCalculation.AssessmentLevel.HasValue)
            {
                pipingCalculation.InputParameters.UseAssessmentLevelManualInput = true;
                pipingCalculation.InputParameters.AssessmentLevel = (RoundedDouble)readCalculation.AssessmentLevel.Value;
            }
        }

        private void ReadSurfaceLine(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.SurfaceLine != null)
            {
                RingtoetsPipingSurfaceLine surfaceLine = failureMechanism.SurfaceLines
                                                                         .FirstOrDefault(sl => sl.Name == readCalculation.SurfaceLine);

                if (surfaceLine != null)
                {
                    pipingCalculation.InputParameters.SurfaceLine = surfaceLine;
                }
                else
                {
                    throw new CriticalFileValidationException("Profielschematisatie bestaat niet.");
                }
            }
        }
        private static void ReadEntryExitPoint(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.EntryPointL.HasValue)
            {
                pipingCalculation.InputParameters.EntryPointL = (RoundedDouble)readCalculation.EntryPointL.Value;
            }

            if (readCalculation.ExitPointL.HasValue)
            {
                pipingCalculation.InputParameters.ExitPointL = (RoundedDouble)readCalculation.ExitPointL.Value;
            }
        }

        private void ReadStochasticSoilModel(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.StochasticSoilModel != null)
            {
                StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels
                                                                .FirstOrDefault(ssm => ssm.Name == readCalculation.StochasticSoilModel);

                if (soilModel != null)
                {
                    if (pipingCalculation.InputParameters.SurfaceLine != null)
                    {
                        if (!soilModel.IntersectsWithSurfaceLineGeometry(pipingCalculation.InputParameters.SurfaceLine))
                        {
                            throw new CriticalFileValidationException("Ondergrondmodel kruist niet met de profielschematisatie.");
                        }
                    }

                    pipingCalculation.InputParameters.StochasticSoilModel = soilModel;

                    ReadStochasticSoilProfile(readCalculation, pipingCalculation, soilModel);
                }
                else
                {
                    throw new CriticalFileValidationException("Ondergrondmodel bestaat niet.");
                }
            }
        }

        private static void ReadStochasticSoilProfile(ReadPipingCalculation readCalculation,
                                                      PipingCalculationScenario pipingCalculation,
                                                      StochasticSoilModel soilModel)
        {
            if (readCalculation.StochasticSoilProfile != null)
            {
                StochasticSoilProfile soilProfile = soilModel.StochasticSoilProfiles
                                                             .FirstOrDefault(ssp => ssp.SoilProfile.Name == readCalculation.StochasticSoilProfile);

                if (soilProfile != null)
                {
                    pipingCalculation.InputParameters.StochasticSoilProfile = soilProfile;
                }
                else
                {
                    throw new CriticalFileValidationException("Ondergrondprofiel bestaat niet.");
                }
            }
        }

        private static void ReadStochastics(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.DampingFactorExitMean.HasValue && readCalculation.DampingFactorExitStandardDeviation.HasValue)
            {
                pipingCalculation.InputParameters.DampingFactorExit = new LogNormalDistribution
                {
                    Mean = (RoundedDouble)readCalculation.DampingFactorExitMean.Value,
                    StandardDeviation = (RoundedDouble)readCalculation.DampingFactorExitStandardDeviation.Value
                };
            }

            if (readCalculation.PhreaticLevelExitMean.HasValue && readCalculation.PhreaticLevelExitStandardDeviation.HasValue)
            {
                pipingCalculation.InputParameters.PhreaticLevelExit = new NormalDistribution
                {
                    Mean = (RoundedDouble)readCalculation.PhreaticLevelExitMean.Value,
                    StandardDeviation = (RoundedDouble)readCalculation.PhreaticLevelExitStandardDeviation.Value
                };
            }
        }

        private static void ValidateCalculationGroup(ReadPipingCalculationGroup readCalculationGroup)
        {
            // Warn user when double name
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