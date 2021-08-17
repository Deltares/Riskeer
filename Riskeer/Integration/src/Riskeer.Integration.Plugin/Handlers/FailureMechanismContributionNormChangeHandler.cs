// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Integration.Plugin.Properties;
using Riskeer.Integration.Service;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for changing the <see cref="FailureMechanismContribution.Norm"/>
    /// value of the <see cref="FailureMechanismContribution"/> of an <see cref="IAssessmentSection"/>
    /// and clearing all data dependent on the original norm value.
    /// </summary>
    public class FailureMechanismContributionNormChangeHandler : IFailureMechanismContributionNormChangeHandler
    {
        private readonly IAssessmentSection assessmentSection;
        private readonly ILog log = LogManager.GetLogger(typeof(FailureMechanismContributionNormChangeHandler));

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionNormChangeHandler"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to change the contribution norm for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public FailureMechanismContributionNormChangeHandler(IAssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;
        }

        public void ChangeNormativeNormType(Action action)
        {
            // Clear only Piping and macro without manual assessment level
            PerformAction(action, "Als u de norm aanpast, dan worden de rekenresultaten van semi-probabilistische berekeningen zonder handmatig toetspeil verwijderd. " +
                                  Environment.NewLine + Environment.NewLine +
                                  "Weet u zeker dat u wilt doorgaan?",
                          () => new IObservable[0]);
        }

        public void ChangeNormativeNorm(Action action)
        {
            // Clear only locations depending on this norm and Piping and macro without manual assessment level
            PerformAction(action, "Als u de norm aanpast, dan worden de rekenresultaten van alle hydraulische belastingenlocaties behorende bij deze norm en semi-probabilistische berekeningen zonder handmatig toetspeil verwijderd. " +
                                  Environment.NewLine + Environment.NewLine +
                                  "Weet u zeker dat u wilt doorgaan?",
                          () => new IObservable[0]);
        }

        public void ChangeNorm(Action action)
        {
            // Clear only locations depending on this norm
            PerformAction(action, "Als u de norm aanpast, dan worden de rekenresultaten van alle hydraulische belastingenlocaties behorende bij deze norm verwijderd. " +
                                  Environment.NewLine + Environment.NewLine +
                                  "Weet u zeker dat u wilt doorgaan?",
                          () => new IObservable[0]);
        }

        private void PerformAction(Action action, string confirmationMessage, Func<IEnumerable<IObservable>> clearDataFunc)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var affectedObjects = new List<IObservable>();

            if (ConfirmPropertyChange(confirmationMessage))
            {
                action();

                affectedObjects.Add(assessmentSection.FailureMechanismContribution);
                affectedObjects.AddRange(clearDataFunc());

                DoPostUpdateActions(affectedObjects);
            }
        }

        private static bool ConfirmPropertyChange(string confirmationMessage)
        {
            DialogResult result = MessageBox.Show(confirmationMessage,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        private static void DoPostUpdateActions(IEnumerable<IObservable> affectedObjects)
        {
            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        private IEnumerable<IObservable> ClearAllNormDependentCalculationOutput()
        {
            List<IObservable> affectedObjects = RiskeerDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection).ToList();
            if (affectedObjects.Count > 0)
            {
                log.InfoFormat(Resources.ChangeHandler_Results_of_NumberOfCalculations_0_calculations_cleared,
                               affectedObjects.OfType<ICalculation>().Count());
            }

            affectedObjects.AddRange(ClearAllHydraulicBoundaryLocationCalculationOutput());

            return affectedObjects;
        }

        private IEnumerable<IObservable> ClearAllHydraulicBoundaryLocationCalculationOutput()
        {
            IEnumerable<IObservable> affectedObjects = RiskeerDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection);

            if (affectedObjects.Any())
            {
                log.Info(Resources.FailureMechanismContributionNormChangeHandler_Waveheight_and_design_water_level_results_cleared);
                return affectedObjects;
            }

            return Enumerable.Empty<IObservable>();
        }
    }

    public interface IFailureMechanismContributionNormChangeHandler
    {
        void ChangeNormativeNormType(Action action);

        void ChangeNormativeNorm(Action action);

        void ChangeNorm(Action action);
    }
}