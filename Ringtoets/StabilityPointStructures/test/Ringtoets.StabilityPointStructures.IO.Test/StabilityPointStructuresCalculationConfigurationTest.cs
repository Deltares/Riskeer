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

using System;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.StabilityPointStructures.IO.Test
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StabilityPointStructuresCalculationConfiguration(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "some name";

            // Call
            var configuration = new StabilityPointStructuresCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<StructuresCalculationConfiguration>(configuration);
            Assert.AreEqual(name, configuration.Name);
            Assert.IsNull(configuration.AreaFlowApertures);
            Assert.IsNull(configuration.BankWidth);
            Assert.IsNull(configuration.ConstructiveStrengthLinearLoadModel);
            Assert.IsNull(configuration.ConstructiveStrengthQuadraticLoadModel);
            Assert.IsNull(configuration.EvaluationLevel);
            Assert.IsNull(configuration.FailureCollisionEnergy);
        }

        [Test]
        public void Properties_SetNewValues_NewValuesSet()
        {
            // Setup
            var configuration = new StabilityPointStructuresCalculationConfiguration("some name");
            var random = new Random(5432);
            var areaFlowApertures = new MeanStandardDeviationStochastConfiguration();
            var bankWidth = new MeanStandardDeviationStochastConfiguration();
            var constructiveStrengthLinearLoadModel = new MeanVariationCoefficientStochastConfiguration();
            var constructiveStrengthQuadraticLoadModel = new MeanVariationCoefficientStochastConfiguration();
            double evaluationLevel = random.NextDouble();
            var failureCollisionEnergy = new MeanVariationCoefficientStochastConfiguration();

            // Call
            configuration.AreaFlowApertures = areaFlowApertures;
            configuration.BankWidth = bankWidth;
            configuration.ConstructiveStrengthLinearLoadModel = constructiveStrengthLinearLoadModel;
            configuration.ConstructiveStrengthQuadraticLoadModel = constructiveStrengthQuadraticLoadModel;
            configuration.EvaluationLevel = evaluationLevel;
            configuration.FailureCollisionEnergy = failureCollisionEnergy;

            // Assert
            Assert.AreSame(areaFlowApertures, configuration.AreaFlowApertures);
            Assert.AreSame(bankWidth, configuration.BankWidth);
            Assert.AreSame(constructiveStrengthLinearLoadModel, configuration.ConstructiveStrengthLinearLoadModel);
            Assert.AreSame(constructiveStrengthQuadraticLoadModel, configuration.ConstructiveStrengthQuadraticLoadModel);
            Assert.AreEqual(evaluationLevel, configuration.EvaluationLevel);
            Assert.AreSame(failureCollisionEnergy, configuration.FailureCollisionEnergy);
        }
    }
}