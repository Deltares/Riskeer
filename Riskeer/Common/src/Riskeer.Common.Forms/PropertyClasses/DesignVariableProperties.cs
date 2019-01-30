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
using Core.Common.Gui.Attributes;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel base of <see cref="DesignVariable{T}"/> for properties panel.
    /// </summary>
    /// <typeparam name="TDistribution">The type of the distribution.</typeparam>
    public abstract class DesignVariableProperties<TDistribution>
        : DistributionPropertiesBase<TDistribution>
        where TDistribution : IDistribution
    {
        /// <summary>
        /// Creates a new <see cref="DesignVariableProperties{TDistribution}"/>.
        /// </summary>
        /// <param name="propertiesReadOnly">Indicates which properties, if any, should be marked as read-only.</param>
        /// <param name="designVariable">The data of the <see cref="TDistribution"/> to create the properties for.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designVariable"/> is <c>null</c>
        /// or when any number of properties in this class is editable and any other parameter is <c>null</c>.</exception>
        protected DesignVariableProperties(DistributionPropertiesReadOnly propertiesReadOnly,
                                           DesignVariable<TDistribution> designVariable,
                                           IObservablePropertyChangeHandler handler)
            : base(propertiesReadOnly, GetDistribution(designVariable), handler)
        {
            DesignVariable = designVariable;
        }

        public abstract override string DistributionType { get; }

        [PropertyOrder(5)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DesignVariableProperties_DesignValue_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DesignVariableProperties_DesignValue_Description))]
        public RoundedDouble DesignValue
        {
            get
            {
                return DesignVariable.GetDesignValue();
            }
        }

        public override string ToString()
        {
            return string.Format(Resources.DesignVariable_0_Mean_1_StandardDeviation_2,
                                 DesignValue,
                                 Mean,
                                 StandardDeviation);
        }

        /// <summary>
        /// Gets the design variable.
        /// </summary>
        protected DesignVariable<TDistribution> DesignVariable { get; }

        /// <summary>
        /// Gets the <see cref="TDistribution"/> of the <see cref="DesignVariable{T}"/>.
        /// </summary>
        /// <param name="designVariable">The design variable to get the distribution from.</param>
        /// <returns>The distribution of the design variable.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designVariable"/>
        /// is <c>null</c>.</exception>
        private static TDistribution GetDistribution(DesignVariable<TDistribution> designVariable)
        {
            if (designVariable == null)
            {
                throw new ArgumentNullException(nameof(designVariable));
            }

            return designVariable.Distribution;
        }
    }
}