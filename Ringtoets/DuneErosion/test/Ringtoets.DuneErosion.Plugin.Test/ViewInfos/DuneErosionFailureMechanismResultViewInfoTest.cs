// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class DuneErosionFailureMechanismResultViewInfoTest
    {
        private MockRepository mocks;
        private DuneErosionPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new DuneErosionPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(DuneErosionFailureMechanismResultView));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<DuneErosionFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);
            mocks.ReplayAll();

            // Call
            var viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.SectionResults, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            using (var view = new DuneErosionFailureMechanismResultView())
            {
                // Call
                var viewName = info.GetViewName(view, failureMechanism.SectionResults);

                // Assert
                Assert.AreEqual("Resultaat", viewName);
            }
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(DuneErosionFailureMechanismResultView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>), dataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<DuneErosionFailureMechanismSectionResult>), viewDataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, image);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new DuneErosionFailureMechanism();

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            using (var view = new DuneErosionFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new DuneErosionFailureMechanism();
            var otherFailureMechanism = mocks.Stub<FailureMechanismBase>("N", "C");

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                otherFailureMechanism
            });

            mocks.ReplayAll();

            using (var view = new DuneErosionFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new DuneErosionFailureMechanism();
            var otherFailureMechanism = mocks.Stub<FailureMechanismBase>("N", "C");

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                otherFailureMechanism,
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new DuneErosionFailureMechanismResultView()) 
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            using (var view = new DuneErosionFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            using (var view = new DuneErosionFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, new DuneErosionFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new DuneErosionFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanismContext = new DuneErosionFailureMechanismContext(new DuneErosionFailureMechanism(), assessmentSection);

            using (var view = new DuneErosionFailureMechanismResultView())
            {
                var failureMechanism = new DuneErosionFailureMechanism();
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var viewMock = mocks.StrictMock<DuneErosionFailureMechanismResultView>();
            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

            viewMock.Expect(v => v.FailureMechanism = failureMechanism);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, context);

            // Assert
            mocks.VerifyAll();
        }
    }
}