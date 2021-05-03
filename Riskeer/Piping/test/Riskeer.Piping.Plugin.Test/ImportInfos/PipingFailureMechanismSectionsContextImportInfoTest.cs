// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    public class PipingFailureMechanismSectionsContextImportInfoTest
    {
        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            string name = importInfo.Name;

            // Assert
            Assert.AreEqual("Vakindeling", name);
            mocks.VerifyAll();
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            string category = importInfo.Category;

            // Assert
            Assert.AreEqual("Algemeen", category);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            Image image = importInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, image);
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_ReferenceLineWithoutGeometry_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_ReferenceLineWithGeometry_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            mocks.ReplayAll();

            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;

            // Assert
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
            mocks.VerifyAll();
        }

        [Test]
        public void VerifyUpdates_NoProbabilisticCalculations_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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
            mocks.VerifyAll();
        }

        [Test]
        public void VerifyUpdates_ProbabilisticCalculationsWithoutOutput_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new ProbabilisticPipingCalculationScenario());

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            bool updatesVerified = importInfo.VerifyUpdates(context);

            // Assert
            Assert.IsTrue(updatesVerified);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void VerifyUpdates_CalculationWithOutputs_AlwaysReturnsExpectedInquiryMessage(bool isActionConfirmed)
        {
            // Setup
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

            var context = new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            bool updatesVerified = importInfo.VerifyUpdates(context);

            // Assert
            Assert.AreEqual(isActionConfirmed, updatesVerified);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var context = new PipingFailureMechanismSectionsContext(new PipingFailureMechanism(), assessmentSection);
            ImportInfo<PipingFailureMechanismSectionsContext> importInfo = GetImportInfo(inquiryHelper);

            // Call
            IFileImporter importer = importInfo.CreateFileImporter(context, "");

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionsImporter>(importer);
            mocks.VerifyAll();
        }

        private static ImportInfo<PipingFailureMechanismSectionsContext> GetImportInfo(IInquiryHelper inquiryHelper)
        {
            return PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper);
        }
    }
}