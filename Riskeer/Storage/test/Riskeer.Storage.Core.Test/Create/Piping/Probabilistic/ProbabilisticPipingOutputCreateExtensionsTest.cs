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
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Storage.Core.Create.Piping.Probabilistic;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.TestUtil.IllustrationPoints;

namespace Riskeer.Storage.Core.Test.Create.Piping.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingOutputCreateExtensionsTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((ProbabilisticPipingOutput) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Create_CalculationWithOutput_ReturnEntityWithOutput()
        {
            // Setup
            var output = new ProbabilisticPipingOutput(
                PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null),
                PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(null));

            // Call
            ProbabilisticPipingCalculationOutputEntity entity = output.Create();

            // Assert
            Assert.AreEqual(output.ProfileSpecificOutput.Reliability, entity.ProfileSpecificReliability);
            Assert.AreEqual(output.SectionSpecificOutput.Reliability, entity.SectionSpecificReliability);
            Assert.IsNull(entity.GeneralResultFaultTreeIllustrationPointEntity);
            Assert.IsNull(entity.GeneralResultFaultTreeIllustrationPointEntity1);
        }

        [Test]
        public void Create_CalculationWithNaNOutput_ReturnEntityWithNullOutput()
        {
            // Setup
            var output = new ProbabilisticPipingOutput(
                new PartialProbabilisticPipingOutput<TopLevelFaultTreeIllustrationPoint>(double.NaN, null),
                new PartialProbabilisticPipingOutput<TopLevelFaultTreeIllustrationPoint>(double.NaN, null));

            // Call
            ProbabilisticPipingCalculationOutputEntity entity = output.Create();

            // Assert
            Assert.IsNull(entity.ProfileSpecificReliability);
            Assert.IsNull(entity.SectionSpecificReliability);
            Assert.IsNull(entity.GeneralResultFaultTreeIllustrationPointEntity);
            Assert.IsNull(entity.GeneralResultFaultTreeIllustrationPointEntity1);
        }

        [Test]
        public void Create_CalculationWithOutputAndGeneralResult_ReturnEntityWithOutputAndGeneralResult()
        {
            // Setup
            var output = new ProbabilisticPipingOutput(
                PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput(),
                PipingTestDataGenerator.GetRandomPartialProbabilisticPipingOutput());

            // Call
            ProbabilisticPipingCalculationOutputEntity entity = output.Create();

            // Assert
            Assert.AreEqual(output.ProfileSpecificOutput.Reliability, entity.ProfileSpecificReliability);
            Assert.AreEqual(output.SectionSpecificOutput.Reliability, entity.SectionSpecificReliability);
            GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(((PartialProbabilisticPipingOutput<TopLevelFaultTreeIllustrationPoint>)output.ProfileSpecificOutput).GeneralResult,
                                                                            entity.GeneralResultFaultTreeIllustrationPointEntity);
            GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(((PartialProbabilisticPipingOutput<TopLevelFaultTreeIllustrationPoint>)output.SectionSpecificOutput).GeneralResult,
                                                                            entity.GeneralResultFaultTreeIllustrationPointEntity1);
        }
    }
}