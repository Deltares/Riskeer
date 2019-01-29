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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DesignVariable{T}"/> of <see cref="LogNormalDistribution"/> 
    /// including the shift for properties panel.
    /// </summary>
    public class ShiftedLogNormalDistributionDesignVariableProperties : LogNormalDistributionDesignVariableProperties
    {
        /// <summary>
        /// Creates a new read-only <see cref="ShiftedLogNormalDistributionDesignVariableProperties"/>.
        /// </summary>
        /// <param name="designVariable">The <see cref="DesignVariable{T}"/> to create the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designVariable"/> is <c>null</c>.</exception>
        public ShiftedLogNormalDistributionDesignVariableProperties(DesignVariable<LogNormalDistribution> designVariable)
            : base(designVariable) {}

        /// <summary>
        /// Creates a new <see cref="ShiftedLogNormalDistributionDesignVariableProperties"/>.
        /// </summary>
        /// <param name="propertiesReadOnly">Indicates which properties, if any, should be marked as read-only.</param>
        /// <param name="designVariable">The <see cref="DesignVariable{T}"/> to create the properties for.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designVariable"/> is <c>null</c>
        /// or when any number of properties in this class is editable and any other parameter is <c>null</c>.</exception>
        public ShiftedLogNormalDistributionDesignVariableProperties(DistributionPropertiesReadOnly propertiesReadOnly,
                                                                    DesignVariable<LogNormalDistribution> designVariable,
                                                                    IObservablePropertyChangeHandler handler)
            : base(propertiesReadOnly, designVariable, handler) {}

        [PropertyOrder(4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Probabilistics_Shift_Symbol))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Probabilistics_Shift_Description))]
        public RoundedDouble Shift
        {
            get
            {
                return DesignVariable.Distribution.Shift;
            }
        }

        public override string ToString()
        {
            return $"{DesignValue} ({RiskeerCommonFormsResources.NormalDistribution_Mean_DisplayName} = {Mean}, " +
                   $"{RiskeerCommonFormsResources.NormalDistribution_StandardDeviation_DisplayName} = {StandardDeviation}, " +
                   $"{Resources.Probabilistics_Shift_Symbol} = {Shift})";
        }
    }
}