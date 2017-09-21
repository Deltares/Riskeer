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

using Deltares.WTIStability;
using Deltares.WTIStability.Data.Geo;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil
{
    /// <summary>
    /// Factory to create simple <see cref="SlidingDualCircle"/>
    /// instances that can be used for testing.
    /// </summary>
    public static class SlidingDualCircleTestFactory
    {
        /// <summary>
        /// Creates a  simple <see cref="SlidingDualCircle"/>.
        /// </summary>
        /// <returns>A simple <see cref="SlidingDualCircle"/> with default values.</returns>
        public static SlidingDualCircle Create()
        {
            return new SlidingDualCircle
            {
                LeftCircleIsActive = false,
                ActiveCircle = new GeometryPoint(0.1, 0.2),
                ActiveForce = 0.3,
                ActiveForce0 = 0.4,
                ActiveRadius = 0.5,
                DrivingMomentActive = 0.6,
                ResistingMomentActive = 0.7,
                PassiveCircle = new GeometryPoint(0.8, 0.9),
                PassiveForce = 1.0,
                PassiveForce0 = 1.1,
                PassiveRadius = 1.2,
                DrivingMomentPassive = 1.3,
                ResistingMomentPassive = 1.4,
                HorizontalForce = 1.5,
                HorizontalForce0 = 1.6,
                Slices =
                {
                    new Slice
                    {
                        TopLeftX = 1.7,
                        TopLeftZ = 1.8,
                        TopRightX = 1.9,
                        TopRightZ = 2.0,
                        BottomLeftX = 2.1,
                        BottomLeftZ = 2.2,
                        BottomRightX = 2.3,
                        BottomRightZ = 2.4,
                        Cohesion = 2.5,
                        Phi = 2.6,
                        PGrens = 2.7,
                        OCR = 2.8,
                        POP = 2.9,
                        DegreeofConsolidationPorePressure = 3.0,
                        PorePressureDueToDegreeOfConsolidationLoad = 3.1,
                        Dilatancy = 3.2,
                        ExternalLoad = 3.3,
                        HydrostaticPorePressure = 3.4,
                        LeftForce = 3.5,
                        LeftForceAngle = 3.6,
                        LeftForceY = 3.7,
                        RightForce = 3.8,
                        RightForceAngle = 3.9,
                        RightForceY = 4.0,
                        LoadStress = 4.1,
                        NormalStress = 4.2,
                        PoreOnSurface = 4.3,
                        HPoreOnSurface = 4.4,
                        VPoreOnSurface = 4.5,
                        PiezometricPorePressure = 4.6,
                        EffectiveStress = 4.7,
                        EffectiveStressDaily = 4.8,
                        ExcessPorePressure = 4.9,
                        ShearStress = 5.0,
                        SoilStress = 5.1,
                        TotalPorePressure = 5.2,
                        TotalStress = 5.3,
                        Weight = 5.4
                    }
                }
            };
        }
    }
}