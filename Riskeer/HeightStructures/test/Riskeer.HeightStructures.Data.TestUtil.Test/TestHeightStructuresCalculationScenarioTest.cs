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
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.HeightStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class TestHeightStructuresCalculationScenarioTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Call
            var calculation = new TestHeightStructuresCalculationScenario();

            // Assert
            Assert.IsInstanceOf<StructuresCalculationScenario<HeightStructuresInput>>(calculation);
            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.AreEqual(1, calculation.Contribution, calculation.Contribution.GetAccuracy());
            Assert.IsTrue(calculation.IsRelevant);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsNull(calculation.Comments.Body);
            Assert.IsFalse(calculation.HasOutput);

            Assert.IsNotNull(calculation.InputParameters.Structure);
            Assert.AreEqual(2, calculation.InputParameters.LevelCrestStructure.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(5.74, calculation.InputParameters.LevelCrestStructure.Mean.Value);
            Assert.AreEqual(2, calculation.InputParameters.StructureNormalOrientation.NumberOfDecimalPlaces);
            Assert.AreEqual(115, calculation.InputParameters.StructureNormalOrientation.Value);
            Assert.AreEqual(2, calculation.InputParameters.AllowedLevelIncreaseStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, calculation.InputParameters.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(2, calculation.InputParameters.StorageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, calculation.InputParameters.StorageStructureArea.Mean.Value);
            Assert.AreEqual(2, calculation.InputParameters.FlowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(18, calculation.InputParameters.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(2, calculation.InputParameters.CriticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1, calculation.InputParameters.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(2, calculation.InputParameters.WidthFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(18, calculation.InputParameters.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(2, calculation.InputParameters.DeviationWaveDirection.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, calculation.InputParameters.DeviationWaveDirection.Value);
            Assert.AreEqual(1.0, calculation.InputParameters.FailureProbabilityStructureWithErosion);
        }
    }
}