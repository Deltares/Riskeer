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

using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Forms.Views.SectionResultViews;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverSlipOffInwardsResultViewInfoTest
    {
        private MockRepository mocks;
        private RiskeerPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RiskeerPlugin();
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
            Assert.AreEqual(typeof(IObservableEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            var context = new FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                failureMechanism.SectionResults,
                failureMechanism);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.SectionResults, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Resultaat", viewName);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            using (var view = new GrassCoverSlipOffInwardsResultView(failureMechanism.SectionResults, failureMechanism))
            {
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
            var otherFailureMechanism = mocks.Stub<IFailureMechanism>();

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                otherFailureMechanism
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverSlipOffInwardsResultView(failureMechanism.SectionResults, failureMechanism))
            {
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
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverSlipOffInwardsResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var failurePathContext = mocks.StrictMock<IFailurePathContext<IFailureMechanism>>();
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            failurePathContext.Expect(fm => fm.WrappedData).Return(failureMechanism);

            mocks.ReplayAll();

            using (var view = new GrassCoverSlipOffInwardsResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, failurePathContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var failurePathContext = mocks.StrictMock<IFailurePathContext<IFailureMechanism>>();
            failurePathContext.Expect(fm => fm.WrappedData).Return(new GrassCoverSlipOffInwardsFailureMechanism());
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            using (var view = new GrassCoverSlipOffInwardsResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, failurePathContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CreateInstance_WithContext_ReturnsView()
        {
            // Setup
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            var context = new FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>(
                failureMechanism.SectionResults,
                failureMechanism);

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<GrassCoverSlipOffInwardsResultView>(view);
        }
    }
}