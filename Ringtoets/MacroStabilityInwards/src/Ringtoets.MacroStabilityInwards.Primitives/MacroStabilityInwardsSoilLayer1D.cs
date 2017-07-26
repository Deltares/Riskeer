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
using System.Drawing;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// This class represents profiles that were imported from D-Soil Model and will later on be used to create the
    /// necessary input for executing a macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsSoilLayer1D
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer1D"/>, where the top is set to <paramref name="top"/>.
        /// </summary>
        /// <param name="top">The top level of the layer.</param>
        public MacroStabilityInwardsSoilLayer1D(double top)
        {
            Top = top;
            Properties = new SoilLayerProperties();
        }

        /// <summary>
        /// Gets the properties of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        public SoilLayerProperties Properties { get; }

        /// <summary>
        /// Gets the top level of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        public double Top { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="MacroStabilityInwardsSoilLayer1D"/> is an aquifer.
        /// </summary>
        public bool IsAquifer
        {
            get
            {
                return Properties.IsAquifer;
            }
            set
            {
                Properties.IsAquifer = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use POP for the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        public bool UsePop
        {
            get
            {
                return Properties.UsePop;
            }
            set
            {
                Properties.UsePop = value;
            }
        }

        /// <summary>
        /// Gets or sets the shear strength model used for the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        public ShearStrengthModel ShearStrengthModel
        {
            get
            {
                return Properties.ShearStrengthModel;
            }
            set
            {
                Properties.ShearStrengthModel = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean of the distrubtion for the volumic weight of the <see cref="MacroStabilityInwardsSoilLayer1D"/> above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double AbovePhreaticLevelMean
        {
            get
            {
                return Properties.AbovePhreaticLevelMean;
            }
            set
            {
                Properties.AbovePhreaticLevelMean = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation of the distrubtion for the volumic weight of the <see cref="MacroStabilityInwardsSoilLayer1D"/> above the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double AbovePhreaticLevelDeviation
        {
            get
            {
                return Properties.AbovePhreaticLevelDeviation;
            }
            set
            {
                Properties.AbovePhreaticLevelDeviation = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean of the distrubtion for the volumic weight of the <see cref="MacroStabilityInwardsSoilLayer1D"/> below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelMean
        {
            get
            {
                return Properties.BelowPhreaticLevelMean;
            }
            set
            {
                Properties.BelowPhreaticLevelMean = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation of the distrubtion for the volumic weight of the <see cref="MacroStabilityInwardsSoilLayer1D"/> below the phreatic level.
        /// [kN/m³]
        /// </summary>
        public double BelowPhreaticLevelDeviation
        {
            get
            {
                return Properties.BelowPhreaticLevelDeviation;
            }
            set
            {
                Properties.BelowPhreaticLevelDeviation = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean of the distribution for the friction angle of the <see cref="MacroStabilityInwardsSoilLayer1D"/>
        /// [°]
        /// </summary>
        public double FrictionAngleMean
        {
            get
            {
                return Properties.FrictionAngleMean;
            }
            set
            {
                Properties.FrictionAngleMean = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the friction angle of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [°]
        /// </summary>
        public double FrictionAngleDeviation
        {
            get
            {
                return Properties.FrictionAngleDeviation;
            }
            set
            {
                Properties.FrictionAngleDeviation = value;
            }
        }

        /// <summary>
        /// Gets or sets the shift of the distrubtion for the friction angle of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [°]
        /// </summary>
        public double FrictionAngleShift
        {
            get
            {
                return Properties.FrictionAngleShift;
            }
            set
            {
                Properties.FrictionAngleShift = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean of the distribution for the ratio of shear strength S of the <see cref="MacroStabilityInwardsSoilLayer1D"/>
        /// [-]
        /// </summary>
        public double ShearStrengthRatioMean
        {
            get
            {
                return Properties.ShearStrengthRatioMean;
            }
            set
            {
                Properties.ShearStrengthRatioMean = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the ratio of shear strength S of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [-]
        /// </summary>
        public double ShearStrengthRatioDeviation
        {
            get
            {
                return Properties.ShearStrengthRatioDeviation;
            }
            set
            {
                Properties.ShearStrengthRatioDeviation = value;
            }
        }

        /// <summary>
        /// Gets or sets the shift of the distrubtion for the ratio of shear strength S of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [-]
        /// </summary>
        public double ShearStrengthRatioShift
        {
            get
            {
                return Properties.ShearStrengthRatioShift;
            }
            set
            {
                Properties.ShearStrengthRatioShift = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean of the distribution for the strength increase exponent (m) of the <see cref="MacroStabilityInwardsSoilLayer1D"/>
        /// [-]
        /// </summary>
        public double StrengthIncreaseExponentMean
        {
            get
            {
                return Properties.StrengthIncreaseExponentMean;
            }
            set
            {
                Properties.StrengthIncreaseExponentMean = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the strength increase exponent (m) of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [-]
        /// </summary>
        public double StrengthIncreaseExponentDeviation
        {
            get
            {
                return Properties.StrengthIncreaseExponentDeviation;
            }
            set
            {
                Properties.StrengthIncreaseExponentDeviation = value;
            }
        }

        /// <summary>
        /// Gets or sets the shift of the distrubtion for the strength increase exponent (m) of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [-]
        /// </summary>
        public double StrengthIncreaseExponentShift
        {
            get
            {
                return Properties.StrengthIncreaseExponentShift;
            }
            set
            {
                Properties.StrengthIncreaseExponentShift = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean of the distribution for the cohesion of the <see cref="MacroStabilityInwardsSoilLayer1D"/>
        /// [kN/m³]
        /// </summary>
        public double CohesionMean
        {
            get
            {
                return Properties.CohesionMean;
            }
            set
            {
                Properties.CohesionMean = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the cohesion of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [kN/m³]
        /// </summary>
        public double CohesionDeviation
        {
            get
            {
                return Properties.CohesionDeviation;
            }
            set
            {
                Properties.CohesionDeviation = value;
            }
        }

        /// <summary>
        /// Gets or sets the shift of the distrubtion for the cohesion of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [kN/m³]
        /// </summary>
        public double CohesionShift
        {
            get
            {
                return Properties.CohesionShift;
            }
            set
            {
                Properties.CohesionShift = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean of the distribution for the POP of the <see cref="MacroStabilityInwardsSoilLayer1D"/>
        /// [kN/m²]
        /// </summary>
        public double PopMean
        {
            get
            {
                return Properties.PopMean;
            }
            set
            {
                Properties.PopMean = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation of the distribution for the POP of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [kN/m²]
        /// </summary>
        public double PopDeviation
        {
            get
            {
                return Properties.PopDeviation;
            }
            set
            {
                Properties.PopDeviation = value;
            }
        }

        /// <summary>
        /// Gets or sets the shift of the distrubtion for the POP of the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// [kN/m²]
        /// </summary>
        public double PopShift
        {
            get
            {
                return Properties.PopShift;
            }
            set
            {
                Properties.PopShift = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the material that was assigned to the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public string MaterialName
        {
            get
            {
                return Properties.MaterialName;
            }
            set
            {
                Properties.MaterialName = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Color"/> that was used to represent the <see cref="MacroStabilityInwardsSoilLayer1D"/>.
        /// </summary>
        public Color Color
        {
            get
            {
                return Properties.Color;
            }
            set
            {
                Properties.Color = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            var other = obj as MacroStabilityInwardsSoilLayer1D;
            return other != null && Equals((MacroStabilityInwardsSoilLayer1D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = MaterialName?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Top.GetHashCode();
                hashCode = (hashCode * 397) ^ Properties.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsSoilLayer1D other)
        {
            return string.Equals(MaterialName, other.MaterialName)
                   && Top.Equals(other.Top)
                   && Properties.Equals(other.Properties);
        }
    }
}