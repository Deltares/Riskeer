﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using GeneralResult = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.GeneralResult;
using HydraGeneralResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;
using HydraWindDirection = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;
using HydraWindStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using WindDirection = Ringtoets.Common.Data.Hydraulics.IllustrationPoints.WindDirection;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultConverterTest
    {
        [Test]
        public void CreateGeneralResult_HydraGeneralResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GeneralResultConverter.CreateGeneralResult(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraGeneralResult", paramName);
        }

        [Test]
        public void CreateGeneralResult_HydraGoverningWindNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraGeneralResult = new HydraGeneralResult
            {
                Beta = 0,
                GoverningWind = new HydraWindDirection(),
                Stochasts = new List<HydraWindStochast>()
            };

            // Call
            TestDelegate call = () => GeneralResultConverter.CreateGeneralResult(hydraGeneralResult);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("name", paramName);
        }

        [Test]
        public void CreateGeneralResult_ValidArgumentsWithoutIllustrationPoints_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraGoverningWindDirection = new HydraWindDirection
            {
                Angle = random.GetFromRange(0.0, 360.0),
                Name = "Name"
            };

            var hydraGeneralResult = new HydraGeneralResult
            {
                Beta = random.NextDouble(),
                GoverningWind = hydraGoverningWindDirection,
                Stochasts = new List<HydraWindStochast>(),
                IllustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>()
            };

            // Call
            GeneralResult generalResult = GeneralResultConverter.CreateGeneralResult(hydraGeneralResult);

            // Assert
            AssertWindDirection(hydraGoverningWindDirection, generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.WindDirectionClosingSituationIllustrationPoints);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
        }

        [Test]
        public void CreateGeneralResult_GeneralResultWithSubmechanismIllustrationPointsOnly_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);

            const string closingSituation = "Closing situation";
            var hydraWindDirection = new HydraWindDirection
            {
                Angle = random.NextDouble(),
                Name = "SSE"
            };
            var hydraWindDirectionClosingSituation =
                new WindDirectionClosingSituation(hydraWindDirection, closingSituation);

            var hydraIllustrationPoint = new HydraSubMechanismIllustrationPoint
            {
                Beta = random.NextDouble(),
                Name = " IllustrationPoint"
            };
            var hydraIllustrationTreeNode = new IllustrationPointTreeNode(hydraIllustrationPoint);

            var governingHydraWindDirection = new HydraWindDirection
            {
                Angle = random.GetFromRange(0.0, 360.0),
                Name = "Name"
            };
            var hydraGeneralResult = new HydraGeneralResult
            {
                Beta = random.NextDouble(),
                GoverningWind = governingHydraWindDirection,
                Stochasts = new List<HydraWindStochast>(),
                IllustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
                {
                    {
                        hydraWindDirectionClosingSituation, hydraIllustrationTreeNode
                    }
                }
            };

            // Call
            GeneralResult generalResult = GeneralResultConverter.CreateGeneralResult(hydraGeneralResult);

            // Assert
            WindDirection generalResultGoverningWindDirection = generalResult.GoverningWindDirection;
            AssertWindDirection(governingHydraWindDirection, generalResultGoverningWindDirection);

            CollectionAssert.IsEmpty(generalResult.Stochasts);

            WindDirectionClosingSituationIllustrationPoint combination = generalResult.WindDirectionClosingSituationIllustrationPoints.Single();
            AssertWindDirection(hydraWindDirection, combination.WindDirection);
            Assert.AreEqual(closingSituation, combination.ClosingSituation);

            IllustrationPoint illustrationPoint = combination.IllustrationPoint;
            Assert.AreEqual(hydraIllustrationPoint.Name, illustrationPoint.Name);
            Assert.AreEqual(hydraIllustrationPoint.Beta, illustrationPoint.Beta, illustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(illustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(illustrationPoint.IllustrationPointResults);
        }

        [Test]
        public void CreateGeneralResult_GeneralResultWithFaultTreeIllustrationPointsOnly_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);

            const string closingSituation = "Closing situation";
            var hydraWindDirection = new HydraWindDirection
            {
                Angle = random.NextDouble(),
                Name = "SSE"
            };
            var hydraWindDirectionClosingSituation =
                new WindDirectionClosingSituation(hydraWindDirection, closingSituation);

            var hydraIllustrationPoint = new FaultTreeIllustrationPoint
            {
                Beta = random.NextDouble(),
                Name = " IllustrationPoint"
            };
            var hydraIllustrationTreeNode = new IllustrationPointTreeNode(hydraIllustrationPoint);

            var governingHydraWindDirection = new HydraWindDirection
            {
                Angle = random.GetFromRange(0.0, 360.0),
                Name = "Name"
            };
            var hydraGeneralResult = new HydraGeneralResult
            {
                Beta = random.NextDouble(),
                GoverningWind = governingHydraWindDirection,
                Stochasts = new List<HydraWindStochast>(),
                IllustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
                {
                    {
                        hydraWindDirectionClosingSituation, hydraIllustrationTreeNode
                    }
                }
            };

            // Call
            GeneralResult generalResult = GeneralResultConverter.CreateGeneralResult(hydraGeneralResult);

            // Assert
            WindDirection generalResultGoverningWindDirection = generalResult.GoverningWindDirection;
            AssertWindDirection(governingHydraWindDirection, generalResultGoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
            CollectionAssert.IsEmpty(generalResult.WindDirectionClosingSituationIllustrationPoints);
        }

        private static void AssertWindDirection(HydraWindDirection hydraWindDirection, WindDirection windDirection)
        {
            Assert.AreEqual(hydraWindDirection.Name, windDirection.Name);
            Assert.AreEqual(hydraWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
        }
    }
}