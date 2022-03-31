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
using System.ComponentModel;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the norm values in the <see cref="FailureMechanismContribution"/> for properties panel.
    /// </summary>
    public class NormProperties : ObjectProperties<FailureMechanismContribution>
    {
        private readonly IFailureMechanismContributionNormChangeHandler normChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="NormProperties"/>.
        /// </summary>
        /// <param name="failureMechanismContribution">The <see cref="FailureMechanismContribution"/> for which the properties are shown.</param>
        /// <param name="normChangeHandler">The <see cref="IObservablePropertyChangeHandler"/> for when the norm changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public NormProperties(FailureMechanismContribution failureMechanismContribution, IFailureMechanismContributionNormChangeHandler normChangeHandler)
        {
            if (failureMechanismContribution == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismContribution));
            }

            if (normChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(normChangeHandler));
            }

            Data = failureMechanismContribution;
            this.normChangeHandler = normChangeHandler;
        }

        [PropertyOrder(1)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MaximumAllowableFloodingProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MaximumAllowableFloodingProbability_Description))]
        public double MaximumAllowableFloodingProbability
        {
            get => data.MaximumAllowableFloodingProbability;
            set
            {
                ChangeNorm(() => data.MaximumAllowableFloodingProbability = value, NormativeProbabilityType.MaximumAllowableFloodingProbability);
            }
        }

        [PropertyOrder(2)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SignalFloodingProbability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SignalFloodingProbability_Description))]
        public double SignalFloodingProbability
        {
            get => data.SignalFloodingProbability;
            set
            {
                ChangeNorm(() => data.SignalFloodingProbability = value, NormativeProbabilityType.SignalFloodingProbability);
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.NormativeProbabilityType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.NormativeProbabilityType_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public NormativeProbabilityType NormativeProbabilityType
        {
            get => data.NormativeProbabilityType;
            set
            {
                normChangeHandler.ChangeNormativeProbabilityType(() => data.NormativeProbabilityType = value);
            }
        }

        private void ChangeNorm(Action action, NormativeProbabilityType normativeProbabilityType)
        {
            if (data.NormativeProbabilityType == normativeProbabilityType)
            {
                normChangeHandler.ChangeNormativeProbability(action);
            }
            else
            {
                normChangeHandler.ChangeProbability(action);
            }
        }
    }
}