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
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LowerLimitNorm_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LowerLimitNorm_Description))]
        public double LowerLimitNorm
        {
            get => data.LowerLimitNorm;
            set
            {
                ChangeNorm(() => data.LowerLimitNorm = value, NormType.LowerLimit);
            }
        }

        [PropertyOrder(2)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SignalingNorm_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SignalingNorm_Description))]
        public double SignalingNorm
        {
            get => data.SignalingNorm;
            set
            {
                ChangeNorm(() => data.SignalingNorm = value, NormType.Signaling);
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.NormType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.NormType_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public NormType NormativeNorm
        {
            get => data.NormativeNorm;
            set
            {
                normChangeHandler.ChangeNormativeNormType(() => data.NormativeNorm = value);
            }
        }

        private void ChangeNorm(Action action, NormType normType)
        {
            if (data.NormativeNorm == normType)
            {
                normChangeHandler.ChangeNormativeNorm(action);
            }
            else
            {
                normChangeHandler.ChangeNorm(action);
            }
        }
    }
}