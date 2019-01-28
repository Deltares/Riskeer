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

using System;
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.WaveImpactAsphaltCover.Data.TestUtil;

namespace Ringtoets.WaveImpactAsphaltCover.Data.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsOutputTest
    {
        [Test]
        public void Constructor_OutputItemsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveImpactAsphaltCoverWaveConditionsOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("items", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var outputItems = new[]
            {
                new TestWaveConditionsOutput()
            };

            // Call
            var output = new WaveImpactAsphaltCoverWaveConditionsOutput(outputItems);

            // Assert
            Assert.IsInstanceOf<CloneableObservable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);

            Assert.AreSame(outputItems, output.Items);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsOutput original = WaveImpactAsphaltCoverTestDataGenerator.GetRandomWaveImpactAsphaltCoverWaveConditionsOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, WaveImpactAsphaltCoverCloneAssert.AreClones);
        }
    }
}