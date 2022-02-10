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
using Riskeer.AssemblyTool.Data.TestUtil;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsResultViewInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionOutwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>));
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
            Assert.AreEqual(typeof(GrassCoverErosionOutwardsFailureMechanismSectionResultContext), info.DataType);
            Assert.AreEqual(typeof(IObservableEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewData_WithContext_ReturnsWrappedFailureMechanismResult()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsFailureMechanismSectionResultContext(
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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism,
                fm => fm.GeneralInput.N,
                fm => fm.GeneralInput.ApplyLengthEffectInSection,
                sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult()))
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var otherFailureMechanism = mocks.Stub<IFailureMechanism>();

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                otherFailureMechanism
            });

            mocks.ReplayAll();

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism,
                fm => fm.GeneralInput.N,
                fm => fm.GeneralInput.ApplyLengthEffectInSection,
                sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult()))
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism,
                fm => fm.GeneralInput.N,
                fm => fm.GeneralInput.ApplyLengthEffectInSection,
                sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult()))
            {
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism,
                fm => fm.GeneralInput.N,
                fm => fm.GeneralInput.ApplyLengthEffectInSection,
                sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult()))
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism,
                fm => fm.GeneralInput.N,
                fm => fm.GeneralInput.ApplyLengthEffectInSection,
                sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult()))
            {
                // Call
                bool closeForData = info.CloseForData(view, new GrassCoverErosionOutwardsFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailurePathContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsFailurePathContext(failureMechanism, assessmentSection);

            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism,
                fm => fm.GeneralInput.N,
                fm => fm.GeneralInput.ApplyLengthEffectInSection,
                sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult()))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailurePathContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new GrassCoverErosionOutwardsFailurePathContext(
                new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism,
                fm => fm.GeneralInput.N,
                fm => fm.GeneralInput.ApplyLengthEffectInSection,
                sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult()))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsFailureMechanismSectionResultContext(
                failureMechanism.SectionResults, failureMechanism, assessmentSection);

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<NonAdoptableWithProfileProbabilityFailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism>>(view);
            mocks.VerifyAll();
        }
    }
}