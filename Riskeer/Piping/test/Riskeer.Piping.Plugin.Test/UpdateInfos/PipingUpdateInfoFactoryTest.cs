// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Plugin.UpdateInfos;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class PipingUpdateInfoFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithArguments_ExpectedPropertiesSet()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Call
            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // Assert
            Assert.AreEqual("Vakindeling", updateInfo.Name);
            Assert.AreEqual("Algemeen", updateInfo.Category);

            FileFilterGenerator fileFilterGenerator = updateInfo.FileFilterGenerator;
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, updateInfo.Image);
            Assert.IsNotNull(updateInfo.VerifyUpdates);
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithArguments_ReturnsExpectedCreatedFileImporter()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
            // Call
            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // Assert
            var failureMechanismSectionsContext = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            Assert.IsInstanceOf<FailureMechanismSectionsImporter>(updateInfo.CreateFileImporter(failureMechanismSectionsContext, ""));
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithSourcePath_ReturnsIsEnabledTrue()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
            // Call
            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // Assert
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), "path/to/sections");
            var failureMechanismSectionsContext = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            Assert.IsTrue(updateInfo.IsEnabled(failureMechanismSectionsContext));
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithoutSourcePath_ReturnsIsEnabledFalse()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
            // Call
            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // Assert
            var failureMechanismSectionsContext = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            Assert.IsFalse(updateInfo.IsEnabled(failureMechanismSectionsContext));
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithSourcePath_ReturnsSourcePath()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
            // Call
            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // Assert
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), "path/to/sections");
            var failureMechanismSectionsContext = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            Assert.AreEqual(failureMechanism.FailureMechanismSectionSourcePath,
                            updateInfo.CurrentPath(failureMechanismSectionsContext));
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithoutSourcePath_ReturnsNullPath()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
            // Call
            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // Assert
            var failureMechanismSectionsContext = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            Assert.IsNull(updateInfo.CurrentPath(failureMechanismSectionsContext));
        }

        [Test]
        public void GivenUpdateInfoWithoutProbabilisticCalculations_WhenVerifyUpdates_ThenReturnsTrue()
        {
            // Given
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new SemiProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            });

            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // When
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            bool updatesVerified = updateInfo.VerifyUpdates(context);

            // Then
            Assert.IsTrue(updatesVerified);
        }

        [Test]
        public void GivenUpdateInfoWithProbabilisticCalculationWithoutOutput_WhenVerifyUpdates_ThenReturnsTrue()
        {
            // Given
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new ProbabilisticPipingCalculationScenario());

            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // When
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            bool updatesVerified = updateInfo.VerifyUpdates(context);

            // Then
            Assert.IsTrue(updatesVerified);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenUpdateInfoWithProbabilisticCalculationWithOutput_WhenVerifyUpdates_ThenInquiryMessageSetAndExpectedActionConfirmed(
            bool isActionConfirmed)
        {
            // Given
            string expectedInquiryMessage = "Als u de vakindeling wijzigt, dan worden de resultaten van alle probabilistische piping berekeningen verwijderd." +
                                            $"{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.InquireContinuation(expectedInquiryMessage).Returns(isActionConfirmed);
            var assessmentSection = Substitute.For<IAssessmentSection>();
            TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            var calculationWithOutput = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);

            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // When
            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            bool updatesVerified = updateInfo.VerifyUpdates(context);

            // Then
            Assert.AreEqual(isActionConfirmed, updatesVerified);
        }
    }
}