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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input
{
    /// <summary>
    /// This class contains all the parameters that are required to perform an Uplift Van calculation.
    /// </summary>
    public class UpliftVanCalculatorInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanCalculatorInput"/>.
        /// </summary>
        /// <param name="properties">The object containing the values for the properties 
        /// of the new <see cref="UpliftVanCalculatorInput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when one of the following properties is <c>null</c>:
        /// <list type="bullet">
        /// <item><see cref="ConstructionProperties.SurfaceLine"/>;</item>
        /// <item><see cref="ConstructionProperties.SoilProfile"/>;</item>
        /// <item><see cref="ConstructionProperties.DrainageConstruction"/>;</item>
        /// <item><see cref="ConstructionProperties.PhreaticLineOffsetsExtreme"/>;</item>
        /// <item><see cref="ConstructionProperties.PhreaticLineOffsetsDaily"/>;</item>
        /// <item><see cref="ConstructionProperties.SlipPlane"/>.</item>
        /// </list>
        /// </exception>
        public UpliftVanCalculatorInput(ConstructionProperties properties)
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
            if (properties.PhreaticLineOffsetsExtreme == null)
            {
                throw new ArgumentException("PhreaticLineOffsetsExtreme must be set.");
            }
            if (properties.PhreaticLineOffsetsDaily == null)
            {
                throw new ArgumentException("PhreaticLineOffsetsDaily must be set.");
            }
            if (properties.SlipPlane == null)
            {
                throw new ArgumentException("SlipPlane must be set.");
            }

            WaternetCreationMode = properties.WaternetCreationMode;
            PlLineCreationMethod = properties.PlLineCreationMethod;
            AssessmentLevel = properties.AssessmentLevel;
            LandwardDirection = properties.LandwardDirection;
            SurfaceLine = properties.SurfaceLine;
            SoilProfile = properties.SoilProfile;
            DrainageConstruction = properties.DrainageConstruction;
            PhreaticLineOffsetsExtreme = properties.PhreaticLineOffsetsExtreme;
            PhreaticLineOffsetsDaily = properties.PhreaticLineOffsetsDaily;
            SlipPlane = properties.SlipPlane;
            WaterLevelRiverAverage = properties.WaterLevelRiverAverage;
            WaterLevelPolderExtreme = properties.WaterLevelPolderExtreme;
            WaterLevelPolderDaily = properties.WaterLevelPolderDaily;
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
            MoveGrid = properties.MoveGrid;
            MaximumSliceWidth = properties.MaximumSliceWidth;
            CreateZones = properties.CreateZones;
            AutomaticForbiddenZones = properties.AutomaticForbiddenZones;
            SlipPlaneMinimumDepth = properties.SlipPlaneMinimumDepth;
            SlipPlaneMinimumLength = properties.SlipPlaneMinimumLength;
        }

        /// <summary>
        /// Container for properties for constructing a <see cref="UpliftVanCalculatorInput"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Creates new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                WaternetCreationMode = UpliftVanWaternetCreationMode.CreateWaternet;
                PlLineCreationMethod = UpliftVanPlLineCreationMethod.RingtoetsWti2017;
                AssessmentLevel = double.NaN;
                LandwardDirection = UpliftVanLandwardDirection.PositiveX;
                WaterLevelRiverAverage = double.NaN;
                WaterLevelPolderExtreme = double.NaN;
                WaterLevelPolderDaily = double.NaN;
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
                MaximumSliceWidth = double.NaN;
                SlipPlaneMinimumDepth = double.NaN;
                SlipPlaneMinimumLength = double.NaN;
            }

            #region Properties

            /// <summary>
            /// Gets or sets the waternet creation mode.
            /// </summary>
            public UpliftVanWaternetCreationMode WaternetCreationMode { internal get; set; }

            /// <summary>
            /// Gets or sets the pl line creation method.
            /// </summary>
            public UpliftVanPlLineCreationMethod PlLineCreationMethod { internal get; set; }

            /// <summary>
            /// Gets or sets the outside high water level.
            /// [m+NAP]
            /// </summary>
            public double AssessmentLevel { internal get; set; }

            /// <summary>
            /// Gets or sets the landward direction of <see cref="SurfaceLine"/>.
            /// </summary>
            public UpliftVanLandwardDirection LandwardDirection { internal get; set; }

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
            public UpliftVanDrainageConstruction DrainageConstruction { internal get; set; }

            /// <summary>
            /// Gets or sets the phreatic line offsets under extreme circumstances.
            /// </summary>
            public UpliftVanPhreaticLineOffsets PhreaticLineOffsetsExtreme { internal get; set; }

            /// <summary>
            /// Gets or sets the phreatic line offsets under daily circumstances.
            /// </summary>
            public UpliftVanPhreaticLineOffsets PhreaticLineOffsetsDaily { internal get; set; }

            /// <summary>
            /// Gets or sets the slip plane.
            /// </summary>
            public UpliftVanSlipPlane SlipPlane { internal get; set; }

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
            /// Gets or sets the polder water level under extreme circumstances.
            /// [m+NAP]
            /// </summary>
            public double WaterLevelPolderExtreme { internal get; set; }

            /// <summary>
            /// Gets or sets the polder water level under daily circumstances.
            /// [m+NAP]
            /// </summary>
            public double WaterLevelPolderDaily { internal get; set; }

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

            /// <summary>
            /// Gets or sets the value whether the grid should be moved.
            /// </summary>
            public bool MoveGrid { internal get; set; }

            /// <summary>
            /// Gets or sets the maximum slice width.
            /// [m]
            /// </summary>
            public double MaximumSliceWidth { internal get; set; }

            /// <summary>
            /// Gets or sets whether zones should be created.
            /// </summary>
            public bool CreateZones { internal get; set; }

            /// <summary>
            /// Gets or sets whether forbidden zones are automatically determined or not.
            /// </summary>
            public bool AutomaticForbiddenZones { internal get; set; }

            /// <summary>
            /// Gets or sets the minimum depth of the slip plane.
            /// [m]
            /// </summary>
            public double SlipPlaneMinimumDepth { internal get; set; }

            /// <summary>
            /// Gets or sets the minimum length of the slip plane.
            /// [m]
            /// </summary>
            public double SlipPlaneMinimumLength { internal get; set; }

            #endregion
        }

        #region Properties

        /// <summary>
        /// Gets the waternet creation mode.
        /// </summary>
        public UpliftVanWaternetCreationMode WaternetCreationMode { get; }

        /// <summary>
        /// Gets the pl line creation method.
        /// </summary>
        public UpliftVanPlLineCreationMethod PlLineCreationMethod { get; }

        /// <summary>
        /// Gets the outside high water level.
        /// [m+NAP]
        /// </summary>
        public double AssessmentLevel { get; }

        /// <summary>
        /// Gets the landward direction of <see cref="SurfaceLine"/>.
        /// </summary>
        public UpliftVanLandwardDirection LandwardDirection { get; }

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
        public UpliftVanDrainageConstruction DrainageConstruction { get; }

        /// <summary>
        /// Gets the phreatic line offsets under extreme circumstances.
        /// </summary>
        public UpliftVanPhreaticLineOffsets PhreaticLineOffsetsExtreme { get; }

        /// <summary>
        /// Gets the phreatic line offsets under daily circumstances.
        /// </summary>
        public UpliftVanPhreaticLineOffsets PhreaticLineOffsetsDaily { get; }

        /// <summary>
        /// Gets the slip plane.
        /// </summary>
        public UpliftVanSlipPlane SlipPlane { get; }

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
        /// Gets the polder water level under extreme circumstances.
        /// [m+NAP]
        /// </summary>
        public double WaterLevelPolderExtreme { get; }

        /// <summary>
        /// Gets the polder water level under daily circumstances.
        /// [m+NAP]
        /// </summary>
        public double WaterLevelPolderDaily { get; }

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

        /// <summary>
        /// Gets the value whether the grid should be moved.
        /// </summary>
        public bool MoveGrid { get; }

        /// <summary>
        /// Gets the maximum slice width.
        /// [m]
        /// </summary>
        public double MaximumSliceWidth { get; }

        /// <summary>
        /// Gets whether zones should be created.
        /// </summary>
        public bool CreateZones { get; }

        /// <summary>
        /// Gets whether forbidden zones are automatically determined or not.
        /// </summary>
        public bool AutomaticForbiddenZones { get; }

        /// <summary>
        /// Gets the minimum depth of the slip plane.
        /// [m]
        /// </summary>
        public double SlipPlaneMinimumDepth { get; }

        /// <summary>
        /// Gets the minimum length of the slip plane.
        /// [m]
        /// </summary>
        public double SlipPlaneMinimumLength { get; }

        #endregion
    }
}