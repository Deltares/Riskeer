// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.IO.Readers;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Configurations.Import
{
    /// <summary>
    /// Base class for importing a calculation configuration from an XML file and storing it on a <see cref="CalculationGroup"/>.
    /// </summary>
    /// <typeparam name="TCalculationConfigurationReader">The type of the reader to use for reading the XML file.</typeparam>
    /// <typeparam name="TReadCalculation">The type of the data read from the XML file by the reader.</typeparam>
    public abstract class CalculationConfigurationImporter<TCalculationConfigurationReader, TReadCalculation>
        : FileImporterBase<CalculationGroup>
        where TCalculationConfigurationReader : CalculationConfigurationReader<TReadCalculation>
        where TReadCalculation : class, IConfigurationItem
    {
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
            Log.Info(Resources.CalculationConfigurationImporter_LogImportCanceledMessage_Import_canceled_No_data_changed);
        }

        protected override bool OnImport()
        {
            NotifyProgress(Resources.CalculationConfigurationImporter_ProgressText_Reading_configuration, 1, 3);

            ReadResult<IConfigurationItem> readResult = ReadConfiguration();
            if (readResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            NotifyProgress(Resources.CalculationConfigurationImporter_ProgressText_Validating_imported_data, 2, 3);

            var parsedCalculationItems = new List<ICalculationBase>();

            foreach (IConfigurationItem readItem in readResult.Items)
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
        /// Parses a calculation from the provided <paramref name="calculationConfiguration"/>.
        /// </summary>
        /// <param name="calculationConfiguration">The calculation read from XML.</param>
        /// <returns>A parsed calculation instance, or <c>null</c> when parsing failed.</returns>
        protected abstract ICalculation ParseReadCalculation(TReadCalculation calculationConfiguration);

        /// <summary>
        /// Tries to find the hydraulic boundary location with the given <paramref name="locationName"/> 
        /// in the <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="locationName">The name of the location to find.</param>
        /// <param name="calculationName">Name of the calculation to assign the location to.</param>
        /// <param name="hydraulicBoundaryLocations">The collection of <see cref="HydraulicBoundaryLocation"/>
        /// to search in.</param>
        /// <param name="foundLocation">The location with the name equal to <paramref name="locationName"/>
        /// if there was any.</param>
        /// <returns><c>true</c> if no <paramref name="locationName"/> is given, or when a location with 
        /// the name <paramref name="locationName"/> was found, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationName"/>
        ///  or <paramref name="hydraulicBoundaryLocations"/> is <c>null</c>.</exception>
        protected bool TryReadHydraulicBoundaryLocation(
            string locationName,
            string calculationName,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
            out HydraulicBoundaryLocation foundLocation)
        {
            if (calculationName == null)
            {
                throw new ArgumentNullException(nameof(calculationName));
            }
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            foundLocation = null;

            if (locationName != null)
            {
                HydraulicBoundaryLocation location = hydraulicBoundaryLocations.FirstOrDefault(l => l.Name == locationName);

                if (location == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.CalculationConfigurationImporter_ReadHydraulicBoundaryLocation_HydraulicBoundaryLocation_0_does_not_exist,
                                                          locationName),
                                                      calculationName);

                    return false;
                }

                foundLocation = location;
            }
            return true;
        }

        /// <summary>
        /// Tries to find the structure with the given <paramref name="structureId"/> 
        /// in the <paramref name="structures"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="StructureBase"/> to read.</typeparam>
        /// <param name="structureId">The id of the structure to find.</param>
        /// <param name="calculationName">Name of the calculation to assign the structure to.</param>
        /// <param name="structures">The collection of <typeparamref name="T"/> to search in.</param>
        /// <param name="foundStructure">The structure with the name equal to <paramref name="structureId"/>
        /// if there was any.</param>
        /// <returns><c>true</c> if no <paramref name="structureId"/> is given, or when a structure with 
        /// the name <paramref name="structureId"/> was found, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationName"/>
        /// or <paramref name="structures"/> is <c>null</c>.</exception>
        protected bool TryReadStructure<T>(
            string structureId,
            string calculationName,
            IEnumerable<T> structures,
            out T foundStructure)
            where T : StructureBase
        {
            if (calculationName == null)
            {
                throw new ArgumentNullException(nameof(calculationName));
            }
            if (structures == null)
            {
                throw new ArgumentNullException(nameof(structures));
            }
            foundStructure = null;
            if (structureId != null)
            {
                T structure = structures.FirstOrDefault(l => l.Id == structureId);

                if (structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.CalculationConfigurationImporter_ReadStructure_Structure_0_does_not_exist,
                                                          structureId),
                                                      calculationName);

                    return false;
                }

                foundStructure = structure;
            }

            return true;
        }

        /// <summary>
        /// Tries to find the foreshore profile with the given <paramref name="foreshoreProfileId"/> 
        /// in the <paramref name="foreshoreProfiles"/>.
        /// </summary>
        /// <param name="foreshoreProfileId">The id of the foreshore profile to find.</param>
        /// <param name="calculationName">Name of the calculation to assign the foreshore profile to.</param>
        /// <param name="foreshoreProfiles">The collection of <see cref="ForeshoreProfile"/> to search in.</param>
        /// <param name="foundForeshoreProfile">The foreshore profile with the name equal to 
        /// <paramref name="foreshoreProfileId"/> if there was any.</param>
        /// <returns><c>true</c> if no <paramref name="foreshoreProfileId"/> is given, or when a foreshore profile with 
        /// the name <paramref name="foreshoreProfileId"/> was found, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationName"/>
        /// or <paramref name="foreshoreProfiles"/> is <c>null</c>.</exception>
        protected bool TryReadForeshoreProfile(
            string foreshoreProfileId,
            string calculationName,
            IEnumerable<ForeshoreProfile> foreshoreProfiles,
            out ForeshoreProfile foundForeshoreProfile)
        {
            if (calculationName == null)
            {
                throw new ArgumentNullException(nameof(calculationName));
            }
            if (foreshoreProfiles == null)
            {
                throw new ArgumentNullException(nameof(foreshoreProfiles));
            }

            foundForeshoreProfile = null;

            if (foreshoreProfileId != null)
            {
                ForeshoreProfile foreshoreProfile = foreshoreProfiles.FirstOrDefault(fp => fp.Id == foreshoreProfileId);

                if (foreshoreProfile == null)
                {
                    Log.LogCalculationConversionError(string.Format(
                                                          Resources.CalculationConfigurationImporter_ReadForeshoreProfile_ForeshoreProfile_0_does_not_exist,
                                                          foreshoreProfileId),
                                                      calculationName);

                    return false;
                }

                foundForeshoreProfile = foreshoreProfile;
            }

            return true;
        }

        /// <summary>
        /// Assigns the <paramref name="waveReduction"/> parameters to the <paramref name="input"/>.
        /// </summary>
        /// <typeparam name="T">Type of the input for which values are assigned from the configuration.</typeparam>
        /// <param name="waveReduction">The wave reduction configuration containing values for the parameters.</param>
        /// <param name="input">The input to assign the values to.</param>
        protected static void SetWaveReductionParameters<T>(WaveReductionConfiguration waveReduction, T input)
            where T : IUseBreakWater, IUseForeshore
        {
            if (waveReduction != null)
            {
                if (waveReduction.UseForeshoreProfile.HasValue)
                {
                    input.UseForeshore = waveReduction.UseForeshoreProfile.Value;
                }

                if (waveReduction.UseBreakWater.HasValue)
                {
                    input.UseBreakWater = waveReduction.UseBreakWater.Value;
                }

                if (waveReduction.BreakWaterType.HasValue)
                {
                    input.BreakWater.Type = (BreakWaterType) new ConfigurationBreakWaterTypeConverter().ConvertTo(waveReduction.BreakWaterType.Value, typeof(BreakWaterType));
                }

                if (waveReduction.BreakWaterHeight.HasValue)
                {
                    input.BreakWater.Height = (RoundedDouble) waveReduction.BreakWaterHeight.Value;
                }
            }
        }

        /// <summary>
        /// Assigns the <paramref name="scenarioConfiguration"/> parameters to the <paramref name="scenario"/>.
        /// </summary>
        /// <param name="scenarioConfiguration">The scenario configuration containing values for the parameters.</param>
        /// <param name="scenario">The input to assign the values to.</param>
        /// <returns><c>true</c> if no <paramref name="scenarioConfiguration"/> was given, or when 
        /// the <paramref name="scenarioConfiguration"/> is not empty, <c>false</c> otherwise.</returns>
        protected bool TrySetScenarioParameters(ScenarioConfiguration scenarioConfiguration, ICalculationScenario scenario)
        {
            if (scenarioConfiguration == null)
            {
                return true;
            }

            bool hasContribution = scenarioConfiguration.Contribution.HasValue;
            bool hasRelevance = scenarioConfiguration.IsRelevant.HasValue;

            if (!hasContribution && !hasRelevance)
            {
                Log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_TrySetScenarioParameters_Scenario_empty,
                                                  scenario.Name);
                return false;
            }

            if (hasContribution)
            {
                scenario.Contribution = (RoundedDouble) (scenarioConfiguration.Contribution.Value / 100);
            }

            if (hasRelevance)
            {
                scenario.IsRelevant = scenarioConfiguration.IsRelevant.Value;
            }

            return true;
        }

        private ReadResult<IConfigurationItem> ReadConfiguration()
        {
            try
            {
                return new ReadResult<IConfigurationItem>(false)
                {
                    Items = CreateCalculationConfigurationReader(FilePath).Read().ToList()
                };
            }
            catch (Exception exception) when (exception is ArgumentException
                                              || exception is CriticalFileReadException)
            {
                string errorMessage = string.Format(Resources.CalculationConfigurationImporter_HandleCriticalFileReadError_Error_0_no_configuration_imported,
                                                    exception.Message);
                Log.Error(errorMessage, exception);
                return new ReadResult<IConfigurationItem>(true);
            }
        }

        /// <summary>
        /// Parses the read configuration item.
        /// </summary>
        /// <param name="readConfigurationItem">The read item to parse.</param>
        /// <returns>A parsed calculation item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the item to parse is not valid.</exception>
        private ICalculationBase ParseReadConfigurationItem(IConfigurationItem readConfigurationItem)
        {
            var readCalculationGroup = readConfigurationItem as CalculationGroupConfiguration;
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
        private CalculationGroup ParseReadCalculationGroup(CalculationGroupConfiguration readCalculationGroup)
        {
            var calculationGroup = new CalculationGroup
            {
                Name = readCalculationGroup.Name
            };

            foreach (IConfigurationItem item in readCalculationGroup.Items)
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