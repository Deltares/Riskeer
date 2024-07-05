﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Gui.Attributes;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties class for implementations of <see cref="TruncatedNormalDistribution"/>.
    /// </summary>
    public class TruncatedNormalDistributionProperties : DistributionPropertiesBase<TruncatedNormalDistribution>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TruncatedNormalDistributionProperties"/>
        /// in which the properties of <paramref name="distribution"/> are displayed read-only.
        /// </summary>
        /// <param name="distribution">The <see cref="TruncatedNormalDistribution"/> to create the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        public TruncatedNormalDistributionProperties(TruncatedNormalDistribution distribution)
            : this(DistributionReadOnlyProperties.All, distribution, null) {}

        /// <summary>
        /// Creates a new instance of <see cref="TruncatedNormalDistributionProperties"/>.
        /// </summary>
        /// <param name="readOnlyProperties">Indicates which properties, if any, should be
        /// marked as read-only.</param>
        /// <param name="distribution">The <see cref="TruncatedNormalDistribution"/> to create the properties for.</param>
        /// <param name="handler">Optional handler that is used to handle property changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="distribution"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when any number of properties in this class is editable and the 
        /// <paramref name="handler"/> is <c>null</c>.</exception>
        public TruncatedNormalDistributionProperties(
            DistributionReadOnlyProperties readOnlyProperties,
            TruncatedNormalDistribution distribution,
            IObservablePropertyChangeHandler handler) :
            base(readOnlyProperties, distribution, handler) {}

        public override string DistributionType
        {
            get
            {
                return Resources.DistributionType_TruncatedNormal;
            }
        }

        [ResourcesDescription(typeof(Resources), nameof(Resources.TruncatedNormalDistribution_Mean_Description))]
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

        [ResourcesDescription(typeof(Resources), nameof(Resources.TruncatedNormalDistribution_StandardDeviation_Description))]
        public override RoundedDouble StandardDeviation
        {
            get
            {
                return base.StandardDeviation;
            }
            set
            {
                base.StandardDeviation = value;
            }
        }

        [PropertyOrder(4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TruncatedNormalDistribution_LowerBoundary_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.TruncatedNormalDistribution_LowerBoundary_Description))]
        public RoundedDouble LowerBoundary
        {
            get
            {
                return data.LowerBoundary;
            }
        }

        [PropertyOrder(5)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TruncatedNormalDistribution_UpperBoundary_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.TruncatedNormalDistribution_UpperBoundary_Description))]
        public RoundedDouble UpperBoundary
        {
            get
            {
                return data.UpperBoundary;
            }
        }
    }
}