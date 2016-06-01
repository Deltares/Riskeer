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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// An <see cref="ObjectProperties{T}"/> implementation for <see cref="NormalDistribution"/>
    /// properties that display variation coefficient.
    /// </summary>
    public class NormalDistributionVariationProperties : NormalDistributionProperties
    {
        /// <summary>
        /// Creates a new read-only instance of <see cref="NormalDistributionVariationProperties"/>.
        /// </summary>
        public NormalDistributionVariationProperties() : this(null, DistributionPropertiesReadOnly.All) {}

        /// <summary>
        /// Creates a new instance of <see cref="NormalDistributionVariationProperties"/>.
        /// </summary>
        /// <param name="observerable">Object to observe to notify upon change.</param>
        /// <param name="propertiesReadOnly">Sets if <see cref="DistributionProperties.Mean"/> and/or 
        /// <see cref="DistributionProperties.StandardDeviation"/> should be marked read-only.</param>
        public NormalDistributionVariationProperties(IObservable observerable, DistributionPropertiesReadOnly propertiesReadOnly)
            : base(observerable, propertiesReadOnly) {}

        public override RoundedDouble Mean
        {
            get
            {
                return base.Mean;
            }
            set
            {
                var variationCoefficient = data.GetVariationCoefficient();
                base.Mean = value;
                if (!double.IsInfinity(variationCoefficient))
                {
                    data.SetStandardDeviationFromVariationCoefficient(variationCoefficient);
                    Observerable.NotifyObservers();
                }
            }
        }

        [Browsable(false)]
        public override RoundedDouble StandardDeviation { get; set; }

        [PropertyOrder(3)]
        [DynamicReadOnly]
        [ResourcesDisplayName(typeof(Resources), "Distribution_VariationCoefficient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "NormalDistribution_VariationCoefficient_Description")]
        public RoundedDouble VariationCoefficient
        {
            get
            {
                return data.GetVariationCoefficient();
            }
            set
            {
                if (IsVariationCoefficientReadOnly)
                {
                    throw new ArgumentException("Variation coefficient is set to be read-only.");
                }
                if (Observerable == null)
                {
                    throw new ArgumentException("No observerable object set.");
                }
                data.SetStandardDeviationFromVariationCoefficient(value);
                Observerable.NotifyObservers();
            }
        }

        public override string ToString()
        {
            return data == null ? Resources.Distribution_VariationCoefficient_DisplayName :
                       string.Format("{0} ({1} = {2})", Mean, Resources.Distribution_VariationCoefficient_DisplayName, VariationCoefficient);
        }
    }
}