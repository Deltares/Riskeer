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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismResultViewInfoTest
    {
        private MockRepository mocks;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            using (var plugin = new ClosingStructuresPlugin())
            {
                info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(ClosingStructuresFailureMechanismResultView));
            }
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<ClosingStructuresFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var context = new FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.SectionResults, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var viewMock = mocks.StrictMock<ClosingStructuresFailureMechanismResultView>();
            mocks.ReplayAll();

            // Call
            string viewName = info.GetViewName(viewMock, failureMechanism.SectionResults);

            // Assert
            Assert.AreEqual("Resultaat", viewName);
            mocks.VerifyAll();
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            Type viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(ClosingStructuresFailureMechanismResultView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            Type dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>), dataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            Type viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<ClosingStructuresFailureMechanismSectionResult>), viewDataType);
        }

        [Test]
        public void Image_Always_ReturnsFailureMechanismSectionResultIcon()
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
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
            mocks.ReplayAll();

            using (var view = new ClosingStructuresFailureMechanismResultView())
            {
                var failureMechanism = new ClosingStructuresFailureMechanism();
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var otherFailureMechanism = mocks.Stub<FailureMechanismBase>("N", "C");

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                otherFailureMechanism
            });

            mocks.ReplayAll();

            using (var view = new ClosingStructuresFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new TestFailureMechanism(),
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new ClosingStructuresFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            using (var view = new ClosingStructuresFailureMechanismResultView())
            {
                var failureMechanism = new ClosingStructuresFailureMechanism();
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
            using (var view = new ClosingStructuresFailureMechanismResultView())
            {
                var failureMechanism = new ClosingStructuresFailureMechanism();
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, new ClosingStructuresFailureMechanism());

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

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new ClosingStructuresFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            using (var view = new ClosingStructuresFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                var failureMechanismContext = new ClosingStructuresFailureMechanismContext(new ClosingStructuresFailureMechanism(),
                                                                                           assessmentSection);

                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var view = mocks.StrictMock<ClosingStructuresFailureMechanismResultView>();
            var context = new FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);
            view.Expect(v => v.FailureMechanism = failureMechanism);
            mocks.ReplayAll();

            // Call
            info.AfterCreate(view, context);

            // Assert
            mocks.VerifyAll();
        }

        private class TestFailureMechanism : FailureMechanismBase
        {
            public TestFailureMechanism() : base("Name", "FailureMechanismCode") {}

            public override IEnumerable<ICalculation> Calculations
            {
                get
                {
                    return null;
                }
            }
        }
    }
}