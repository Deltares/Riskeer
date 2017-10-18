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
using System.Drawing;
using Core.Common.Utils;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class defines a formatted version of an <see cref="IMacroStabilityInwardsSoilLayerData"/> object.
    /// </summary>
    public class MacroStabilityInwardsFormattedSoilLayerDataRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFormattedSoilLayerDataRow"/>.
        /// </summary>
        /// <param name="layerData">The <see cref="IMacroStabilityInwardsSoilLayerData"/> to format.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layerData"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsFormattedSoilLayerDataRow(IMacroStabilityInwardsSoilLayerData layerData)
        {
            if (layerData == null)
            {
                throw new ArgumentNullException(nameof(layerData));
            }

            MaterialName = layerData.MaterialName;
            Color = layerData.Color;
            IsAquifer = layerData.IsAquifer;
            AbovePhreaticLevel = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(layerData));
            BelowPhreaticLevel = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(layerData));
            ShearStrengthModel = layerData.ShearStrengthModel;
            Cohesion = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(layerData));
            FrictionAngle = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(layerData));
            ShearStrengthRatio = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(layerData));
            StrengthIncreaseExponent = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(layerData));
            UsePop = layerData.UsePop;
            Pop = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(layerData));
        }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="IMacroStabilityInwardsSoilLayerData"/> is an aquifer.
        /// </summary>
        public bool IsAquifer { get; }

        /// <summary>
        /// Gets the name of the material that was assigned to the <see cref="IMacroStabilityInwardsSoilLayerData"/>.
        /// </summary>
        public string MaterialName { get; }

        /// <summary>
        /// Gets the <see cref="Color"/> that was used to represent the <see cref="IMacroStabilityInwardsSoilLayerData"/>.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.AbovePhreaticLevel"/>.
        /// </summary>
        public string AbovePhreaticLevel { get; }

        /// <summary>
        /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.BelowPhreaticLevel"/>.
        /// </summary>
        public string BelowPhreaticLevel { get; }

        /// <summary>
        /// Gets the <see cref="IMacroStabilityInwardsSoilLayerData.ShearStrengthModel"/> type.
        /// </summary>
        [TypeConverter(typeof(EnumTypeConverter))]
        public MacroStabilityInwardsShearStrengthModel ShearStrengthModel { get; }

        /// <summary>
        /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.Cohesion"/>.
        /// </summary>
        public string Cohesion { get; }

        /// <summary>
        /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.FrictionAngle"/>.
        /// </summary>
        public string FrictionAngle { get; }

        /// <summary>
        /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.ShearStrengthRatio"/>.
        /// </summary>
        public string ShearStrengthRatio { get; }

        /// <summary>
        /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.StrengthIncreaseExponent"/>.
        /// </summary>
        public string StrengthIncreaseExponent { get; }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="IMacroStabilityInwardsSoilLayerData"/> is using POP.
        /// </summary>
        public bool UsePop { get; }

        /// <summary>
        /// Gets the formatted design variable for <see cref="IMacroStabilityInwardsSoilLayerData.Pop"/>.
        /// </summary>
        public string Pop { get; }

        private static string FormatVariationCoefficientDesignVariable(VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> designVariable)
        {
            return string.Format(RingtoetsCommonFormsResources.VariationCoefficientDesignVariable_0_Mean_is_1_CoefficientOfVariation_is_2,
                                 designVariable.GetDesignValue(),
                                 designVariable.Distribution.Mean,
                                 designVariable.Distribution.CoefficientOfVariation);
        }
    }
}