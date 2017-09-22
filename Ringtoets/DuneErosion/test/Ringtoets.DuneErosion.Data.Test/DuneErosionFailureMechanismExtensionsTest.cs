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

namespace Ringtoets.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneErosionFailureMechanismExtensionsTest
    {
        [Test]
        public void GetMechanismSpecificNorm_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneErosionFailureMechanismExtensions.GetMechanismSpecificNorm(null, 0.5);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        [TestCase(0, 0.005, 0)]
        [TestCase(10, 0.005, 0.0005375)]
        [TestCase(10, 0.01, 0.001075)]
        public void GetMechanismSpecificNorm_WithValidData_ReturnMechanismSpecificNorm(double contribution, double norm, double expectedNorm)
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = contribution
            };

            // Call
            double mechanismSpecificNorm = failureMechanism.GetMechanismSpecificNorm(norm);

            // Assert
            Assert.AreEqual(expectedNorm, mechanismSpecificNorm);
        }
    }
}