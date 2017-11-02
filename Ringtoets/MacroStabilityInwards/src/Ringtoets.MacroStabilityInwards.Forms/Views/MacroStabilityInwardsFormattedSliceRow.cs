﻿// Copyright (C) Stichting Deltares ²017. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Ringtoets.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="MacroStabilityInwardsSlice"/>.
    /// </summary>
    public class MacroStabilityInwardsFormattedSliceRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFormattedSoilLayerDataRow"/>.
        /// </summary>
        /// <param name="slice">The <see cref="MacroStabilityInwardsSlice"/> to format.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="slice"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsFormattedSliceRow(MacroStabilityInwardsSlice slice, int index)
        {
            if (slice == null)
            {
                throw new ArgumentNullException(nameof(slice));
            }

            Name = string.Format(Resources.MacroStabilityInwardsSlicesTable_Name_Slice_0, index);
            XCenter = new RoundedDouble(2, (slice.TopLeftPoint.X + slice.TopRightPoint.X) / 2.0);
            ZCenterBottom = new RoundedDouble(2, new Segment2D(slice.BottomLeftPoint, slice.BottomRightPoint).Interpolate(XCenter));
            Width = new RoundedDouble(2, slice.TopRightPoint.X - slice.TopLeftPoint.X);
            ArcLength = new RoundedDouble(2, slice.BottomLeftPoint.GetEuclideanDistanceTo(slice.BottomRightPoint));
            BottomAngle = new RoundedDouble(2, GetAngleBetween(slice.BottomRightPoint, slice.BottomLeftPoint));
            TopAngle = new RoundedDouble(2, GetAngleBetween(slice.TopRightPoint, slice.TopLeftPoint));
            FrictionAngle = new RoundedDouble(3, slice.FrictionAngle);
            Cohesion = new RoundedDouble(3, slice.Cohesion);
            EffectiveStress = new RoundedDouble(3, slice.EffectiveStress);
            TotalPorePressure = new RoundedDouble(3, slice.TotalPorePressure);
            Weight = new RoundedDouble(3, slice.Weight);
            PiezometricPorePressure = new RoundedDouble(3, slice.PiezometricPorePressure);
            DegreeOfConsolidationPorePressureSoil = new RoundedDouble(3, slice.DegreeOfConsolidationPorePressureSoil);
            DegreeOfConsolidationPorePressureLoad = new RoundedDouble(3, slice.DegreeOfConsolidationPorePressureLoad);
            PorePressure = new RoundedDouble(3, slice.PorePressure);
            VerticalPorePressure = new RoundedDouble(3, slice.VerticalPorePressure);
            HorizontalPorePressure = new RoundedDouble(3, slice.HorizontalPorePressure);
            ExternalLoad = new RoundedDouble(3, slice.ExternalLoad);
            OverConsolidationRatio = new RoundedDouble(3, slice.OverConsolidationRatio);
            Pop = new RoundedDouble(3, slice.Pop);
            NormalStress = new RoundedDouble(3, slice.NormalStress);
            ShearStress = new RoundedDouble(3, slice.ShearStress);
            LoadStress = new RoundedDouble(3, slice.LoadStress);
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
        /// Gets the external load of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble ExternalLoad { get; }

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
        /// Gets the pore pressure from degree of consolidation load of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble DegreeOfConsolidationPorePressureLoad { get; }

        /// <summary>
        /// Gets the pore pressure from degree of consolidation soil of the slice.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble DegreeOfConsolidationPorePressureSoil { get; }

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

        private static double GetAngleBetween(Point2D pointA, Point2D pointB)
        {
            return Math.Atan2(pointA.Y - pointB.Y,
                              pointA.X - pointB.X) * (180 / Math.PI);
        }
    }
}