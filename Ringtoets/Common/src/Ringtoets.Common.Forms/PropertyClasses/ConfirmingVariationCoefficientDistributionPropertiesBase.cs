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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties class for implementations of <see cref="IDistribution"/>.
    /// </summary>
    public abstract class ConfirmingVariationCoefficientDistributionPropertiesBase<TDistribution, TCalculationInput> : ObjectProperties<TDistribution>
        where TDistribution : IVariationCoefficientDistribution
        where TCalculationInput : ICalculationInput
    {
        private const string meanPropertyName = nameof(Mean);
        private readonly string variationCoefficientPropertyName = nameof(CoefficientOfVariation);
        private readonly bool isMeanReadOnly;
        private readonly bool isVariationCoefficientReadOnly;
        private readonly TCalculationInput calculationInput;
        private readonly ICalculation calculation;
        private readonly ICalculationInputPropertyChangeHandler changeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="ConfirmingVariationCoefficientDistributionPropertiesBase{TDistribution,TCalculationInput}"/>.
        /// </summary>
        /// <param name="propertiesReadOnly">Indicates which properties, if any, should be marked as read-only.</param>
        /// <param name="data">The data of the <see cref="TDistribution"/> to create the properties for.</param>
        /// <param name="calculation">The calculation the <paramref name="data"/> belongs to.</param>
        /// <param name="calculationInput">The calculation input the <paramref name="data"/> belongs to.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>
        /// or when any number of properties in this class is editable and any other parameter is <c>null</c>.</exception>
        protected ConfirmingVariationCoefficientDistributionPropertiesBase(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly,
                                                       TDistribution data,
                                                       ICalculation calculation,
                                                       TCalculationInput calculationInput,
                                                       ICalculationInputPropertyChangeHandler handler)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (!propertiesReadOnly.HasFlag(VariationCoefficientDistributionPropertiesReadOnly.All))
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

            isMeanReadOnly = propertiesReadOnly.HasFlag(VariationCoefficientDistributionPropertiesReadOnly.Mean);
            isVariationCoefficientReadOnly = propertiesReadOnly.HasFlag(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation);

            this.calculationInput = calculationInput;
            this.calculation = calculation;
            changeHandler = handler;
        }

        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Distribution_DistributionType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Distribution_DistributionType_Description))]
        public abstract string DistributionType { get; }

        [PropertyOrder(2)]
        [DynamicReadOnly]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.NormalDistribution_Mean_DisplayName))]
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

                ChangePropertyAndNotify((input, newValue) => data.Mean = newValue, value);
            }
        }

        [PropertyOrder(3)]
        [DynamicReadOnly]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Distribution_VariationCoefficient_DisplayName))]
        public virtual RoundedDouble CoefficientOfVariation
        {
            get
            {
                return data.CoefficientOfVariation;
            }
            set
            {
                if (isVariationCoefficientReadOnly)
                {
                    throw new InvalidOperationException($"{nameof(CoefficientOfVariation)} is set to be read-only.");
                }

                ChangePropertyAndNotify((input, newValue) => data.CoefficientOfVariation = newValue, value);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == meanPropertyName)
            {
                return isMeanReadOnly;
            }
            if (propertyName == variationCoefficientPropertyName)
            {
                return isVariationCoefficientReadOnly;
            }
            return false;
        }

        public override string ToString()
        {
            return data == null ? string.Empty :
                       $"{Mean} ({Resources.Distribution_VariationCoefficient_DisplayName} = {CoefficientOfVariation})";
        }

        private void ChangePropertyAndNotify(SetCalculationInputPropertyValueDelegate<TCalculationInput, RoundedDouble> setPropertyValue,
                                             RoundedDouble value)
        {
            IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(calculationInput,
                                                                                                       calculation,
                                                                                                       value,
                                                                                                       setPropertyValue);
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