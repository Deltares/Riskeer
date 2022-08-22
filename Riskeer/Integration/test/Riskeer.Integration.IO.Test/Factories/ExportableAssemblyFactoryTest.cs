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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Factories;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableAssemblyFactoryTest
    {
        [Test]
        public void CreateExportableAssembly_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportableAssemblyFactory.CreateExportableAssembly(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableAssembly_WithValidAssessmentSection_ReturnsExpectedAssembly()
        {
            // Setup
            const string id = "assessmentSectionId";

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Id = id
            };
            ReferenceLineTestFactory.SetReferenceLineGeometry(assessmentSection.ReferenceLine);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                ExportableAssembly result = ExportableAssemblyFactory.CreateExportableAssembly(assessmentSection);

                // Assert
                Assert.AreEqual("assemblage.0", result.Id);
                AssertExportableAssessmentSection(assessmentSection, result.AssessmentSection);
                AssertExportableAssessmentProcess(result.AssessmentProcess);
            }
        }

        private static void AssertExportableAssessmentSection(
            AssessmentSection assessmentSection, ExportableAssessmentSection exportableAssessmentSection)
        {
            Assert.AreEqual(assessmentSection.Name, exportableAssessmentSection.Name);
            Assert.AreEqual($"Wks.{assessmentSection.Id}", exportableAssessmentSection.Id);
            CollectionAssert.AreEqual(assessmentSection.ReferenceLine.Points, exportableAssessmentSection.Geometry);

            int nrOfFailureMechanisms = assessmentSection.GetFailureMechanisms()
                                                         .Concat(assessmentSection.SpecificFailureMechanisms.Select(fm => fm))
                                                         .Count(fm => fm.InAssembly);
            Assert.AreEqual(nrOfFailureMechanisms, exportableAssessmentSection.FailureMechanisms.Count());
            Assert.AreEqual(1, exportableAssessmentSection.CombinedSectionAssemblies.Count());
        }

        private static void AssertExportableAssessmentProcess(ExportableAssessmentProcess exportableAssessmentProcess)
        {
            Assert.AreEqual("Bp.0", exportableAssessmentProcess.Id);
            Assert.AreEqual(2023, exportableAssessmentProcess.StartYear);
            Assert.AreEqual(2035, exportableAssessmentProcess.EndYear);
        }
    }
}