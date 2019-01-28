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
using System.ComponentModel;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TypeConverters;
using Riskeer.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the norm values in the <see cref="FailureMechanismContribution"/> for properties panel.
    /// </summary>
    public class NormProperties : ObjectProperties<FailureMechanismContribution>
    {
        private readonly IObservablePropertyChangeHandler normChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="NormProperties"/>.
        /// </summary>
        /// <param name="failureMechanismContribution">The <see cref="FailureMechanismContribution"/> for which the properties are shown.</param>
        /// <param name="normChangeHandler">The <see cref="IObservablePropertyChangeHandler"/> for when the norm changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public NormProperties(FailureMechanismContribution failureMechanismContribution, IObservablePropertyChangeHandler normChangeHandler)
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SignalingNorm_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SignalingNorm_Description))]
        public double SignalingNorm
        {
            get
            {
                return data.SignalingNorm;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.SignalingNorm = value, normChangeHandler);
            }
        }

        [PropertyOrder(2)]
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LowerLimitNorm_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LowerLimitNorm_Description))]
        public double LowerLimitNorm
        {
            get
            {
                return data.LowerLimitNorm;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.LowerLimitNorm = value, normChangeHandler);
            }
        }

        [PropertyOrder(3)]
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