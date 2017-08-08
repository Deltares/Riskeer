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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil.IllustrationPoints;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Application.Ringtoets.Storage.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultEntityTestHelperTest
    {
        [Test]
        public void AssertGeneralResultEntity_WithNullArguments_DoesNotThrowException()
        {
            // Call
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResultEntity(null, null);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void AssertGeneralResultEntity_WithExpectedProperties_DoesNotThrowException()
        {
            // Setup
            var random = new Random(21);

            var governingWindDirection = new WindDirection("SSE", random.NextDouble());
            var stochast = new Stochast("Stochast", random.NextDouble(), random.NextDouble());
            var topLevelFaultTreeIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                            string.Empty,
                                                                                            new IllustrationPointNode(new TestFaultTreeIllustrationPoint()));

            var generalResult = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                governingWindDirection, new[]
                {
                    stochast
                }, new[]
                {
                    topLevelFaultTreeIllustrationPoint
                });

            var generalResultEntity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = governingWindDirection.Name,
                GoverningWindDirectionAngle = governingWindDirection.Angle
            };
            generalResultEntity.StochastEntities.Add(new StochastEntity());
            generalResultEntity.TopLevelFaultTreeIllustrationPointEntities.Add(new TopLevelFaultTreeIllustrationPointEntity());

            // Call
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResultEntity(generalResult, generalResultEntity);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource(nameof(GetDifferentGeneralResultEntityProperties))]
        public void AssertGeneralResultEntity_DifferentPropertyValues_ThrowsAssertionException(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult,
                                                                                               GeneralResultFaultTreeIllustrationPointEntity generalResultEntity)
        {
            // Call
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResultEntity(generalResult, generalResultEntity);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertGeneralResult_WithNullArguments_DoesNotThrowException()
        {
            // Call
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResult(null, null);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void AssertGeneralResult_WithExpectedProperties_DoesNotThrowException()
        {
            // Setup
            var random = new Random(21);

            var generalResultEntity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = "SSE",
                GoverningWindDirectionAngle = 30
            };
            generalResultEntity.StochastEntities.Add(new StochastEntity());
            generalResultEntity.TopLevelFaultTreeIllustrationPointEntities.Add(new TopLevelFaultTreeIllustrationPointEntity());

            var governingWindDirection = new WindDirection(generalResultEntity.GoverningWindDirectionName,
                                                           generalResultEntity.GoverningWindDirectionAngle);
            var stochast = new Stochast("Stochast", random.NextDouble(), random.NextDouble());
            var topLevelFaultTreeIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                            string.Empty,
                                                                                            new IllustrationPointNode(new TestFaultTreeIllustrationPoint()));

            var generalResult = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                governingWindDirection, new[]
                {
                    stochast
                }, new[]
                {
                    topLevelFaultTreeIllustrationPoint
                });

            // Call
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResult(generalResultEntity, generalResult);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource(nameof(GetDifferentGeneralResultProperties))]
        public void AssertGeneralResult_DifferentPropertyValues_ThrowsAssertionException(GeneralResultFaultTreeIllustrationPointEntity generalResultEntity,
                                                                                         GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            // Call
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResult(generalResultEntity, generalResult);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        #region TestData GeneralResult

        private static IEnumerable<TestCaseData> GetDifferentGeneralResultProperties()
        {
            var generalResultEntity = new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = "SSE",
                GoverningWindDirectionAngle = 30
            };
            var baseLineGoverningWindDirection = new WindDirection(generalResultEntity.GoverningWindDirectionName,
                                                                   generalResultEntity.GoverningWindDirectionAngle);

            yield return new TestCaseData(null, new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                                              baseLineGoverningWindDirection,
                                              Enumerable.Empty<Stochast>(),
                                              Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>()))
                .SetName("GeneralResultNotNullWhenExpectedNull");
            yield return new TestCaseData(generalResultEntity, null).SetName("GeneralResultNullWhenExpectedNotNull");

            yield return new TestCaseData(generalResultEntity,
                                          new GeneralResult<TopLevelFaultTreeIllustrationPoint>(new WindDirection("DifferentName",
                                                                                                                  baseLineGoverningWindDirection.Angle),
                                                                                                Enumerable.Empty<Stochast>(),
                                                                                                Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>()))
                .SetName("GeneralResultDoesNotMatchWindDirectionName");

            yield return new TestCaseData(generalResultEntity,
                                          new GeneralResult<TopLevelFaultTreeIllustrationPoint>(new WindDirection(baseLineGoverningWindDirection.Name,
                                                                                                                  10),
                                                                                                Enumerable.Empty<Stochast>(),
                                                                                                Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>()))
                .SetName("GeneralResultDoesNotMatchWindDirectionAngle");

            yield return new TestCaseData(generalResultEntity,
                                          new GeneralResult<TopLevelFaultTreeIllustrationPoint>(baseLineGoverningWindDirection,
                                                                                                new[]
                                                                                                {
                                                                                                    new Stochast("Wrong item", double.NaN, double.NaN)
                                                                                                }, Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>()))
                .SetName("GeneralResultDoesNotMatchStochastCount");
            yield return new TestCaseData(generalResultEntity,
                                          new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                                              baseLineGoverningWindDirection,
                                              Enumerable.Empty<Stochast>(),
                                              new[]
                                              {
                                                  new TopLevelFaultTreeIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                         string.Empty,
                                                                                         new IllustrationPointNode(new TestFaultTreeIllustrationPoint()))
                                              }))
                .SetName("GeneralResultDoesNotMatchTopLevelIllustrationPointCount");
        }

        #endregion

        #region TestData GeneralResultEntity

        private static IEnumerable<TestCaseData> GetDifferentGeneralResultEntityProperties()
        {
            var governingWindDirection = new WindDirection("SSE", 50);
            var generalResult = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(governingWindDirection,
                                                                                      Enumerable.Empty<Stochast>(),
                                                                                      Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>());

            yield return new TestCaseData(null, new GeneralResultFaultTreeIllustrationPointEntity()).SetName("EntityNotNullWhenExpectedNull");
            yield return new TestCaseData(new TestGeneralResultFaultTreeIllustrationPoint(), null).SetName("EntityNullWhenExpectedNotNull");

            GeneralResultFaultTreeIllustrationPointEntity generalResultEntityWrongStochastCount =
                GetBaseLineGeneralResultEntity(governingWindDirection);
            generalResultEntityWrongStochastCount.TopLevelFaultTreeIllustrationPointEntities.Add(new TopLevelFaultTreeIllustrationPointEntity());
            yield return new TestCaseData(generalResult, null).SetName("EntityDoesNotMatchStochastCount");

            GeneralResultFaultTreeIllustrationPointEntity generalResultEntityWrongTopLevelCount =
                GetBaseLineGeneralResultEntity(governingWindDirection);
            generalResultEntityWrongTopLevelCount.TopLevelFaultTreeIllustrationPointEntities.Add(new TopLevelFaultTreeIllustrationPointEntity());
            yield return new TestCaseData(generalResult, generalResultEntityWrongTopLevelCount).SetName("EntityDoesNotMatchTopLevelIllustrationPointCount");

            GeneralResultFaultTreeIllustrationPointEntity generalResultEntityWrongName = GetBaseLineGeneralResultEntity(governingWindDirection);
            generalResultEntityWrongName.GoverningWindDirectionName = "totally different";
            yield return new TestCaseData(generalResult, generalResultEntityWrongName).SetName("EntityDoesNotMatchGoverningWindDirectionName");

            GeneralResultFaultTreeIllustrationPointEntity generalResultEntityWrongAngle = GetBaseLineGeneralResultEntity(governingWindDirection);
            generalResultEntityWrongAngle.GoverningWindDirectionAngle = 60;
            yield return new TestCaseData(generalResult, generalResultEntityWrongAngle).SetName("EntityDoesNotMatchGoverningWindDirectionAngle");
        }

        /// <summary>
        /// Creates a <see cref="GeneralResultFaultTreeIllustrationPointEntity"/>
        /// based on the <see cref="WindDirection"/>.
        /// </summary>
        /// <param name="windDirection">The wind direction to base the 
        /// <see cref="GeneralResultFaultTreeIllustrationPointEntity"/> on.</param>
        /// <returns>A <see cref="GeneralResultFaultTreeIllustrationPointEntity"/>.</returns>
        private static GeneralResultFaultTreeIllustrationPointEntity GetBaseLineGeneralResultEntity(WindDirection windDirection)
        {
            return new GeneralResultFaultTreeIllustrationPointEntity
            {
                GoverningWindDirectionName = windDirection.Name,
                GoverningWindDirectionAngle = windDirection.Angle
            };
        }

        #endregion
    }
}