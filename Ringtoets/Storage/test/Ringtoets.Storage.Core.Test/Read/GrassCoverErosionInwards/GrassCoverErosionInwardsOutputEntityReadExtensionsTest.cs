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
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Storage.Core.TestUtil.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.GrassCoverErosionInwards;

namespace Ringtoets.Storage.Core.Test.Read.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((GrassCoverErosionInwardsOutputEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidEntity_ReturnGrassCoverErosionInwardsOutput()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                WaveHeight = 1.1,
                IsOvertoppingDominant = Convert.ToByte(false),
                Reliability = 0.2
            };

            // Call
            GrassCoverErosionInwardsOutput output = entity.Read();

            // Assert
            OvertoppingOutput overtoppingOutput = output.OvertoppingOutput;

            Assert.AreEqual(entity.WaveHeight, overtoppingOutput.WaveHeight.Value);
            Assert.IsFalse(overtoppingOutput.IsOvertoppingDominant);
            Assert.IsNull(output.DikeHeightOutput);
            Assert.IsNull(output.OvertoppingRateOutput);
            Assert.AreEqual(entity.Reliability, overtoppingOutput.Reliability);
        }

        [Test]
        public void Read_ValidEntityWithNullValues_ReturnGrassCoverErosionInwardsOutput()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                WaveHeight = null,
                IsOvertoppingDominant = Convert.ToByte(true),
                Reliability = null,
                GeneralResultFaultTreeIllustrationPointEntity = null
            };

            // Call
            GrassCoverErosionInwardsOutput output = entity.Read();

            // Assert
            OvertoppingOutput overtoppingOutput = output.OvertoppingOutput;

            Assert.IsNaN(overtoppingOutput.WaveHeight);
            Assert.IsTrue(overtoppingOutput.IsOvertoppingDominant);
            Assert.IsNull(output.DikeHeightOutput);
            Assert.IsNull(output.OvertoppingRateOutput);
            Assert.IsNaN(overtoppingOutput.Reliability);
            Assert.IsNull(overtoppingOutput.GeneralResult);
        }

        [Test]
        public void Read_ValidEntityWithGeneralResultEntity_ReturnsGrassCoverErosionInwardsOutputWithGeneralResult()
        {
            // Setup
            var generalResultEntity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionAngle = 10,
                GoverningWindDirectionName = "SSE"
            };
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                WaveHeight = null,
                IsOvertoppingDominant = Convert.ToByte(true),
                Reliability = null,
                GeneralResultFaultTreeIllustrationPointEntity = generalResultEntity
            };

            // Call
            GrassCoverErosionInwardsOutput output = entity.Read();

            // Assert
            OvertoppingOutput overtoppingOutput = output.OvertoppingOutput;
            GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(overtoppingOutput.GeneralResult,
                                                                            generalResultEntity);
        }

        [Test]
        public void Read_ValidEntityWithDikeHeightOutputEntity_ReturnGrassCoverErosionInwardsOutputWithDikeHeightOutput()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsDikeHeightOutputEntities =
                {
                    new GrassCoverErosionInwardsDikeHeightOutputEntity()
                }
            };

            // Call
            GrassCoverErosionInwardsOutput output = entity.Read();

            // Assert
            Assert.IsNotNull(output.DikeHeightOutput);
        }

        [Test]
        public void Read_ValidEntityWithOvertoppingRateOutputEntity_ReturnGrassCoverErosionInwardsOutputWithOvertoppingRateOutput()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOvertoppingRateOutputEntities =
                {
                    new GrassCoverErosionInwardsOvertoppingRateOutputEntity()
                }
            };

            // Call
            GrassCoverErosionInwardsOutput output = entity.Read();

            // Assert
            Assert.IsNotNull(output.OvertoppingRateOutput);
        }
    }
}