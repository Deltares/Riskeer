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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraGeneralResult = Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints.GeneralResult;
using HydraWindDirection = Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints.WindDirection;
using HydraWindStochast = Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints.Stochast;

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
        public void CreateGeneralResult_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraGeneralResult = new HydraGeneralResult
            {
                Beta = random.NextDouble(),
                GoverningWind = new HydraWindDirection
                {
                    Angle = random.GetFromRange(0.0, 360.0),
                    Name = "Name"
                },
                Stochasts = new List<HydraWindStochast>()
            };

            // Call
            GeneralResult generalResult = GeneralResultConverter.CreateGeneralResult(hydraGeneralResult);

            // Assert
            Assert.AreEqual(hydraGeneralResult.Beta, generalResult.Beta);
        }
    }
}