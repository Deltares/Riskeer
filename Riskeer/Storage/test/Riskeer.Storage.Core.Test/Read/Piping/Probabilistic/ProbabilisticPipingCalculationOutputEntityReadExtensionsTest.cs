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
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.Piping.Probabilistic;
using Riskeer.Storage.Core.TestUtil.IllustrationPoints;

namespace Riskeer.Storage.Core.Test.Read.Piping.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingCalculationOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((ProbabilisticPipingCalculationOutputEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntity_ReturnProbabilisticPipingOutput()
        {
            // Setup
            var random = new Random(21);
            var profileSpecificGeneralResultEntity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = "SSE",
                GoverningWindDirectionAngle = random.NextDouble()
            };
            var sectionSpecificGeneralResultEntity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = "SSW",
                GoverningWindDirectionAngle = random.NextDouble()
            };

            var entity = new ProbabilisticPipingCalculationOutputEntity
            {
                ProfileSpecificReliability = random.NextDouble(),
                SectionSpecificReliability = random.NextDouble(),
                GeneralResultFaultTreeIllustrationPointEntity = profileSpecificGeneralResultEntity,
                GeneralResultFaultTreeIllustrationPointEntity1 = sectionSpecificGeneralResultEntity
            };

            // Call
            ProbabilisticPipingOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.ProfileSpecificReliability, output.ProfileSpecificOutput.Reliability);
            Assert.AreEqual(entity.SectionSpecificReliability, output.SectionSpecificOutput.Reliability);

            GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(
                ((PartialProbabilisticFaultTreePipingOutput) output.ProfileSpecificOutput).GeneralResult, profileSpecificGeneralResultEntity);
            GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(
                ((PartialProbabilisticFaultTreePipingOutput) output.SectionSpecificOutput).GeneralResult, sectionSpecificGeneralResultEntity);
        }

        [Test]
        public void Read_ValidEntityWithNullParameterValues_ReturnProbabilisticPipingOutput()
        {
            // Setup
            var entity = new ProbabilisticPipingCalculationOutputEntity
            {
                ProfileSpecificReliability = null,
                SectionSpecificReliability = null,
                GeneralResultFaultTreeIllustrationPointEntity = null,
                GeneralResultFaultTreeIllustrationPointEntity1 = null
            };

            // Call
            ProbabilisticPipingOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.ProfileSpecificOutput.Reliability);
            Assert.IsNull(((PartialProbabilisticPipingOutput<TopLevelFaultTreeIllustrationPoint>) output.ProfileSpecificOutput).GeneralResult);
            Assert.IsNaN(output.SectionSpecificOutput.Reliability);
            Assert.IsNull(((PartialProbabilisticPipingOutput<TopLevelFaultTreeIllustrationPoint>) output.ProfileSpecificOutput).GeneralResult);
        }
    }
}