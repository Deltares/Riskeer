﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.StabilityStoneCover.Data.Test
{
    [TestFixture]
    public class GeneralStabilityStoneCoverWaveConditionsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var generalInput = new GeneralStabilityStoneCoverWaveConditionsInput();

            // Assert
            const double aBlocks = 1.0;
            const double bBlocks = 1.0;
            const double cBlocks = 1.0;

            const double aColumns = 1.0;
            const double bColumns = 0.4;
            const double cColumns = 0.8;
            Assert.AreEqual(aBlocks, generalInput.GeneralBlocksWaveConditionsInput.A, generalInput.GeneralBlocksWaveConditionsInput.A.GetAccuracy());
            Assert.AreEqual(bBlocks, generalInput.GeneralBlocksWaveConditionsInput.B, generalInput.GeneralBlocksWaveConditionsInput.B.GetAccuracy());
            Assert.AreEqual(cBlocks, generalInput.GeneralBlocksWaveConditionsInput.C, generalInput.GeneralBlocksWaveConditionsInput.C.GetAccuracy());
            Assert.AreEqual(aColumns, generalInput.GeneralColumnsWaveConditionsInput.A, generalInput.GeneralColumnsWaveConditionsInput.A.GetAccuracy());
            Assert.AreEqual(bColumns, generalInput.GeneralColumnsWaveConditionsInput.B, generalInput.GeneralColumnsWaveConditionsInput.B.GetAccuracy());
            Assert.AreEqual(cColumns, generalInput.GeneralColumnsWaveConditionsInput.C, generalInput.GeneralColumnsWaveConditionsInput.C.GetAccuracy());
        }
    }
}