// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Integration.Forms.Properties;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="FailureMechanismContribution"/> for properties panel.
    /// </summary>
    public class FailureMechanismContributionContextProperties : ObjectProperties<FailureMechanismContribution>
    {
        private IFailureMechanismContributionNormChangeHandler normChangeHandler;
        private IAssessmentSectionCompositionChangeHandler compositionChangeHandler;

        [TypeConverter(typeof(EnumConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "FailureMechanismContribution_Composition_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FailureMechanismContribution_Composition_Description")]
        public AssessmentSectionComposition AssessmentSectionComposition
        {
            get
            {
                return AssessmentSection.Composition;
            }
            set
            {
                if (compositionChangeHandler.ConfirmCompositionChange())
                {
                    IEnumerable<IObservable> changedObjects = compositionChangeHandler.ChangeComposition(AssessmentSection, value);
                    foreach (IObservable changedObject in changedObjects)
                    {
                        changedObject.NotifyObservers();
                    }
                }
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "FailureMechanismContribution_ReturnPeriod_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FailureMechanismContribution_ReturnPeriod_Description")]
        public int ReturnPeriod
        {
            get
            {
                return Convert.ToInt32(1.0/data.Norm);
            }
            set
            {
                if (value != 0 && normChangeHandler.ConfirmNormChange())
                {
                    double newNormValue = 1.0/Convert.ToInt32(value);
                    IEnumerable<IObservable> changedObjects = normChangeHandler.ChangeNorm(AssessmentSection, newNormValue);
                    foreach (IObservable changedObject in changedObjects)
                    {
                        changedObject.NotifyObservers();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the assessment section this property control belongs to.
        /// </summary>
        [DynamicVisible]
        public IAssessmentSection AssessmentSection { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFailureMechanismContributionNormChangeHandler"/> for when the norm changes.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the <c>null</c>is set.</exception>
        [DynamicVisible]
        public IFailureMechanismContributionNormChangeHandler NormChangeHandler
        {
            private get
            {
                return normChangeHandler;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                normChangeHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IAssessmentSectionCompositionChangeHandler"/> for when the norm changes.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the <c>null</c>is set.</exception>
        [DynamicVisible]
        public IAssessmentSectionCompositionChangeHandler CompositionChangeHandler
        {
            private get
            {
                return compositionChangeHandler;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                compositionChangeHandler = value;
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            // Hide all the properties that are used to set the data
            if (propertyName == TypeUtils.GetMemberName<FailureMechanismContributionContextProperties>(p => p.AssessmentSection))
            {
                return false;
            }
            if (propertyName == TypeUtils.GetMemberName<FailureMechanismContributionContextProperties>(p => p.NormChangeHandler))
            {
                return false;
            }
            if (propertyName == TypeUtils.GetMemberName<FailureMechanismContributionContextProperties>(p => p.CompositionChangeHandler))
            {
                return false;
            }

            return true;
        }
    }
}