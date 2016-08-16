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

using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingScenariosViewInfoTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingScenariosView));
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
            Assert.AreEqual(typeof(PipingScenariosContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ScenariosIcon, info.Image);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();
            var pipingScenariosContext = new PipingScenariosContext(pipingCalculationsGroupMock, pipingFailureMechanismMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.AreEqual(pipingCalculationsGroupMock, info.GetViewData(pipingScenariosContext));
        }

        [Test]
        public void GetViewName_Always_ReturnsScenarios()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingScenariosView>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();

            mocks.ReplayAll();

            pipingCalculationsGroupMock.Name = "Test";

            // Call & Assert
            Assert.AreEqual(RingtoetsCommonFormsResources.Scenarios_DisplayName, info.GetViewName(viewMock, pipingCalculationsGroupMock));
        }

        [Test]
        public void AdditionalDataCheck_PipingScenariosContextWithPipingFailureMechanismParent_ReturnsTrue()
        {
            // Setup
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingScenariosContext = new PipingScenariosContext(pipingFailureMechanismMock.CalculationsGroup, pipingFailureMechanismMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.AdditionalDataCheck(pipingScenariosContext));
        }

        [Test]
        public void AdditionalDataCheck_PipingScenariosContextWithoutPipingFailureMechanismParent_ReturnsFalse()
        {
            // Setup
            var pipingFailureMechanismMock = mocks.Stub<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();
            var pipingScenariosContext = new PipingScenariosContext(pipingCalculationsGroupMock, pipingFailureMechanismMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.AdditionalDataCheck(pipingScenariosContext));
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutPipingFailureMechanism_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingScenariosView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();

            viewMock.Expect(vm => vm.Data).Return(pipingCalculationsGroupMock);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingScenariosView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.Stub<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();

            viewMock.Expect(vm => vm.Data).Return(pipingCalculationsGroupMock);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                pipingFailureMechanismMock
            });

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingScenariosView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            viewMock.Expect(vm => vm.Data).Return(pipingFailureMechanismMock.CalculationsGroup);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                pipingFailureMechanismMock
            });

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var view = new PipingScenariosView();
            var failureMechanism = new PipingFailureMechanism();

            view.Data = new CalculationGroup();

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var view = new PipingScenariosView();
            var failureMechanism = new PipingFailureMechanism();

            view.Data = failureMechanism.CalculationsGroup;

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var view = new PipingScenariosView();
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSectionMock);

            view.Data = failureMechanism.CalculationsGroup;

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var view = new PipingScenariosView();
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSectionMock);

            view.Data = failureMechanism.CalculationsGroup;

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingScenariosView>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();
            var pipingScenariosContext = new PipingScenariosContext(pipingCalculationsGroupMock, pipingFailureMechanismMock);

            viewMock.Expect(v => v.PipingFailureMechanism = pipingFailureMechanismMock);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, pipingScenariosContext);

            // Assert
            mocks.VerifyAll();
        } 
    }
}