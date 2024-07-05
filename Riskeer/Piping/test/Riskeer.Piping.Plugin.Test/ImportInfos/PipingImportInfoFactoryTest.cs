// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Util;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Plugin.ImportInfos;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class PipingImportInfoFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSectionsImportInfo_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismSectionsImportInfo_WithArguments_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper);

            // Assert
            Assert.AreEqual("Vakindeling", importInfo.Name);
            Assert.AreEqual("Algemeen", importInfo.Category);

            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, importInfo.Image);
            Assert.IsNotNull(importInfo.VerifyUpdates);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateFailureMechanismSectionsImportInfo_WithArguments_ReturnsExpectedCreatedFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            // Call
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper);

            // Assert
            var failureMechanismSectionsContext = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            Assert.IsInstanceOf<FailureMechanismSectionsImporter>(importInfo.CreateFileImporter(failureMechanismSectionsContext, ""));

            mocks.VerifyAll();
        }

        [Test]
        public void GivenImportInfoWithReferenceLineWithoutGeometry_WhenIsEnabled_ThenReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper);

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenImportInfoWithReferenceLineWithGeometry_WhenIsEnabled_ThenReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            mocks.ReplayAll();

            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper);

            // Call
            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenImportInfoWithoutProbabilisticCalculations_WhenVerifyUpdates_ThenReturnsTrue()
        {
            // Given
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new SemiProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            });

            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper);

            // When
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            bool updatesVerified = importInfo.VerifyUpdates(context);

            // Then
            Assert.IsTrue(updatesVerified);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenImportInfoWithProbabilisticCalculationWithoutOutput_WhenVerifyUpdates_ThenReturnsTrue()
        {
            // Given
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new ProbabilisticPipingCalculationScenario());

            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper);

            // When
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            bool updatesVerified = importInfo.VerifyUpdates(context);

            // Then
            Assert.IsTrue(updatesVerified);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenImportInfoWithProbabilisticCalculationWithOutput_WhenVerifyUpdates_ThenInquiryMessageSetAndExpectedActionConfirmed(bool isActionConfirmed)
        {
            // Given
            string expectedInquiryMessage = "Als u een vakindeling importeert, dan worden de resultaten van alle probabilistische piping berekeningen verwijderd." +
                                            $"{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.InquireContinuation(expectedInquiryMessage))
                         .Return(isActionConfirmed);
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculationWithOutput = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);

            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper);

            // When
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            bool updatesVerified = importInfo.VerifyUpdates(context);

            // Then
            Assert.AreEqual(isActionConfirmed, updatesVerified);
            mocks.VerifyAll();
        }
    }
}