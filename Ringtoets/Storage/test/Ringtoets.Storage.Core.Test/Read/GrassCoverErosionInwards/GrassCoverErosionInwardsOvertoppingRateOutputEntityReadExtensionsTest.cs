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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Storage.Core.TestUtil.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.GrassCoverErosionInwards;

namespace Ringtoets.Storage.Core.Test.Read.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsOvertoppingRateOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((GrassCoverErosionInwardsOvertoppingRateOutputEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidParameters_ReturnsOvertoppingRateOutput()
        {
            // Setup
            var random = new Random(22);
            double overtoppingRate = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();
            var entity = new GrassCoverErosionInwardsOvertoppingRateOutputEntity
            {
                OvertoppingRate = overtoppingRate,
                TargetProbability = targetProbability,
                TargetReliability = targetReliability,
                CalculatedProbability = calculatedProbability,
                CalculatedReliability = calculatedReliability,
                CalculationConvergence = Convert.ToByte(convergence)
            };

            // Call
            OvertoppingRateOutput output = entity.Read();

            // Assert
            Assert.AreEqual(overtoppingRate, output.OvertoppingRate, output.OvertoppingRate.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);

            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void Read_NullParameters_ReturnsOvertoppingRateOutputWithNaN()
        {
            // Setup
            var random = new Random(22);
            var convergence = random.NextEnumValue<CalculationConvergence>();
            var entity = new GrassCoverErosionInwardsOvertoppingRateOutputEntity
            {
                CalculationConvergence = Convert.ToByte(convergence)
            };

            // Call
            OvertoppingRateOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.OvertoppingRate);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(convergence, output.CalculationConvergence);

            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void Read_ValidEntityWithGeneralResultEntity_ReturnsOvertoppingRateOutputWithGeneralResult()
        {
            // Setup
            var random = new Random(22);
            var generalResultEntity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = "SSE",
                GoverningWindDirectionAngle = random.NextDouble()
            };

            var convergence = random.NextEnumValue<CalculationConvergence>();
            var entity = new GrassCoverErosionInwardsOvertoppingRateOutputEntity
            {
                CalculationConvergence = Convert.ToByte(convergence),
                GeneralResultFaultTreeIllustrationPointEntity = generalResultEntity
            };

            // Call
            OvertoppingRateOutput output = entity.Read();

            // Assert
            GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(output.GeneralResult,
                                                                            generalResultEntity);
        }
    }
}