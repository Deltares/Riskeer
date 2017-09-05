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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Soil layer properties.
    /// </summary>
    public class MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="properties">The object containing the values for the properties 
        /// of the new <see cref="MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) properties.AbovePhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) properties.AbovePhreaticLevelCoefficientOfVariation
            };

            BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) properties.BelowPhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) properties.BelowPhreaticLevelCoefficientOfVariation
            };

            Cohesion = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) properties.CohesionMean,
                CoefficientOfVariation = (RoundedDouble) properties.CohesionCoefficientOfVariation
            };

            FrictionAngle = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) properties.FrictionAngleMean,
                CoefficientOfVariation = (RoundedDouble) properties.FrictionAngleCoefficientOfVariation
            };

            ShearStrengthRatio = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) properties.ShearStrengthRatioMean,
                CoefficientOfVariation = (RoundedDouble) properties.ShearStrengthRatioCoefficientOfVariation
            };

            Pop = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) properties.PopMean,
                CoefficientOfVariation = (RoundedDouble) properties.PopCoefficientOfVariation
            };

            IsAquifer = properties.IsAquifer;
            UsePop = properties.UsePop;
            ShearStrengthModel = properties.ShearStrengthModel;
            MaterialName = properties.MaterialName;
        }

        /// <summary>
        /// Gets a value indicating whether the layer is an aquifer.
        /// </summary>
        public bool IsAquifer { get; }

        /// <summary>
        /// Gets a value indicating whether to use POP for the layer.
        /// </summary>
        public bool UsePop { get; }

        /// <summary>
        /// Gets the shear strength model to use for the layer.
        /// </summary>
        public MacroStabilityInwardsShearStrengthModel ShearStrengthModel { get; }

        /// <summary>
        /// Gets the name of the material that was assigned to the layer.
        /// </summary>
        public string MaterialName { get; }

        #region distributions

        /// <summary>
        /// Gets the distribution for the volumic weight of the layer above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public VariationCoefficientLogNormalDistribution AbovePhreaticLevel { get; }

        /// <summary>
        /// Gets the distribution for the volumic weight of the layer below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public VariationCoefficientLogNormalDistribution BelowPhreaticLevel { get; }

        /// <summary>
        /// Gets the distribution for the cohesion.
        /// [kN/m³]
        /// </summary>
        public VariationCoefficientLogNormalDistribution Cohesion { get; }

        /// <summary>
        /// Gets the friction angle.
        /// [°]
        /// </summary>
        public VariationCoefficientLogNormalDistribution FrictionAngle { get; }

        /// <summary>
        /// Gets the shear strength ratio.
        /// [-]
        /// </summary>
        public VariationCoefficientLogNormalDistribution ShearStrengthRatio { get; }

        /// <summary>
        /// Gets the Pop.
        /// [kN/m²]
        /// </summary>
        public VariationCoefficientLogNormalDistribution Pop { get; }

        #endregion

        #region Design variables

        /// <summary>
        /// Gets or sets the design variable of the distribution for the volumic weight of the layer above the phreatic level.
        /// </summary>
        public RoundedDouble AbovePhreaticLevelDesignVariable { get; set; }

        /// <summary>
        /// Gets or sets the design variable of the distribution for the volumic weight of the layer below the phreatic level.
        /// </summary>
        public RoundedDouble BelowPhreaticLevelDesignVariable { get; set; }

        /// <summary>
        /// Gets or sets the design variable of the distribution for the cohesion.
        /// </summary>
        public RoundedDouble CohesionDesignVariable { get; set; }

        /// <summary>
        /// Gets or sets the design variable of the distribution for the friction angle.
        /// </summary>
        public RoundedDouble FrictionAngleDesignVariable { get; set; }

        /// <summary>
        /// Gets or sets the design variable of the distribution for the shear strength ratio.
        /// </summary>
        public RoundedDouble ShearStrengthRatioDesignVariable { get; set; }

        /// <summary>
        /// Gets or sets the design variable of the distribution for the Pop.
        /// </summary>
        public RoundedDouble PopDesignVariable { get; set; }

        #endregion

        #region ConstructionProperties

        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                AbovePhreaticLevelMean = double.NaN;
                AbovePhreaticLevelCoefficientOfVariation = double.NaN;
                BelowPhreaticLevelMean = double.NaN;
                BelowPhreaticLevelCoefficientOfVariation = double.NaN;
                CohesionMean = double.NaN;
                CohesionCoefficientOfVariation = double.NaN;
                FrictionAngleMean = double.NaN;
                FrictionAngleCoefficientOfVariation = double.NaN;
                ShearStrengthRatioMean = double.NaN;
                ShearStrengthRatioCoefficientOfVariation = double.NaN;
                PopMean = double.NaN;
                PopCoefficientOfVariation = double.NaN;
                ShearStrengthModel = MacroStabilityInwardsShearStrengthModel.None;
                MaterialName = string.Empty;
            }

            /// <summary>
            /// Gets or sets the mean of the distribution for the volumic weight of the layer above the phreatic level.
            /// [kN/m³]
            /// </summary>
            public double AbovePhreaticLevelMean { internal get; set; }

            /// <summary>
            /// Gets or sets the coefficient of variation of the distribution for the volumic weight of the layer above the phreatic level.
            /// [kN/m³]
            /// </summary>
            public double AbovePhreaticLevelCoefficientOfVariation { internal get; set; }

            /// <summary>
            /// Gets or sets the mean of the distribution for the volumic weight of the layer below the phreatic level.
            /// [kN/m³]
            /// </summary>
            public double BelowPhreaticLevelMean { internal get; set; }

            /// <summary>
            /// Gets or sets the coefficient of variation of the distribution for the volumic weight of the layer below the phreatic level.
            /// [kN/m³]
            /// </summary>
            public double BelowPhreaticLevelCoefficientOfVariation { internal get; set; }

            /// <summary>
            /// Gets or sets the mean of the distribution for the cohesion.
            /// [kN/m³]
            /// </summary>
            public double CohesionMean { internal get; set; }

            /// <summary>
            /// Gets or sets the coefficient of variation of the distribution for the cohesion.
            /// [kN/m³]
            /// </summary>
            public double CohesionCoefficientOfVariation { internal get; set; }

            /// <summary>
            /// Gets or sets the mean of the distribution for the friction angle.
            /// [°]
            /// </summary>
            public double FrictionAngleMean { internal get; set; }

            /// <summary>
            /// Gets or sets the coefficient of variation of the distribution for the friction angle.
            /// [°]
            /// </summary>
            public double FrictionAngleCoefficientOfVariation { internal get; set; }

            /// <summary>
            /// Gets or sets the mean of the distribution for the shear strength ratio.
            /// [-]
            /// </summary>
            public double ShearStrengthRatioMean { internal get; set; }

            /// <summary>
            /// Gets or sets the coefficient of variation of the distribution for the shear strength ratio.
            /// [-]
            /// </summary>
            public double ShearStrengthRatioCoefficientOfVariation { internal get; set; }

            /// <summary>
            /// Gets or sets the mean of the distribution for the pop.
            /// [kN/m²]
            /// </summary>
            public double PopMean { internal get; set; }

            /// <summary>
            /// Gets or sets the coefficient of variation of the distribution for the pop.
            /// [kN/m²]
            /// </summary>
            public double PopCoefficientOfVariation { internal get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the layer is an aquifer.
            /// </summary>
            public bool IsAquifer { internal get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to use POP for the layer.
            /// </summary>
            public bool UsePop { internal get; set; }

            /// <summary>
            /// Gets or sets the shear strength model to use for the layer.
            /// </summary>
            public MacroStabilityInwardsShearStrengthModel ShearStrengthModel { internal get; set; }

            /// <summary>
            /// Gets or sets the name of the material that was assigned to the layer.
            /// </summary>
            public string MaterialName { internal get; set; }
        }

        #endregion
    }
}