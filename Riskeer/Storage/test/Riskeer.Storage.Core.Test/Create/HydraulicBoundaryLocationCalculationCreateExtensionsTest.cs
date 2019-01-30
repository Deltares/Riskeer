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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationCreateExtensionsTest
    {
        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            TestDelegate call = () => calculation.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((HydraulicBoundaryLocationCalculation) null).Create(new PersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Create_CalculationWithoutOutput_ReturnsHydraulicLocationCalculationEntity()
        {
            // Setup
            var random = new Random(33);
            bool shouldIllustrationPointsBeCalculated = random.NextBoolean();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = shouldIllustrationPointsBeCalculated
                },
                Output = null
            };

            var registry = new PersistenceRegistry();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            HydraulicLocationCalculationEntity entity = calculation.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToByte(shouldIllustrationPointsBeCalculated), entity.ShouldIllustrationPointsBeCalculated);
            Assert.IsEmpty(entity.HydraulicLocationOutputEntities);
        }

        [Test]
        public void Create_CalculationWithOutput_ReturnsHydraulicLocationCalculationEntityWithOutput()
        {
            // Setup
            var random = new Random(33);
            bool shouldIllustrationPointsBeCalculated = random.NextBoolean();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = shouldIllustrationPointsBeCalculated
                },
                Output = new HydraulicBoundaryLocationCalculationOutput(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble(),
                    random.NextDouble(), random.NextEnumValue<CalculationConvergence>(),
                    null)
            };

            var registry = new PersistenceRegistry();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            HydraulicLocationCalculationEntity entity = calculation.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToByte(shouldIllustrationPointsBeCalculated), entity.ShouldIllustrationPointsBeCalculated);

            HydraulicLocationOutputEntity outputEntity = entity.HydraulicLocationOutputEntities.Single();

            HydraulicBoundaryLocationCalculationOutput expectedOutput = calculation.Output;
            Assert.AreEqual(expectedOutput.CalculatedProbability, outputEntity.CalculatedProbability);
            Assert.AreEqual(expectedOutput.CalculatedReliability, outputEntity.CalculatedReliability);
            Assert.AreEqual(expectedOutput.TargetReliability, outputEntity.TargetReliability);
            Assert.AreEqual(expectedOutput.TargetProbability, outputEntity.TargetProbability);
            Assert.IsNull(outputEntity.GeneralResultSubMechanismIllustrationPointEntity);
            Assert.AreEqual(Convert.ToByte(expectedOutput.CalculationConvergence), outputEntity.CalculationConvergence);
        }

        [Test]
        public void Create_CalculationWithAlreadyCreatedHydraulicBoundaryLocation_ReturnsEntityWithHydraulicBoundaryLocationEntity()
        {
            // Setup
            var hydraulicLocation = new TestHydraulicBoundaryLocation();

            var registry = new PersistenceRegistry();
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            registry.Register(hydraulicLocationEntity, hydraulicLocation);

            var calculation = new HydraulicBoundaryLocationCalculation(hydraulicLocation);

            // Call
            HydraulicLocationCalculationEntity entity = calculation.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }
    }
}