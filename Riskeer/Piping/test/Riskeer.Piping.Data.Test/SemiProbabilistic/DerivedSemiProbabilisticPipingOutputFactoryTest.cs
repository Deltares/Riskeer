// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Data.Test.SemiProbabilistic
{
    [TestFixture]
    public class DerivedSemiProbabilisticPipingOutputFactoryTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => DerivedSemiProbabilisticPipingOutputFactory.Create(null, 0.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Create_ValidData_ReturnsExpectedValue()
        {
            // Setup
            var calculatorResult = new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties
            {
                UpliftFactorOfSafety = 1.2,
                HeaveFactorOfSafety = 1.4,
                SellmeijerFactorOfSafety = 0.9
            });

            // Call
            DerivedSemiProbabilisticPipingOutput derivedOutput = DerivedSemiProbabilisticPipingOutputFactory.Create(calculatorResult, 0.1);

            // Assert
            Assert.AreEqual(0.0030333773290253025, derivedOutput.UpliftProbability, 1e-6);
            Assert.AreEqual(0.00017624686431291146, derivedOutput.HeaveProbability, 1e-6);
            Assert.AreEqual(0.13596896289025881, derivedOutput.SellmeijerProbability, 1e-6);
            Assert.AreEqual(3.57331, derivedOutput.PipingReliability, derivedOutput.PipingReliability.GetAccuracy());
        }
    }
}