﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.Forms.PresentationObjects.RegistrationState;
using Riskeer.ClosingStructures.Forms.Views;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.ClosingStructures.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class ClosingStructuresScenariosViewInfoTest
    {
        private ClosingStructuresPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(ClosingStructuresScenariosView));
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
            Assert.AreEqual(typeof(ClosingStructuresScenariosContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
        }

        [Test]
        public void GetViewName_Always_ReturnViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Scenario's", viewName);
        }

        [Test]
        public void GetViewData_Always_ReturnWrappedData()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var context = new ClosingStructuresScenariosContext(calculationGroup, failureMechanism);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculationGroup, viewData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnFalse()
        {
            using (var view = new ClosingStructuresScenariosView(new CalculationGroup(), new ClosingStructuresFailureMechanism()))
            {
                // Call
                bool closeForData = info.CloseForData(view, new ClosingStructuresFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnTrue()
        {
            // Setup
            var correspondingFailureMechanism = new ClosingStructuresFailureMechanism();

            using (var view = new ClosingStructuresScenariosView(correspondingFailureMechanism.CalculationsGroup, correspondingFailureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, correspondingFailureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnFalse()
        {
            // Setup
            var unrelatedFailureMechanism = new ClosingStructuresFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                unrelatedFailureMechanism
            });
            mocks.ReplayAll();

            using (var view = new ClosingStructuresScenariosView(new CalculationGroup(), new ClosingStructuresFailureMechanism()))
            {
                // Precondition
                Assert.AreNotSame(view.Data, unrelatedFailureMechanism.CalculationsGroup);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnTrue()
        {
            // Setup
            var relatedFailureMechanism = new ClosingStructuresFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                relatedFailureMechanism
            });
            mocks.ReplayAll();

            using (var view = new ClosingStructuresScenariosView(relatedFailureMechanism.CalculationsGroup, relatedFailureMechanism))
            {
                // Precondition
                Assert.AreSame(view.Data, relatedFailureMechanism.CalculationsGroup);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutClosingStructuresFailureMechanism_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
            mocks.ReplayAll();

            using (var view = new ClosingStructuresScenariosView(new CalculationGroup(), new ClosingStructuresFailureMechanism()))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var context = new ClosingStructuresFailureMechanismContext(new ClosingStructuresFailureMechanism(), assessmentSection);

            using (var view = new ClosingStructuresScenariosView(failureMechanism.CalculationsGroup, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var context = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new ClosingStructuresScenariosView(failureMechanism.CalculationsGroup, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_ReturnsClosingStructuresScenariosView()
        {
            // Setup
            var group = new CalculationGroup();
            var context = new ClosingStructuresScenariosContext(group, new ClosingStructuresFailureMechanism());

            // Call
            using (IView view = info.CreateInstance(context))
            {
                // Assert
                Assert.IsInstanceOf<ClosingStructuresScenariosView>(view);
                Assert.AreSame(group, view.Data);
            }
        }
    }
}