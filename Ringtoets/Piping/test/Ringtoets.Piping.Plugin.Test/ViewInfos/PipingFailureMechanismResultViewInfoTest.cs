﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingFailureMechanismResultViewInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingFailureMechanismResultView));
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
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<PipingFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var context = new FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);
            mocks.ReplayAll();

            // Call
            var viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.SectionResults, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();

            mocks.ReplayAll();

            // Call
            var viewName = info.GetViewName(viewMock, failureMechanism.SectionResults);

            // Assert
            Assert.AreEqual("Resultaat", viewName);
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(PipingFailureMechanismResultView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>), dataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<PipingFailureMechanismSectionResult>), viewDataType);
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
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();

            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mocks.Stub<FailureMechanismBase>("N", "C");
            var failureMechanism = new PipingFailureMechanism();

            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanismMock
            });

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();

            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new TestFailureMechanism(),
                failureMechanism
            });

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();
            var failureMechanism = new PipingFailureMechanism();
            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();
            var failureMechanism = new PipingFailureMechanism();
            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, new PipingFailureMechanism());

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var view = new PipingFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var view = new PipingFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            var failureMechanismContext = new PipingFailureMechanismContext(
                new PipingFailureMechanism(),
                assessmentSectionMock
                );

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();
            var failureMechanism = new PipingFailureMechanism();
            var context = new FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

            viewMock.Expect(v => v.FailureMechanism = failureMechanism);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, context);

            // Assert
            mocks.VerifyAll();
        }
    }
}