// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.AssemblyTool.Data.Test
{
    [TestFixture]
    public class CombinedFailureMechanismSectionAssemblyResultWrapperTest
    {
        [Test]
        public void Constructor_AssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => new CombinedFailureMechanismSectionAssemblyResultWrapper(
                null, random.NextEnumValue<AssemblyMethod>(),
                random.NextEnumValue<AssemblyMethod>(),
                random.NextEnumValue<AssemblyMethod>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assemblyResults", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assemblyResults = new[]
            {
                new CombinedFailureMechanismSectionAssembly(
                    new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()),
                    Array.Empty<FailureMechanismSectionAssemblyGroup>())
            };
            var commonSectionAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var failureMechanismResultsAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var combinedSectionResultAssemblyMethod = random.NextEnumValue<AssemblyMethod>();

            // Call
            var wrapper = new CombinedFailureMechanismSectionAssemblyResultWrapper(
                assemblyResults, commonSectionAssemblyMethod, failureMechanismResultsAssemblyMethod, combinedSectionResultAssemblyMethod);

            // Assert
            Assert.AreSame(assemblyResults, wrapper.AssemblyResults);
            Assert.AreEqual(commonSectionAssemblyMethod, wrapper.CommonSectionAssemblyMethod);
            Assert.AreEqual(failureMechanismResultsAssemblyMethod, wrapper.FailureMechanismResultsAssemblyMethod);
            Assert.AreEqual(combinedSectionResultAssemblyMethod, wrapper.CombinedSectionResultAssemblyMethod);
        }
    }
}