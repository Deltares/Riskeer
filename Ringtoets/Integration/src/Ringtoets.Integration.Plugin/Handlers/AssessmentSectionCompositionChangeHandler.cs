﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.DuneErosion.Data;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.Integration.Service;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for changing the <see cref="IAssessmentSection.Composition"/>
    /// value clearing all data dependent on the original composition value.
    /// </summary>
    public class AssessmentSectionCompositionChangeHandler : IAssessmentSectionCompositionChangeHandler
    {
        private readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionCompositionChangeHandler));

        public bool ConfirmCompositionChange()
        {
            DialogResult result = MessageBox.Show(Resources.AssessmentSectionCompositionChangeHandler_Confirm_change_composition_and_clear_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        public IEnumerable<IObservable> ChangeComposition(IAssessmentSection assessmentSection, AssessmentSectionComposition newComposition)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var oldFailureMechanismContributions = assessmentSection.GetFailureMechanisms().ToDictionary(f => f, f => f.Contribution);

            var affectedObjects = new List<IObservable>();
            if (assessmentSection.Composition != newComposition)
            {
                assessmentSection.ChangeComposition(newComposition);

                affectedObjects.Add(assessmentSection);

                IObservable[] affectedCalculations =
                    RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(
                                                           GetFailureMechanismsToUpdate(assessmentSection, oldFailureMechanismContributions))
                                                       .ToArray();

                if (affectedCalculations.Length > 0)
                {
                    affectedObjects.AddRange(affectedCalculations);
                    log.InfoFormat(Resources.ChangeHandler_Results_of_NumberOfCalculations_0_calculations_cleared,
                                   affectedObjects.OfType<ICalculation>().Count());
                }

                affectedObjects.AddRange(ClearHydraulicBoundaryLocationOutput(GetFailureMechanismsToUpdate(assessmentSection, oldFailureMechanismContributions)));
            }
            return affectedObjects;
        }

        private static IFailureMechanism[] GetFailureMechanismsToUpdate(IAssessmentSection assessmentSection,
                                                                        Dictionary<IFailureMechanism, double> oldFailureMechanismContributions)
        {
            var failureMechanismsToClearOutputFor = new List<IFailureMechanism>();
            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                foreach (KeyValuePair<IFailureMechanism, double> oldFailureMechanismContribution in oldFailureMechanismContributions)
                {
                    if (failureMechanism is StabilityStoneCoverFailureMechanism || failureMechanism is WaveImpactAsphaltCoverFailureMechanism)
                    {
                        continue;
                    }

                    if (failureMechanism is DuneErosionFailureMechanism
                        || oldFailureMechanismContribution.Key.Equals(failureMechanism)
                        && Math.Abs(oldFailureMechanismContribution.Value) > 1e-6
                        && Math.Abs(oldFailureMechanismContribution.Value - failureMechanism.Contribution) > 1e-6)
                    {
                        failureMechanismsToClearOutputFor.Add(failureMechanism);
                        break;
                    }
                }
            }

            return failureMechanismsToClearOutputFor.ToArray();
        }

        private IEnumerable<IObservable> ClearHydraulicBoundaryLocationOutput(IEnumerable<IFailureMechanism> failureMechanismsToClearOutputFor)
        {
            IEnumerable<IObservable> affectedObjects =
                RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationOutputOfFailureMechanisms(failureMechanismsToClearOutputFor);
            if (affectedObjects.Any())
            {
                log.Info(Resources.AssessmentSectionCompositionChangeHandler_Waveheight_and_design_water_level_results_cleared);
                return RingtoetsDataSynchronizationService.GetHydraulicBoundaryLocationCollectionsOfFailureMechanisms(failureMechanismsToClearOutputFor);
            }
            return Enumerable.Empty<IObservable>();
        }
    }
}