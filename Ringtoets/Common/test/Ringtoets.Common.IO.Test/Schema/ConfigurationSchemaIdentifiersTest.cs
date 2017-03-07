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
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Test.Schema
{
    [TestFixture]
    public class ConfigurationSchemaIdentifiersTest
    {
        [Test]
        public void ConfigurationSchemaIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("configuratie", ConfigurationSchemaIdentifiers.ConfigurationElement);
            Assert.AreEqual("berekening", ConfigurationSchemaIdentifiers.CalculationElement);
            Assert.AreEqual("map", ConfigurationSchemaIdentifiers.FolderElement);
            Assert.AreEqual("naam", ConfigurationSchemaIdentifiers.NameAttribute);
            Assert.AreEqual("hrlocatie", ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement);
            Assert.AreEqual("stochasten", ConfigurationSchemaIdentifiers.StochastsElement);
            Assert.AreEqual("stochast", ConfigurationSchemaIdentifiers.StochastElement);
            Assert.AreEqual("verwachtingswaarde", ConfigurationSchemaIdentifiers.MeanElement);
            Assert.AreEqual("standaardafwijking", ConfigurationSchemaIdentifiers.StandardDeviationElement);
        }
    }
}