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
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties class for implementations of <see cref="ShiftedLogNormalDistributionProperties"/>.
    /// </summary>
    public class ShiftedLogNormalDistributionProperties : LogNormalDistributionProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="ShiftedLogNormalDistributionProperties"/>
        /// in which the properties of <paramref name="distribution"/> are displayed read-only.
        /// </summary>
        /// <param name="distribution">The <see cref="LogNormalDistributionProperties"/> to create the properties for.</param>
        public ShiftedLogNormalDistributionProperties(LogNormalDistribution distribution) : base(distribution)
        {}

        /// <summary>
        /// Creates a new instance of <see cref="ShiftedLogNormalDistributionProperties"/>.
        /// </summary>
        /// <param name="readOnlyProperties">Indicates which properties, if any, should be
        /// marked as read-only.</param>
        /// <param name="distribution">The <see cref="LogNormalDistribution"/> to create the properties for.</param>
        /// <param name="handler">Optional handler that is used to handle property changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Any number of properties in this class is editable and the 
        /// <paramref name="handler"/> is <c>null</c>.</exception>
        public ShiftedLogNormalDistributionProperties(DistributionReadOnlyProperties readOnlyProperties, LogNormalDistribution distribution, IObservablePropertyChangeHandler handler) : base(readOnlyProperties, distribution, handler)
        {}

        [PropertyOrder(4)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Probabilistics_Shift_Symbol))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Probabilistics_Shift_Description))]
        public RoundedDouble Shift
        {
            get
            {
                return ((LogNormalDistribution) Data).Shift;
            }
        }

        public override string ToString()
        {
            return $"{Mean} ({RiskeerCommonFormsResources.NormalDistribution_StandardDeviation_DisplayName} = {StandardDeviation}) ({RiskeerCommonFormsResources.Probabilistics_Shift_Symbol} = {Shift})";
        }
    }
}
