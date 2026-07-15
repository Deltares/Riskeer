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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using NUnit.Framework;
using NSubstitute;
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
    public class PipingFailureMechanismSectionsContextImportInfoTest
    {
        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            string name = importInfo.Name;

            // Assert
            Assert.AreEqual("Vakindeling", name);
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            string category = importInfo.Category;

            // Assert
            Assert.AreEqual("Algemeen", category);
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            Image image = importInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, image);
        }

        [Test]
        public void IsEnabled_ReferenceLineWithoutGeometry_ReturnFalse()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
        }

        [Test]
        public void IsEnabled_ReferenceLineWithGeometry_ReturnTrue()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
        }

        [Test]
        public void VerifyUpdates_NoProbabilisticCalculations_ReturnsTrue()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new SemiProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            });

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            bool updatesVerified = importInfo.VerifyUpdates(context);

            // Assert
            Assert.IsTrue(updatesVerified);
        }

        [Test]
        public void VerifyUpdates_ProbabilisticCalculationsWithoutOutput_ReturnsTrue()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new ProbabilisticPipingCalculationScenario());

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            bool updatesVerified = importInfo.VerifyUpdates(context);

            // Assert
            Assert.IsTrue(updatesVerified);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void VerifyUpdates_CalculationWithOutputs_AlwaysReturnsExpectedInquiryMessage(bool isActionConfirmed)
        {
            // Setup
            string expectedInquiryMessage = "Als u een vakindeling importeert, dan worden de resultaten van alle probabilistische piping berekeningen verwijderd." +
                                            $"{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.InquireContinuation(expectedInquiryMessage)
                         .Returns(isActionConfirmed);
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();
            var calculationWithOutput = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            bool updatesVerified = importInfo.VerifyUpdates(context);

            // Assert
            Assert.AreEqual(isActionConfirmed, updatesVerified);
        }

        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.ReferenceLine.Returns(new ReferenceLine());
            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            IFileImporter importer = importInfo.CreateFileImporter(context, "");

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionsImporter>(importer);
        }

        private static ImportInfo<PipingFailureMechanismSectionsContext> GetImportInfo(IInquiryHelper inquiryHelper)
        {
            return PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper);
        }
    }
}