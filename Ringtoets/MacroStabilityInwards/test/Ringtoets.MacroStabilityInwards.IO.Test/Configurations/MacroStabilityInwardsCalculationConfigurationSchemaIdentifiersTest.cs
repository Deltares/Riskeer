﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.IO.Configurations;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationSchemaIdentifiersTest
    {
        [Test]
        public void MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("toetspeil", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.AssessmentLevelElement);
            Assert.AreEqual("profielschematisatie", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SurfaceLineElement);
            Assert.AreEqual("ondergrondmodel", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement);
            Assert.AreEqual("ondergrondschematisatie", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement);

            Assert.AreEqual("dijktype", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioElement);
            Assert.AreEqual("1A", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioClayDikeOnClay);
            Assert.AreEqual("2A", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioSandDikeOnClay);
            Assert.AreEqual("1B", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioClayDikeOnSand);
            Assert.AreEqual("2B", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioSandDikeOnSand);

            Assert.AreEqual("waterspanningen", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterStressesElement);
            Assert.AreEqual("ghw", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterLevelRiverAverageElement);
            Assert.AreEqual("initielehoogte", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineMinimumLevelElement);
            Assert.AreEqual("pl1", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine1Element);
            Assert.AreEqual("pl2", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine2Element);
            Assert.AreEqual("pl3", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine3Element);
            Assert.AreEqual("pl4", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLine4Element);
            Assert.AreEqual("buitenkruin", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MinimumLevelPhreaticLineAtDikeTopRiverElement);
            Assert.AreEqual("binnenkruin", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MinimumLevelPhreaticLineAtDikeTopPolderElement);
            Assert.AreEqual("leklengtes", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LeakageLengthElement);
            Assert.AreEqual("stijghoogtes", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PiezometricHeadElement);
            Assert.AreEqual("binnenwaarts", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineInwardsElement);
            Assert.AreEqual("buitenwaarts", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOutwardsElement);

            Assert.AreEqual("corrigeervooropbarsten", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.AdjustPhreaticLine3And4ForUpliftElement);
            Assert.AreEqual("dagelijks", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LocationInputDailyElement);
            Assert.AreEqual("extreem", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LocationInputExtremeElement);
            Assert.AreEqual("polderpeil", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.WaterLevelPolderElement);
            Assert.AreEqual("indringingslengte", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PenetrationLengthElement);
            Assert.AreEqual("offsets", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LocationInputOffsetElement);
            Assert.AreEqual("gebruikdefaults", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.UseDefaultOffsetsElement);
            Assert.AreEqual("buitenkruin", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeTopAtRiverElement);
            Assert.AreEqual("binnenkruin", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeTopAtPolderElement);
            Assert.AreEqual("insteekbinnenberm", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowShoulderBaseInsideElement);
            Assert.AreEqual("teendijkbinnenwaarts", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.PhreaticLineOffsetBelowDikeToeAtPolderElement);

            Assert.AreEqual("drainage", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DrainageConstructionElement);
            Assert.AreEqual("aanwezig", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DrainageConstructionPresentElement);
            Assert.AreEqual("x", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.XCoordinateDrainageConstructionElement);
            Assert.AreEqual("z", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZCoordinateDrainageConstructionElement);

            Assert.AreEqual("minimaleglijvlakdiepte", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumDepthElement);
            Assert.AreEqual("minimaleglijvlaklengte", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumLengthElement);
            Assert.AreEqual("maximalelamelbreedte", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MaximumSliceWidthElement);

            Assert.AreEqual("zonering", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZonesElement);
            Assert.AreEqual("bepaling", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.CreateZonesElement);

            Assert.AreEqual("grids", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridElement);
            Assert.AreEqual("verplaatsgrid", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MoveGridElement);

            Assert.AreEqual("bepaling", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeElement);
            Assert.AreEqual("automatisch", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeAutomatic);
            Assert.AreEqual("handmatig", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeManual);

            Assert.AreEqual("tangentlijnen", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineElement);
            Assert.AreEqual("bepalingtangentlijnen", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeElement);
            Assert.AreEqual("laagscheiding", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeLayerSeparated);
            Assert.AreEqual("gespecificeerd", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeSpecified);
            Assert.AreEqual("zboven", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineZTopElement);
            Assert.AreEqual("zonder", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineZBottomElement);
            Assert.AreEqual("aantal", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineNumberElement);

            Assert.AreEqual("linkergrid", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LeftGridElement);
            Assert.AreEqual("rechtergrid", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.RightGridElement);
            Assert.AreEqual("links", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXLeftElement);
            Assert.AreEqual("rechts", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXRightElement);
            Assert.AreEqual("boven", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZTopElement);
            Assert.AreEqual("onder", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZBottomElement);
            Assert.AreEqual("aantalpuntenhorizontaal", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfHorizontalPointsElement);
            Assert.AreEqual("aantalpuntenverticaal", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfVerticalPointsElement);
        }
    }
}