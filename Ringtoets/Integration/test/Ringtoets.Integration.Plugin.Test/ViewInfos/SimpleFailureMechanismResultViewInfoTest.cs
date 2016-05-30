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
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.StandAlone.SectionResult;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Piping.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class SimpleFailureMechanismResultViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(SimpleFailureMechanismResultView));
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
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<SimpleFailureMechanismSectionResult>), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<SimpleFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var failureMechanism = new Simple();
            var context = new FailureMechanismSectionResultContext<SimpleFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);
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
            var failureMechanism = new Simple();
            var viewMock = mocks.StrictMock<SimpleFailureMechanismResultView>();

            mocks.ReplayAll();

            // Call
            var viewName = info.GetViewName(viewMock, failureMechanism.SectionResults);

            // Assert
            Assert.AreEqual("Oordeel", viewName);
            mocks.VerifyAll();
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(SimpleFailureMechanismResultView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<SimpleFailureMechanismSectionResult>), dataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<SimpleFailureMechanismSectionResult>), viewDataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<SimpleFailureMechanismResultView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new Simple();

            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<SimpleFailureMechanismResultView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mocks.Stub<FailureMechanismBase>("N","C");
            var failureMechanism = new Simple();

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
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<SimpleFailureMechanismResultView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new Simple();

            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new PipingFailureMechanism(),
                failureMechanism
            });

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<SimpleFailureMechanismResultView>();
            var failureMechanism = new Simple();
            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<SimpleFailureMechanismResultView>();
            var failureMechanism = new Simple();
            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, new Simple());

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<SimpleFailureMechanismResultView>();
            var failureMechanismContext = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();
            var failureMechanism = new Simple();
            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);
            failureMechanismContext.Expect(fm => fm.WrappedData).Return(failureMechanism);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<SimpleFailureMechanismResultView>();
            var failureMechanismContext = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();
            var failureMechanism = new Simple();
            viewMock.Expect(vm => vm.Data).Return(failureMechanism.SectionResults);
            failureMechanismContext.Expect(fm => fm.WrappedData).Return(new Simple());

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, failureMechanismContext);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var viewMock = mocks.StrictMock<SimpleFailureMechanismResultView>();
            var failureMechanism = new Simple();
            var context = new FailureMechanismSectionResultContext<SimpleFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

            viewMock.Expect(v => v.FailureMechanism = failureMechanism);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, context);

            // Assert
            mocks.VerifyAll();
        }

        private class Simple : FailureMechanismBase, IHasSectionResults<SimpleFailureMechanismSectionResult>
        {
            public Simple() : base("simple failure mechanism", "simple code")
            {
                SectionResults = new List<SimpleFailureMechanismSectionResult>();
            }

            public IEnumerable<SimpleFailureMechanismSectionResult> SectionResults { get; private set; }

            public override IEnumerable<ICalculation> Calculations
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}