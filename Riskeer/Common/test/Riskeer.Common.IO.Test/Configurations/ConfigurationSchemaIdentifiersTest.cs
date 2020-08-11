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

using NUnit.Framework;
using Riskeer.Common.IO.Configurations;

namespace Riskeer.Common.IO.Test.Configurations
{
    [TestFixture]
    public class ConfigurationSchemaIdentifiersTest
    {
        [Test]
        public void ConfigurationSchemaIdentifiers_ExpectedValues()
        {
            Assert.AreEqual("configuratie", ConfigurationSchemaIdentifiers.ConfigurationElement);
            Assert.AreEqual("versie", ConfigurationSchemaIdentifiers.VersionAttribute);
            Assert.AreEqual("berekening", ConfigurationSchemaIdentifiers.CalculationElement);
            Assert.AreEqual("map", ConfigurationSchemaIdentifiers.FolderElement);
            Assert.AreEqual("naam", ConfigurationSchemaIdentifiers.NameAttribute);
            Assert.AreEqual("hblocatie", ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement);
            Assert.AreEqual("orientatie", ConfigurationSchemaIdentifiers.Orientation);
            Assert.AreEqual("illustratiepunteninlezen", ConfigurationSchemaIdentifiers.ShouldIllustrationPointsBeCalculatedElement);

            Assert.AreEqual("stochasten", ConfigurationSchemaIdentifiers.StochastsElement);
            Assert.AreEqual("stochast", ConfigurationSchemaIdentifiers.StochastElement);
            Assert.AreEqual("verwachtingswaarde", ConfigurationSchemaIdentifiers.MeanElement);
            Assert.AreEqual("standaardafwijking", ConfigurationSchemaIdentifiers.StandardDeviationElement);
            Assert.AreEqual("variatiecoefficient", ConfigurationSchemaIdentifiers.VariationCoefficientElement);
            Assert.AreEqual("peilverhogingkomberging", ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName);
            Assert.AreEqual("kritiekinstromenddebiet", ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName);
            Assert.AreEqual("modelfactoroverloopdebiet", ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName);
            Assert.AreEqual("breedtebodembescherming", ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName);
            Assert.AreEqual("kombergendoppervlak", ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName);
            Assert.AreEqual("stormduur", ConfigurationSchemaIdentifiers.StormDurationStochastName);
            Assert.AreEqual("breedtedoorstroomopening", ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName);

            Assert.AreEqual("golfreductie", ConfigurationSchemaIdentifiers.WaveReduction);
            Assert.AreEqual("damgebruiken", ConfigurationSchemaIdentifiers.UseBreakWater);
            Assert.AreEqual("damtype", ConfigurationSchemaIdentifiers.BreakWaterType);
            Assert.AreEqual("damhoogte", ConfigurationSchemaIdentifiers.BreakWaterHeight);
            Assert.AreEqual("voorlandgebruiken", ConfigurationSchemaIdentifiers.UseForeshore);
            Assert.AreEqual("caisson", ConfigurationSchemaIdentifiers.BreakWaterCaisson);
            Assert.AreEqual("havendam", ConfigurationSchemaIdentifiers.BreakWaterDam);
            Assert.AreEqual("verticalewand", ConfigurationSchemaIdentifiers.BreakWaterWall);

            Assert.AreEqual("faalkansgegevenerosiebodem", ConfigurationSchemaIdentifiers.FailureProbabilityStructureWithErosionElement);
            Assert.AreEqual("kunstwerk", ConfigurationSchemaIdentifiers.StructureElement);
            Assert.AreEqual("voorlandprofiel", ConfigurationSchemaIdentifiers.ForeshoreProfileNameElement);

            Assert.AreEqual("scenario", ConfigurationSchemaIdentifiers.ScenarioElement);
            Assert.AreEqual("bijdrage", ConfigurationSchemaIdentifiers.ScenarioContribution);
            Assert.AreEqual("gebruik", ConfigurationSchemaIdentifiers.IsRelevantForScenario);
        }
    }
}