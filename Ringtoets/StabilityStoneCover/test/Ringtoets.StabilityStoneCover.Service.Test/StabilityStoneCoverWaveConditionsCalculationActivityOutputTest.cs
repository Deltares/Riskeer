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
using NUnit.Framework;
using Ringtoets.Revetment.Data;

namespace Ringtoets.StabilityStoneCover.Service.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationActivityOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var output = new StabilityStoneCoverWaveConditionsCalculationActivityOutput();
            
            // Assert
            CollectionAssert.IsEmpty(output.BlocksOutput);
            CollectionAssert.IsEmpty(output.ColumnsOutput);
        }

        [Test]
        public void AddBlocksOutput_ElementNull_ThrowsArgumentNullException()
        {
            // Setup
            var output = new StabilityStoneCoverWaveConditionsCalculationActivityOutput();

            // Call
            TestDelegate test = () => output.AddBlocksOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("element", exception.ParamName);
        }

        [Test]
        public void AddBlocksOutput_WithElement_ElementAdded()
        {
            // Setup
            var output = new StabilityStoneCoverWaveConditionsCalculationActivityOutput();
            var element = new WaveConditionsOutput(3.0, 0.3, 8.2, 29);

            // Precondition
            CollectionAssert.IsEmpty(output.BlocksOutput);

            // Call
            output.AddBlocksOutput(element);

            // Assert
            CollectionAssert.AreEqual(new[] { element }, output.BlocksOutput);
        }

        [Test]
        public void AddColumnsOutput_ElementNull_ThrowsArgumentNullException()
        {
            // Setup
            var output = new StabilityStoneCoverWaveConditionsCalculationActivityOutput();

            // Call
            TestDelegate test = () => output.AddColumnsOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("element", exception.ParamName);
        }

        [Test]
        public void AddColumnsOutput_WithElement_ElementAdded()
        {
            // Setup
            var output = new StabilityStoneCoverWaveConditionsCalculationActivityOutput();
            var element = new WaveConditionsOutput(3.0, 0.3, 8.2, 29);

            // Precondition
            CollectionAssert.IsEmpty(output.ColumnsOutput);

            // Call
            output.AddColumnsOutput(element);

            // Assert
            CollectionAssert.AreEqual(new[] { element }, output.ColumnsOutput);
        }
    }
}