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
using System.ComponentModel;
using System.Linq;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.Properties;
using Riskeer.Revetment.Forms.PropertyClasses;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsOutputProperties : ObjectProperties<GrassCoverErosionOutwardsWaveConditionsOutput>
    {
        private readonly GrassCoverErosionOutwardsWaveConditionsInput input;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsOutputProperties"/>.
        /// </summary>
        /// <param name="output">The data to show.</param>
        /// <param name="input">The input belonging to the output.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveConditionsOutputProperties(GrassCoverErosionOutwardsWaveConditionsOutput output,
                                                                       GrassCoverErosionOutwardsWaveConditionsInput input)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            this.input = input;

            Data = output;
        }

        [DynamicVisible]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsOutputProperties_WaveRunUpOutput_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsOutputProperties_WaveRunUpOutput_Description))]
        public WaveConditionsOutputProperties[] WaveRunUpOutput
        {
            get
            {
                return GetWaveRunUpOutput();
            }
        }

        [DynamicVisible]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsOutputProperties_WaveImpactOutput_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsOutputProperties_WaveImpactOutput_Description))]
        public WaveConditionsOutputProperties[] WaveImpactOutput
        {
            get
            {
                return GetWaveImpactOutput();
            }
        }

        [DynamicVisible]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsOutputProperties_WaveImpactWithWaveDirectionOutput_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GrassCoverErosionOutwardsWaveConditionsOutputProperties_WaveImpactWithWaveDirectionOutput_Description))]
        public WaveConditionsOutputProperties[] WaveImpactWithWaveDirectionOutput
        {
            get
            {
                return data.WaveImpactWithWaveDirectionOutput.Select(output => new WaveConditionsOutputProperties
                {
                    Data = output
                }).ToArray();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(WaveRunUpOutput):
                    return input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp
                           || input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpactWithWaveDirection
                           || input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact
                           || input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All;
                case nameof(WaveImpactOutput):
                    return input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact
                           || input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact
                           || input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All;
                case nameof(WaveImpactWithWaveDirectionOutput):
                    return input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpactWithWaveDirection
                           || input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpactWithWaveDirection
                           || input.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All;
                default:
                    return false;
            }
        }

        private WaveConditionsOutputProperties[] GetWaveRunUpOutput()
        {
            return data.WaveRunUpOutput.Select(output => new WaveConditionsOutputProperties
            {
                Data = output
            }).ToArray();
        }

        private WaveConditionsOutputProperties[] GetWaveImpactOutput()
        {
            return data.WaveImpactOutput.Select(output => new WaveConditionsOutputProperties
            {
                Data = output
            }).ToArray();
        }
    }
}