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
using Core.Common.Base.Data;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="MacroStabilityInwardsSlice"/>.
    /// </summary>
    public class MacroStabilityInwardsSliceRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSliceRow"/>.
        /// </summary>
        /// <param name="slice">The <see cref="MacroStabilityInwardsSlice"/> to use.</param>
        /// <param name="index">The index of the slice.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="slice"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsSliceRow(MacroStabilityInwardsSlice slice, int index)
        {
            if (slice == null)
            {
                throw new ArgumentNullException(nameof(slice));
            }

            Name = string.Format(Resources.MacroStabilityInwardsSlicesTable_Name_Slice_0, index);
            XCenter = slice.XCenter;
            ZCenterBottom = slice.ZCenterBottom;
            Width = slice.Width;
            ArcLength = slice.ArcLength;
            BottomAngle = slice.BottomAngle;
            TopAngle = slice.TopAngle;
            FrictionAngle = slice.FrictionAngle;
            Cohesion = slice.Cohesion;
            EffectiveStress = slice.EffectiveStress;
            EffectiveStressDaily = slice.EffectiveStressDaily;
            TotalPorePressure = slice.TotalPorePressure;
            Weight = slice.Weight;
            PiezometricPorePressure = slice.PiezometricPorePressure;
            PorePressure = slice.PorePressure;
            VerticalPorePressure = slice.VerticalPorePressure;
            HorizontalPorePressure = slice.HorizontalPorePressure;
            OverConsolidationRatio = slice.OverConsolidationRatio;
            Pop = slice.Pop;
            NormalStress = slice.NormalStress;
            ShearStress = slice.ShearStress;
            LoadStress = slice.LoadStress;
        }

        /// <summary>
        /// Gets the name of the slice.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the load stress of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble LoadStress { get; }

        /// <summary>
        /// Gets the shear stress of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble ShearStress { get; }

        /// <summary>
        /// Gets the normal stress of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble NormalStress { get; }

        /// <summary>
        /// Gets the POP of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble Pop { get; }

        /// <summary>
        /// Gets the over consolidation ratio of the slice.
        /// [-]
        /// </summary>
        public RoundedDouble OverConsolidationRatio { get; }

        /// <summary>
        /// Gets the horizontal pressure of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble HorizontalPorePressure { get; }

        /// <summary>
        /// Gets the vertical pore pressure of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble VerticalPorePressure { get; }

        /// <summary>
        /// Gets the pore pressure of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble PorePressure { get; }

        /// <summary>
        /// Gets the piezometric pore pressure of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble PiezometricPorePressure { get; }

        /// <summary>
        /// Gets the weight of the slice.
        /// [kN/m]
        /// </summary>
        public RoundedDouble Weight { get; }

        /// <summary>
        /// Gets the total pore pressure of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble TotalPorePressure { get; }

        /// <summary>
        /// Gets the X center point of the slice.
        /// [m]
        /// </summary>
        public RoundedDouble XCenter { get; }

        /// <summary>
        /// Gets the Z center bottom point of the slice.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble ZCenterBottom { get; }

        /// <summary>
        /// Gets the width of the slice.
        /// [m]
        /// </summary>
        public RoundedDouble Width { get; }

        /// <summary>
        /// Gets the arc length of the slice.
        /// [m]
        /// </summary>
        public RoundedDouble ArcLength { get; }

        /// <summary>
        /// Gets the top angle of the slice.
        /// [°]
        /// </summary>
        public RoundedDouble TopAngle { get; }

        /// <summary>
        /// Gets the bottom angle of the slice.
        /// [°]
        /// </summary>
        public RoundedDouble BottomAngle { get; }

        /// <summary>
        /// Gets the friction angle of the slice.
        /// [°]
        /// </summary>
        public RoundedDouble FrictionAngle { get; }

        /// <summary>
        /// Gets the cohesion of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble Cohesion { get; }

        /// <summary>
        /// Gets the effective stress of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble EffectiveStress { get; }

        /// <summary>
        /// Gets the effective stress of the slice under
        /// daily circumstances.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble EffectiveStressDaily { get; }
    }
}