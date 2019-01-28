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

using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraRingCombinationType = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class CombinationTypeConverterTest
    {
        [Test]
        public void Convert_InvalidEnumValue_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => CombinationTypeConverter.Convert((HydraRingCombinationType) 99);

            // Assert
            const string message = "The value of argument 'hydraRingCombinationType' (99) is invalid for Enum type 'CombinationType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(HydraRingCombinationType.And, CombinationType.And)]
        [TestCase(HydraRingCombinationType.Or, CombinationType.Or)]
        public void Convert_HydraRingCombinationType_ReturnCombinationType(HydraRingCombinationType hydraRingCombinationType,
                                                                           CombinationType expectedCombinationType)
        {
            // Call
            CombinationType combinationType = CombinationTypeConverter.Convert(hydraRingCombinationType);

            // Assert
            Assert.AreEqual(expectedCombinationType, combinationType);
        }
    }
}