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
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraRingCombinationType = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class CombinationTypeConverterTest
    {
        [Test]
        public void Create_InvalidEnumValue_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => CombinationTypeConverter.Create((HydraRingCombinationType) 99);

            // Assert
            const string message = "The value of argument 'hydraRingCombinationType' (99) is invalid for Enum type 'CombinationType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        public void Create_ValidHydraRingCombinationType_ReturnCombinationType()
        {
            // Setup
            var random = new Random();
            var hydraRingCombinationType = random.NextEnumValue<HydraRingCombinationType>();

            // Call
            CombinationType combinationType = CombinationTypeConverter.Create(hydraRingCombinationType);

            // Assert
            Assert.AreEqual((int) hydraRingCombinationType, (int) combinationType);
        }
    }
}