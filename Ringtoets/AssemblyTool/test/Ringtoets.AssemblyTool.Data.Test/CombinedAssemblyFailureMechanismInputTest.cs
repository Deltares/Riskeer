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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ringtoets.AssemblyTool.Data.Test
{
    [TestFixture]
    public class CombinedAssemblyFailureMechanismInputTest
    {
        [Test]
        public void Constructor_SectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => new CombinedAssemblyFailureMechanismInput(random.NextDouble(), random.NextDouble(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double n = random.NextDouble();
            double contribution = random.NextDouble();
            IEnumerable<CombinedAssemblyFailureMechanismSection> sections = Enumerable.Empty<CombinedAssemblyFailureMechanismSection>();

            // Call
            var input = new CombinedAssemblyFailureMechanismInput(n, contribution, sections);

            // Assert
            Assert.AreEqual(n, input.N);
            Assert.AreEqual(contribution, input.FailureMechanismContribution);
            Assert.AreSame(sections, input.Sections);
        }
    }
}