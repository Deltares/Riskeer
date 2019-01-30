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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.TestUtil.IllustrationPoints;

namespace Riskeer.Storage.Core.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultEntityTestHelperTest
    {
        [Test]
        public void AssertGeneralResultPropertyValues_WithGeneralResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(null,
                                                                                                      new GeneralResultFaultTreeIllustrationPointEntity());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("generalResult", exception.ParamName);
        }

        [Test]
        public void AssertGeneralResultPropertyValues_GeneralResultFaultTreeIllustrationPointEntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(new TestGeneralResultFaultTreeIllustrationPoint(),
                                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("generalResultEntity", exception.ParamName);
        }

        [Test]
        public void AssertGeneralResultPropertyValues_GeneralResultAndEntityHaveMatchingPropertyValues_DoesNotThrowException()
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
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(generalResult, generalResultEntity);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource(nameof(GetDifferentGeneralResultProperties))]
        public void AssertGeneralResultPropertyValues_DifferentPropertyValues_ThrowsAssertionException(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult,
                                                                                                       GeneralResultFaultTreeIllustrationPointEntity generalResultEntity)
        {
            // Call
            TestDelegate call = () => GeneralResultEntityTestHelper.AssertGeneralResultPropertyValues(generalResult, generalResultEntity);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        private static IEnumerable<TestCaseData> GetDifferentGeneralResultProperties()
        {
            var governingWindDirection = new WindDirection("SSE", 50);
            var generalResult = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(governingWindDirection,
                                                                                      Enumerable.Empty<Stochast>(),
                                                                                      Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>());

            GeneralResultFaultTreeIllustrationPointEntity generalResultEntityWrongStochastCount =
                GetBaseLineGeneralResultEntity(governingWindDirection);
            generalResultEntityWrongStochastCount.StochastEntities.Add(new StochastEntity());
            yield return new TestCaseData(generalResult, generalResultEntityWrongStochastCount)
                .SetName("DoesNotMatchStochastCount");

            GeneralResultFaultTreeIllustrationPointEntity generalResultEntityWrongTopLevelCount =
                GetBaseLineGeneralResultEntity(governingWindDirection);
            generalResultEntityWrongTopLevelCount.TopLevelFaultTreeIllustrationPointEntities.Add(new TopLevelFaultTreeIllustrationPointEntity());
            yield return new TestCaseData(generalResult, generalResultEntityWrongTopLevelCount)
                .SetName("DoesNotMatchTopLevelIllustrationPointCount");

            GeneralResultFaultTreeIllustrationPointEntity generalResultEntityWrongName = GetBaseLineGeneralResultEntity(governingWindDirection);
            generalResultEntityWrongName.GoverningWindDirectionName = "totally different";
            yield return new TestCaseData(generalResult, generalResultEntityWrongName)
                .SetName("DoesNotMatchGoverningWindDirectionName");

            GeneralResultFaultTreeIllustrationPointEntity generalResultEntityWrongAngle = GetBaseLineGeneralResultEntity(governingWindDirection);
            generalResultEntityWrongAngle.GoverningWindDirectionAngle = 60;
            yield return new TestCaseData(generalResult, generalResultEntityWrongAngle)
                .SetName("DoesNotMatchGoverningWindDirectionAngle");
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
    }
}