// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Gui.Commands;
using Core.Common.Util;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.DuneErosion.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Plugin.Properties;
using Riskeer.Integration.Service;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for changing the <see cref="IAssessmentSection.Composition"/>
    /// value clearing all data dependent on the original composition value.
    /// </summary>
    public class AssessmentSectionCompositionChangeHandler : IAssessmentSectionCompositionChangeHandler
    {
        private readonly IViewCommands viewCommands;
        private readonly ILog log = LogManager.GetLogger(typeof(AssessmentSectionCompositionChangeHandler));

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionCompositionChangeHandler"/>.
        /// </summary>
        /// <param name="viewCommands">The view commands used to close views for irrelevant
        /// failure mechanisms.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewCommands"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionCompositionChangeHandler(IViewCommands viewCommands)
        {
            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            this.viewCommands = viewCommands;
        }

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

            Dictionary<IFailureMechanism, double> oldFailureMechanismContributions = assessmentSection.GetFailureMechanisms().ToDictionary(f => f, f => f.Contribution, new ReferenceEqualityComparer<IFailureMechanism>());
            Dictionary<IFailureMechanism, bool> oldFailureMechanismRelevancies = assessmentSection.GetFailureMechanisms().ToDictionary(f => f, f => f.IsRelevant, new ReferenceEqualityComparer<IFailureMechanism>());

            var affectedObjects = new List<IObservable>();
            if (assessmentSection.Composition != newComposition)
            {
                assessmentSection.ChangeComposition(newComposition);

                affectedObjects.Add(assessmentSection);
                affectedObjects.AddRange(assessmentSection.GetFailureMechanisms());

                IFailureMechanism[] failureMechanismsToClearOutputFor = GetFailureMechanismsToClearOutputFor(assessmentSection, oldFailureMechanismContributions).ToArray();

                IObservable[] affectedCalculations =
                    RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(failureMechanismsToClearOutputFor).ToArray();

                if (affectedCalculations.Length > 0)
                {
                    affectedObjects.AddRange(affectedCalculations);
                    log.InfoFormat(Resources.ChangeHandler_Results_of_NumberOfCalculations_0_calculations_cleared,
                                   affectedObjects.OfType<ICalculation>().Count());
                }

                affectedObjects.AddRange(ClearHydraulicBoundaryLocationCalculationOutput(failureMechanismsToClearOutputFor));

                CloseViewsForIrrelevantFailureMechanisms(GetFailureMechanismsWithRelevancyUpdated(oldFailureMechanismRelevancies));
            }

            return affectedObjects;
        }

        private void CloseViewsForIrrelevantFailureMechanisms(IEnumerable<IFailureMechanism> failureMechanisms)
        {
            foreach (IFailureMechanism failureMechanism in failureMechanisms.Where(fm => !fm.IsRelevant))
            {
                viewCommands.RemoveAllViewsForItem(failureMechanism);
            }
        }

        private static IEnumerable<IFailureMechanism> GetFailureMechanismsWithRelevancyUpdated(IDictionary<IFailureMechanism, bool> oldFailureMechanismRelevancies)
        {
            return oldFailureMechanismRelevancies.Where(fmr => fmr.Value != fmr.Key.IsRelevant).Select(fmr => fmr.Key);
        }

        private static IEnumerable<IFailureMechanism> GetFailureMechanismsToClearOutputFor(IAssessmentSection assessmentSection,
                                                                                           IDictionary<IFailureMechanism, double> oldFailureMechanismContributions)
        {
            var failureMechanismsToClearOutputFor = new List<IFailureMechanism>();
            foreach (IFailureMechanism failureMechanism in assessmentSection.GetFailureMechanisms())
            {
                if (failureMechanism is StabilityStoneCoverFailureMechanism || failureMechanism is WaveImpactAsphaltCoverFailureMechanism)
                {
                    continue;
                }

                if (failureMechanism is DuneErosionFailureMechanism)
                {
                    failureMechanismsToClearOutputFor.Add(failureMechanism);
                }

                if (oldFailureMechanismContributions.ContainsKey(failureMechanism))
                {
                    double oldContribution = oldFailureMechanismContributions[failureMechanism];
                    if (Math.Abs(oldContribution) > 1e-6 && Math.Abs(oldContribution - failureMechanism.Contribution) > 1e-6)
                    {
                        failureMechanismsToClearOutputFor.Add(failureMechanism);
                    }
                }
            }

            return failureMechanismsToClearOutputFor;
        }

        private IEnumerable<IObservable> ClearHydraulicBoundaryLocationCalculationOutput(IEnumerable<IFailureMechanism> failureMechanismsToClearOutputFor)
        {
            IEnumerable<IObservable> affectedObjects =
                RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutputOfFailureMechanisms(failureMechanismsToClearOutputFor);
            if (affectedObjects.Any())
            {
                log.Info(Resources.AssessmentSectionCompositionChangeHandler_Waveheight_and_design_water_level_results_cleared);
                return affectedObjects;
            }

            return Enumerable.Empty<IObservable>();
        }
    }
}