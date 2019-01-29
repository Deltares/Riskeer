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

using System;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class HydraulicLocationCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            TestDelegate call = () => ((HydraulicLocationCalculationEntity) null).Read(calculation);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_HydraulicBoundaryLocationCalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new HydraulicLocationCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Read_CalculationWithoutOutput_HydraulicBoundaryLocationCalculationWithExpectedValues()
        {
            // Setup
            HydraulicLocationCalculationEntity entity = CreateCalculationEntity(21);

            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            entity.Read(calculation);

            // Assert
            AssertHydraulicBoundaryLocationCalculation(entity, calculation);
        }

        [Test]
        public void Read_CalculationWithOutputWithoutIllustrationPoints_HydraulicBoundaryLocationCalculationWithExpectedValues()
        {
            // Setup
            HydraulicLocationCalculationEntity entity = CreateCalculationEntityWithOutputEntity(21);

            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            entity.Read(calculation);

            // Assert
            AssertHydraulicBoundaryLocationCalculation(entity, calculation);
        }

        [Test]
        public void Read_CalculationWithOutputAndIllustrationPoints_HydraulicBoundaryLocationCalculationWithExpectedValues()
        {
            // Setup
            HydraulicLocationCalculationEntity entity = CreateCalculationEntityWithOutputAndGeneralResultEntity(21);

            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            entity.Read(calculation);

            // Assert
            AssertHydraulicBoundaryLocationCalculation(entity, calculation);
        }

        private static HydraulicLocationCalculationEntity CreateCalculationEntity(int seed)
        {
            var random = new Random(seed);

            return new HydraulicLocationCalculationEntity
            {
                ShouldIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean())
            };
        }

        private static HydraulicLocationOutputEntity CreateHydraulicOutputEntity(int seed)
        {
            var random = new Random(seed);
            var hydraulicLocationOutputEntity = new HydraulicLocationOutputEntity
            {
                Result = random.NextDouble(),
                TargetProbability = random.NextDouble(),
                TargetReliability = random.NextDouble(),
                CalculatedProbability = random.NextDouble(),
                CalculatedReliability = random.NextDouble(),
                CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated)
            };
            return hydraulicLocationOutputEntity;
        }

        private static HydraulicLocationCalculationEntity CreateCalculationEntityWithOutputEntity(int seed)
        {
            HydraulicLocationCalculationEntity calculationEntity = CreateCalculationEntity(seed);
            calculationEntity.HydraulicLocationOutputEntities.Add(CreateHydraulicOutputEntity(seed));

            return calculationEntity;
        }

        private static HydraulicLocationCalculationEntity CreateCalculationEntityWithOutputAndGeneralResultEntity(int seed)
        {
            var random = new Random(seed);
            var generalResultEntity = new GeneralResultSubMechanismIllustrationPointEntity
            {
                GoverningWindDirectionName = "A wind direction",
                GoverningWindDirectionAngle = random.NextDouble()
            };

            HydraulicLocationOutputEntity hydraulicLocationOutputEntity = CreateHydraulicOutputEntity(seed);
            hydraulicLocationOutputEntity.GeneralResultSubMechanismIllustrationPointEntity = generalResultEntity;

            HydraulicLocationCalculationEntity calculationEntity = CreateCalculationEntity(seed);
            calculationEntity.HydraulicLocationOutputEntities.Add(hydraulicLocationOutputEntity);

            return calculationEntity;
        }

        private static void AssertHydraulicBoundaryLocationCalculation(HydraulicLocationCalculationEntity expected,
                                                                       HydraulicBoundaryLocationCalculation actual)
        {
            Assert.AreEqual(Convert.ToBoolean(expected.ShouldIllustrationPointsBeCalculated),
                            actual.InputParameters.ShouldIllustrationPointsBeCalculated);

            AssertHydraulicBoundaryLocationCalculationOutput(expected.HydraulicLocationOutputEntities.SingleOrDefault(),
                                                             actual.Output);
        }

        private static void AssertHydraulicBoundaryLocationCalculationOutput(HydraulicLocationOutputEntity expected, HydraulicBoundaryLocationCalculationOutput actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.IsNotNull(expected.Result);
            Assert.AreEqual((RoundedDouble) expected.Result, actual.Result, actual.Result.GetAccuracy());
            Assert.IsNotNull(expected.TargetReliability);
            Assert.AreEqual((RoundedDouble) expected.TargetReliability, actual.TargetReliability, actual.TargetReliability.GetAccuracy());
            Assert.IsNotNull(expected.TargetProbability);
            Assert.AreEqual(expected.TargetProbability, actual.TargetProbability);
            Assert.IsNotNull(expected.CalculatedReliability);
            Assert.AreEqual((RoundedDouble) expected.CalculatedReliability, actual.CalculatedReliability, actual.CalculatedReliability.GetAccuracy());
            Assert.IsNotNull(expected.CalculatedProbability);
            Assert.AreEqual(expected.CalculatedProbability, actual.CalculatedProbability);
            Assert.AreEqual((CalculationConvergence) expected.CalculationConvergence, actual.CalculationConvergence);

            AssertGeneralResult(expected.GeneralResultSubMechanismIllustrationPointEntity, actual.GeneralResult);
        }

        private static void AssertGeneralResult(GeneralResultSubMechanismIllustrationPointEntity expected,
                                                GeneralResult<TopLevelSubMechanismIllustrationPoint> illustrationPoint)
        {
            if (expected == null)
            {
                Assert.IsNull(illustrationPoint);
                return;
            }

            WindDirection actualGoverningWindDirection = illustrationPoint.GoverningWindDirection;
            Assert.AreEqual(expected.GoverningWindDirectionName, actualGoverningWindDirection.Name);
            Assert.AreEqual(expected.GoverningWindDirectionAngle, actualGoverningWindDirection.Angle,
                            actualGoverningWindDirection.Angle.GetAccuracy());

            Assert.AreEqual(expected.TopLevelSubMechanismIllustrationPointEntities.Count,
                            illustrationPoint.TopLevelIllustrationPoints.Count());
            Assert.AreEqual(expected.StochastEntities.Count, illustrationPoint.Stochasts.Count());
        }
    }
}