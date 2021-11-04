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
using Core.Common.Base;
using Core.Common.Util;
using Core.Gui.Commands;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Forms.PropertyClasses;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for changing the <see cref="IAssessmentSection.Composition"/> value.
    /// </summary>
    public class AssessmentSectionCompositionChangeHandler : IAssessmentSectionCompositionChangeHandler
    {
        private readonly IViewCommands viewCommands;

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

        public IEnumerable<IObservable> ChangeComposition(IAssessmentSection assessmentSection, AssessmentSectionComposition newComposition)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            Dictionary<IFailureMechanism, bool> oldFailureMechanismRelevancies = assessmentSection.GetFailureMechanisms().ToDictionary(f => f, f => f.InAssembly, new ReferenceEqualityComparer<IFailureMechanism>());

            var affectedObjects = new List<IObservable>();
            if (assessmentSection.Composition != newComposition)
            {
                assessmentSection.ChangeComposition(newComposition);

                affectedObjects.Add(assessmentSection);
                affectedObjects.Add(assessmentSection.FailureMechanismContribution);
                affectedObjects.AddRange(assessmentSection.GetFailureMechanisms());

                CloseViewsForFailureMechanismsNotInAssembly(GetFailureMechanismsWithInAssemblyUpdated(oldFailureMechanismRelevancies));
            }

            return affectedObjects;
        }

        private void CloseViewsForFailureMechanismsNotInAssembly(IEnumerable<IFailureMechanism> failureMechanisms)
        {
            foreach (IFailureMechanism failureMechanism in failureMechanisms.Where(fm => !fm.InAssembly))
            {
                viewCommands.RemoveAllViewsForItem(failureMechanism);
            }
        }

        private static IEnumerable<IFailureMechanism> GetFailureMechanismsWithInAssemblyUpdated(IDictionary<IFailureMechanism, bool> oldFailureMechanismRelevancies)
        {
            return oldFailureMechanismRelevancies.Where(fmr => fmr.Value != fmr.Key.InAssembly).Select(fmr => fmr.Key);
        }
    }
}