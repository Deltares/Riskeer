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
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.Integration.IO.AggregatedSerializable;

namespace Ringtoets.Integration.IO.Test.AggregatedSerializable
{
    [TestFixture]
    public class AggregatedSerializableCombinedFailureMechanismSectionAssembliesTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AggregatedSerializableCombinedFailureMechanismSectionAssemblies(null,
                                                                                                          Enumerable.Empty<SerializableFailureMechanismSection>(),
                                                                                                          Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionCollection", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AggregatedSerializableCombinedFailureMechanismSectionAssemblies(new SerializableFailureMechanismSectionCollection(),
                                                                                                          null,
                                                                                                          Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSections", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedFailureMechanismSectionAssembliesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AggregatedSerializableCombinedFailureMechanismSectionAssemblies(new SerializableFailureMechanismSectionCollection(),
                                                                                                          Enumerable.Empty<SerializableFailureMechanismSection>(),
                                                                                                          null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedFailureMechanismSectionAssemblies", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var failureMechanismSectionCollection = new SerializableFailureMechanismSectionCollection();
            IEnumerable<SerializableFailureMechanismSection> failureMechanismSections =
                Enumerable.Empty<SerializableFailureMechanismSection>();
            IEnumerable<SerializableCombinedFailureMechanismSectionAssembly> failureMechanismSectionAssemblies =
                Enumerable.Empty<SerializableCombinedFailureMechanismSectionAssembly>();

            // Call
            var aggregate = new AggregatedSerializableCombinedFailureMechanismSectionAssemblies(failureMechanismSectionCollection,
                                                                                                failureMechanismSections,
                                                                                                failureMechanismSectionAssemblies);

            // Assert
            Assert.AreSame(failureMechanismSectionCollection, aggregate.FailureMechanismSectionCollection);
            Assert.AreSame(failureMechanismSections, aggregate.FailureMechanismSections);
            Assert.AreSame(failureMechanismSectionAssemblies, aggregate.CombinedFailureMechanismSectionAssemblies);
        }
    }
}