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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.AssessmentSection;
using Riskeer.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the composition of an <see cref="IAssessmentSection"/> for properties panel.
    /// </summary>
    public class AssessmentSectionCompositionProperties : ObjectProperties<IAssessmentSection>
    {
        private readonly IAssessmentSectionCompositionChangeHandler compositionChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionCompositionProperties"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section for which the <see cref="IAssessmentSection.Composition"/> properties are shown.</param>
        /// <param name="compositionChangeHandler">The <see cref="IAssessmentSectionCompositionChangeHandler"/> for when the composition changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssessmentSectionCompositionProperties(IAssessmentSection assessmentSection, IAssessmentSectionCompositionChangeHandler compositionChangeHandler)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (compositionChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(compositionChangeHandler));
            }

            Data = assessmentSection;
            this.compositionChangeHandler = compositionChangeHandler;
        }

        [PropertyOrder(1)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionComposition_Composition_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.AssessmentSectionComposition_Composition_Description))]
        public AssessmentSectionComposition Composition
        {
            get
            {
                return data.Composition;
            }
            set
            {
                if (compositionChangeHandler.ConfirmCompositionChange())
                {
                    IEnumerable<IObservable> changedObjects = compositionChangeHandler.ChangeComposition(data, value);
                    foreach (IObservable changedObject in changedObjects)
                    {
                        changedObject.NotifyObservers();
                    }

                    data.FailureMechanismContribution.NotifyObservers();
                }
            }
        }
    }
}