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

            Assert.AreEqual("minimaleglijvlakdiepte", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumDepthElement);
            Assert.AreEqual("minimaleglijvlaklengte", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SlipPlaneMinimumLengthElement);
            Assert.AreEqual("maximalelamelbreedte", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MaximumSliceWidthElement);

            Assert.AreEqual("zonering", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.ZonesElement);
            Assert.AreEqual("bepaling", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.CreateZones);

            Assert.AreEqual("grids", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridElement);
            Assert.AreEqual("verplaatsgrid", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.MoveGrid);

            Assert.AreEqual("bepaling", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeElement);
            Assert.AreEqual("automatisch", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeAutomatic);
            Assert.AreEqual("handmatig", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridDeterminationTypeManual);

            Assert.AreEqual("tangentlijnen", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineElement);
            Assert.AreEqual("bepalingtangentlijnen", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeElement);
            Assert.AreEqual("laagscheiding", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeLayerSeparated);
            Assert.AreEqual("gespecificeerd", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeSpecified);
            Assert.AreEqual("zboven", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineZTop);
            Assert.AreEqual("zonder", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineZBottom);
            Assert.AreEqual("aantal", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineNumber);

            Assert.AreEqual("linkergrid", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.LeftGridElement);
            Assert.AreEqual("rechtergrid", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.RightGridElement);
            Assert.AreEqual("links", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXLeft);
            Assert.AreEqual("rechts", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridXRight);
            Assert.AreEqual("boven", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZTop);
            Assert.AreEqual("onder", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridZBottom);
            Assert.AreEqual("aantalpuntenhorizontaal", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfHorizontalPoints);
            Assert.AreEqual("aantalpuntenverticaal", MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.GridNumberOfVerticalPoints);
        }
    }
}