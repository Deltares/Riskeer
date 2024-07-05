// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Forms.Properties;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Piping.Forms.ChangeHandlers
{
    /// <summary>
    /// Class which can, if required, inquire the user for a confirmation when a change to the
    /// <see cref="PipingFailureMechanismSectionConfiguration"/> requires calculation results to be altered.
    /// </summary>
    public class PipingFailureMechanismSectionConfigurationChangeHandler : IObservablePropertyChangeHandler
    {
        private readonly PipingFailureMechanismSectionConfiguration sectionConfiguration;
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionConfigurationChangeHandler"/>.
        /// </summary>
        /// <param name="sectionConfiguration">The <see cref="PipingFailureMechanismSectionConfiguration"/> for which to handle the changes.</param>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> the section configuration belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismSectionConfigurationChangeHandler(PipingFailureMechanismSectionConfiguration sectionConfiguration,
                                                                       PipingFailureMechanism failureMechanism)
        {
            if (sectionConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sectionConfiguration));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            this.sectionConfiguration = sectionConfiguration;
            this.failureMechanism = failureMechanism;
        }

        public IEnumerable<IObservable> SetPropertyValueAfterConfirmation(SetObservablePropertyValueDelegate setValue)
        {
            if (setValue == null)
            {
                throw new ArgumentNullException(nameof(setValue));
            }

            var changedObjects = new List<IObservable>();

            IEnumerable<ProbabilisticPipingCalculationScenario> affectedCalculations = GetProbabilisticPipingCalculationScenarios().ToArray();
            if (affectedCalculations.Any())
            {
                if (ConfirmPropertyChange())
                {
                    setValue();
                    PropertyChanged(affectedCalculations);
                    changedObjects.Add(sectionConfiguration);
                    changedObjects.AddRange(affectedCalculations);
                }
            }
            else
            {
                setValue();
                changedObjects.Add(sectionConfiguration);
            }

            return changedObjects;
        }

        private IEnumerable<ProbabilisticPipingCalculationScenario> GetProbabilisticPipingCalculationScenarios()
        {
            FailureMechanismSection failureMechanismSection = sectionConfiguration.Section;
            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(failureMechanismSection.Points);

            return failureMechanism.Calculations.OfType<ProbabilisticPipingCalculationScenario>()
                                   .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments) && pc.HasOutput);
        }

        private static void PropertyChanged(IEnumerable<ProbabilisticPipingCalculationScenario> affectedCalculations)
        {
            foreach (ProbabilisticPipingCalculationScenario calculation in affectedCalculations)
            {
                calculation.ClearOutput();
            }
        }

        private static bool ConfirmPropertyChange()
        {
            DialogResult result = MessageBox.Show(
                Resources.PipingFailureMechanismSectionConfigurationChangeHandler_Confirm_section_configuration_property_change_and_clear_dependent_data,
                CoreCommonBaseResources.Confirm,
                MessageBoxButtons.OKCancel);

            return result == DialogResult.OK;
        }
    }
}