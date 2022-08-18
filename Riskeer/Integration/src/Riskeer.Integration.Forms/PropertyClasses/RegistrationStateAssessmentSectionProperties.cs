﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Util.Attributes;
using Core.Common.Util.Enums;
using Core.Gui.Attributes;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="AssessmentSection"/> for properties panel in registration state.
    /// </summary>
    public class RegistrationStateAssessmentSectionProperties : AssessmentSectionProperties
    {
        private readonly IAssessmentSectionCompositionChangeHandler compositionChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="RegistrationStateAssessmentSectionProperties"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/>
        /// to show the properties of.</param>
        /// <param name="compositionChangeHandler">The <see cref="IAssessmentSectionCompositionChangeHandler"/>
        /// for when the composition changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public RegistrationStateAssessmentSectionProperties(IAssessmentSection assessmentSection, IAssessmentSectionCompositionChangeHandler compositionChangeHandler)
            : base(assessmentSection)
        {
            if (compositionChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(compositionChangeHandler));
            }

            this.compositionChangeHandler = compositionChangeHandler;
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
                IEnumerable<IObservable> changedObjects = compositionChangeHandler.ChangeComposition(data, value);
                foreach (IObservable changedObject in changedObjects)
                {
                    changedObject.NotifyObservers();
                }
            }
        }
    }
}