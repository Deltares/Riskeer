// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Core.Common.Base.Data;
using NUnit.Framework;

namespace Riskeer.GrassCoverErosionInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class TestGrassCoverErosionInwardsOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var output = new TestGrassCoverErosionInwardsOutput();

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionInwardsOutput>(output);
            Assert.IsInstanceOf<TestOvertoppingOutput>(output.OvertoppingOutput);
            Assert.IsInstanceOf<TestDikeHeightOutput>(output.DikeHeightOutput);
            Assert.AreEqual((RoundedDouble) 0, output.DikeHeightOutput.DikeHeight);
            Assert.IsInstanceOf<TestOvertoppingRateOutput>(output.OvertoppingRateOutput);
            Assert.AreEqual((RoundedDouble) 0, output.OvertoppingRateOutput.OvertoppingRate);
        }
    }
}