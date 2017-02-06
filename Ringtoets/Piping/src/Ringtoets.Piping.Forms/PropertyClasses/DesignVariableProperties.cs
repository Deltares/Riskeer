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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    public abstract class DesignVariableProperties<TDistribution> : PipingDistributionPropertiesBase<TDistribution, PipingInput, PipingCalculationScenario>
        where TDistribution : IDistribution
    {
        protected DesignVariableProperties(DistributionPropertiesReadOnly propertiesReadOnly,
                                           DesignVariable<TDistribution> designVariable,
                                           PipingCalculationScenario calculation,
                                           PipingInput calculationInput,
                                           ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario> handler)
            : base(propertiesReadOnly, GetDistribution(designVariable), calculation, calculationInput, handler)
        {
            DesignVariable = designVariable;
        }

        public abstract override string DistributionType { get; }

        [PropertyOrder(5)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DesignVariableTypeConverter_DesignValue_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DesignVariableTypeConverter_DesignValue_Description))]
        public RoundedDouble DesignValue
        {
            get
            {
                return DesignVariable.GetDesignValue();
            }
        }

        public override string ToString()
        {
            return $"{DesignValue} ({RingtoetsCommonFormsResources.NormalDistribution_Mean_DisplayName} = {Mean}, " +
                   $"{RingtoetsCommonFormsResources.NormalDistribution_StandardDeviation_DisplayName} = {StandardDeviation})";
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