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
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.TestUtil;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAggregatedFailureMechanismSectionAssemblyResultTest
    {
        [Test]
        public void Constructor_SimpleAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("simpleAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_DetailedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("detailedAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_TailorMadeAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                null,
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("tailorMadeAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableAggregatedFailureMechanismSectionAssemblyResult(
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedAssembly", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            ExportableFailureMechanismSection failureMechanismSection = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            ExportableSectionAssemblyResult simpleAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();
            ExportableSectionAssemblyResult detailedAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();
            ExportableSectionAssemblyResult tailorMadeAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();
            ExportableSectionAssemblyResult combinedAssembly = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();

            // Call
            var assemblyResult = new ExportableAggregatedFailureMechanismSectionAssemblyResult(failureMechanismSection,
                                                                                               simpleAssembly,
                                                                                               detailedAssembly,
                                                                                               tailorMadeAssembly,
                                                                                               combinedAssembly);

            // Assert
            Assert.IsInstanceOf<ExportableAggregatedFailureMechanismSectionAssemblyResultBase>(assemblyResult);

            Assert.AreSame(failureMechanismSection, assemblyResult.FailureMechanismSection);
            Assert.AreSame(simpleAssembly, assemblyResult.SimpleAssembly);
            Assert.AreSame(detailedAssembly, assemblyResult.DetailedAssembly);
            Assert.AreSame(tailorMadeAssembly, assemblyResult.TailorMadeAssembly);
            Assert.AreSame(combinedAssembly, assemblyResult.CombinedAssembly);
        }
    }
}