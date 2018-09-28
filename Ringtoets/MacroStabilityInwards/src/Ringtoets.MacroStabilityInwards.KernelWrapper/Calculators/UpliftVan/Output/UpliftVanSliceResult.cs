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
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output
{
    /// <summary>
    /// The slice result of an Uplift Van calculation.
    /// </summary>
    public class UpliftVanSliceResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSliceResult"/>.
        /// </summary>
        /// <param name="topLeftPoint">The top left point of the slice.</param>
        /// <param name="topRightPoint">The top right point of the slice.</param>
        /// <param name="bottomLeftPoint">The bottom left point of the slice.</param>
        /// <param name="bottomRightPoint">The bottom right point of the slice.</param>
        /// <param name="properties">The object containing the values for the properties 
        /// of the new <see cref="UpliftVanSliceResult"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal UpliftVanSliceResult(Point2D topLeftPoint, Point2D topRightPoint,
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

            Cohesion = properties.Cohesion;
            FrictionAngle = properties.FrictionAngle;
            CriticalPressure = properties.CriticalPressure;
            OverConsolidationRatio = properties.OverConsolidationRatio;
            Pop = properties.Pop;
            DegreeOfConsolidationPorePressureSoil = properties.DegreeOfConsolidationPorePressureSoil;
            DegreeOfConsolidationPorePressureLoad = properties.DegreeOfConsolidationPorePressureLoad;
            Dilatancy = properties.Dilatancy;
            ExternalLoad = properties.ExternalLoad;
            HydrostaticPorePressure = properties.HydrostaticPorePressure;
            LeftForce = properties.LeftForce;
            LeftForceAngle = properties.LeftForceAngle;
            LeftForceY = properties.LeftForceY;
            RightForce = properties.RightForce;
            RightForceAngle = properties.RightForceAngle;
            RightForceY = properties.RightForceY;
            LoadStress = properties.LoadStress;
            NormalStress = properties.NormalStress;
            PorePressure = properties.PorePressure;
            HorizontalPorePressure = properties.HorizontalPorePressure;
            VerticalPorePressure = properties.VerticalPorePressure;
            PiezometricPorePressure = properties.PiezometricPorePressure;
            EffectiveStress = properties.EffectiveStress;
            EffectiveStressDaily = properties.EffectiveStressDaily;
            ExcessPorePressure = properties.ExcessPorePressure;
            ShearStress = properties.ShearStress;
            SoilStress = properties.SoilStress;
            TotalPorePressure = properties.TotalPorePressure;
            TotalStress = properties.TotalStress;
            Weight = properties.Weight;
        }

        /// <summary>
        /// Gets the top left point.
        /// </summary>
        public Point2D TopLeftPoint { get; }

        /// <summary>
        /// Gets the top right point.
        /// </summary>
        public Point2D TopRightPoint { get; }

        /// <summary>
        /// Gets the bottom left point.
        /// </summary>
        public Point2D BottomLeftPoint { get; }

        /// <summary>
        /// Gets the bottom right point.
        /// </summary>
        public Point2D BottomRightPoint { get; }

        /// <summary>
        /// Gets the cohesion.
        /// [kN/m²]
        /// </summary>
        public double Cohesion { get; }

        /// <summary>
        /// Gets the friction angle.
        /// [°]
        /// </summary>
        public double FrictionAngle { get; }

        /// <summary>
        /// Gets the critical pressure.
        /// [kN/m²]
        /// </summary>
        public double CriticalPressure { get; }

        /// <summary>
        /// Gets the over consolidation ratio.
        /// [-]
        /// </summary>
        public double OverConsolidationRatio { get; }

        /// <summary>
        /// Gets the POP.
        /// [kN/m²]
        /// </summary>
        public double Pop { get; }

        /// <summary>
        /// Gets the pore pressure from degree of consolidation soil.
        /// [kN/m²]
        /// </summary>
        public double DegreeOfConsolidationPorePressureSoil { get; }

        /// <summary>
        /// Gets the pore pressure from degree of consolidation load.
        /// [kN/m²]
        /// </summary>
        public double DegreeOfConsolidationPorePressureLoad { get; }

        /// <summary>
        /// Gets the dilatancy of the slice.
        /// </summary>
        public double Dilatancy { get; }

        /// <summary>
        /// Gets the external load.
        /// [kN/m²]
        /// </summary>
        public double ExternalLoad { get; }

        /// <summary>
        /// Gets the hydrostatic pore pressure.
        /// [kN/m²]
        /// </summary>
        public double HydrostaticPorePressure { get; }

        /// <summary>
        /// Gets the left force.
        /// [kN/m²]
        /// </summary>
        public double LeftForce { get; }

        /// <summary>
        /// Gets the left force angle.
        /// [°]
        /// </summary>
        public double LeftForceAngle { get; }

        /// <summary>
        /// Gets the left force y.
        /// [kN/m²]
        /// </summary>
        public double LeftForceY { get; }

        /// <summary>
        /// Gets the right force.
        /// [kN/m²]
        /// </summary>
        public double RightForce { get; }

        /// <summary>
        /// Gets the right force angle.
        /// [°]
        /// </summary>
        public double RightForceAngle { get; }

        /// <summary>
        /// Gets the right force y.
        /// [kN/m²]
        /// </summary>
        public double RightForceY { get; }

        /// <summary>
        /// Gets the load stress.
        /// [kN/m²]
        /// </summary>
        public double LoadStress { get; }

        /// <summary>
        /// Gets the normal stress.
        /// [kN/m²]
        /// </summary>
        public double NormalStress { get; }

        /// <summary>
        /// Gets the pore pressure.
        /// [kN/m²]
        /// </summary>
        public double PorePressure { get; }

        /// <summary>
        /// Gets the horizontal pore pressure.
        /// [kN/m²]
        /// </summary>
        public double HorizontalPorePressure { get; }

        /// <summary>
        /// Gets the vertical pore pressure.
        /// [kN/m²]
        /// </summary>
        public double VerticalPorePressure { get; }

        /// <summary>
        /// Gets the piezometric pore pressure.
        /// [kN/m²]
        /// </summary>
        public double PiezometricPorePressure { get; }

        /// <summary>
        /// Gets the effective stress.
        /// [kN/m²]
        /// </summary>
        public double EffectiveStress { get; }

        /// <summary>
        /// Gets the daily effective stress.
        /// [kN/m²]
        /// </summary>
        public double EffectiveStressDaily { get; }

        /// <summary>
        /// Gets the excess pore pressure.
        /// [kN/m²]
        /// </summary>
        public double ExcessPorePressure { get; }

        /// <summary>
        /// Gets the shear stress.
        /// [kN/m²]
        /// </summary>
        public double ShearStress { get; }

        /// <summary>
        /// Gets the soil stress.
        /// [kN/m²]
        /// </summary>
        public double SoilStress { get; }

        /// <summary>
        /// Gets the total pore pressure.
        /// [kN/m²]
        /// </summary>
        public double TotalPorePressure { get; }

        /// <summary>
        /// Gets the total stress.
        /// [kN/m²]
        /// </summary>
        public double TotalStress { get; }

        /// <summary>
        /// Gets the weight.
        /// [kN/m]
        /// </summary>
        public double Weight { get; }

        /// <summary>
        /// Container for properties for constructing a <see cref="UpliftVanSliceResult"/>.
        /// </summary>
        internal class ConstructionProperties
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
                EffectiveStressDaily = double.NaN;
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
            /// Gets or sets the over consolidation ratio.
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
            /// Gets or sets the hydrostatic pore pressure.
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
            /// Gets or sets the daily effective stress.
            /// [kN/m²]
            /// </summary>
            public double EffectiveStressDaily { internal get; set; }

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