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

using NUnit.Framework;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestStructuresOutputTest
    {
        [Test]
        public void Constructor_SetExpectedValues()
        {
            // Call
            var output = new TestStructuresOutput();

            // Assert
            Assert.IsInstanceOf<StructuresOutput>(output);
            Assert.AreEqual(0, output.Reliability);
            Assert.IsNull(output.GeneralResult);
            Assert.IsFalse(output.HasGeneralResult);
        }

        [Test]
        public void GeneralResultConstructor_SetExpectedValues()
        {
            // Setup
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();

            // Call
            var output = new TestStructuresOutput(generalResult);

            // Assert
            Assert.IsInstanceOf<StructuresOutput>(output);
            Assert.AreEqual(0, output.Reliability);
            Assert.AreSame(generalResult, output.GeneralResult);
            Assert.IsTrue(output.HasGeneralResult);
        }

        [Test]
        public void ReliabilityConstructor_SetExpectedValues()
        {
            // Setup
            const double reliability = 0.5;

            // Call
            var output = new TestStructuresOutput(reliability);

            // Assert
            Assert.IsInstanceOf<StructuresOutput>(output);
            Assert.AreEqual(reliability, output.Reliability);
            Assert.IsNull(output.GeneralResult);
            Assert.IsFalse(output.HasGeneralResult);
        }
    }
}