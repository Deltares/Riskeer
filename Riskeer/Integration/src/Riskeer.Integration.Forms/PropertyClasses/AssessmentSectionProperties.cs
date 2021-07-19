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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="AssessmentSection"/> for properties panel.
    /// </summary>
    public class AssessmentSectionProperties : ObjectProperties<IAssessmentSection>
    {
        private readonly IAssessmentSectionCompositionChangeHandler compositionChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionProperties"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/>
        /// to show the properties of.</param>
        /// <param name="compositionChangeHandler">The <see cref="IAssessmentSectionCompositionChangeHandler"/>
        /// for when the composition changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssessmentSectionProperties(IAssessmentSection assessmentSection, IAssessmentSectionCompositionChangeHandler compositionChangeHandler)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (compositionChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(compositionChangeHandler));
            }

            this.compositionChangeHandler = compositionChangeHandler;

            Data = assessmentSection;
        }
        
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSection_Id_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.AssessmentSection_Id_Description))]
        public string Id => data.Id;

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSection_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.AssessmentSection_Name_Description))]
        public string Name
        {
            get => data.Name;
            set
            {
                data.Name = value;
                data.NotifyObservers();
            }
        }
        
        [PropertyOrder(3)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionComposition_Composition_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.AssessmentSectionComposition_Composition_Description))]
        public AssessmentSectionComposition Composition
        {
            get => data.Composition;
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