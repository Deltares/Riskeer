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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingFailureMechanismView));
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
            Assert.AreEqual(typeof(PipingFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(PipingFailureMechanismContext), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsTextFromResources()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismView>();

            mocks.ReplayAll();

            // Call & Assert
            Assert.AreEqual(PipingDataResources.PipingFailureMechanism_DisplayName, info.GetViewName(viewMock, null));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var otherAssessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingFailureMechanismContextMock = mocks.StrictMock<PipingFailureMechanismContext>(pipingFailureMechanismMock, assessmentSectionMock);

            viewMock.Expect(vm => vm.Data).Return(pipingFailureMechanismContextMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(viewMock, otherAssessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingFailureMechanismContextMock = mocks.StrictMock<PipingFailureMechanismContext>(pipingFailureMechanismMock, assessmentSectionMock);

            viewMock.Expect(vm => vm.Data).Return(pipingFailureMechanismContextMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var pipingFailureMechanismMock = new PipingFailureMechanism();
            var otherPipingFailureMechanismMock = new PipingFailureMechanism();

            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            var pipingFailureMechanismContext = mocks.StrictMock<PipingFailureMechanismContext>(pipingFailureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            view.Data = pipingFailureMechanismContext;

            // Call
            var closeForData = info.CloseForData(view, otherPipingFailureMechanismMock);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var pipingFailureMechanism = new PipingFailureMechanism();

            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            var pipingFailureMechanismContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            view.Data = pipingFailureMechanismContext;

            // Call
            var closeForData = info.CloseForData(view, pipingFailureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AdditionalDataCheck_Always_ReturnTrueOnlyIfFailureMechanismRelevant(bool isRelevant)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = isRelevant
            };

            var context = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool result = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(isRelevant, result);
            mocks.VerifyAll();
        }
    }
}
