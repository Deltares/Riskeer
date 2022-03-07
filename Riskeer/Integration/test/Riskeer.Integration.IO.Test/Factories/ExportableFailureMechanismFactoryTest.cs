﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;
using Riskeer.Integration.IO.TestUtil;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableFailureMechanismFactoryTest
    {
        [Test]
        public void CreateDefaultExportableFailureMechanismWithProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            var random = new Random(21);
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var failureMechanismAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            void Call() => ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithProbability(
                null, failureMechanismCode, failureMechanismAssemblyMethod);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateDefaultExportableFailureMechanismWithProbability_WithValidArguments_ReturnsExportableFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var failureMechanismAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism =
                ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithProbability(assessmentSection,
                                                                                                         failureMechanismCode,
                                                                                                         failureMechanismAssemblyMethod);

            // Assert
            ExportableFailureMechanismTestHelper.AssertDefaultFailureMechanismWithProbability(assessmentSection.ReferenceLine.Points,
                                                                                              failureMechanismCode,
                                                                                              failureMechanismAssemblyMethod,
                                                                                              exportableFailureMechanism);
        }

        [Test]
        public void CreateDefaultExportableFailureMechanismWithoutProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            var random = new Random(21);
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var failureMechanismAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            TestDelegate call = () => ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithoutProbability(null,
                                                                                                                                  failureMechanismCode,
                                                                                                                                  failureMechanismAssemblyMethod);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateDefaultExportableFailureMechanismWithoutProbability_WithValidArguments_ReturnsExportableFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanismCode = random.NextEnumValue<ExportableFailureMechanismType>();
            var failureMechanismAssemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();

            // Call
            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableFailureMechanism =
                ExportableFailureMechanismFactory.CreateDefaultExportableFailureMechanismWithoutProbability(assessmentSection,
                                                                                                            failureMechanismCode,
                                                                                                            failureMechanismAssemblyMethod);

            // Assert
            ExportableFailureMechanismTestHelper.AssertDefaultFailureMechanismWithoutProbability(assessmentSection.ReferenceLine.Points,
                                                                                                 failureMechanismCode,
                                                                                                 failureMechanismAssemblyMethod,
                                                                                                 exportableFailureMechanism);
        }
    }
}