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
using System.Windows.Forms;
using Core.Common.Base;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.Integration.Service;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for changing the <see cref="FailureMechanismContribution.Norm"/>
    /// value of the <see cref="FailureMechanismContribution"/> of an <see cref="IAssessmentSection"/>
    /// and clearing all data dependent on the original norm value.
    /// </summary>
    public class FailureMechanismContributionNormChangeHandler : IFailureMechanismContributionNormChangeHandler
    {
        private readonly ILog log = LogManager.GetLogger(typeof(FailureMechanismContributionNormChangeHandler));

        public bool ConfirmNormChange()
        {
            DialogResult result = MessageBox.Show(Resources.FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        public IEnumerable<IObservable> ChangeNorm(IAssessmentSection assessmentSection, double newNormValue)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException("assessmentSection");
            }

            var changedObjects = new List<IObservable>();
            if (assessmentSection.FailureMechanismContribution.Norm.CompareTo(newNormValue) != 0)
            {
                assessmentSection.FailureMechanismContribution.Norm = newNormValue;

                changedObjects.AddRange(ClearAllNormDependentCalculationOutput(assessmentSection));
                changedObjects.Add(assessmentSection.FailureMechanismContribution);
                changedObjects.AddRange(assessmentSection.GetFailureMechanisms());
            }
            return changedObjects;
        }

        private IEnumerable<IObservable> ClearAllNormDependentCalculationOutput(IAssessmentSection assessmentSection)
        {
            List<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection).ToList();
            if (affectedObjects.Count > 0)
            {
                log.InfoFormat(Resources.ChangeHandler_Results_of_NumberOfCalculations_0_calculations_cleared,
                               affectedObjects.Count);
            }

            if (assessmentSection.HydraulicBoundaryDatabase != null)
            {
                affectedObjects.AddRange(ClearAllHydraulicBoundaryLocationOutput(assessmentSection));
            }
            return affectedObjects;
        }

        private IEnumerable<IObservable> ClearAllHydraulicBoundaryLocationOutput(IAssessmentSection assessmentSection)
        {
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                                                                                   .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                                                                   .First();

            IEnumerable<IObservable> hydraulicBoundaryLocationAffected = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(
                assessmentSection.HydraulicBoundaryDatabase, grassCoverErosionOutwardsFailureMechanism);
            if (hydraulicBoundaryLocationAffected.Any())
            {
                log.Info(Resources.FailureMechanismContributionNormChangeHandler_Waveheight_and_design_water_level_results_cleared);
                return new IObservable[]
                {
                    grassCoverErosionOutwardsFailureMechanism.HydraulicBoundaryLocations,
                    assessmentSection.HydraulicBoundaryDatabase
                };
            }
            return Enumerable.Empty<IObservable>();
        }
    }
}