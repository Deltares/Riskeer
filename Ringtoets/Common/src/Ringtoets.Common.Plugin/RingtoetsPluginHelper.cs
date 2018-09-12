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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Plugin
{
    /// <summary>
    /// Class with helper methods to be used in Ringtoets implementations of <see cref="PluginBase"/>.
    /// </summary>
    public static class RingtoetsPluginHelper
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
            var context = removedObject as ICalculationContext<ICalculation, IFailureMechanism>;
            if (context != null)
            {
                return ReferenceEquals(view.Data, context.WrappedData);
            }

            IEnumerable<ICalculation> calculations;

            var calculationGroupContext = removedObject as ICalculationContext<CalculationGroup, IFailureMechanism>;
            if (calculationGroupContext != null)
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
            var assessmentSection = removedObject as IAssessmentSection;
            var failureMechanismContext = removedObject as IFailureMechanismContext<IFailureMechanism>;
            var failureMechanism = removedObject as IFailureMechanism;

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .FirstOrDefault(fm => fm == view.FailureMechanism);
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        private static IEnumerable<ICalculation> GetCalculationsFromFailureMechanisms(object o)
        {
            var failureMechanism = o as IFailureMechanism;

            var context = o as IFailureMechanismContext<IFailureMechanism>;
            if (context != null)
            {
                failureMechanism = context.WrappedData;
            }

            if (failureMechanism != null)
            {
                return failureMechanism.Calculations;
            }

            var assessmentSection = o as IAssessmentSection;
            if (assessmentSection != null)
            {
                return assessmentSection.GetFailureMechanisms()
                                        .SelectMany(fm => fm.Calculations);
            }

            return Enumerable.Empty<ICalculation>();
        }
    }
}