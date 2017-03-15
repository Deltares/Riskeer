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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.Readers;
using Ringtoets.Piping.IO.Schema;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Importers
{
    /// <summary>
    /// Imports a piping configuration from an XML file and stores it on a
    /// <see cref="CalculationGroup"/>.
    /// </summary>
    public class PipingCalculationConfigurationImporter : CalculationConfigurationImporter<PipingCalculationConfigurationReader, ReadPipingCalculation>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations;
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationConfigurationImporter"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="failureMechanism">The piping failure mechanism used to check
        /// if the imported objects contain the right data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        public PipingCalculationConfigurationImporter(string xmlFilePath,
                                           CalculationGroup importTarget,
                                           IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                           PipingFailureMechanism failureMechanism)
            : base(xmlFilePath, importTarget)
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
        }

        protected override PipingCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new PipingCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculationBase ParseReadCalculation(ReadPipingCalculation readCalculation)
        {
            var pipingCalculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Name = readCalculation.Name
            };

            ReadHydraulicBoundaryData(readCalculation, pipingCalculation);
            ReadSurfaceLine(readCalculation, pipingCalculation);
            ReadEntryExitPoint(readCalculation, pipingCalculation);
            ReadStochasticSoilModel(readCalculation, pipingCalculation);
            ReadStochasticSoilProfile(readCalculation, pipingCalculation);
            ReadStochasts(readCalculation, pipingCalculation);

            return pipingCalculation;
        }

        /// <summary>
        /// Reads the hydraulic boundary location or the assessment level that is manually set.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when the <paramref name="readCalculation"/>
        /// has a <see cref="HydraulicBoundaryLocation"/> set which is not available in <see cref="hydraulicBoundaryLocations"/>.</exception>
        private void ReadHydraulicBoundaryData(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.HydraulicBoundaryLocation != null)
            {
                HydraulicBoundaryLocation location = hydraulicBoundaryLocations
                    .FirstOrDefault(l => l.Name == readCalculation.HydraulicBoundaryLocation);

                if (location == null)
                {
                    throw new CriticalFileValidationException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadHydraulicBoundaryLocation_Hydraulic_boundary_location_0_does_not_exist,
                                                                            readCalculation.HydraulicBoundaryLocation));
                }

                pipingCalculation.InputParameters.HydraulicBoundaryLocation = location;
            }
            else if (readCalculation.AssessmentLevel.HasValue)
            {
                pipingCalculation.InputParameters.UseAssessmentLevelManualInput = true;
                pipingCalculation.InputParameters.AssessmentLevel = (RoundedDouble) readCalculation.AssessmentLevel;
            }
        }

        /// <summary>
        /// Reads the surface line.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when the <paramref name="readCalculation"/>
        /// has a <see cref="RingtoetsPipingSurfaceLine"/> set which is not available in the failure mechanism.</exception>
        private void ReadSurfaceLine(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.SurfaceLine != null)
            {
                RingtoetsPipingSurfaceLine surfaceLine = failureMechanism.SurfaceLines
                                                                         .FirstOrDefault(sl => sl.Name == readCalculation.SurfaceLine);

                if (surfaceLine == null)
                {
                    throw new CriticalFileValidationException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadSurfaceLine_SurfaceLine_0_does_not_exist,
                                                                            readCalculation.SurfaceLine));
                }

                pipingCalculation.InputParameters.SurfaceLine = surfaceLine;
            }
        }

        /// <summary>
        /// Reads the entry point and exit point.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when the entry point or exit point is invalid.</exception>
        private static void ReadEntryExitPoint(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            bool hasEntryPoint = readCalculation.EntryPointL.HasValue;
            bool hasExitPoint = readCalculation.ExitPointL.HasValue;

            if (readCalculation.SurfaceLine == null && (hasEntryPoint || hasExitPoint))
            {
                throw new CriticalFileValidationException(Resources.PipingCalculationConfigurationImporter_ReadSurfaceLine_EntryPointL_or_ExitPointL_defined_without_SurfaceLine);
            }

            if (hasEntryPoint)
            {
                var entryPoint = (double) readCalculation.EntryPointL;

                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => pipingCalculation.InputParameters.EntryPointL = (RoundedDouble) entryPoint,
                    string.Format(Resources.PipingCalculationConfigurationImporter_ReadEntryExitPoint_Entry_point_invalid, entryPoint));
            }

            if (hasExitPoint)
            {
                var exitPoint = (double) readCalculation.ExitPointL;

                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => pipingCalculation.InputParameters.ExitPointL = (RoundedDouble) exitPoint,
                    string.Format(Resources.PipingCalculationConfigurationImporter_ReadEntryExitPoint_Exit_point_invalid, exitPoint));
            }
        }

        /// <summary>
        /// Reads the stochastic soil model.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when
        /// <list type="bullet">
        /// <item>the <paramref name="readCalculation"/> has a <see cref="StochasticSoilModel"/> set
        /// which is not available in the failure mechanism.</item>
        /// <item>The <see cref="StochasticSoilModel"/> does not intersect with the <see cref="RingtoetsPipingSurfaceLine"/>
        /// when this is set.</item>
        /// </list>
        /// </exception>
        private void ReadStochasticSoilModel(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.StochasticSoilModel != null)
            {
                StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels
                                                                .FirstOrDefault(ssm => ssm.Name == readCalculation.StochasticSoilModel);

                if (soilModel == null)
                {
                    throw new CriticalFileValidationException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_exist,
                                                                            readCalculation.StochasticSoilModel));
                }

                if (pipingCalculation.InputParameters.SurfaceLine != null
                    && !soilModel.IntersectsWithSurfaceLineGeometry(pipingCalculation.InputParameters.SurfaceLine))
                {
                    throw new CriticalFileValidationException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_intersect_with_surfaceLine_1,
                                                                            readCalculation.StochasticSoilModel,
                                                                            readCalculation.SurfaceLine));
                }

                pipingCalculation.InputParameters.StochasticSoilModel = soilModel;
            }
        }

        /// <summary>
        /// Reads the stochastic soil profile.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when the <paramref name="readCalculation"/> has:
        /// <list type="bullet">
        /// <item>a <see cref="StochasticSoilProfile"/> set but no <see cref="StochasticSoilModel"/> is specified;</item>
        /// <item>a <see cref="StochasticSoilProfile"/> set which is not available in the <see cref="StochasticSoilModel"/>.</item>
        /// </list>
        /// </exception>
        private static void ReadStochasticSoilProfile(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.StochasticSoilProfile != null)
            {
                if (pipingCalculation.InputParameters.StochasticSoilModel == null)
                {
                    throw new CriticalFileValidationException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilProfile_No_soil_model_provided_for_soil_profile_with_name_0,
                                                                            readCalculation.StochasticSoilProfile));
                }

                StochasticSoilProfile soilProfile = pipingCalculation.InputParameters.StochasticSoilModel.StochasticSoilProfiles
                                                                     .FirstOrDefault(ssp => ssp.SoilProfile.Name == readCalculation.StochasticSoilProfile);

                if (soilProfile == null)
                {
                    throw new CriticalFileValidationException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilProfile_Stochastic_soil_profile_0_does_not_exist_within_soil_model_1,
                                                                            readCalculation.StochasticSoilProfile, readCalculation.StochasticSoilModel));
                }

                pipingCalculation.InputParameters.StochasticSoilProfile = soilProfile;
            }
        }

        /// <summary>
        /// Reads the stochasts.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when a stochast value (mean or standard deviation) is invalid.</exception>
        private static void ReadStochasts(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.PhreaticLevelExitMean.HasValue && readCalculation.PhreaticLevelExitStandardDeviation.HasValue)
            {
                var normalDistribution = new NormalDistribution();

                var mean = (double) readCalculation.PhreaticLevelExitMean;
                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => normalDistribution.Mean = (RoundedDouble) mean,
                    string.Format(Resources.PipingCalculationConfigurationImporter_ReadStochasts_Invalid_mean_0_for_stochast_with_name_1, mean,
                                  PipingCalculationConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName));

                var standardDeviation = (double) readCalculation.PhreaticLevelExitStandardDeviation;
                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => normalDistribution.StandardDeviation = (RoundedDouble) standardDeviation,
                    string.Format(Resources.PipingCalculationConfigurationImporter_ReadStochasts_Invalid_standard_deviation_0_for_stochast_with_name_1, standardDeviation,
                                  PipingCalculationConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName));

                pipingCalculation.InputParameters.PhreaticLevelExit = normalDistribution;
            }

            if (readCalculation.DampingFactorExitMean.HasValue && readCalculation.DampingFactorExitStandardDeviation.HasValue)
            {
                var logNormalDistribution = new LogNormalDistribution();

                var mean = (double) readCalculation.DampingFactorExitMean;
                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => logNormalDistribution.Mean = (RoundedDouble) mean,
                    string.Format(Resources.PipingCalculationConfigurationImporter_ReadStochasts_Invalid_mean_0_for_stochast_with_name_1, mean,
                                  PipingCalculationConfigurationSchemaIdentifiers.DampingFactorExitStochastName));

                var standardDeviation = (double) readCalculation.DampingFactorExitStandardDeviation;
                PerformActionHandlingAnyArgumentOutOfRangeException(
                    () => logNormalDistribution.StandardDeviation = (RoundedDouble) standardDeviation,
                    string.Format(Resources.PipingCalculationConfigurationImporter_ReadStochasts_Invalid_standard_deviation_0_for_stochast_with_name_1, standardDeviation,
                                  PipingCalculationConfigurationSchemaIdentifiers.DampingFactorExitStochastName));

                pipingCalculation.InputParameters.DampingFactorExit = logNormalDistribution;
            }
        }

        /// <summary>
        /// Performs the provided <paramref name="action"/> and handles any thrown <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="errorMessage">The error message to provide when rethrowing any thrown <see cref="ArgumentOutOfRangeException"/>.</param>
        /// <exception cref="CriticalFileValidationException">Thrown when <paramref name="action"/> throws an <see cref="ArgumentOutOfRangeException"/>.</exception>
        private static void PerformActionHandlingAnyArgumentOutOfRangeException(Action action, string errorMessage)
        {
            try
            {
                action();
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new CriticalFileValidationException($"{errorMessage} {e.Message}");
            }
        }
    }
}