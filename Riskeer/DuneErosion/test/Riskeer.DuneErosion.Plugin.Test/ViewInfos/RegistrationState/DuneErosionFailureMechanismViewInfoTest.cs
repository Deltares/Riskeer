﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects.RegistrationState;
using Riskeer.DuneErosion.Forms.Views.RegistrationState;

namespace Riskeer.DuneErosion.Plugin.Test.ViewInfos.RegistrationState
{
    [TestFixture]
    public class DuneErosionFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private DuneErosionPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new DuneErosionPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(DuneErosionFailureMechanismView));
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
            Assert.AreEqual(typeof(DuneErosionFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(DuneErosionFailureMechanismContext), info.ViewDataType);
        }

        [Test]
        public void GetViewName_WithContext_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            string viewName = info.GetViewName(null, context);

            // Assert
            Assert.AreEqual(failureMechanism.Name, viewName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AdditionalDataCheck_Always_ReturnTrueOnlyIfFailureMechanismInAssembly(bool inAssembly)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                InAssembly = inAssembly
            };

            var context = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool result = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(inAssembly, result);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_ReturnDuneFailureMechanismView()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();

            var context = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            var view = (DuneErosionFailureMechanismView) info.CreateInstance(context);

            // Assert
            Assert.AreSame(failureMechanism, view.FailureMechanism);
            Assert.AreSame(assessmentSection, view.AssessmentSection);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            var view = new DuneErosionFailureMechanismView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, otherAssessmentSection);

            // Assert
            Assert.IsFalse(closeForData);

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();

            var view = new DuneErosionFailureMechanismView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            var otherFailureMechanism = new DuneErosionFailureMechanism();

            var view = new DuneErosionFailureMechanismView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, otherFailureMechanism);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();

            var view = new DuneErosionFailureMechanismView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }
    }
}