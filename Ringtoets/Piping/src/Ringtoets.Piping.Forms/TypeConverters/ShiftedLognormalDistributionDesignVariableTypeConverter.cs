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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.ComponentModel;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="ShiftedLognormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationContextProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public sealed class ShiftedLognormalDistributionDesignVariableTypeConverter : DesignVariableTypeConverter<ShiftedLognormalDistribution>
    {
        private readonly ParameterDefinition<ShiftedLognormalDistribution>[] parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShiftedLognormalDistributionDesignVariableTypeConverter"/> class.
        /// </summary>
        public ShiftedLognormalDistributionDesignVariableTypeConverter()
        {
            var lowerCaseDistributionName = DistributionName.ToLower();
            parameters = new[]
            {
                new ParameterDefinition<ShiftedLognormalDistribution>(d => d.Mean)
                {
                    Symbol = Resources.Probabilistics_Mean_Symbol,
                    Description = String.Format(Resources.Probabilistics_Mean_description_for_Distribution_0_,
                                                lowerCaseDistributionName)
                },
                new ParameterDefinition<ShiftedLognormalDistribution>(d => d.StandardDeviation)
                {
                    Symbol = Resources.Probabilistics_StandardDeviation_Symbol,
                    Description = String.Format(Resources.Probabilistics_StandardDeviation_description_for_Distribution_0_,
                                                lowerCaseDistributionName)
                },
                new ParameterDefinition<ShiftedLognormalDistribution>(d => d.Shift)
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
                return Resources.ShiftedLognormalDistribution_DisplayName;
            }
        }

        protected override string DistributionShortName
        {
            get
            {
                return Resources.ShiftedLognormalDistribution_ShortName;
            }
        }

        protected override ParameterDefinition<ShiftedLognormalDistribution>[] Parameters
        {
            get
            {
                return parameters;
            }
        }
    }
}