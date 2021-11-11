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
using Core.Common.Controls.Views;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class MacroStabilityInwardsScenariosViewInfoTest
    {
        private MacroStabilityInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(MacroStabilityInwardsScenariosView));
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
            Assert.AreEqual(typeof(MacroStabilityInwardsScenariosContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationsGroup = new CalculationGroup();
            var scenariosContext = new MacroStabilityInwardsScenariosContext(calculationsGroup, failureMechanism);

            // Call
            object viewData = info.GetViewData(scenariosContext);

            // Assert
            Assert.AreSame(calculationsGroup, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsScenarios()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Scenario's", viewName);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var calculationsGroup = new CalculationGroup();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsScenariosView(calculationsGroup, new MacroStabilityInwardsFailureMechanism()))
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
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationsGroup = new CalculationGroup();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsScenariosView(calculationsGroup, failureMechanism))
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
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsScenariosView(failureMechanism.CalculationsGroup, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnFalse()
        {
            using (var view = new MacroStabilityInwardsScenariosView(new CalculationGroup(), new MacroStabilityInwardsFailureMechanism()))
            {
                // Call
                bool closeForData = info.CloseForData(view, new MacroStabilityInwardsFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnTrue()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            using (var view = new MacroStabilityInwardsScenariosView(failureMechanism.CalculationsGroup, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailurePathContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var context = new MacroStabilityInwardsFailurePathContext(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            using (var view = new MacroStabilityInwardsScenariosView(failureMechanism.CalculationsGroup, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailurePathContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var context = new MacroStabilityInwardsFailurePathContext(failureMechanism, assessmentSection);

            using (var view = new MacroStabilityInwardsScenariosView(failureMechanism.CalculationsGroup, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_ReturnsView()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationsGroup = new CalculationGroup();
            var context = new MacroStabilityInwardsScenariosContext(calculationsGroup, failureMechanism);

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsScenariosView>(view);
        }
    }
}