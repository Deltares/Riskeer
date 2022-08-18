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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Assembly;
using Riskeer.AssemblyTool.IO.TestUtil;

namespace Riskeer.AssemblyTool.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableCombinedSectionAssemblyTest
    {
        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Setup
            ExportableFailureMechanismSection section = ExportableFailureMechanismSectionTestFactory.CreateExportableFailureMechanismSection();
            ExportableFailureMechanismSectionAssemblyResult combinedAssemblyResult = ExportableFailureMechanismSectionAssemblyResultTestFactory.Create(section, 21);

            // Call
            void Call() => new ExportableCombinedSectionAssembly(
                null, combinedAssemblyResult, Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var section = new ExportableCombinedFailureMechanismSection(
                Enumerable.Empty<Point2D>(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<ExportableAssemblyMethod>());

            // Call
            void Call() => new ExportableCombinedSectionAssembly(
                section, null, Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("combinedSectionAssemblyResult", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var section = new ExportableCombinedFailureMechanismSection(
                Enumerable.Empty<Point2D>(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<ExportableAssemblyMethod>());

            ExportableFailureMechanismSectionAssemblyResult combinedAssemblyResult = ExportableFailureMechanismSectionAssemblyResultTestFactory.Create(
                section, random.Next());

            // Call
            void Call() => new ExportableCombinedSectionAssembly(section, combinedAssemblyResult, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismResults", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var section = new ExportableCombinedFailureMechanismSection(
                Enumerable.Empty<Point2D>(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<ExportableAssemblyMethod>());

            ExportableFailureMechanismSectionAssemblyResult combinedAssemblyResult = ExportableFailureMechanismSectionAssemblyResultTestFactory.Create(
                section, random.Next());

            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults =
                Enumerable.Empty<ExportableFailureMechanismCombinedSectionAssemblyResult>();

            // Call
            var result = new ExportableCombinedSectionAssembly(section, combinedAssemblyResult, failureMechanismResults);

            // Assert
            Assert.AreSame(section, result.Section);
            Assert.AreSame(combinedAssemblyResult, result.CombinedSectionAssemblyResult);
            Assert.AreSame(failureMechanismResults, result.FailureMechanismResults);
        }
    }
}