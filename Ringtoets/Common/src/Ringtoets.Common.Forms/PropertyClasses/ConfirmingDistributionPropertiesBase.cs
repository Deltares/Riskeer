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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Probabilistics;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties class for implementations of <see cref="IDistribution"/>.
    /// </summary>
    public abstract class ConfirmingDistributionPropertiesBase<TDistribution> : ObjectProperties<TDistribution>
        where TDistribution : IDistribution
    {
        private const string meanPropertyName = nameof(Mean);
        private const string standardDeviationPropertyName = nameof(StandardDeviation);
        private readonly bool isMeanReadOnly;
        private readonly bool isStandardDeviationReadOnly;
        private readonly IObservablePropertyChangeHandler changeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="ConfirmingDistributionPropertiesBase{TDistribution}"/>.
        /// </summary>
        /// <param name="propertiesReadOnly">Indicates which properties, if any, should be marked as read-only.</param>
        /// <param name="distribution">The data of the <see cref="TDistribution"/> to create the properties for.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>
        /// or when any number of properties in this class is editable and any other parameter is <c>null</c>.</exception>
        protected ConfirmingDistributionPropertiesBase(DistributionPropertiesReadOnly propertiesReadOnly,
                                                       TDistribution distribution,
                                                       IObservablePropertyChangeHandler handler)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }
            if (!propertiesReadOnly.HasFlag(DistributionPropertiesReadOnly.All))
            {
                if (handler == null)
                {
                    throw new ArgumentException(@"Change handler required if changes are possible.", nameof(handler));
                }
            }
            Data = distribution;

            isMeanReadOnly = propertiesReadOnly.HasFlag(DistributionPropertiesReadOnly.Mean);
            isStandardDeviationReadOnly = propertiesReadOnly.HasFlag(DistributionPropertiesReadOnly.StandardDeviation);

            changeHandler = handler;
        }

        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Distribution_DistributionType_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Distribution_DistributionType_Description))]
        public abstract string DistributionType { get; }

        [PropertyOrder(2)]
        [DynamicReadOnly]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.NormalDistribution_Mean_DisplayName))]
        public virtual RoundedDouble Mean
        {
            get
            {
                return data.Mean;
            }
            set
            {
                if (isMeanReadOnly)
                {
                    throw new InvalidOperationException("Mean is set to be read-only.");
                }

                ChangePropertyAndNotify(() => data.Mean = value);
            }
        }

        [PropertyOrder(3)]
        [DynamicReadOnly]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.NormalDistribution_StandardDeviation_DisplayName))]
        public virtual RoundedDouble StandardDeviation
        {
            get
            {
                return data.StandardDeviation;
            }
            set
            {
                if (isStandardDeviationReadOnly)
                {
                    throw new InvalidOperationException("StandardDeviation is set to be read-only.");
                }

                ChangePropertyAndNotify(() => data.StandardDeviation = value);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return propertyName == meanPropertyName
                       ? isMeanReadOnly
                       : propertyName == standardDeviationPropertyName
                         && isStandardDeviationReadOnly;
        }

        public override string ToString()
        {
            return $"{Mean} ({RingtoetsCommonFormsResources.NormalDistribution_StandardDeviation_DisplayName} = {StandardDeviation})";
        }

        private void ChangePropertyAndNotify(SetObservablePropertyValueDelegate setPropertyValue)
        {
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(setPropertyValue);
            NotifyAffectedObjects(affectedObjects);
        }

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }
    }
}