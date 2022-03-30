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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Gui.Plugin;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Views;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Common.Plugin
{
    /// <summary>
    /// Class with helper methods to be used in Riskeer implementations of <see cref="PluginBase"/>.
    /// </summary>
    public static class RiskeerPluginHelper
    {
        /// <summary>
        /// Checks whether a view that contains <see cref="ICalculation"/> as <see cref="IView.Data"/>
        /// should be closed based on the removal of <paramref name="removedObject"/>.
        /// </summary>
        /// <param name="view">The view to be checked.</param>
        /// <param name="removedObject">The object that is removed.</param>
        /// <returns>Whether the view should be closed.</returns>
        public static bool ShouldCloseViewWithCalculationData(IView view, object removedObject)
        {
            if (removedObject is ICalculationContext<ICalculation, ICalculatableFailureMechanism> context)
            {
                return ReferenceEquals(view.Data, context.WrappedData);
            }

            IEnumerable<ICalculation> calculations;
            if (removedObject is ICalculationContext<CalculationGroup, ICalculatableFailureMechanism> calculationGroupContext)
            {
                calculations = calculationGroupContext.WrappedData
                                                      .GetCalculations();
            }
            else
            {
                calculations = GetCalculationsFromFailureMechanisms(removedObject);
            }

            return calculations.Any(c => ReferenceEquals(view.Data, c));
        }

        /// <summary>
        /// Checks whether <paramref name="view"/> should be closed based on the removal of
        /// <paramref name="removedObject"/>.
        /// </summary>
        /// <param name="view">The view to be checked.</param>
        /// <param name="removedObject">The object that is removed.</param>
        /// <returns>Whether the view should be closed.</returns>
        public static bool ShouldCloseForFailureMechanismView(CloseForFailureMechanismView view, object removedObject)
        {
            var failureMechanism = removedObject as IFailureMechanism;

            if (removedObject is IFailureMechanismContext<IFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (removedObject is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .FirstOrDefault(fm => fm == view.FailureMechanism)
                                   ?? assessmentSection.SpecificFailureMechanisms
                                                       .FirstOrDefault(fp => fp == view.FailureMechanism);
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        private static IEnumerable<ICalculation> GetCalculationsFromFailureMechanisms(object o)
        {
            if (o is ICalculatableFailureMechanism failureMechanism)
            {
                return failureMechanism.Calculations;
            }

            if (o is IAssessmentSection assessmentSection)
            {
                return assessmentSection.GetFailureMechanisms()
                                        .OfType<ICalculatableFailureMechanism>()
                                        .SelectMany(fm => fm.Calculations);
            }

            return Enumerable.Empty<ICalculation>();
        }
    }
}