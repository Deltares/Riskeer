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
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraIllustrationPointResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointResultConverterTest
    {
        [Test]
        public void CreateIllustrationPointResult_HydraIllustrationPointResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => IllustrationPointResultConverter.CreateIllustrationPointResult(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraIllustrationPointResult", paramName);
        }

        [Test]
        public void CreateIllustrationPointResult_ValidArguments_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraIllustrationPointResult = new HydraIllustrationPointResult("Description",
                                                                                random.NextDouble());

            // Call
            IllustrationPointResult illustrationPointResult = IllustrationPointResultConverter.CreateIllustrationPointResult(hydraIllustrationPointResult);

            // Assert
            Assert.AreEqual(illustrationPointResult.Description, illustrationPointResult.Description);
            Assert.AreEqual(illustrationPointResult.Value, illustrationPointResult.Value, illustrationPointResult.Value.GetAccuracy());
        }
    }
}