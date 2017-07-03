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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraGeneralResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;
using HydraWindDirection = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;
using HydraStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraWindDirectionClosingSituation = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraIllustrationPointTreeNode = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;
using HydraCombinationType = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;
using HydraIllustrationPointResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using HydraFaultTreeIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraSubMechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;
using HydraSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;

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
        public void CreateGeneralResult_HydraGeneralResultWithoutIllustrationPoints_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            double angle = random.NextDouble();
            var hydraGoverningWindDirection = new HydraWindDirection("Name", angle);

            var hydraGeneralResult =
                new HydraGeneralResult(random.NextDouble(),
                                       hydraGoverningWindDirection,
                                       Enumerable.Empty<HydraStochast>(),
                                       new Dictionary<
                                           HydraWindDirectionClosingSituation,
                                           HydraIllustrationPointTreeNode>());

            // Call
            GeneralResult generalResult = GeneralResultConverter.CreateGeneralResult(hydraGeneralResult);

            // Assert
            AssertWindDirection(hydraGoverningWindDirection, generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.TopLevelSubMechanismIllustrationPoints);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
        }

        [Test]
        public void CreateGeneralResult_HydraGeneralResultWithSubMechanismIllustrationPointsOnly_ExpectedProperties()
        {
            // Setup
            const string closingSituation = "Closing situation";

            var random = new Random(21);
            double windDirectionAngle = random.NextDouble();
            var hydraWindDirection = new HydraWindDirection("SSE", windDirectionAngle);
            var hydraWindDirectionClosingSituation =
                new HydraWindDirectionClosingSituation(hydraWindDirection, closingSituation);

            double beta = random.NextDouble();
            var hydraIllustrationPoint =
                new HydraSubMechanismIllustrationPoint("Illustration Point",
                                                       Enumerable.Empty<HydraSubMechanismIllustrationPointStochast>(),
                                                       Enumerable.Empty<HydraIllustrationPointResult>(),
                                                       beta);
            var hydraIllustrationTreeNode = new HydraIllustrationPointTreeNode(hydraIllustrationPoint);

            double governingWindDirectionAngle = random.NextDouble();
            var governingHydraWindDirection = new HydraWindDirection("Name", governingWindDirectionAngle);
            var hydraGeneralResult =
                new HydraGeneralResult(random.NextDouble(),
                                       governingHydraWindDirection,
                                       Enumerable.Empty<HydraStochast>(),
                                       new Dictionary<HydraWindDirectionClosingSituation, HydraIllustrationPointTreeNode>
                                       {
                                           {
                                               hydraWindDirectionClosingSituation, hydraIllustrationTreeNode
                                           }
                                       });

            // Call
            GeneralResult generalResult = GeneralResultConverter.CreateGeneralResult(hydraGeneralResult);

            // Assert
            WindDirection generalResultGoverningWindDirection = generalResult.GoverningWindDirection;
            AssertWindDirection(governingHydraWindDirection, generalResultGoverningWindDirection);

            CollectionAssert.IsEmpty(generalResult.Stochasts);

            TopLevelSubMechanismIllustrationPoint combination = generalResult.TopLevelSubMechanismIllustrationPoints.Single();
            AssertWindDirection(hydraWindDirection, combination.WindDirection);
            Assert.AreEqual(closingSituation, combination.ClosingSituation);

            SubMechanismIllustrationPoint subMechanismIllustrationPoint = combination.SubMechanismIllustrationPoint;
            Assert.AreEqual(hydraIllustrationPoint.Name, subMechanismIllustrationPoint.Name);
            Assert.AreEqual(hydraIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(subMechanismIllustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(subMechanismIllustrationPoint.IllustrationPointResults);
        }

        [Test]
        public void CreateGeneralResult_HydraGeneralResultWithFaultTreeIllustrationPointsOnly_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);

            const string closingSituation = "Closing situation";
            double windDirectionAngle = random.NextDouble();
            var hydraWindDirection = new HydraWindDirection("SSE", windDirectionAngle);
            var hydraWindDirectionClosingSituation =
                new HydraWindDirectionClosingSituation(hydraWindDirection, closingSituation);

            var hydraIllustrationPoint =
                new HydraFaultTreeIllustrationPoint(" IllustrationPoint",
                                                    random.NextDouble(),
                                                    random.NextEnumValue<HydraCombinationType>());
            var hydraIllustrationTreeNode = new HydraIllustrationPointTreeNode(hydraIllustrationPoint);

            double governingWindDirectionAngle = random.NextDouble();
            var governingHydraWindDirection = new HydraWindDirection("Name", governingWindDirectionAngle);
            var hydraGeneralResult =
                new HydraGeneralResult(random.NextDouble(),
                                       governingHydraWindDirection,
                                       Enumerable.Empty<HydraStochast>(),
                                       new Dictionary<HydraWindDirectionClosingSituation, HydraIllustrationPointTreeNode>
                                       {
                                           {
                                               hydraWindDirectionClosingSituation, hydraIllustrationTreeNode
                                           }
                                       });

            // Call
            GeneralResult generalResult = GeneralResultConverter.CreateGeneralResult(hydraGeneralResult);

            // Assert
            WindDirection generalResultGoverningWindDirection = generalResult.GoverningWindDirection;
            AssertWindDirection(governingHydraWindDirection, generalResultGoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
            CollectionAssert.IsEmpty(generalResult.TopLevelSubMechanismIllustrationPoints);
        }

        private static void AssertWindDirection(HydraWindDirection hydraWindDirection, WindDirection windDirection)
        {
            Assert.AreEqual(hydraWindDirection.Name, windDirection.Name);
            Assert.AreEqual(hydraWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
        }
    }
}