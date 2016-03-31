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
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingFailureMechanismResultViewInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingFailureMechanismResultView));
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
            Assert.AreEqual(typeof(List<PipingFailureMechanismSectionResult>), info.DataType);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedPipingFailureMechanismResult()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();

            // Call
            var viewData = info.GetViewData(pipingFailureMechanism.PipingFailureMechanismSectionResults);

            // Assert
            Assert.AreSame(pipingFailureMechanism.PipingFailureMechanismSectionResults, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();

            mocks.ReplayAll();

            // Call
            var viewName = info.GetViewName(viewMock, pipingFailureMechanism.PipingFailureMechanismSectionResults);

            //
            Assert.AreEqual("Oordeel", viewName);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutPipingFailureMechanism_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanism = new PipingFailureMechanism();

            viewMock.Expect(vm => vm.Data).Return(pipingFailureMechanism.PipingFailureMechanismSectionResults);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingFailureMechanism = new PipingFailureMechanism();

            viewMock.Expect(vm => vm.Data).Return(pipingFailureMechanism.PipingFailureMechanismSectionResults);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                pipingFailureMechanismMock
            });

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismResultView>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            viewMock.Expect(vm => vm.Data).Return(pipingFailureMechanismMock.PipingFailureMechanismSectionResults);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                pipingFailureMechanismMock
            });
            
            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsTrue(closeForData);
        }
    }
}