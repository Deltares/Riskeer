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
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsResultViewInfoTest
    {
        private GrassCoverErosionOutwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GrassCoverErosionOutwardsFailureMechanismResultView));
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
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

            // Call
            var viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.SectionResults, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView())
            {
                // Call
                var viewName = info.GetViewName(view, null);

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
            Assert.AreEqual(typeof(GrassCoverErosionOutwardsFailureMechanismResultView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>), dataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>), viewDataType);
        }

        [Test]
        public void Image_Always_ReturnsFailureMechanismSectionResultIcon()
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
            MockRepository mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, assessmentSectionMock);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mocks.Stub<FailureMechanismBase>("N", "C");

            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanismMock
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView())
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, assessmentSectionMock);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new OtherFailureMechanism(),
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, assessmentSectionMock);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView())
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView())
            {
                view.Data = failureMechanism.SectionResults;

                // Call
                var closeForData = info.CloseForData(view, new GrassCoverErosionOutwardsFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var failureMechanismContext = mocks.StrictMock<IFailureMechanismContext<GrassCoverErosionOutwardsFailureMechanism>>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanismContext.Expect(fm => fm.WrappedData).Return(failureMechanism);
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView())
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
            MockRepository mocks = new MockRepository();
            var failureMechanismContext = mocks.StrictMock<IFailureMechanismContext<GrassCoverErosionOutwardsFailureMechanism>>();
            failureMechanismContext.Expect(fm => fm.WrappedData).Return(new GrassCoverErosionOutwardsFailureMechanism());

            mocks.ReplayAll();

            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView())
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
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
            MockRepository mocks = new MockRepository();
            var viewMock = mocks.StrictMock<GrassCoverErosionOutwardsFailureMechanismResultView>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

            viewMock.Expect(v => v.FailureMechanism = failureMechanism);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, context);

            // Assert
            mocks.VerifyAll();
        }

        private class OtherFailureMechanism : FailureMechanismBase
        {
            public OtherFailureMechanism()
                : base(@"OtherFailureMechanism", @"failureMechanismCode") {}

            public override IEnumerable<ICalculation> Calculations
            {
                get
                {
                    yield break;
                }
            }
        }
    }
}