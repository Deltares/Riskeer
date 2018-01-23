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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationCreateExtensionsTest
    {
        [Test]
        public void Create_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((HydraulicBoundaryLocationCalculation) null).Create();

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
            var calculation = new HydraulicBoundaryLocationCalculation
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = shouldIllustrationPointsBeCalculated
                },
                Output = null
            };

            // Call
            HydraulicLocationCalculationEntity entity = calculation.Create();

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
            var calculation = new HydraulicBoundaryLocationCalculation
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = shouldIllustrationPointsBeCalculated
                },
                Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
            };

            // Call
            HydraulicLocationCalculationEntity entity = calculation.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToByte(shouldIllustrationPointsBeCalculated), entity.ShouldIllustrationPointsBeCalculated);

            HydraulicLocationOutputEntity outputEntity = entity.HydraulicLocationOutputEntities.Single();
            AssertHydraulicLocationOutput(calculation.Output, outputEntity);
        }

        private static void AssertHydraulicLocationOutput(HydraulicBoundaryLocationOutput expectedOutput,
                                                          HydraulicLocationOutputEntity actualOutput)
        {
            AssertNullableDouble(expectedOutput.CalculatedProbability, actualOutput.CalculatedProbability);
            AssertNullableDouble(expectedOutput.CalculatedReliability, actualOutput.CalculatedReliability);
            AssertNullableDouble(expectedOutput.TargetReliability, actualOutput.TargetReliability);
            AssertNullableDouble(expectedOutput.TargetProbability, actualOutput.TargetProbability);

            if (expectedOutput.GeneralResult != null)
            {
                Assert.IsNotNull(actualOutput.GeneralResultSubMechanismIllustrationPointEntity);
            }
            else
            {
                Assert.IsNull(actualOutput.GeneralResultSubMechanismIllustrationPointEntity);
            }

            Assert.AreEqual(Convert.ToByte(expectedOutput.CalculationConvergence), actualOutput.CalculationConvergence);
        }

        private static void AssertNullableDouble(double expectedDouble, double? actualDouble)
        {
            if (double.IsNaN(expectedDouble))
            {
                Assert.IsNull(actualDouble);
            }
            else
            {
                Assert.AreEqual(expectedDouble, actualDouble);
            }
        }
    }
}