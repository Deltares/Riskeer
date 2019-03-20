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
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.Revetment.Forms.PropertyClasses;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityStoneCoverWaveConditionsOutput"/> for properties panel.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsOutputProperties : ObjectProperties<StabilityStoneCoverWaveConditionsOutput>
    {
        private readonly StabilityStoneCoverWaveConditionsInput input;

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsOutputProperties"/>.
        /// </summary>
        /// <param name="output">The data to show.</param>
        /// <param name="input">The input to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsOutputProperties(
            StabilityStoneCoverWaveConditionsOutput output, StabilityStoneCoverWaveConditionsInput input)
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
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsOutputProperties_Blocks_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsOutputProperties_Blocks_Description))]
        public WaveConditionsOutputProperties[] Blocks
        {
            get
            {
                return data.BlocksOutput.Select(waveConditionsOutput => new WaveConditionsOutputProperties
                {
                    Data = waveConditionsOutput
                }).ToArray();
            }
        }

        [DynamicVisible]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsOutputProperties_Columns_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.StabilityStoneCoverWaveConditionsOutputProperties_Columns_Description))]
        public WaveConditionsOutputProperties[] Columns
        {
            get
            {
                return data.ColumnsOutput.Select(waveConditionsOutput => new WaveConditionsOutputProperties
                {
                    Data = waveConditionsOutput
                }).ToArray();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName == nameof(Blocks))
            {
                return input.CalculationType != StabilityStoneCoverWaveConditionsCalculationType.Columns;
            }

            if (propertyName == nameof(Columns))
            {
                return input.CalculationType != StabilityStoneCoverWaveConditionsCalculationType.Blocks;
            }

            return true;
        }
    }
}