// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.StabilityPointStructures.IO.Configurations;

namespace Riskeer.StabilityPointStructures.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityPointStructuresConfigurationSchemaIdentifiersTest
    {
        [Test]
        public void Properties_Always_ExpectedValues()
        {
            // Assert
            Assert.AreEqual("doorstroomoppervlak", StabilityPointStructuresConfigurationSchemaIdentifiers.AreaFlowAperturesStochastName);
            Assert.AreEqual("bermbreedte", StabilityPointStructuresConfigurationSchemaIdentifiers.BankWidthStochastName);
            Assert.AreEqual("lineairebelastingschematiseringsterkte", StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthLinearLoadModelStochastName);
            Assert.AreEqual("kwadratischebelastingschematiseringsterkte", StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthQuadraticLoadModelStochastName);
            Assert.AreEqual("afvoercoefficient", StabilityPointStructuresConfigurationSchemaIdentifiers.DrainCoefficientStochastName);
            Assert.AreEqual("analysehoogte", StabilityPointStructuresConfigurationSchemaIdentifiers.EvaluationLevelElement);
            Assert.AreEqual("factorstormduur", StabilityPointStructuresConfigurationSchemaIdentifiers.FactorStormDurationOpenStructureElement);
            Assert.AreEqual("bezwijkwaardeaanvaarenergie", StabilityPointStructuresConfigurationSchemaIdentifiers.FailureCollisionEnergyStochastName);
            Assert.AreEqual("faalkansherstel", StabilityPointStructuresConfigurationSchemaIdentifiers.FailureProbabilityRepairClosureElement);
            Assert.AreEqual("kritiekestroomsnelheid", StabilityPointStructuresConfigurationSchemaIdentifiers.FlowVelocityStructureClosableStochastName);
            Assert.AreEqual("instroommodel", StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelTypeElement);
            Assert.AreEqual("lagedrempel", StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelLowSillStructure);
            Assert.AreEqual("verdronkenkoker", StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelFloodedCulvertStructure);
            Assert.AreEqual("binnenwaterstand", StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelStochastName);
            Assert.AreEqual("binnenwaterstandbijfalen", StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelFailureConstructionStochastName);
            Assert.AreEqual("kerendehoogte", StabilityPointStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName);
            Assert.AreEqual("nrnivelleringen", StabilityPointStructuresConfigurationSchemaIdentifiers.LevellingCountElement);
            Assert.AreEqual("belastingschematisering", StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationTypeElement);
            Assert.AreEqual("lineair", StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationLinearStructure);
            Assert.AreEqual("kwadratisch", StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationQuadraticStructure);
            Assert.AreEqual("kansaanvaringtweedekeermiddel", StabilityPointStructuresConfigurationSchemaIdentifiers.ProbabilityCollisionSecondaryStructureElement);
            Assert.AreEqual("massaschip", StabilityPointStructuresConfigurationSchemaIdentifiers.ShipMassStochastName);
            Assert.AreEqual("aanvaarsnelheid", StabilityPointStructuresConfigurationSchemaIdentifiers.ShipVelocityStochastName);
            Assert.AreEqual("lineairebelastingschematiseringstabiliteit", StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityLinearLoadModelStochastName);
            Assert.AreEqual("kwadratischebelastingschematiseringstabiliteit", StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityQuadraticLoadModelStochastName);
            Assert.AreEqual("drempelhoogte", StabilityPointStructuresConfigurationSchemaIdentifiers.ThresholdHeightOpenWeirStochastName);
            Assert.AreEqual("afstandonderkantwandteendijk", StabilityPointStructuresConfigurationSchemaIdentifiers.VerticalDistanceElement);
            Assert.AreEqual("volumiekgewichtwater", StabilityPointStructuresConfigurationSchemaIdentifiers.VolumicWeightWaterElement);
        }
    }
}