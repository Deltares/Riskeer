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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.Revetment.Data;

namespace Riskeer.StabilityStoneCover.Data.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsOutputFactoryTest
    {
        [Test]
        public void CreateOutputWithBlocks_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithBlocks(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("blocks", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithBlocks_WithBlocksOutput_ReturnsOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> blocksOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            StabilityStoneCoverWaveConditionsOutput output = StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithBlocks(blocksOutput);

            // Assert
            Assert.AreSame(blocksOutput, output.BlocksOutput);
            Assert.IsNull(output.ColumnsOutput);
        }

        [Test]
        public void CreateOutputWithColumns_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithColumns(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("columns", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithColumns_WithColumnsOutput_ReturnsOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> columnsOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            StabilityStoneCoverWaveConditionsOutput output = StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithColumns(columnsOutput);

            // Assert
            Assert.AreSame(columnsOutput, output.ColumnsOutput);
            Assert.IsNull(output.BlocksOutput);
        }

        [Test]
        public void CreateOutputWithColumnsAndBlocks_ColumnsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithColumnsAndBlocks(null, Enumerable.Empty<WaveConditionsOutput>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("columns", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithColumnsAndBlocks_BlocksNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithColumnsAndBlocks(Enumerable.Empty<WaveConditionsOutput>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("blocks", exception.ParamName);
        }

        [Test]
        public void CreateOutputWithColumnsAndBlocks_WithOutput_ReturnsOutput()
        {
            // Setup
            IEnumerable<WaveConditionsOutput> columnsOutput = Enumerable.Empty<WaveConditionsOutput>();
            IEnumerable<WaveConditionsOutput> blocksOutput = Enumerable.Empty<WaveConditionsOutput>();

            // Call
            StabilityStoneCoverWaveConditionsOutput output = StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithColumnsAndBlocks(columnsOutput, blocksOutput);

            // Assert
            Assert.AreSame(columnsOutput, output.ColumnsOutput);
            Assert.AreSame(blocksOutput, output.BlocksOutput);
        }
    }
}