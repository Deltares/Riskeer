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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="FailureMechanismContribution"/> for properties panel.
    /// </summary>
    public class FailureMechanismContributionProperties : ObjectProperties<FailureMechanismContribution>
    {
        private readonly IObservablePropertyChangeHandler normChangeHandler;
        private readonly IAssessmentSectionCompositionChangeHandler compositionChangeHandler;
        private readonly IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionProperties"/>.
        /// </summary>
        /// <param name="failureMechanismContribution">The <see cref="FailureMechanismContribution"/> for which the properties are shown.</param>
        /// <param name="assessmentSection">The assessment section for which the <see cref="IAssessmentSection.FailureMechanismContribution"/> properties are shown.</param>
        /// <param name="normChangeHandler">The <see cref="IObservablePropertyChangeHandler"/> for when the norm changes.</param>
        /// <param name="compositionChangeHandler">The <see cref="IAssessmentSectionCompositionChangeHandler"/> for when the composition changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismContributionProperties(
            FailureMechanismContribution failureMechanismContribution,
            IAssessmentSection assessmentSection,
            IObservablePropertyChangeHandler normChangeHandler,
            IAssessmentSectionCompositionChangeHandler compositionChangeHandler)
        {
            if (failureMechanismContribution == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismContribution));
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }
            if (normChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(normChangeHandler));
            }
            if (compositionChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(compositionChangeHandler));
            }

            Data = failureMechanismContribution;
            this.normChangeHandler = normChangeHandler;
            this.compositionChangeHandler = compositionChangeHandler;
            this.assessmentSection = assessmentSection;
        }

        [PropertyOrder(1)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismContribution_Composition_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismContribution_Composition_Description))]
        public AssessmentSectionComposition AssessmentSectionComposition
        {
            get
            {
                return assessmentSection.Composition;
            }
            set
            {
                if (compositionChangeHandler.ConfirmCompositionChange())
                {
                    IEnumerable<IObservable> changedObjects = compositionChangeHandler.ChangeComposition(assessmentSection, value);
                    foreach (IObservable changedObject in changedObjects)
                    {
                        changedObject.NotifyObservers();
                    }

                    data.NotifyObservers();
                }
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LowerLimitNorm_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LowerLimitNorm_Description))]
        public string LowerLimitNorm
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.LowerLimitNorm);
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.LowerLimitNorm = double.Parse(value), normChangeHandler);
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SignalingNorm_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SignalingNorm_Description))]
        public string SignalingNorm
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.SignalingNorm);
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.SignalingNorm = double.Parse(value), normChangeHandler);
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.NormType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.NormType_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public NormType NormativeNorm
        {
            get
            {
                return data.NormativeNorm;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.NormativeNorm = value, normChangeHandler);
            }
        }
    }
}