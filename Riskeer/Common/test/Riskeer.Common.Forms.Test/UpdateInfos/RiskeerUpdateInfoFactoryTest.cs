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
using System.Linq;
using Core.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.IO.FileImporters;

namespace Riskeer.Common.Forms.Test.UpdateInfos
{
    [TestFixture]
    public class RiskeerUpdateInfoFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_SectionResultUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResultOld>(null);

            // Assert
            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResultUpdateStrategy", paramName);
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithArguments_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResultOld>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResultOld>(sectionResultUpdateStrategy);

            // Assert
            Assert.AreEqual("Vakindeling", updateInfo.Name);
            Assert.AreEqual("Algemeen", updateInfo.Category);

            FileFilterGenerator fileFilterGenerator = updateInfo.FileFilterGenerator;
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(Resources.SectionsIcon, updateInfo.Image);
            Assert.IsNull(updateInfo.VerifyUpdates);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithArguments_ReturnsExpectedCreatedFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResultOld>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResultOld>(sectionResultUpdateStrategy);

            // Assert
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(new TestFailureMechanism(), assessmentSection);
            Assert.IsInstanceOf<FailureMechanismSectionsImporter>(updateInfo.CreateFileImporter(failureMechanismSectionsContext, ""));

            mocks.VerifyAll();
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithSourcePath_ReturnsIsEnabledTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResultOld>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResultOld>(sectionResultUpdateStrategy);

            // Assert
            var testFailureMechanism = new TestFailureMechanism();
            testFailureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), "path/to/sections");
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(testFailureMechanism, assessmentSection);
            Assert.IsTrue(updateInfo.IsEnabled(failureMechanismSectionsContext));
            mocks.VerifyAll();
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithSourcePath_ReturnsSourcePath()
        {
            // Setup
            var mocks = new MockRepository();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResultOld>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResultOld>(sectionResultUpdateStrategy);

            // Assert
            var testFailureMechanism = new TestFailureMechanism();
            testFailureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), "path/to/sections");
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(testFailureMechanism, assessmentSection);
            Assert.AreEqual(testFailureMechanism.FailureMechanismSectionSourcePath,
                            updateInfo.CurrentPath(failureMechanismSectionsContext));
            mocks.VerifyAll();
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithoutSourcePath_ReturnsIsEnabledFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResultOld>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResultOld>(sectionResultUpdateStrategy);

            // Assert
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(new TestFailureMechanism(), assessmentSection);
            Assert.IsFalse(updateInfo.IsEnabled(failureMechanismSectionsContext));
            mocks.VerifyAll();
        }

        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_WithoutSourcePath_ReturnsNullPath()
        {
            // Setup
            var mocks = new MockRepository();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResultOld>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResultOld>(sectionResultUpdateStrategy);

            // Assert
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(new TestFailureMechanism(), assessmentSection);
            Assert.IsNull(updateInfo.CurrentPath(failureMechanismSectionsContext));
            mocks.VerifyAll();
        }
    }
}