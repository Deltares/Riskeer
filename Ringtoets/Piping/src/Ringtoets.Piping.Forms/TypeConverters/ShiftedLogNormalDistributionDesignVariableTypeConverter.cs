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

using System.ComponentModel;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="LogNormalDistribution"/>
    /// properties including the shift.
    /// </summary>
    public sealed class ShiftedLogNormalDistributionDesignVariableTypeConverter : DesignVariableTypeConverter<LogNormalDistribution>
    {
        private readonly ParameterDefinition<LogNormalDistribution>[] parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftedLogNormalDistributionDesignVariableTypeConverter"/> class.
        /// </summary>
        public ShiftedLogNormalDistributionDesignVariableTypeConverter()
        {
            var lowerCaseDistributionName = DistributionName.ToLower();
            parameters = new[]
            {
                new ParameterDefinition<LogNormalDistribution>(d => d.Mean)
                {
                    Symbol = Resources.Probabilistics_Mean_Symbol,
                    Description = string.Format(Resources.Probabilistics_Mean_description_for_Distribution_0_,
                                                lowerCaseDistributionName)
                },
                new ParameterDefinition<LogNormalDistribution>(d => d.StandardDeviation)
                {
                    Symbol = Resources.Probabilistics_StandardDeviation_Symbol,
                    Description = string.Format(Resources.Probabilistics_StandardDeviation_description_for_Distribution_0_,
                                                lowerCaseDistributionName)
                },
                new ParameterDefinition<LogNormalDistribution>(d => d.Shift)
                {
                    Symbol = Resources.Probabilistics_Shift_Symbol, 
                    Description = Resources.Probabilistics_Shift_Description
                },
            };
        }

        protected override string DistributionName
        {
            get
            {
                return Resources.LogNormalDistribution_DisplayName;
            }
        }

        protected override string DistributionShortName
        {
            get
            {
                return Resources.LogNormalDistribution_ShortName;
            }
        }

        protected override ParameterDefinition<LogNormalDistribution>[] Parameters
        {
            get
            {
                return parameters;
            }
        }
    }
}