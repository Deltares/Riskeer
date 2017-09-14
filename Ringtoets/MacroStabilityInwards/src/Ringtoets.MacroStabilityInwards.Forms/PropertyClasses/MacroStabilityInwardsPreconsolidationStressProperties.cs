// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsPreconsolidationStress"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MacroStabilityInwardsPreconsolidationStressProperties : ObjectProperties<MacroStabilityInwardsPreconsolidationStress>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsPreconsolidationStressProperties"/>.
        /// </summary>
        /// <param name="preconsolidationStress">The preconsolidation stress to create the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="preconsolidationStress"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsPreconsolidationStressProperties(
            MacroStabilityInwardsPreconsolidationStress preconsolidationStress)
        {
            if (preconsolidationStress == null)
            {
                throw new ArgumentNullException(nameof(preconsolidationStress));
            }

            Data = preconsolidationStress;
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsPreconsolidationStressProperties_XCoordinate_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsPreconsolidationStressProperties_XCoordinate_Description))]
        public RoundedDouble XCoordinate
        {
            get
            {
                return new RoundedDouble(2, data.XCoordinate);
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsPreconsolidationStressProperties_ZCoordinate_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsPreconsolidationStressProperties_ZCoordinate_Description))]
        public RoundedDouble ZCoordinate
        {
            get
            {
                return new RoundedDouble(2, data.ZCoordinate);
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsPreconsolidationStressProperties_PreconsolidationStress_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsPreconsolidationStressProperties_PreconsolidationStress_Description))]
        public VariationCoefficientLogNormalDistributionProperties PreconsolidationStress
        {
            get
            {
                var variationCoefficientLogNormalDistribution = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = (RoundedDouble) data.PreconsolidationStressMean,
                    CoefficientOfVariation = (RoundedDouble) data.PreconsolidationStressCoefficientOfVariation
                };
                return new VariationCoefficientLogNormalDistributionProperties(variationCoefficientLogNormalDistribution);
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}