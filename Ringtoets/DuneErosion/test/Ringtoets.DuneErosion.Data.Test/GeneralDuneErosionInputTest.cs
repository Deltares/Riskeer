// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.DuneErosion.Data.Test
{
    [TestFixture]
    public class GeneralDuneErosionInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var generalInput = new GeneralDuneErosionInput();

            // Assert
            Assert.AreEqual(2, generalInput.N.Value);
            Assert.AreEqual(2, generalInput.N.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(-45.75)]
        [TestCase(1.0-1e-6)]
        [TestCase(20+1e-6)]
        [TestCase(5987.234)]
        public void N_SetOutsideValidRange_ThrowArgumentOutOfRageException(double lengthEffect)
        {
            // Setup
            var generalInput = new GeneralDuneErosionInput();

            // Call
            TestDelegate call = () => generalInput.N = (RoundedDouble) lengthEffect;

            // Assert
            const string message = "De waarde voor 'N' moet in het bereik [1, 20] liggen.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void N_SetValidValue_GetNewlySetValue()
        {
            // Setup
            var generalInput = new GeneralDuneErosionInput();

            const double lengthEffect = 13.45678;

            // Call
            generalInput.N = (RoundedDouble)lengthEffect;

            // Assert
            Assert.AreEqual(2, generalInput.N.NumberOfDecimalPlaces);
            Assert.AreEqual(13.46, generalInput.N.Value);
        }
    }
}