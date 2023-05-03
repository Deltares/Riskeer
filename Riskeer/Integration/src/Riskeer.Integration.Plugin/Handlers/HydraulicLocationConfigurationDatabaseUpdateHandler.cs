// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.Plugin.Helpers;
using Riskeer.Integration.Plugin.Properties;
using Riskeer.Integration.Service;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class that can properly update <see cref="HydraulicLocationConfigurationDatabase"/> instances.
    /// </summary>
    public class HydraulicLocationConfigurationDatabaseUpdateHandler : IHydraulicLocationConfigurationDatabaseUpdateHandler
    {
        private readonly AssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationDatabaseUpdateHandler"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to use for clearing data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public HydraulicLocationConfigurationDatabaseUpdateHandler(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;
        }

        public bool InquireConfirmation()
        {
            if (!assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Any())
            {
                return true;
            }

            DialogResult result = MessageBox.Show(Resources.HydraulicLocationConfigurationDatabaseUpdateHandler_Confirm_clear_hydraulic_location_configuration_database_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);

            return result == DialogResult.OK;
        }

        public IEnumerable<IObservable> Update(ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                               IDictionary<HydraulicBoundaryDatabase, long> hydraulicBoundaryDatabaseLookup,
                                               string hlcdFilePath)
        {
            if (readHydraulicLocationConfigurationDatabase == null)
            {
                throw new ArgumentNullException(nameof(readHydraulicLocationConfigurationDatabase));
            }

            if (hydraulicBoundaryDatabaseLookup == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabaseLookup));
            }

            if (hlcdFilePath == null)
            {
                throw new ArgumentNullException(nameof(hlcdFilePath));
            }

            HydraulicLocationConfigurationDatabaseUpdateHelper.UpdateHydraulicLocationConfigurationDatabase(
                assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase,
                readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationSettings?.Single(),
                hlcdFilePath);

            foreach (KeyValuePair<HydraulicBoundaryDatabase, long> hydraulicBoundaryDatabaseItem in hydraulicBoundaryDatabaseLookup)
            {
                hydraulicBoundaryDatabaseItem.Key.UsePreprocessorClosure =
                    readHydraulicLocationConfigurationDatabase.ReadTracks
                                                              .FirstOrDefault(rt => rt.TrackId == hydraulicBoundaryDatabaseItem.Value)?
                                                              .UsePreprocessorClosure ?? false;
            }

            var changedObjects = new List<IObservable>
            {
                assessmentSection.HydraulicBoundaryData,
                assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase
            };

            changedObjects.AddRange(RiskeerDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection));
            changedObjects.AddRange(RiskeerDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection));

            return changedObjects;
        }
    }
}