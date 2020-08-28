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
using Core.Common.Base.Geometry;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// A slice of a macro stability inwards sliding curve.
    /// </summary>
    public class MacroStabilityInwardsSlice : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSlice"/>.
        /// </summary>
        /// <param name="topLeftPoint">The top left point of the slice.</param>
        /// <param name="topRightPoint">The top right point of the slice.</param>
        /// <param name="bottomLeftPoint">The bottom left point of the slice.</param>
        /// <param name="bottomRightPoint">The bottom right point of the slice.</param>
        /// <param name="properties">The object containing the values for the properties 
        /// of the new <see cref="MacroStabilityInwardsSlice"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsSlice(Point2D topLeftPoint, Point2D topRightPoint,
                                          Point2D bottomLeftPoint, Point2D bottomRightPoint,
                                          ConstructionProperties properties)
        {
            if (topLeftPoint == null)
            {
                throw new ArgumentNullException(nameof(topLeftPoint));
            }

            if (topRightPoint == null)
            {
                throw new ArgumentNullException(nameof(topRightPoint));
            }

            if (bottomLeftPoint == null)
            {
                throw new ArgumentNullException(nameof(bottomLeftPoint));
            }

            if (bottomRightPoint == null)
            {
                throw new ArgumentNullException(nameof(bottomRightPoint));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            TopLeftPoint = topLeftPoint;
            TopRightPoint = topRightPoint;
            BottomLeftPoint = bottomLeftPoint;
            BottomRightPoint = bottomRightPoint;

            XCenter = new RoundedDouble(2, (topLeftPoint.X + topRightPoint.X) / 2.0);
            ZCenterBottom = new RoundedDouble(2, new Segment2D(bottomLeftPoint, bottomRightPoint).Interpolate(XCenter));
            Width = new RoundedDouble(2, topRightPoint.X - topLeftPoint.X);
            ArcLength = new RoundedDouble(2, bottomLeftPoint.GetEuclideanDistanceTo(bottomRightPoint));
            BottomAngle = new RoundedDouble(2, Math2D.GetAngleBetween(bottomLeftPoint, bottomRightPoint));
            TopAngle = new RoundedDouble(2, Math2D.GetAngleBetween(topLeftPoint, topRightPoint));

            Cohesion = new RoundedDouble(3, properties.Cohesion);
            FrictionAngle = new RoundedDouble(3, properties.FrictionAngle);
            CriticalPressure = new RoundedDouble(3, properties.CriticalPressure);
            OverConsolidationRatio = new RoundedDouble(3, properties.OverConsolidationRatio);
            Pop = new RoundedDouble(3, properties.Pop);
            DegreeOfConsolidationPorePressureSoil = new RoundedDouble(3, properties.DegreeOfConsolidationPorePressureSoil);
            DegreeOfConsolidationPorePressureLoad = new RoundedDouble(3, properties.DegreeOfConsolidationPorePressureLoad);
            Dilatancy = new RoundedDouble(3, properties.Dilatancy);
            ExternalLoad = new RoundedDouble(3, properties.ExternalLoad);
            HydrostaticPorePressure = new RoundedDouble(3, properties.HydrostaticPorePressure);
            LeftForce = new RoundedDouble(3, properties.LeftForce);
            LeftForceAngle = new RoundedDouble(3, properties.LeftForceAngle);
            LeftForceY = new RoundedDouble(3, properties.LeftForceY);
            RightForce = new RoundedDouble(3, properties.RightForce);
            RightForceAngle = new RoundedDouble(3, properties.RightForceAngle);
            RightForceY = new RoundedDouble(3, properties.RightForceY);
            LoadStress = new RoundedDouble(3, properties.LoadStress);
            NormalStress = new RoundedDouble(3, properties.NormalStress);
            PorePressure = new RoundedDouble(3, properties.PorePressure);
            HorizontalPorePressure = new RoundedDouble(3, properties.HorizontalPorePressure);
            VerticalPorePressure = new RoundedDouble(3, properties.VerticalPorePressure);
            PiezometricPorePressure = new RoundedDouble(3, properties.PiezometricPorePressure);
            EffectiveStress = new RoundedDouble(3, properties.EffectiveStress);
            ExcessPorePressure = new RoundedDouble(3, properties.ExcessPorePressure);
            ShearStress = new RoundedDouble(3, properties.ShearStress);
            SoilStress = new RoundedDouble(3, properties.SoilStress);
            TotalPorePressure = new RoundedDouble(3, properties.TotalPorePressure);
            TotalStress = new RoundedDouble(3, properties.TotalStress);
            Weight = new RoundedDouble(3, properties.Weight);
        }

        /// <summary>
        /// Gets the top left point.
        /// </summary>
        public Point2D TopLeftPoint { get; private set; }

        /// <summary>
        /// Gets the top right point.
        /// </summary>
        public Point2D TopRightPoint { get; private set; }

        /// <summary>
        /// Gets the bottom left point.
        /// </summary>
        public Point2D BottomLeftPoint { get; private set; }

        /// <summary>
        /// Gets the bottom right point.
        /// </summary>
        public Point2D BottomRightPoint { get; private set; }

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
        /// Gets the cohesion.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble Cohesion { get; }

        /// <summary>
        /// Gets the friction angle.
        /// [°]
        /// </summary>
        public RoundedDouble FrictionAngle { get; }

        /// <summary>
        /// Gets the critical pressure.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble CriticalPressure { get; }

        /// <summary>
        /// Gets the OCR.
        /// [-]
        /// </summary>
        public RoundedDouble OverConsolidationRatio { get; }

        /// <summary>
        /// Gets the POP.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble Pop { get; }

        /// <summary>
        /// Gets the pore pressure from degree of consolidation soil.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble DegreeOfConsolidationPorePressureSoil { get; }

        /// <summary>
        /// Gets the pore pressure from degree of consolidation load.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble DegreeOfConsolidationPorePressureLoad { get; }

        /// <summary>
        /// Gets the dilatancy of the slice.
        /// </summary>
        public RoundedDouble Dilatancy { get; }

        /// <summary>
        /// Gets the external load.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble ExternalLoad { get; }

        /// <summary>
        /// Gets the hydrostatic pore pressure.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble HydrostaticPorePressure { get; }

        /// <summary>
        /// Gets the left force.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble LeftForce { get; }

        /// <summary>
        /// Gets the left force angle.
        /// [°]
        /// </summary>
        public RoundedDouble LeftForceAngle { get; }

        /// <summary>
        /// Gets the left force y.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble LeftForceY { get; }

        /// <summary>
        /// Gets the right force.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble RightForce { get; }

        /// <summary>
        /// Gets the right force angle.
        /// [°]
        /// </summary>
        public RoundedDouble RightForceAngle { get; }

        /// <summary>
        /// Gets the right force y.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble RightForceY { get; }

        /// <summary>
        /// Gets the load stress.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble LoadStress { get; }

        /// <summary>
        /// Gets the normal stress.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble NormalStress { get; }

        /// <summary>
        /// Gets the pore pressure.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble PorePressure { get; }

        /// <summary>
        /// Gets the horizontal pore pressure.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble HorizontalPorePressure { get; }

        /// <summary>
        /// Gets the vertical pore pressure.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble VerticalPorePressure { get; }

        /// <summary>
        /// Gets the piezometric pore pressure.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble PiezometricPorePressure { get; }

        /// <summary>
        /// Gets the effective stress.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble EffectiveStress { get; }

        /// <summary>
        /// Gets the excess pore pressure.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble ExcessPorePressure { get; }

        /// <summary>
        /// Gets the shear stress.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble ShearStress { get; }

        /// <summary>
        /// Gets the soil stress.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble SoilStress { get; }

        /// <summary>
        /// Gets the total pore pressure.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble TotalPorePressure { get; }

        /// <summary>
        /// Gets the total stress.
        /// [kN/m²]
        /// </summary>
        public RoundedDouble TotalStress { get; }

        /// <summary>
        /// Gets the weight.
        /// [kN/m]
        /// </summary>
        public RoundedDouble Weight { get; }

        public object Clone()
        {
            var clone = (MacroStabilityInwardsSlice) MemberwiseClone();
            clone.TopLeftPoint = (Point2D) TopLeftPoint.Clone();
            clone.TopRightPoint = (Point2D) TopRightPoint.Clone();
            clone.BottomLeftPoint = (Point2D) BottomLeftPoint.Clone();
            clone.BottomRightPoint = (Point2D) BottomRightPoint.Clone();

            return clone;
        }

        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                Cohesion = double.NaN;
                FrictionAngle = double.NaN;
                CriticalPressure = double.NaN;
                OverConsolidationRatio = double.NaN;
                Pop = double.NaN;
                DegreeOfConsolidationPorePressureSoil = double.NaN;
                DegreeOfConsolidationPorePressureLoad = double.NaN;
                Dilatancy = double.NaN;
                ExternalLoad = double.NaN;
                HydrostaticPorePressure = double.NaN;
                LeftForce = double.NaN;
                LeftForceAngle = double.NaN;
                LeftForceY = double.NaN;
                RightForce = double.NaN;
                RightForceAngle = double.NaN;
                RightForceY = double.NaN;
                LoadStress = double.NaN;
                NormalStress = double.NaN;
                PorePressure = double.NaN;
                HorizontalPorePressure = double.NaN;
                VerticalPorePressure = double.NaN;
                PiezometricPorePressure = double.NaN;
                EffectiveStress = double.NaN;
                ExcessPorePressure = double.NaN;
                ShearStress = double.NaN;
                SoilStress = double.NaN;
                TotalPorePressure = double.NaN;
                TotalStress = double.NaN;
                Weight = double.NaN;
            }

            /// <summary>
            /// Gets or sets the cohesion.
            /// [kN/m²]
            /// </summary>
            public double Cohesion { internal get; set; }

            /// <summary>
            /// Gets or sets the friction angle.
            /// [°]
            /// </summary>
            public double FrictionAngle { internal get; set; }

            /// <summary>
            /// Gets or sets the critical pressure.
            /// [kN/m²]
            /// </summary>
            public double CriticalPressure { internal get; set; }

            /// <summary>
            /// Gets or sets the OCR.
            /// [-]
            /// </summary>
            public double OverConsolidationRatio { internal get; set; }

            /// <summary>
            /// Gets or sets the POP.
            /// [kN/m²]
            /// </summary>
            public double Pop { internal get; set; }

            /// <summary>
            /// Gets or sets the pore pressure from degree of consolidation soil.
            /// [kN/m²]
            /// </summary>
            public double DegreeOfConsolidationPorePressureSoil { internal get; set; }

            /// <summary>
            /// Gets or sets the pore pressure from degree of consolidation load.
            /// [kN/m²]
            /// </summary>
            public double DegreeOfConsolidationPorePressureLoad { internal get; set; }

            /// <summary>
            /// Gets or sets the dilatancy of the slice.
            /// </summary>
            public double Dilatancy { internal get; set; }

            /// <summary>
            /// Gets or sets the external load.
            /// [kN/m²]
            /// </summary>
            public double ExternalLoad { internal get; set; }

            /// <summary>
            /// Gets or sets the hydraostatic pore pressure.
            /// [kN/m²]
            /// </summary>
            public double HydrostaticPorePressure { internal get; set; }

            /// <summary>
            /// Gets or sets the left force.
            /// [kN/m²]
            /// </summary>
            public double LeftForce { internal get; set; }

            /// <summary>
            /// Gets or sets the left force angle.
            /// [°]
            /// </summary>
            public double LeftForceAngle { internal get; set; }

            /// <summary>
            /// Gets or sets the left force y.
            /// [kN/m²]
            /// </summary>
            public double LeftForceY { internal get; set; }

            /// <summary>
            /// Gets or sets the right force.
            /// [kN/m²]
            /// </summary>
            public double RightForce { internal get; set; }

            /// <summary>
            /// Gets or sets the right force angle.
            /// [°]
            /// </summary>
            public double RightForceAngle { internal get; set; }

            /// <summary>
            /// Gets or sets the right force y.
            /// [kN/m²]
            /// </summary>
            public double RightForceY { internal get; set; }

            /// <summary>
            /// Gets or sets the load stress.
            /// [kN/m²]
            /// </summary>
            public double LoadStress { internal get; set; }

            /// <summary>
            /// Gets or sets the normal stress.
            /// [kN/m²]
            /// </summary>
            public double NormalStress { internal get; set; }

            /// <summary>
            /// Gets or sets the pore pressure.
            /// [kN/m²]
            /// </summary>
            public double PorePressure { internal get; set; }

            /// <summary>
            /// Gets or sets the horizontal pore pressure.
            /// [kN/m²]
            /// </summary>
            public double HorizontalPorePressure { internal get; set; }

            /// <summary>
            /// Gets or sets the vertical pore pressure.
            /// [kN/m²]
            /// </summary>
            public double VerticalPorePressure { internal get; set; }

            /// <summary>
            /// Gets or sets the piezometric pore pressure.
            /// [kN/m²]
            /// </summary>
            public double PiezometricPorePressure { internal get; set; }

            /// <summary>
            /// Gets or sets the effective stress.
            /// [kN/m²]
            /// </summary>
            public double EffectiveStress { internal get; set; }

            /// <summary>
            /// Gets or sets the excess pore pressure.
            /// [kN/m²]
            /// </summary>
            public double ExcessPorePressure { internal get; set; }

            /// <summary>
            /// Gets or sets the shear stress.
            /// [kN/m²]
            /// </summary>
            public double ShearStress { internal get; set; }

            /// <summary>
            /// Gets or sets the soil stress.
            /// [kN/m²]
            /// </summary>
            public double SoilStress { internal get; set; }

            /// <summary>
            /// Gets or sets the total pore pressure.
            /// [kN/m²]
            /// </summary>
            public double TotalPorePressure { internal get; set; }

            /// <summary>
            /// Gets or sets the total stress.
            /// [kN/m²]
            /// </summary>
            public double TotalStress { internal get; set; }

            /// <summary>
            /// Gets or sets the weight.
            /// [kN/m]
            /// </summary>
            public double Weight { internal get; set; }
        }
    }
}