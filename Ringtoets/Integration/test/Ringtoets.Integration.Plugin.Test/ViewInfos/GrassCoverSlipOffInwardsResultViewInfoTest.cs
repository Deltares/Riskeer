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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultViews;
using Ringtoets.Piping.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverSlipOffInwardsResultViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GrassCoverSlipOffInwardsResultView));
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
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            var context = new FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);
            mocks.ReplayAll();

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.SectionResults, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            using (var view = new GrassCoverSlipOffInwardsResultView())
            {
                // Call
                string viewName = info.GetViewName(view, failureMechanism.SectionResults);

                // Assert
                Assert.AreEqual("Resultaat", viewName);
            }
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            Type viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(GrassCoverSlipOffInwardsResultView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            Type dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>), dataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            Type viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult>), viewDataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, image);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            using (var view = new GrassCoverSlipOffInwardsResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

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
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            var otherFailureMechanism = mocks.Stub<FailureMechanismBase>("N", "C");

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                otherFailureMechanism
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverSlipOffInwardsResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

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
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new PipingFailureMechanism(),
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverSlipOffInwardsResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            using (var view = new GrassCoverSlipOffInwardsResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            using (var view = new GrassCoverSlipOffInwardsResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, new GrassCoverSlipOffInwardsFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var failureMechanismContext = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            failureMechanismContext.Expect(fm => fm.WrappedData).Return(failureMechanism);

            mocks.ReplayAll();

            using (var view = new GrassCoverSlipOffInwardsResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var failureMechanismContext = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();
            failureMechanismContext.Expect(fm => fm.WrappedData).Return(new GrassCoverSlipOffInwardsFailureMechanism());

            mocks.ReplayAll();

            using (var view = new GrassCoverSlipOffInwardsResultView())
            {
                var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var viewMock = mocks.StrictMock<GrassCoverSlipOffInwardsResultView>();
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            var context = new FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

            viewMock.Expect(v => v.FailureMechanism = failureMechanism);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, context);

            // Assert
            mocks.VerifyAll();
        }
    }
}