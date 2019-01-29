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
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Forms.PropertyClasses;
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
    public class FailureMechanismContributionNormChangeHandler : IObservablePropertyChangeHandler
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

        public IEnumerable<IObservable> SetPropertyValueAfterConfirmation(SetObservablePropertyValueDelegate setValue)
        {
            if (setValue == null)
            {
                throw new ArgumentNullException(nameof(setValue));
            }

            var affectedObjects = new List<IObservable>();

            if (ConfirmPropertyChange())
            {
                setValue();

                affectedObjects.AddRange(ClearAllNormDependentCalculationOutput());
                affectedObjects.Add(assessmentSection.FailureMechanismContribution);
                affectedObjects.AddRange(assessmentSection.GetFailureMechanisms());
            }

            return affectedObjects;
        }

        private static bool ConfirmPropertyChange()
        {
            DialogResult result = MessageBox.Show(Resources.FailureMechanismContributionNormChangeHandler_Confirm_change_norm_and_clear_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        private IEnumerable<IObservable> ClearAllNormDependentCalculationOutput()
        {
            List<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearFailureMechanismCalculationOutputs(assessmentSection).ToList();
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
            IEnumerable<IObservable> affectedObjects = RingtoetsDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationOutput(assessmentSection);

            if (affectedObjects.Any())
            {
                log.Info(Resources.FailureMechanismContributionNormChangeHandler_Waveheight_and_design_water_level_results_cleared);
                return affectedObjects;
            }

            return Enumerable.Empty<IObservable>();
        }
    }
}