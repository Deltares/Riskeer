// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.IO.Model;

namespace Ringtoets.Integration.IO.Test
{
    [TestFixture]
    public class AggregatedSerializableFailureMechanismSectionAssemblyTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                new AggregatedSerializableFailureMechanismSectionAssembly(null, new SerializableFailureMechanismSectionAssembly());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSection", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                new AggregatedSerializableFailureMechanismSectionAssembly(new SerializableFailureMechanismSection(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var section = new SerializableFailureMechanismSection();
            var sectionAssemblyResult = new SerializableFailureMechanismSectionAssembly();

            // Call
            var aggregatedResult =
                new AggregatedSerializableFailureMechanismSectionAssembly(section, sectionAssemblyResult);

            // Assert
            Assert.AreSame(section, aggregatedResult.FailureMechanismSection);
            Assert.AreSame(sectionAssemblyResult, aggregatedResult.FailureMechanismSectionAssembly);
        }
    }
}