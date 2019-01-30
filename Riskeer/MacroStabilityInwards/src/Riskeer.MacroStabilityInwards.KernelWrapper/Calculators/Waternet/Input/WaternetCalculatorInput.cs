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
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input
{
    /// <summary>
    /// This class contains all the parameters that are required to perform a Waternet calculation.
    /// </summary>
    public class WaternetCalculatorInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaternetCalculatorInput"/>.
        /// </summary>
        /// <param name="properties">The object containing the values for the properties 
        /// of the new <see cref="WaternetCalculatorInput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when one of the following properties is <c>null</c>:
        /// <list type="bullet">
        /// <item><see cref="ConstructionProperties.SurfaceLine"/>;</item>
        /// <item><see cref="ConstructionProperties.SoilProfile"/>;</item>
        /// <item><see cref="ConstructionProperties.DrainageConstruction"/>;</item>
        /// <item><see cref="ConstructionProperties.PhreaticLineOffsets"/>.</item>
        /// </list>
        /// </exception>
        public WaternetCalculatorInput(ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (properties.SurfaceLine == null)
            {
                throw new ArgumentException("SurfaceLine must be set.");
            }

            if (properties.SoilProfile == null)
            {
                throw new ArgumentException("SoilProfile must be set.");
            }

            if (properties.DrainageConstruction == null)
            {
                throw new ArgumentException("DrainageConstruction must be set.");
            }

            if (properties.PhreaticLineOffsets == null)
            {
                throw new ArgumentException("PhreaticLineOffsets must be set.");
            }

            WaternetCreationMode = properties.WaternetCreationMode;
            PlLineCreationMethod = properties.PlLineCreationMethod;
            AssessmentLevel = properties.AssessmentLevel;
            LandwardDirection = properties.LandwardDirection;
            SurfaceLine = properties.SurfaceLine;
            SoilProfile = properties.SoilProfile;
            DrainageConstruction = properties.DrainageConstruction;
            PhreaticLineOffsets = properties.PhreaticLineOffsets;
            WaterLevelRiverAverage = properties.WaterLevelRiverAverage;
            WaterLevelPolder = properties.WaterLevelPolder;
            MinimumLevelPhreaticLineAtDikeTopRiver = properties.MinimumLevelPhreaticLineAtDikeTopRiver;
            MinimumLevelPhreaticLineAtDikeTopPolder = properties.MinimumLevelPhreaticLineAtDikeTopPolder;
            LeakageLengthOutwardsPhreaticLine3 = properties.LeakageLengthOutwardsPhreaticLine3;
            LeakageLengthInwardsPhreaticLine3 = properties.LeakageLengthInwardsPhreaticLine3;
            LeakageLengthOutwardsPhreaticLine4 = properties.LeakageLengthOutwardsPhreaticLine4;
            LeakageLengthInwardsPhreaticLine4 = properties.LeakageLengthInwardsPhreaticLine4;
            PiezometricHeadPhreaticLine2Outwards = properties.PiezometricHeadPhreaticLine2Outwards;
            PiezometricHeadPhreaticLine2Inwards = properties.PiezometricHeadPhreaticLine2Inwards;
            PenetrationLength = properties.PenetrationLength;
            AdjustPhreaticLine3And4ForUplift = properties.AdjustPhreaticLine3And4ForUplift;
            DikeSoilScenario = properties.DikeSoilScenario;
        }

        /// <summary>
        /// Container for properties for constructing a <see cref="WaternetCalculatorInput"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Creates new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                WaternetCreationMode = WaternetCreationMode.CreateWaternet;
                PlLineCreationMethod = PlLineCreationMethod.RingtoetsWti2017;
                AssessmentLevel = double.NaN;
                LandwardDirection = LandwardDirection.PositiveX;
                WaterLevelRiverAverage = double.NaN;
                WaterLevelPolder = double.NaN;
                MinimumLevelPhreaticLineAtDikeTopRiver = double.NaN;
                MinimumLevelPhreaticLineAtDikeTopPolder = double.NaN;
                LeakageLengthOutwardsPhreaticLine3 = double.NaN;
                LeakageLengthInwardsPhreaticLine3 = double.NaN;
                LeakageLengthOutwardsPhreaticLine4 = double.NaN;
                LeakageLengthInwardsPhreaticLine4 = double.NaN;
                PiezometricHeadPhreaticLine2Outwards = double.NaN;
                PiezometricHeadPhreaticLine2Inwards = double.NaN;
                PenetrationLength = double.NaN;
                DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay;
            }

            #region Properties

            /// <summary>
            /// Gets or sets the waternet creation mode.
            /// </summary>
            public WaternetCreationMode WaternetCreationMode { internal get; set; }

            /// <summary>
            /// Gets or sets the pl line creation method.
            /// </summary>
            public PlLineCreationMethod PlLineCreationMethod { internal get; set; }

            /// <summary>
            /// Gets or sets the outside high water level.
            /// [m+NAP]
            /// </summary>
            public double AssessmentLevel { internal get; set; }

            /// <summary>
            /// Gets or sets the landward direction of <see cref="SurfaceLine"/>.
            /// </summary>
            public LandwardDirection LandwardDirection { internal get; set; }

            /// <summary>
            /// Gets or sets the surface line.
            /// </summary>
            public MacroStabilityInwardsSurfaceLine SurfaceLine { internal get; set; }

            /// <summary>
            /// Gets or sets the profile which contains a definition of soil layers with properties.
            /// </summary>
            public SoilProfile SoilProfile { internal get; set; }

            /// <summary>
            /// Gets or sets the drainage construction.
            /// </summary>
            public DrainageConstruction DrainageConstruction { internal get; set; }

            /// <summary>
            /// Gets or sets the phreatic line offsets.
            /// </summary>
            public PhreaticLineOffsets PhreaticLineOffsets { internal get; set; }

            /// <summary>
            /// Gets or sets the dike soil scenario.
            /// </summary>
            public MacroStabilityInwardsDikeSoilScenario DikeSoilScenario { internal get; set; }

            /// <summary>
            /// Gets or sets the average river water level.
            /// [m+NAP]
            /// </summary>
            public double WaterLevelRiverAverage { internal get; set; }

            /// <summary>
            /// Gets or sets the polder water level.
            /// [m+NAP]
            /// </summary>
            public double WaterLevelPolder { internal get; set; }

            /// <summary>
            /// Gets or sets the minimum level phreatic line at dike top river.
            /// [m+NAP]
            /// </summary>
            public double MinimumLevelPhreaticLineAtDikeTopRiver { internal get; set; }

            /// <summary>
            /// Gets or sets the minimum level phreatic line at dike top polder.
            /// [m+NAP]
            /// </summary>
            public double MinimumLevelPhreaticLineAtDikeTopPolder { internal get; set; }

            /// <summary>
            /// Gets or sets whether phreatic line 3 and 4 should be adjusted for Uplift.
            /// </summary>
            public bool AdjustPhreaticLine3And4ForUplift { internal get; set; }

            /// <summary>
            /// Gets or sets the leakage length outwards of phreatic line 3.
            /// [m]
            /// </summary>
            public double LeakageLengthOutwardsPhreaticLine3 { internal get; set; }

            /// <summary>
            /// Gets or sets the leakage length inwards of phreatic line 3.
            /// [m]
            /// </summary>
            public double LeakageLengthInwardsPhreaticLine3 { internal get; set; }

            /// <summary>
            /// Gets or sets the leakage length outwards of phreatic line 4.
            /// [m]
            /// </summary>
            public double LeakageLengthOutwardsPhreaticLine4 { internal get; set; }

            /// <summary>
            /// Gets or sets the leakage length inwards of phreatic line 4.
            /// [m]
            /// </summary>
            public double LeakageLengthInwardsPhreaticLine4 { internal get; set; }

            /// <summary>
            /// Gets or sets the piezometric head of the phreatic line 2 outwards.
            /// [m+NAP]
            /// </summary>
            public double PiezometricHeadPhreaticLine2Outwards { internal get; set; }

            /// <summary>
            /// Gets or sets the piezometric head of the phreatic line 2 inwards.
            /// [m+NAP]
            /// </summary>
            public double PiezometricHeadPhreaticLine2Inwards { internal get; set; }

            /// <summary>
            /// Gets or sets the penetration length.
            /// [m]
            /// </summary>
            public double PenetrationLength { internal get; set; }

            #endregion
        }

        #region Properties

        /// <summary>
        /// Gets the waternet creation mode.
        /// </summary>
        public WaternetCreationMode WaternetCreationMode { get; }

        /// <summary>
        /// Gets the pl line creation method.
        /// </summary>
        public PlLineCreationMethod PlLineCreationMethod { get; }

        /// <summary>
        /// Gets the outside high water level.
        /// [m+NAP]
        /// </summary>
        public double AssessmentLevel { get; }

        /// <summary>
        /// Gets the landward direction of <see cref="SurfaceLine"/>.
        /// </summary>
        public LandwardDirection LandwardDirection { get; }

        /// <summary>
        /// Gets the surface line.
        /// </summary>
        public MacroStabilityInwardsSurfaceLine SurfaceLine { get; }

        /// <summary>
        /// Gets the profile which contains a definition of soil layers with properties.
        /// </summary>
        public SoilProfile SoilProfile { get; }

        /// <summary>
        /// Gets the drainage construction.
        /// </summary>
        public DrainageConstruction DrainageConstruction { get; }

        /// <summary>
        /// Gets the phreatic line offsets.
        /// </summary>
        public PhreaticLineOffsets PhreaticLineOffsets { get; }

        /// <summary>
        /// Gets the dike soil scenario.
        /// </summary>
        public MacroStabilityInwardsDikeSoilScenario DikeSoilScenario { get; }

        /// <summary>
        /// Gets the average river water level.
        /// [m+NAP]
        /// </summary>
        public double WaterLevelRiverAverage { get; }

        /// <summary>
        /// Gets the polder water level.
        /// [m+NAP]
        /// </summary>
        public double WaterLevelPolder { get; }

        /// <summary>
        /// Gets the minimum level phreatic line at dike top river.
        /// [m+NAP]
        /// </summary>
        public double MinimumLevelPhreaticLineAtDikeTopRiver { get; }

        /// <summary>
        /// Gets the minimum level phreatic line at dike top polder.
        /// [m+NAP]
        /// </summary>
        public double MinimumLevelPhreaticLineAtDikeTopPolder { get; }

        /// <summary>
        /// Gets whether phreatic line 3 and 4 should be adjusted for Uplift.
        /// </summary>
        public bool AdjustPhreaticLine3And4ForUplift { get; }

        /// <summary>
        /// Gets the leakage length outwards of phreatic line 3.
        /// [m]
        /// </summary>
        public double LeakageLengthOutwardsPhreaticLine3 { get; }

        /// <summary>
        /// Gets the leakage length inwards of phreatic line 3.
        /// [m]
        /// </summary>
        public double LeakageLengthInwardsPhreaticLine3 { get; }

        /// <summary>
        /// Gets the leakage length outwards of phreatic line 4.
        /// [m]
        /// </summary>
        public double LeakageLengthOutwardsPhreaticLine4 { get; }

        /// <summary>
        /// Gets the leakage length inwards of phreatic line 4.
        /// [m]
        /// </summary>
        public double LeakageLengthInwardsPhreaticLine4 { get; }

        /// <summary>
        /// Gets the piezometric head of the phreatic line 2 outwards.
        /// [m+NAP]
        /// </summary>
        public double PiezometricHeadPhreaticLine2Outwards { get; }

        /// <summary>
        /// Gets the piezometric head of the phreatic line 2 inwards.
        /// [m+NAP]
        /// </summary>
        public double PiezometricHeadPhreaticLine2Inwards { get; }

        /// <summary>
        /// Gets the penetration length.
        /// [m]
        /// </summary>
        public double PenetrationLength { get; }

        #endregion
    }
}