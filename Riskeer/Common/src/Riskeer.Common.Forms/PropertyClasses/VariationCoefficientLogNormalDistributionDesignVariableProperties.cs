// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DesignVariable{T}"/> of <see cref="LogNormalDistribution"/> for properties panel.
    /// </summary>
    public class VariationCoefficientLogNormalDistributionDesignVariableProperties
        : VariationCoefficientDesignVariableProperties<VariationCoefficientLogNormalDistribution>
    {
        /// <summary>
        /// Creates a new read-only <see cref="VariationCoefficientLogNormalDistributionDesignVariableProperties"/>.
        /// </summary>
        /// <param name="designVariable">The <see cref="DesignVariable{T}"/> to create the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designVariable"/> is <c>null</c>.</exception>
        public VariationCoefficientLogNormalDistributionDesignVariableProperties(VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> designVariable)
            : this(VariationCoefficientDistributionPropertiesReadOnly.All, designVariable, null) {}

        /// <summary>
        /// Creates a new <see cref="VariationCoefficientLogNormalDistributionDesignVariableProperties"/>.
        /// </summary>
        /// <param name="propertiesReadOnly">Indicates which properties, if any, should be marked as read-only.</param>
        /// <param name="designVariable">The <see cref="DesignVariable{T}"/> to create the properties for.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designVariable"/> is <c>null</c>
        /// or when any number of properties in this class is editable and any other parameter is <c>null</c>.</exception>
        public VariationCoefficientLogNormalDistributionDesignVariableProperties(VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly,
                                                                                 VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> designVariable,
                                                                                 IObservablePropertyChangeHandler handler)
            : base(propertiesReadOnly,
                   designVariable,
                   handler) {}

        public override string DistributionType
        {
            get
            {
                return Resources.DistributionType_LogNormal;
            }
        }

        [ResourcesDescription(typeof(Resources), nameof(Resources.LogNormalDistribution_Mean_Description))]
        public override RoundedDouble Mean
        {
            get
            {
                return base.Mean;
            }
            set
            {
                base.Mean = value;
            }
        }

        [ResourcesDescription(typeof(Resources), nameof(Resources.LogNormalDistribution_VariationCoefficient_Description))]
        public override RoundedDouble CoefficientOfVariation
        {
            get
            {
                return base.CoefficientOfVariation;
            }
            set
            {
                base.CoefficientOfVariation = value;
            }
        }
    }
}