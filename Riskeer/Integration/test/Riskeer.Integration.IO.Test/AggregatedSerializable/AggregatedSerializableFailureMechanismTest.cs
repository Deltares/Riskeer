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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Integration.IO.AggregatedSerializable;

namespace Riskeer.Integration.IO.Test.AggregatedSerializable
{
    [TestFixture]
    public class AggregatedSerializableFailureMechanismTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanismSectionCollection = new SerializableFailureMechanismSectionCollection();
            IEnumerable<SerializableFailureMechanismSection> failureMechanismSections =
                Enumerable.Empty<SerializableFailureMechanismSection>();
            IEnumerable<SerializableFailureMechanismSectionAssembly> failureMechanismSectionAssemblyResults =
                Enumerable.Empty<SerializableFailureMechanismSectionAssembly>();

            // Call
            TestDelegate call = () => new AggregatedSerializableFailureMechanism(null,
                                                                                 failureMechanismSectionCollection,
                                                                                 failureMechanismSections,
                                                                                 failureMechanismSectionAssemblyResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new SerializableFailureMechanism();
            IEnumerable<SerializableFailureMechanismSection> failureMechanismSections =
                Enumerable.Empty<SerializableFailureMechanismSection>();
            IEnumerable<SerializableFailureMechanismSectionAssembly> failureMechanismSectionAssemblyResults =
                Enumerable.Empty<SerializableFailureMechanismSectionAssembly>();

            // Call
            TestDelegate call = () => new AggregatedSerializableFailureMechanism(failureMechanism,
                                                                                 null,
                                                                                 failureMechanismSections,
                                                                                 failureMechanismSectionAssemblyResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionCollection", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new SerializableFailureMechanism();
            var failureMechanismSectionCollection = new SerializableFailureMechanismSectionCollection();
            IEnumerable<SerializableFailureMechanismSectionAssembly> failureMechanismSectionAssemblyResults =
                Enumerable.Empty<SerializableFailureMechanismSectionAssembly>();

            // Call
            TestDelegate call = () => new AggregatedSerializableFailureMechanism(failureMechanism,
                                                                                 failureMechanismSectionCollection,
                                                                                 null,
                                                                                 failureMechanismSectionAssemblyResults);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSections", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new SerializableFailureMechanism();
            var failureMechanismSectionCollection = new SerializableFailureMechanismSectionCollection();
            IEnumerable<SerializableFailureMechanismSection> failureMechanismSections =
                Enumerable.Empty<SerializableFailureMechanismSection>();

            // Call
            TestDelegate call = () => new AggregatedSerializableFailureMechanism(failureMechanism,
                                                                                 failureMechanismSectionCollection,
                                                                                 failureMechanismSections,
                                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var failureMechanism = new SerializableFailureMechanism();
            var failureMechanismSectionCollection = new SerializableFailureMechanismSectionCollection();
            IEnumerable<SerializableFailureMechanismSection> failureMechanismSections =
                Enumerable.Empty<SerializableFailureMechanismSection>();
            IEnumerable<SerializableFailureMechanismSectionAssembly> failureMechanismSectionAssemblyResults =
                Enumerable.Empty<SerializableFailureMechanismSectionAssembly>();

            // Call
            var aggregatedFailureMechanism = new AggregatedSerializableFailureMechanism(failureMechanism,
                                                                                        failureMechanismSectionCollection,
                                                                                        failureMechanismSections,
                                                                                        failureMechanismSectionAssemblyResults);

            // Assert
            Assert.AreSame(failureMechanism, aggregatedFailureMechanism.FailureMechanism);
            Assert.AreSame(failureMechanismSectionCollection, aggregatedFailureMechanism.FailureMechanismSectionCollection);
            Assert.AreSame(failureMechanismSections, aggregatedFailureMechanism.FailureMechanismSections);
            Assert.AreSame(failureMechanismSectionAssemblyResults, aggregatedFailureMechanism.FailureMechanismSectionAssemblyResults);
        }
    }
}