// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Runtime.Serialization;
using Core.Common.Base.Geometry;
using Ringtoets.MacroStabilityInwards.Data;

namespace Riskeer.Storage.Core.Serializers
{
    /// <summary>
    /// Converter class that converts between a collection of <see cref="MacroStabilityInwardsSlice"/> 
    /// and an XML representation of that data.
    /// </summary>
    internal class MacroStabilityInwardsSliceCollectionXmlSerializer : DataCollectionSerializer<MacroStabilityInwardsSlice,
        MacroStabilityInwardsSliceCollectionXmlSerializer.SerializableMacroStabilityInwardsSlice>
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
        [DataContract(Name = nameof(SerializableMacroStabilityInwardsSlice), Namespace = "")]
        internal class SerializableMacroStabilityInwardsSlice
        {
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

            #region Top coordinates

            [DataMember]
            private readonly double topLeftPointX;

            [DataMember]
            private readonly double topLeftPointY;

            [DataMember]
            private readonly double topRightPointX;

            [DataMember]
            private readonly double topRightPointY;

            #endregion

            #region Bottom coordinates

            [DataMember]
            private readonly double bottomLeftPointX;

            [DataMember]
            private readonly double bottomLeftPointY;

            [DataMember]
            private readonly double bottomRightPointX;

            [DataMember]
            private readonly double bottomRightPointY;

            #endregion

            #region Data

            [DataMember]
            private readonly double cohesion;

            [DataMember]
            private readonly double frictionAngle;

            [DataMember]
            private readonly double criticalPressure;

            [DataMember]
            private readonly double overConsolidationRatio;

            [DataMember]
            private readonly double pop;

            [DataMember]
            private readonly double degreeOfConsolidationPorePressureSoil;

            [DataMember]
            private readonly double degreeOfConsolidationPorePressureLoad;

            [DataMember]
            private readonly double dilatancy;

            [DataMember]
            private readonly double externalLoad;

            [DataMember]
            private readonly double hydrostaticPorePressure;

            [DataMember]
            private readonly double leftForce;

            [DataMember]
            private readonly double leftForceAngle;

            [DataMember]
            private readonly double leftForceY;

            [DataMember]
            private readonly double rightForce;

            [DataMember]
            private readonly double rightForceAngle;

            [DataMember]
            private readonly double rightForceY;

            [DataMember]
            private readonly double loadStress;

            [DataMember]
            private readonly double normalStress;

            [DataMember]
            private readonly double porePressure;

            [DataMember]
            private readonly double horizontalPorePressure;

            [DataMember]
            private readonly double verticalPorePressure;

            [DataMember]
            private readonly double piezometricPorePressure;

            [DataMember]
            private readonly double effectiveStress;

            [DataMember]
            private readonly double effectiveStressDaily;

            [DataMember]
            private readonly double excessPorePressure;

            [DataMember]
            private readonly double shearStress;

            [DataMember]
            private readonly double soilStress;

            [DataMember]
            private readonly double totalPorePressure;

            [DataMember]
            private readonly double totalStress;

            [DataMember]
            private readonly double weight;

            #endregion
        }
    }
}