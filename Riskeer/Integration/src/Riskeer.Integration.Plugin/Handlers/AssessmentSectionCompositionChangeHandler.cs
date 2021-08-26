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
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Forms.PropertyClasses;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Riskeer.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class responsible for changing the <see cref="IAssessmentSection.Composition"/> value.
    /// </summary>
    public class AssessmentSectionCompositionChangeHandler : IAssessmentSectionCompositionChangeHandler
    {
        public IEnumerable<IObservable> ChangeComposition(IAssessmentSection assessmentSection, AssessmentSectionComposition newComposition)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var affectedObjects = new List<IObservable>();
            if (assessmentSection.Composition != newComposition)
            {
                assessmentSection.ChangeComposition(newComposition);

                affectedObjects.Add(assessmentSection);
                affectedObjects.Add(assessmentSection.FailureMechanismContribution);
                affectedObjects.AddRange(assessmentSection.GetFailureMechanisms());
            }

            return affectedObjects;
        }
    }
}