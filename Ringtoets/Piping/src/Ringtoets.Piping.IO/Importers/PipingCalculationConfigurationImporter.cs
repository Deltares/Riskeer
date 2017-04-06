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
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.Readers;
using Ringtoets.Piping.IO.Schema;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Piping.IO.Importers
{
    /// <summary>
    /// Imports a piping calculation configuration from an XML file and stores it on a
    /// <see cref="CalculationGroup"/>.
    /// </summary>
    public class PipingCalculationConfigurationImporter : CalculationConfigurationImporter<PipingCalculationConfigurationReader, ReadPipingCalculation>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationConfigurationImporter"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="availableHydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="failureMechanism">The piping failure mechanism used to check
        /// if the imported objects contain the right data.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        public PipingCalculationConfigurationImporter(string xmlFilePath,
                                                      CalculationGroup importTarget,
                                                      IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations,
                                                      PipingFailureMechanism failureMechanism)
            : base(xmlFilePath, importTarget)
        {
            if (availableHydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(availableHydraulicBoundaryLocations));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            this.availableHydraulicBoundaryLocations = availableHydraulicBoundaryLocations;
            this.failureMechanism = failureMechanism;
        }

        protected override PipingCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new PipingCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(ReadPipingCalculation readCalculation)
        {
            var pipingCalculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Name = readCalculation.Name
            };

            if (TryReadHydraulicBoundaryData(readCalculation, pipingCalculation)
                && TryReadSurfaceLine(readCalculation, pipingCalculation)
                && TryReadEntryExitPoint(readCalculation, pipingCalculation)
                && TryReadStochasticSoilModel(readCalculation, pipingCalculation)
                && TryReadStochasticSoilProfile(readCalculation, pipingCalculation)
                && TryReadStochasts(readCalculation, pipingCalculation))
            {
                return pipingCalculation;
            }
            return null;
        }

        /// <summary>
        /// Reads the hydraulic boundary location or the assessment level that is manually set.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="readCalculation"/> has a <see cref="HydraulicBoundaryLocation"/>
        /// set which is not available in <see cref="availableHydraulicBoundaryLocations"/>, <c>true</c> otherwise.</returns>
        private bool TryReadHydraulicBoundaryData(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            HydraulicBoundaryLocation location;

            bool locationRead = TryReadHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocation, readCalculation.Name, availableHydraulicBoundaryLocations, out location);

            if (!locationRead)
            {
                return false;
            }

            if (location != null)
            {
                pipingCalculation.InputParameters.HydraulicBoundaryLocation = location;
            }
            else if(readCalculation.AssessmentLevel.HasValue)
            {
                pipingCalculation.InputParameters.UseAssessmentLevelManualInput = true;
                pipingCalculation.InputParameters.AssessmentLevel = (RoundedDouble) readCalculation.AssessmentLevel.Value;
            }

            return true;
        }

        /// <summary>
        /// Reads the surface line.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="readCalculation"/> has a <see cref="RingtoetsPipingSurfaceLine"/>
        /// set which is not available in <see cref="PipingFailureMechanism.SurfaceLines"/>, <c>true</c> otherwise.</returns>
        private bool TryReadSurfaceLine(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.SurfaceLine != null)
            {
                RingtoetsPipingSurfaceLine surfaceLine = failureMechanism.SurfaceLines
                                                                         .FirstOrDefault(sl => sl.Name == readCalculation.SurfaceLine);

                if (surfaceLine == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                              Resources.PipingCalculationConfigurationImporter_ReadSurfaceLine_SurfaceLine_0_does_not_exist,
                              readCalculation.SurfaceLine),
                          pipingCalculation.Name);
                    return false;
                }

                pipingCalculation.InputParameters.SurfaceLine = surfaceLine;
            }
            return true;
        }

        /// <summary>
        /// Reads the entry point and exit point.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when entry or exit point is set without <see cref="RingtoetsPipingSurfaceLine"/>,
        /// or when entry or exit point is invalid, <c>true</c> otherwise.</returns>
        private bool TryReadEntryExitPoint(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            bool hasEntryPoint = readCalculation.EntryPointL.HasValue;
            bool hasExitPoint = readCalculation.ExitPointL.HasValue;

            if (readCalculation.SurfaceLine == null && (hasEntryPoint || hasExitPoint))
            {
                Log.LogCalculationConversionError(Resources.PipingCalculationConfigurationImporter_ReadSurfaceLine_EntryPointL_or_ExitPointL_defined_without_SurfaceLine,
                      pipingCalculation.Name);
                return false;
            }

            if (hasEntryPoint)
            {
                double entryPoint = readCalculation.EntryPointL.Value;

                try
                {
                    pipingCalculation.InputParameters.EntryPointL = (RoundedDouble) entryPoint;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadEntryExitPoint_Entry_point_invalid, entryPoint),
                          pipingCalculation.Name,
                          e);
                    return false;
                }
            }

            if (hasExitPoint)
            {
                double exitPoint = readCalculation.ExitPointL.Value;

                try
                {
                    pipingCalculation.InputParameters.ExitPointL = (RoundedDouble) exitPoint;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(Resources.PipingCalculationConfigurationImporter_ReadEntryExitPoint_Exit_point_invalid, exitPoint),
                          pipingCalculation.Name,
                          e);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the stochastic soil model.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when
        /// <list type="bullet">
        /// <item>the <paramref name="readCalculation"/> has a <see cref="StochasticSoilModel"/> set
        /// which is not available in the failure mechanism.</item>
        /// <item>The <see cref="StochasticSoilModel"/> does not intersect with the <see cref="RingtoetsPipingSurfaceLine"/>
        /// when this is set.</item>
        /// </list>
        /// <c>true</c> otherwise.</returns>
        private bool TryReadStochasticSoilModel(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.StochasticSoilModel != null)
            {
                StochasticSoilModel soilModel = failureMechanism.StochasticSoilModels
                                                                .FirstOrDefault(ssm => ssm.Name == readCalculation.StochasticSoilModel);

                if (soilModel == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                              Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_exist,
                              readCalculation.StochasticSoilModel),
                          pipingCalculation.Name);
                    return false;
                }

                if (pipingCalculation.InputParameters.SurfaceLine != null
                    && !soilModel.IntersectsWithSurfaceLineGeometry(pipingCalculation.InputParameters.SurfaceLine))
                {
                    Log.LogCalculationConversionError(string.Format(
                              Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilModel_Stochastische_soil_model_0_does_not_intersect_with_surfaceLine_1,
                              readCalculation.StochasticSoilModel,
                              readCalculation.SurfaceLine),
                          pipingCalculation.Name);
                    return false;
                }

                pipingCalculation.InputParameters.StochasticSoilModel = soilModel;
            }
            return true;
        }

        /// <summary>
        /// Reads the stochastic soil profile.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the <paramref name="readCalculation"/> has:
        /// <list type="bullet">
        /// <item>a <see cref="StochasticSoilProfile"/> set but no <see cref="StochasticSoilModel"/> is specified;</item>
        /// <item>a <see cref="StochasticSoilProfile"/> set which is not available in the <see cref="StochasticSoilModel"/>.</item>
        /// </list>
        /// <c>true</c> otherwise.</returns>
        private bool TryReadStochasticSoilProfile(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            if (readCalculation.StochasticSoilProfile != null)
            {
                if (pipingCalculation.InputParameters.StochasticSoilModel == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                              Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilProfile_No_soil_model_provided_for_soil_profile_with_name_0,
                              readCalculation.StochasticSoilProfile),
                          pipingCalculation.Name);
                    return false;
                }

                StochasticSoilProfile soilProfile = pipingCalculation.InputParameters.StochasticSoilModel.StochasticSoilProfiles
                                                                     .FirstOrDefault(ssp => ssp.SoilProfile.Name == readCalculation.StochasticSoilProfile);

                if (soilProfile == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                              Resources.PipingCalculationConfigurationImporter_ReadStochasticSoilProfile_Stochastic_soil_profile_0_does_not_exist_within_soil_model_1,
                              readCalculation.StochasticSoilProfile, readCalculation.StochasticSoilModel),
                          pipingCalculation.Name);
                    return false;
                }

                pipingCalculation.InputParameters.StochasticSoilProfile = soilProfile;
            }
            return true;
        }

        /// <summary>
        /// Reads the stochasts.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="pipingCalculation">The calculation to configure.</param>
        /// <returns><c>false</c> when a stochast value (mean or standard deviation) is invalid, <c>true</c> otherwise.</returns>
        private bool TryReadStochasts(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            return TryReadPhreaticLevelExit(readCalculation, pipingCalculation)
                   && TryReadDampingFactorExit(readCalculation, pipingCalculation);
        }

        private bool TryReadDampingFactorExit(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            var distribution = (LogNormalDistribution) pipingCalculation.InputParameters.DampingFactorExit.Clone();

            if (!distribution.TrySetDistributionProperties(readCalculation.DampingFactorExitMean,
                                                           readCalculation.DampingFactorExitStandardDeviation,
                                                           PipingCalculationConfigurationSchemaIdentifiers.DampingFactorExitStochastName,
                                                           pipingCalculation.Name))
            {
                return false;
            }

            pipingCalculation.InputParameters.DampingFactorExit = distribution;
            return true;
        }

        private bool TryReadPhreaticLevelExit(ReadPipingCalculation readCalculation, PipingCalculationScenario pipingCalculation)
        {
            var distribution = (NormalDistribution) pipingCalculation.InputParameters.PhreaticLevelExit.Clone();

            if (!distribution.TrySetDistributionProperties(readCalculation.PhreaticLevelExitMean,
                                                           readCalculation.PhreaticLevelExitStandardDeviation,
                                                           PipingCalculationConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName,
                                                           pipingCalculation.Name))
            {
                return false;
            }

            pipingCalculation.InputParameters.PhreaticLevelExit = distribution;
            return true;
        }
    }
}