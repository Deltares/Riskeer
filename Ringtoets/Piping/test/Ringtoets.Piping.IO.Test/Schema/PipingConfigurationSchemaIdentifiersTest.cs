// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.IO.Schema;

namespace Ringtoets.Piping.IO.Test.Schema
{
    [TestFixture]
    public class PipingConfigurationSchemaIdentifiersTest
    {
        [Test]
        public void PipingConfigurationSchemaIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("root", PipingConfigurationSchemaIdentifiers.RootElement);
            Assert.AreEqual("berekening", PipingConfigurationSchemaIdentifiers.CalculationElement);
            Assert.AreEqual("folder", PipingConfigurationSchemaIdentifiers.FolderElement);
            Assert.AreEqual("naam", PipingConfigurationSchemaIdentifiers.NameAttribute);
            Assert.AreEqual("toetspeil", PipingConfigurationSchemaIdentifiers.AssessmentLevelElement);
            Assert.AreEqual("hrlocatie", PipingConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement);
            Assert.AreEqual("profielschematisatie", PipingConfigurationSchemaIdentifiers.SurfaceLineElement);
            Assert.AreEqual("intredepunt", PipingConfigurationSchemaIdentifiers.EntryPointElement);
            Assert.AreEqual("uittredepunt", PipingConfigurationSchemaIdentifiers.ExitPointElement);
            Assert.AreEqual("ondergrondmodel", PipingConfigurationSchemaIdentifiers.StochasticSoilModelElement);
            Assert.AreEqual("ondergrondschematisatie", PipingConfigurationSchemaIdentifiers.StochasticSoilProfileElement);
            Assert.AreEqual("stochast", PipingConfigurationSchemaIdentifiers.StochastElement);
            Assert.AreEqual("verwachtingswaarde", PipingConfigurationSchemaIdentifiers.MeanElement);
            Assert.AreEqual("standaardafwijking", PipingConfigurationSchemaIdentifiers.StandardDeviationElement);
            Assert.AreEqual("polderpeil", PipingConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName);
            Assert.AreEqual("dempingsfactor", PipingConfigurationSchemaIdentifiers.DampingFactorExitStochastName);
        }
    }
}