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
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingCalculationsViewInfoTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingCalculationsView));
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
            Assert.AreEqual(typeof(PipingCalculationGroupContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, info.Image);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new PipingCalculationGroupContext(calculationGroup,
                                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                                            failureMechanism,
                                                                            assessmentSection);

            // Call & Assert
            Assert.AreEqual(calculationGroup, info.GetViewData(calculationGroupContext));
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewName_Always_ReturnsCalculationGroupName()
        {
            // Setup
            mocks.ReplayAll();

            var calculationsView = new PipingCalculationsView();
            var calculationGroup = new CalculationGroup
            {
                Name = "Test"
            };

            // Call
            string name = info.GetViewName(calculationsView, calculationGroup);

            // Assert
            Assert.AreEqual("Test", name);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_PipingCalculationGroupContextWithPipingFailureMechanismParent_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingFailureMechanism.CalculationsGroup,
                                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                  Enumerable.Empty<StochasticSoilModel>(),
                                                                                  pipingFailureMechanism,
                                                                                  assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(pipingCalculationGroupContext);

            // Assert
            Assert.IsTrue(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_PipingCalculationGroupContextWithoutPipingFailureMechanismParent_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(calculationGroup,
                                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                  Enumerable.Empty<StochasticSoilModel>(),
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(pipingCalculationGroupContext);

            // Assert
            Assert.IsFalse(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutPipingFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
            mocks.ReplayAll();

            var view = new PipingCalculationsView()
            {
                Data = new CalculationGroup()
            };

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                new PipingFailureMechanism()
            });
            mocks.ReplayAll();

            var view = new PipingCalculationsView()
            {
                Data = new CalculationGroup()
            };

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new PipingFailureMechanism();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            var view = new PipingCalculationsView()
            {
                Data = failureMechanism.CalculationsGroup
            };

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            mocks.ReplayAll();

            var view = new PipingCalculationsView();
            var failureMechanism = new PipingFailureMechanism();

            view.Data = new CalculationGroup();

            // Call
            var closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            var view = new PipingCalculationsView();
            var failureMechanism = new PipingFailureMechanism();

            view.Data = failureMechanism.CalculationsGroup;

            // Call
            var closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var view = new PipingCalculationsView();
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

            view.Data = failureMechanism.CalculationsGroup;

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var view = new PipingCalculationsView();
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            view.Data = failureMechanism.CalculationsGroup;

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingCalculationsView>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingCalculationsGroupMock, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<StochasticSoilModel>(), pipingFailureMechanismMock, assessmentSection);

            viewMock.Expect(v => v.AssessmentSection = assessmentSection);
            viewMock.Expect(v => v.PipingFailureMechanism = pipingFailureMechanismMock);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, pipingCalculationGroupContext);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_Always_NewViewWithHandler()
        {
            // Call
            var view = info.CreateInstance();

            // Assert
            Assert.IsInstanceOf<PipingCalculationsView>(view);
        }
    }
}