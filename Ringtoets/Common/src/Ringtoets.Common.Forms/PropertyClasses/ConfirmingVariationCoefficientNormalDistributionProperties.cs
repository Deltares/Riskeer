﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// An <see cref="ObjectProperties{T}"/> implementation for <see cref="VariationCoefficientNormalDistribution"/>.
    /// </summary>
    public class ConfirmingVariationCoefficientNormalDistributionProperties : ConfirmingVariationCoefficientDistributionPropertiesBase<VariationCoefficientNormalDistribution>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ConfirmingVariationCoefficientNormalDistributionProperties"/>
        /// in which the properties of <paramref name="distribution"/> are displayed read-only.
        /// </summary>
        /// <param name="distribution">The <see cref="VariationCoefficientNormalDistribution"/> to create the properties for.</param>
        public ConfirmingVariationCoefficientNormalDistributionProperties(VariationCoefficientNormalDistribution distribution) : base(distribution) { }

        /// <summary>
        /// Creates a new instance of <see cref="ConfirmingVariationCoefficientNormalDistributionProperties"/>.
        /// </summary>
        /// <param name="propertiesReadOnly">Indicates which properties, if any, should be
        /// marked as read-only.</param>
        /// <param name="distribution">The <see cref="VariationCoefficientNormalDistribution"/> to create the properties for.</param>
        /// <param name="handler">Optional handler that is used to handle property changes.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="distribution"/> is null 
        /// or when any number of properties in this class is editable and any other parameter is <c>null</c>.</exception>
        public ConfirmingVariationCoefficientNormalDistributionProperties(
            VariationCoefficientDistributionPropertiesReadOnly propertiesReadOnly,
            VariationCoefficientNormalDistribution distribution,
            IObservablePropertyChangeHandler handler)
            : base(propertiesReadOnly, distribution, handler) {}

        public override string DistributionType
        {
            get
            {
                return Resources.DistributionType_Normal;
            }
        }

        [ResourcesDescription(typeof(Resources), nameof(Resources.NormalDistribution_Mean_Description))]
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

        [ResourcesDescription(typeof(Resources), nameof(Resources.NormalDistribution_VariationCoefficient_Description))]
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