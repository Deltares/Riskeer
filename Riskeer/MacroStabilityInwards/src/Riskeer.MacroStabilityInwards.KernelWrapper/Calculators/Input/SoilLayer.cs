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
using System.Collections.Generic;
using Core.Common.Base.Geometry;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input
{
    /// <summary>
    /// A 2D soil layer that has been adapted to perform a calculation.
    /// </summary>
    public class SoilLayer
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer"/>.
        /// </summary>
        /// <param name="outerRing">The outer ring of the soil layer.</param>
        /// <param name="nestedLayers">The nested layers of the soil layer.</param>
        /// <param name="properties">The object containing the values for
        /// the soil data properties of the new <see cref="SoilLayer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public SoilLayer(IEnumerable<Point2D> outerRing, ConstructionProperties properties, IEnumerable<SoilLayer> nestedLayers)
        {
            if (outerRing == null)
            {
                throw new ArgumentNullException(nameof(outerRing));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (nestedLayers == null)
            {
                throw new ArgumentNullException(nameof(nestedLayers));
            }

            OuterRing = outerRing;

            IsAquifer = properties.IsAquifer;
            UsePop = properties.UsePop;
            ShearStrengthModel = properties.ShearStrengthModel;
            MaterialName = properties.MaterialName;
            AbovePhreaticLevel = properties.AbovePhreaticLevel;
            BelowPhreaticLevel = properties.BelowPhreaticLevel;
            Cohesion = properties.Cohesion;
            FrictionAngle = properties.FrictionAngle;
            StrengthIncreaseExponent = properties.StrengthIncreaseExponent;
            ShearStrengthRatio = properties.ShearStrengthRatio;
            Pop = properties.Pop;
            WaterPressureInterpolationModel = properties.WaterPressureInterpolationModel;

            NestedLayers = nestedLayers;
        }

        /// <summary>
        /// Gets the outer ring of the soil layer.
        /// </summary>
        public IEnumerable<Point2D> OuterRing { get; }

        /// <summary>
        /// Gets the nested layers of the soil layer.
        /// </summary>
        public IEnumerable<SoilLayer> NestedLayers { get; }

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
        public ShearStrengthModel ShearStrengthModel { get; }

        /// <summary>
        /// Gets the name of the material that was assigned to the layer.
        /// </summary>
        public string MaterialName { get; }

        /// <summary>
        /// Gets the volumic weight of the layer above the phreatic level.
        /// </summary>
        public double AbovePhreaticLevel { get; }

        /// <summary>
        /// Gets the volumic weight of the layer below the phreatic level.
        /// </summary>
        public double BelowPhreaticLevel { get; }

        /// <summary>
        /// Gets the cohesion.
        /// </summary>
        public double Cohesion { get; }

        /// <summary>
        /// Gets the friction angle.
        /// </summary>
        public double FrictionAngle { get; }

        /// <summary>
        /// Gets the strength increase exponent.
        /// </summary>
        public double StrengthIncreaseExponent { get; }

        /// <summary>
        /// Gets the shear strength ratio.
        /// </summary>
        public double ShearStrengthRatio { get; }

        /// <summary>
        /// Gets the POP.
        /// </summary>
        public double Pop { get; }

        /// <summary>
        /// Gets the water pressure interpolation model.
        /// </summary>
        public WaterPressureInterpolationModel WaterPressureInterpolationModel { get; }

        /// <summary>
        /// The construction properties of the soil layer.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                ShearStrengthModel = ShearStrengthModel.CPhi;
                MaterialName = string.Empty;
                AbovePhreaticLevel = double.NaN;
                BelowPhreaticLevel = double.NaN;
                Cohesion = double.NaN;
                FrictionAngle = double.NaN;
                StrengthIncreaseExponent = double.NaN;
                ShearStrengthRatio = double.NaN;
                Pop = double.NaN;
                Dilatancy = 0.0;
                WaterPressureInterpolationModel = WaterPressureInterpolationModel.Automatic;
            }

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
            public ShearStrengthModel ShearStrengthModel { internal get; set; }

            /// <summary>
            /// Gets or sets the name of the material that was assigned to the layer.
            /// </summary>
            public string MaterialName { internal get; set; }

            /// <summary>
            /// Gets or sets the volumic weight of the layer above the phreatic level.
            /// </summary>
            public double AbovePhreaticLevel { internal get; set; }

            /// <summary>
            /// Gets or sets the volumic weight of the layer below the phreatic level.
            /// </summary>
            public double BelowPhreaticLevel { internal get; set; }

            /// <summary>
            /// Gets or sets the cohesion.
            /// </summary>
            public double Cohesion { internal get; set; }

            /// <summary>
            /// Gets or sets the friction angle.
            /// </summary>
            public double FrictionAngle { internal get; set; }

            /// <summary>
            /// Gets or sets the strength increase exponent.
            /// </summary>
            public double StrengthIncreaseExponent { internal get; set; }

            /// <summary>
            /// Gets or sets the shear strength ratio.
            /// </summary>
            public double ShearStrengthRatio { internal get; set; }

            /// <summary>
            /// Gets or sets the POP.
            /// </summary>
            public double Pop { internal get; set; }

            /// <summary>
            /// Gets or sets the dilatancy.
            /// </summary>
            public double Dilatancy { internal get; set; }

            /// <summary>
            /// Gets or sets the water pressure interpolation model.
            /// </summary>
            public WaterPressureInterpolationModel WaterPressureInterpolationModel { internal get; set; }
        }
    }
}