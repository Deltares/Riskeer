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

using System;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Revetment.Data;

namespace Ringtoets.StabilityStoneCover.Data.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsOutputTest
    {
        [Test]
        public void Constructor_ColumnsOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsOutput(null, Enumerable.Empty<WaveConditionsOutput>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("columnsOutput", exception.ParamName);
        }

        [Test]
        public void Constructor_BlocksOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("blocksOutput", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var columnsOutput = new[]
            {
                new WaveConditionsOutput(1, 0, 3, 5),
                new WaveConditionsOutput(8, 2, 6, 1)
            };

            var blocksOutput = new[]
            {
                new WaveConditionsOutput(6, 2, 9, 4),
                new WaveConditionsOutput(4, 1, 7, 3)
            };

            // Call
            var output = new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput);

            // Assert
            Assert.AreSame(columnsOutput, output.ColumnsOutput);
            Assert.AreSame(blocksOutput, output.BlocksOutput);
        }
    }
}