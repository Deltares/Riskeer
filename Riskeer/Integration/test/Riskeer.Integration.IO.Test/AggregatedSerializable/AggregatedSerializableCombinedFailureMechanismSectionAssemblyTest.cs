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
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Integration.IO.AggregatedSerializable;

namespace Riskeer.Integration.IO.Test.AggregatedSerializable
{
    [TestFixture]
    public class AggregatedSerializableCombinedFailureMechanismSectionAssemblyTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AggregatedSerializableCombinedFailureMechanismSectionAssembly(null,
                                                                                                        new SerializableCombinedFailureMechanismSectionAssembly());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSection", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedFailureMechanismSectionAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AggregatedSerializableCombinedFailureMechanismSectionAssembly(new SerializableFailureMechanismSection(),
                                                                                                        null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedFailureMechanismSectionAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var failureMechanismSection = new SerializableFailureMechanismSection();
            var sectionAssemblyResult = new SerializableCombinedFailureMechanismSectionAssembly();

            // Call
            var aggregate = new AggregatedSerializableCombinedFailureMechanismSectionAssembly(failureMechanismSection,
                                                                                              sectionAssemblyResult);

            // Assert
            Assert.AreSame(failureMechanismSection, aggregate.FailureMechanismSection);
            Assert.AreSame(sectionAssemblyResult, aggregate.CombinedFailureMechanismSectionAssembly);
        }
    }
}