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

using NUnit.Framework;

namespace Ringtoets.StabilityStoneCover.Data.Test
{
    [TestFixture]
    public class GeneralStabilityStoneCoverWaveConditionsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            double aBlocks = 1.0;
            double bBlocks = 1.0;
            double cBlocks = 1.0;

            double aColumns = 1.0;
            double bColumns = 0.4;
            double cColumns = 0.8;

            // Call
            var generalInput = new GeneralStabilityStoneCoverWaveConditionsInput();

            // Assert
            Assert.AreEqual(aBlocks, generalInput.ABlocks);
            Assert.AreEqual(bBlocks, generalInput.BBlocks);
            Assert.AreEqual(cBlocks, generalInput.CBlocks);
            Assert.AreEqual(aColumns, generalInput.AColumns);
            Assert.AreEqual(bColumns, generalInput.BColumns);
            Assert.AreEqual(cColumns, generalInput.CColumns);
        }
    }
}