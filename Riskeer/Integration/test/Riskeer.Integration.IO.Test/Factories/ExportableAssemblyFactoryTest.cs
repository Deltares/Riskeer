﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
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

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Id = "assessmentSectionId",
                SpecificFailureMechanisms =
                {
                    new SpecificFailureMechanism()
                }
            };
            assessmentSection.SpecificFailureMechanisms.ForEachElementDo(sfm => sfm.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections);
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm => fm.AssemblyResult.ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections);
            ReferenceLineTestFactory.SetReferenceLineGeometry(assessmentSection.ReferenceLine);
            AddFailureMechanismSections(assessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                ExportableAssembly result = ExportableAssemblyFactory.CreateExportableAssembly(assessmentSection);

                // Assert
                Assert.AreEqual("assemblage.0", result.Id);

                Assert.AreEqual(assessmentSection.Name, result.AssessmentSection.Name);
                Assert.AreEqual($"Wks.{assessmentSection.Id}", result.AssessmentSection.Id);
                CollectionAssert.AreEqual(assessmentSection.ReferenceLine.Points, result.AssessmentSection.Geometry);

                int nrOfFailureMechanisms = assessmentSection.GetFailureMechanisms()
                                                             .Concat(assessmentSection.SpecificFailureMechanisms.Select(fm => fm))
                                                             .Count();
                Assert.AreEqual(nrOfFailureMechanisms, result.AssessmentSection.FailureMechanisms.Count());

                Assert.AreEqual("Bp.0", result.AssessmentProcess.Id);
                Assert.AreEqual(2023, result.AssessmentProcess.StartYear);
                Assert.AreEqual(2035, result.AssessmentProcess.EndYear);
            }
        }

        private static void AddFailureMechanismSections(AssessmentSection assessmentSection)
        {
            var random = new Random(21);
            FailureMechanismTestHelper.AddSections(assessmentSection.Piping, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.MacroStabilityInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.HeightStructures, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.ClosingStructures, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityPointStructures, random.Next(1, 10));

            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityStoneCover, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.WaveImpactAsphaltCover, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionOutwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.DuneErosion, random.Next(1, 10));

            FailureMechanismTestHelper.AddSections(assessmentSection.Microstability, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffOutwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.PipingStructure, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.WaterPressureAsphaltCover, random.Next(1, 10));

            FailureMechanismTestHelper.AddSections(assessmentSection.SpecificFailureMechanisms.First(), random.Next(1, 10));
        }
    }
}