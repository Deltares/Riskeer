// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Util;
using Riskeer.Common.Data.Helpers;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="MacroStabilityInwardsSoilLayerData"/>.
    /// </summary>
    public class MacroStabilityInwardsFormattedSoilLayerDataRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFormattedSoilLayerDataRow"/>.
        /// </summary>
        /// <param name="layerData">The <see cref="MacroStabilityInwardsSoilLayerData"/> to format.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layerData"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsFormattedSoilLayerDataRow(MacroStabilityInwardsSoilLayerData layerData)
        {
            if (layerData == null)
            {
                throw new ArgumentNullException(nameof(layerData));
            }

            MaterialName = SoilLayerDataHelper.GetValidName(layerData.MaterialName);
            Color = SoilLayerDataHelper.GetValidColor(layerData.Color);
            IsAquifer = layerData.IsAquifer;
            AbovePhreaticLevel = FormatVariationCoefficientDesignVariableWithShift(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(layerData));
            BelowPhreaticLevel = FormatVariationCoefficientDesignVariableWithShift(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(layerData));
            ShearStrengthModel = layerData.ShearStrengthModel;
            Cohesion = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(layerData));
            FrictionAngle = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(layerData));
            ShearStrengthRatio = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(layerData));
            StrengthIncreaseExponent = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(layerData));
            UsePop = layerData.UsePop;
            Pop = FormatVariationCoefficientDesignVariable(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(layerData));
        }

        /// <summary>
        /// Gets a value indicating whether the layer is an aquifer.
        /// </summary>
        public bool IsAquifer { get; }

        /// <summary>
        /// Gets the material name of the layer.
        /// </summary>
        public string MaterialName { get; }

        /// <summary>
        /// Gets the color of the layer.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Gets the above phreatic level of the layer.
        /// [kN/m³]
        /// </summary>
        public string AbovePhreaticLevel { get; }

        /// <summary>
        /// Gets the below phreatic level of the layer.
        /// [kN/m³]
        /// </summary>
        public string BelowPhreaticLevel { get; }

        /// <summary>
        /// Gets the shear strength model of the layer.
        /// </summary>
        [TypeConverter(typeof(EnumTypeConverter))]
        public MacroStabilityInwardsShearStrengthModel ShearStrengthModel { get; }

        /// <summary>
        /// Gets the cohesion of the layer.
        /// [kN/m²]
        /// </summary>
        public string Cohesion { get; }

        /// <summary>
        /// Gets the friction angle of the layer.
        /// [°]
        /// </summary>
        public string FrictionAngle { get; }

        /// <summary>
        /// Gets the shear strength ratio of the layer.
        /// [-]
        /// </summary>
        public string ShearStrengthRatio { get; }

        /// <summary>
        /// Gets the strength increase exponent of the layer.
        /// [-]
        /// </summary>
        public string StrengthIncreaseExponent { get; }

        /// <summary>
        /// Gets a value indicating whether the layer is using POP.
        /// </summary>
        public bool UsePop { get; }

        /// <summary>
        /// Gets the POP of the layer.
        /// [kN/m²]
        /// </summary>
        public string Pop { get; }

        private static string FormatVariationCoefficientDesignVariable(VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> designVariable)
        {
            RoundedDouble designValue = designVariable.GetDesignValue();
            return double.IsNaN(designValue)
                       ? double.NaN.ToString(CultureInfo.CurrentCulture)
                       : string.Format(RingtoetsCommonFormsResources.VariationCoefficientDesignVariable_0_Mean_1_CoefficientOfVariation_2,
                                       designValue,
                                       designVariable.Distribution.Mean,
                                       designVariable.Distribution.CoefficientOfVariation);
        }

        private static string FormatVariationCoefficientDesignVariableWithShift(VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> designVariable)
        {
            RoundedDouble designValue = designVariable.GetDesignValue();
            return double.IsNaN(designValue)
                       ? double.NaN.ToString(CultureInfo.CurrentCulture)
                       : string.Format(RingtoetsCommonFormsResources.VariationCoefficientDesignVariable_0_Mean_1_CoefficientOfVariation_2_Shift_3,
                                       designValue,
                                       designVariable.Distribution.Mean,
                                       designVariable.Distribution.CoefficientOfVariation,
                                       designVariable.Distribution.Shift);
        }
    }
}