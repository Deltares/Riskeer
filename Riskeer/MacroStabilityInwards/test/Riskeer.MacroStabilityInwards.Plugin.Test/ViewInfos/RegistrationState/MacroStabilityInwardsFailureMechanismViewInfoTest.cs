﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects.RegistrationState;
using Riskeer.MacroStabilityInwards.Forms.Views.RegistrationState;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.ViewInfos.RegistrationState
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private MacroStabilityInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(MacroStabilityInwardsFailureMechanismView));
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
            Assert.AreEqual(typeof(MacroStabilityInwardsFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(MacroStabilityInwardsFailureMechanismContext), info.ViewDataType);
        }

        [Test]
        public void GetViewName_WithContext_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            string viewName = info.GetViewName(null, context);

            // Assert
            Assert.AreEqual(failureMechanism.Name, viewName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AdditionalDataCheck_Always_ReturnTrueOnlyIfFailureMechanismInAssembly(bool inAssembly)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                InAssembly = inAssembly
            };

            var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool result = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(inAssembly, result);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_ReturnMacroStabilityInwardsFailureMechanismView()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            var view = (MacroStabilityInwardsFailureMechanismView) info.CreateInstance(context);

            // Assert
            Assert.AreSame(failureMechanism, view.FailureMechanism);
            Assert.AreSame(assessmentSection, view.AssessmentSection);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var otherAssessmentSection = new AssessmentSectionStub();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, otherAssessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, assessmentSection);

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
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var otherMacroStabilityInwardsFailureMechanism = new MacroStabilityInwardsFailureMechanism();

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, otherMacroStabilityInwardsFailureMechanism);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var view = new MacroStabilityInwardsFailureMechanismView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }
    }
}