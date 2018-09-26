// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.UpdateInfos;
using Ringtoets.Common.IO.FileImporters;

namespace Ringtoets.Common.Forms.Test.UpdateInfos
{
    [TestFixture]
    public class RingtoetsUpdateInfoFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_SectionResultUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResult>(null);

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
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResult>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResult>(sectionResultUpdateStrategy);

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
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResult>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResult>(sectionResultUpdateStrategy);

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
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResult>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResult>(sectionResultUpdateStrategy);

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
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResult>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResult>(sectionResultUpdateStrategy);

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
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResult>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResult>(sectionResultUpdateStrategy);

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
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResult>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            // Call
            UpdateInfo<FailureMechanismSectionsContext> updateInfo = RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                FailureMechanismSectionsContext, TestFailureMechanism, FailureMechanismSectionResult>(sectionResultUpdateStrategy);

            // Assert
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(new TestFailureMechanism(), assessmentSection);
            Assert.IsNull(updateInfo.CurrentPath(failureMechanismSectionsContext));
            mocks.VerifyAll();
        }
    }
}