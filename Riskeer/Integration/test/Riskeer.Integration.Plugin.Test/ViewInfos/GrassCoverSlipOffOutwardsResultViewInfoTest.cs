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
using Riskeer.Common.Forms.Views;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Forms.PresentationObjects.StandAlone;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverSlipOffOutwardsResultViewInfoTest
    {
        private MockRepository mocks;
        private RiskeerPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RiskeerPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverSlipOffOutwardsFailureMechanism>));
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
            Assert.AreEqual(typeof(GrassCoverSlipOffOutwardsFailureMechanismSectionResultContext), info.DataType);
            Assert.AreEqual(typeof(IObservableEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_WithContext_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();
            var context = new GrassCoverSlipOffOutwardsFailureMechanismSectionResultContext(
                failureMechanism.SectionResults, failureMechanism, assessmentSection);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.SectionResults, viewData);
            mocks.VerifyAll();
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
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverSlipOffOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism, assessmentSection, fm => fm.GeneralInput.N, fm => fm.GeneralInput.ApplyLengthEffectInSection))
            {
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
            var otherFailureMechanism = mocks.Stub<IFailureMechanism>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                otherFailureMechanism
            });
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverSlipOffOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism, assessmentSection, fm => fm.GeneralInput.N, fm => fm.GeneralInput.ApplyLengthEffectInSection))
            {
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
            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverSlipOffOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism, assessmentSection, fm => fm.GeneralInput.N, fm => fm.GeneralInput.ApplyLengthEffectInSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failurePathContext = mocks.StrictMock<IFailurePathContext<IFailureMechanism>>();
            failurePathContext.Expect(fm => fm.WrappedData).Return(failureMechanism);
            mocks.ReplayAll();

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverSlipOffOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism, assessmentSection, fm => fm.GeneralInput.N, fm => fm.GeneralInput.ApplyLengthEffectInSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, failurePathContext);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failurePathContext = mocks.StrictMock<IFailurePathContext<IFailureMechanism>>();
            failurePathContext.Expect(fm => fm.WrappedData).Return(new GrassCoverSlipOffOutwardsFailureMechanism());
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverSlipOffOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism, assessmentSection, fm => fm.GeneralInput.N, fm => fm.GeneralInput.ApplyLengthEffectInSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, failurePathContext);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_ReturnsView()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverSlipOffOutwardsFailureMechanism();
            var context = new GrassCoverSlipOffOutwardsFailureMechanismSectionResultContext(
                failureMechanism.SectionResults, failureMechanism, assessmentSection);

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverSlipOffOutwardsFailureMechanism>>(view);
            mocks.VerifyAll();
        }
    }
}