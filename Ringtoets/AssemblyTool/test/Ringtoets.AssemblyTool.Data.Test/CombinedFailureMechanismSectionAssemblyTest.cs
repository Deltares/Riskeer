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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.Data.Test
{
    [TestFixture]
    public class CombinedFailureMechanismSectionAssemblyTest
    {
        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new CombinedFailureMechanismSectionAssembly(null, Enumerable.Empty<FailureMechanismSectionAssemblyCategoryGroup>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var section = new CombinedAssemblyFailureMechanismSection(random.NextDouble(), random.NextDouble(),
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            // Call
            TestDelegate call = () => new CombinedFailureMechanismSectionAssembly(section, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismResults", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var section = new CombinedAssemblyFailureMechanismSection(random.NextDouble(), random.NextDouble(),
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismResults = Enumerable.Empty<FailureMechanismSectionAssemblyCategoryGroup>();

            // Call
            var assembly = new CombinedFailureMechanismSectionAssembly(section, failureMechanismResults);

            // Assert
            Assert.AreSame(section, assembly.Section);
            Assert.AreSame(failureMechanismResults, assembly.FailureMechanismResults);
        }
    }
}