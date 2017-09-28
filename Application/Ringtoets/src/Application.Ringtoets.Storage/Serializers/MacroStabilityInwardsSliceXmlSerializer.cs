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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.MacroStabilityInwards.Data;

namespace Application.Ringtoets.Storage.Serializers
{
    /// <summary>
    /// Converter class that converts between a collection of <see cref="MacroStabilityInwardsSlice"/> 
    /// and an XML representation of that data.
    /// </summary>
    internal class MacroStabilityInwardsSliceXmlSerializer : DataCollectionSerializer<MacroStabilityInwardsSlice,
        MacroStabilityInwardsSliceXmlSerializer.SerializableMacroStabilityInwardsSlice>
    {
        protected override SerializableMacroStabilityInwardsSlice[] ToSerializableData(IEnumerable<MacroStabilityInwardsSlice> elements)
        {
            return elements.Select(s => new SerializableMacroStabilityInwardsSlice(s)).ToArray();
        }

        protected override MacroStabilityInwardsSlice[] FromSerializableData(IEnumerable<SerializableMacroStabilityInwardsSlice> serializedElements)
        {
            return serializedElements.Select(se => se.ToMacroStabilityInwardsSlice()).ToArray();
        }

        [Serializable]
        internal class SerializableMacroStabilityInwardsSlice
        {
            private readonly double topLeftPointX;
            private readonly double topLeftPointY;
            private readonly double topRightPointX;
            private readonly double topRightPointY;

            private readonly double bottomLeftPointX;
            private readonly double bottomLeftPointY;
            private readonly double bottomRightPointX;
            private readonly double bottomRightPointY;

            private readonly double cohesion;
            private readonly double frictionAngle;
            private readonly double criticalPressure;
            private readonly double overConsolidationRatio;
            private readonly double pop;
            private readonly double degreeOfConsolidationPorePressureSoil;
            private readonly double degreeOfConsolidationPorePressureLoad;
            private readonly double dilatancy;
            private readonly double externalLoad;
            private readonly double hydrostaticPorePressure;
            private readonly double leftForce;
            private readonly double leftForceAngle;
            private readonly double leftForceY;
            private readonly double rightForce;
            private readonly double rightForceAngle;
            private readonly double rightForceY;
            private readonly double loadStress;
            private readonly double normalStress;
            private readonly double porePressure;
            private readonly double horizontalPorePressure;
            private readonly double verticalPorePressure;
            private readonly double piezometricPorePressure;
            private readonly double effectiveStress;
            private readonly double effectiveStressDaily;
            private readonly double excessPorePressure;
            private readonly double shearStress;
            private readonly double soilStress;
            private readonly double totalPorePressure;
            private readonly double totalStress;
            private readonly double weight;

            /// <summary>
            /// Creates a new instance of <see cref="SerializableMacroStabilityInwardsSlice"/>.
            /// </summary>
            /// <param name="slice">The <see cref="MacroStabilityInwardsSlice"/> to base the 
            /// <see cref="SerializableMacroStabilityInwardsSlice"/> on.</param>
            public SerializableMacroStabilityInwardsSlice(MacroStabilityInwardsSlice slice)
            {
                topLeftPointX = slice.TopLeftPoint.X;
                topLeftPointY = slice.TopLeftPoint.Y;
                topRightPointX = slice.TopRightPoint.X;
                topRightPointY = slice.TopRightPoint.Y;

                bottomLeftPointX = slice.BottomLeftPoint.X;
                bottomLeftPointY = slice.BottomLeftPoint.Y;
                bottomRightPointX = slice.BottomRightPoint.X;
                bottomRightPointY = slice.BottomRightPoint.Y;

                cohesion = slice.Cohesion;
                frictionAngle = slice.FrictionAngle;
                criticalPressure = slice.CriticalPressure;
                overConsolidationRatio = slice.OverConsolidationRatio;
                pop = slice.Pop;
                degreeOfConsolidationPorePressureSoil = slice.DegreeOfConsolidationPorePressureSoil;
                degreeOfConsolidationPorePressureLoad = slice.DegreeOfConsolidationPorePressureLoad;
                dilatancy = slice.Dilatancy;
                externalLoad = slice.ExternalLoad;
                hydrostaticPorePressure = slice.HydrostaticPorePressure;
                leftForce = slice.LeftForce;
                leftForceAngle = slice.LeftForceAngle;
                leftForceY = slice.LeftForceY;
                rightForce = slice.RightForce;
                rightForceAngle = slice.RightForceAngle;
                rightForceY = slice.RightForceY;
                loadStress = slice.LoadStress;
                normalStress = slice.NormalStress;
                porePressure = slice.PorePressure;
                horizontalPorePressure = slice.HorizontalPorePressure;
                verticalPorePressure = slice.VerticalPorePressure;
                piezometricPorePressure = slice.PiezometricPorePressure;
                effectiveStress = slice.EffectiveStress;
                effectiveStressDaily = slice.EffectiveStressDaily;
                excessPorePressure = slice.ExcessPorePressure;
                shearStress = slice.ShearStress;
                soilStress = slice.SoilStress;
                totalPorePressure = slice.TotalPorePressure;
                totalStress = slice.TotalStress;
                weight = slice.Weight;
            }

            /// <summary>
            /// Creates a new instance of <see cref="MacroStabilityInwardsSlice"/>.
            /// </summary>
            /// <returns>The new instance of <see cref="MacroStabilityInwardsSlice"/>.</returns>
            public MacroStabilityInwardsSlice ToMacroStabilityInwardsSlice()
            {
                return new MacroStabilityInwardsSlice(
                    new Point2D(topLeftPointX, topLeftPointY),
                    new Point2D(topRightPointX, topRightPointY),
                    new Point2D(bottomLeftPointX, bottomLeftPointY),
                    new Point2D(bottomRightPointX, bottomRightPointY),
                    new MacroStabilityInwardsSlice.ConstructionProperties
                    {
                        Cohesion = cohesion,
                        FrictionAngle = frictionAngle,
                        CriticalPressure = criticalPressure,
                        OverConsolidationRatio = overConsolidationRatio,
                        Pop = pop,
                        DegreeOfConsolidationPorePressureSoil = degreeOfConsolidationPorePressureSoil,
                        DegreeOfConsolidationPorePressureLoad = degreeOfConsolidationPorePressureLoad,
                        Dilatancy = dilatancy,
                        ExternalLoad = externalLoad,
                        HydrostaticPorePressure = hydrostaticPorePressure,
                        LeftForce = leftForce,
                        LeftForceAngle = leftForceAngle,
                        LeftForceY = leftForceY,
                        RightForce = rightForce,
                        RightForceAngle = rightForceAngle,
                        RightForceY = rightForceY,
                        LoadStress = loadStress,
                        NormalStress = normalStress,
                        PorePressure = porePressure,
                        HorizontalPorePressure = horizontalPorePressure,
                        VerticalPorePressure = verticalPorePressure,
                        PiezometricPorePressure = piezometricPorePressure,
                        EffectiveStress = effectiveStress,
                        EffectiveStressDaily = effectiveStressDaily,
                        ExcessPorePressure = excessPorePressure,
                        ShearStress = shearStress,
                        SoilStress = soilStress,
                        TotalPorePressure = totalPorePressure,
                        TotalStress = totalStress,
                        Weight = weight
                    });
            }
        }
    }
}