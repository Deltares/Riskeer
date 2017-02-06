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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class StabilityPointStructuresResultViewInfoTest
    {
        private MockRepository mocks;
        private StabilityPointStructuresPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new StabilityPointStructuresPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(StabilityPointStructuresFailureMechanismResultView));
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
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<StabilityPointStructuresFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var context = new FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

            // Call
            var viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.SectionResults, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var constructionResultView = new StabilityPointStructuresFailureMechanismResultView();

            // Call
            var viewName = info.GetViewName(constructionResultView, failureMechanism.SectionResults);

            // Assert
            Assert.AreEqual("Resultaat", viewName);
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(StabilityPointStructuresFailureMechanismResultView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>), dataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<StabilityPointStructuresFailureMechanismSectionResult>), viewDataType);
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
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
            mocks.ReplayAll();

            using (var view = new StabilityPointStructuresFailureMechanismResultView())
            {
                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var otherFailureMechanism = mocks.Stub<FailureMechanismBase>("N", "C");

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                otherFailureMechanism
            });

            mocks.ReplayAll();

            using (var view = new StabilityPointStructuresFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new TestFailureMechanism(),
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new StabilityPointStructuresFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismResultView())
            {
                var failureMechanism = new StabilityPointStructuresFailureMechanism();
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
            using (var view = new StabilityPointStructuresFailureMechanismResultView())
            {
                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, new StabilityPointStructuresFailureMechanism());

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

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new StabilityPointStructuresFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, failureMechanismContext);

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

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(new StabilityPointStructuresFailureMechanism(),
                                                                                              assessmentSection);

            using (var view = new StabilityPointStructuresFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var view = mocks.StrictMock<StabilityPointStructuresFailureMechanismResultView>();
            view.Expect(v => v.FailureMechanism = failureMechanism);

            mocks.ReplayAll();

            var context = new FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

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