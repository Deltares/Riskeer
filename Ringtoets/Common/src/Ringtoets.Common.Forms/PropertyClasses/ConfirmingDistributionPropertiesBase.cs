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
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties class for implementations of <see cref="IDistribution"/>.
    /// </summary>
    public abstract class ConfirmingDistributionPropertiesBase<TDistribution, TCalculationInput, TCalculation> : ObjectProperties<TDistribution>
        where TDistribution : IDistribution
        where TCalculationInput : ICalculationInput
        where TCalculation : ICalculation
    {
        private readonly string meanPropertyName;
        private readonly string standardDeviationPropertyName;
        private readonly bool isMeanReadOnly;
        private readonly bool isStandardDeviationReadOnly;
        private readonly TCalculationInput calculationInput;
        private readonly TCalculation calculation;
        private readonly ICalculationInputPropertyChangeHandler changeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="ConfirmingDistributionPropertiesBase{TDistribution,TCalculationInput,TCalculation}"/>.
        /// </summary>
        /// <param name="propertiesReadOnly">Indicates which properties, if any, should be marked as read-only.</param>
        /// <param name="data">The data of the <see cref="TDistribution"/> to create the properties for.</param>
        /// <param name="calculation">The calculation the <paramref name="data"/> belongs to.</param>
        /// <param name="calculationInput">The calculation input the <paramref name="data"/> belongs to.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>
        /// or when any number of properties in this class is editable and any other parameter is <c>null</c>.</exception>
        protected ConfirmingDistributionPropertiesBase(DistributionPropertiesReadOnly propertiesReadOnly,
                                                   TDistribution data,
                                                   TCalculation calculation,
                                                   TCalculationInput calculationInput,
                                                   ICalculationInputPropertyChangeHandler handler)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (!propertiesReadOnly.HasFlag(DistributionPropertiesReadOnly.All))
            {
                if (calculation == null)
                {
                    throw new ArgumentException(@"Calculation required if changes are possible.", nameof(calculation));
                }
                if (calculationInput == null)
                {
                    throw new ArgumentException(@"CalculationInput required if changes are possible.", nameof(calculationInput));
                }
                if (handler == null)
                {
                    throw new ArgumentException(@"Change handler required if changes are possible.", nameof(handler));
                }
            }
            Data = data;

            meanPropertyName = TypeUtils.GetMemberName<ConfirmingDistributionPropertiesBase<TDistribution, TCalculationInput, TCalculation>>(dpb => dpb.Mean);
            standardDeviationPropertyName = TypeUtils.GetMemberName<ConfirmingDistributionPropertiesBase<TDistribution, TCalculationInput, TCalculation>>(dpb => dpb.StandardDeviation);

            isMeanReadOnly = propertiesReadOnly.HasFlag(DistributionPropertiesReadOnly.Mean);
            isStandardDeviationReadOnly = propertiesReadOnly.HasFlag(DistributionPropertiesReadOnly.StandardDeviation);

            this.calculationInput = calculationInput;
            this.calculation = calculation;
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
                    throw new ArgumentException("Mean is set to be read-only.");
                }

                IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(calculationInput, calculation, value, (input, d) => data.Mean = d);
                NotifyAffectedObjects(affectedObjects);
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
                    throw new ArgumentException("StandardDeviation is set to be read-only.");
                }

                IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(calculationInput, calculation, value, (input, d) => data.StandardDeviation = d);
                NotifyAffectedObjects(affectedObjects);
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

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (var affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }
    }
}