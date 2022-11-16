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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.TestUtil;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class ExportableCombinedFailureMechanismSectionHelperTest
    {
        [Test]
        public void GetExportableFailureMechanismSectionAssemblyResult_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedFailureMechanismSectionHelper.GetExportableFailureMechanismSectionAssemblyResult(
                null, Enumerable.Empty<FailureMechanismSectionResult>(), ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void GetExportableFailureMechanismSectionAssemblyResult_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedFailureMechanismSectionHelper.GetExportableFailureMechanismSectionAssemblyResult(
                new ExportableModelRegistry(), null, ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void GetExportableFailureMechanismSectionAssemblyResult_ExportableCombinedFailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableCombinedFailureMechanismSectionHelper.GetExportableFailureMechanismSectionAssemblyResult(
                new ExportableModelRegistry(), Enumerable.Empty<FailureMechanismSectionResult>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("exportableCombinedFailureMechanismSection", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetSectionConfigurations))]
        public void GetExportableFailureMechanismSectionAssemblyResult_WithVaryingCombinedSection_ReturnsExpectedAssemblyResult(
            ExportableModelRegistry registry, IEnumerable<FailureMechanismSectionResult> sectionResults,
            ExportableCombinedFailureMechanismSection combinedFailureMechanismSection, ExportableFailureMechanismSectionAssemblyResult expectedResult)
        {
            // Call
            ExportableFailureMechanismSectionAssemblyResult result =
                ExportableCombinedFailureMechanismSectionHelper.GetExportableFailureMechanismSectionAssemblyResult(
                    registry, sectionResults, combinedFailureMechanismSection);

            // Assert
            Assert.AreSame(expectedResult, result);
        }

        [Test]
        public void GetExportableFailureMechanismSectionAssemblyResult_WithNoMatchingExportableFailureMechanismSection_ThrowsAssemblyFactoryException()
        {
            // Setup
            AdoptableFailureMechanismSectionResult[] sectionResults =
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult()
            };

            var registry = new ExportableModelRegistry();
            registry.Register(sectionResults[0], CreateResult(1, 2));

            ExportableCombinedFailureMechanismSection combinedFailureMechanismSection =
                ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection(3, 4);

            // Call
            void Call() => ExportableCombinedFailureMechanismSectionHelper.GetExportableFailureMechanismSectionAssemblyResult(
                registry, sectionResults, combinedFailureMechanismSection);

            // Assert
            var exception = Assert.Throws<AssemblyFactoryException>(Call);
            Assert.AreEqual($"No matching {typeof(ExportableFailureMechanismSectionAssemblyResult)} was found for the exportableCombinedFailureMechanismSection.",
                            exception.Message);
        }

        private static ExportableFailureMechanismSectionAssemblyResult CreateResult(double startDistance, double endDistance)
        {
            ExportableFailureMechanismSection exportableSection =
                ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection(startDistance, endDistance);
            return ExportableFailureMechanismSectionAssemblyResultTestFactory.Create(exportableSection, 1);
        }

        private static IEnumerable<TestCaseData> GetSectionConfigurations()
        {
            AdoptableFailureMechanismSectionResult[] sectionResults =
            {
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult(),
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult(),
                FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult()
            };

            var registry = new ExportableModelRegistry();
            registry.Register(sectionResults[0], CreateResult(1, 2));
            registry.Register(sectionResults[1], CreateResult(2, 3));
            registry.Register(sectionResults[2], CreateResult(3, 4));

            ExportableCombinedFailureMechanismSection combinedSection = ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection(2);
            yield return new TestCaseData(registry, sectionResults, combinedSection, registry.Get(sectionResults[1]))
                .SetName("CombinedSection exact overlap with section");

            combinedSection = ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection(2, 2.5);
            yield return new TestCaseData(registry, sectionResults, combinedSection, registry.Get(sectionResults[1]))
                .SetName("CombinedSection partial overlap with section at end");

            combinedSection = ExportableFailureMechanismSectionTestFactory.CreateExportableCombinedFailureMechanismSection(2.5);
            yield return new TestCaseData(registry, sectionResults, combinedSection, registry.Get(sectionResults[1]))
                .SetName("CombinedSection partial overlap with section at start");
        }
    }
}