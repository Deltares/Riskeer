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
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableCombinedSectionAssemblyCollectionTest
    {
        [Test]
        public void Constructor_SectionsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssemblyCollection(null,
                                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssemblyCollection(Enumerable.Empty<ExportableCombinedFailureMechanismSection>(),
                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            IEnumerable<ExportableCombinedFailureMechanismSection> sections = Enumerable.Empty<ExportableCombinedFailureMechanismSection>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            var assembly = new ExportableCombinedSectionAssemblyCollection(sections, combinedSectionAssemblyResults);

            // Assert
            Assert.AreSame(combinedSectionAssemblyResults, assembly.CombinedSectionAssemblyResults);
        }
    }
}