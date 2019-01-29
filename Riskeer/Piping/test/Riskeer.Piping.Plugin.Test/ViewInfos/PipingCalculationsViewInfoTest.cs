// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ViewInfos
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
                                                                            null,
                                                                            Enumerable.Empty<PipingSurfaceLine>(),
                                                                            Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                            failureMechanism,
                                                                            assessmentSection);

            // Call & Assert
            Assert.AreSame(calculationGroup, info.GetViewData(calculationGroupContext));
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewName_WithPipingCalculationGroupContext_ReturnsCalculationGroupName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationsView = new PipingCalculationsView();

            const string calculationGroupName = "Test";

            var calculationGroup = new CalculationGroup
            {
                Name = calculationGroupName
            };

            var pipingCalculationGroupContext = new PipingCalculationGroupContext(calculationGroup,
                                                                                  null,
                                                                                  Enumerable.Empty<PipingSurfaceLine>(),
                                                                                  Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                  new PipingFailureMechanism(),
                                                                                  assessmentSection);

            // Call
            string name = info.GetViewName(calculationsView, pipingCalculationGroupContext);

            // Assert
            Assert.AreEqual(calculationGroupName, name);
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
                                                                                  null,
                                                                                  Enumerable.Empty<PipingSurfaceLine>(),
                                                                                  Enumerable.Empty<PipingStochasticSoilModel>(),
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
                                                                                  null,
                                                                                  Enumerable.Empty<PipingSurfaceLine>(),
                                                                                  Enumerable.Empty<PipingStochasticSoilModel>(),
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

            var view = new PipingCalculationsView
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

            var view = new PipingCalculationsView
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

            var view = new PipingCalculationsView
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
            bool closeForData = info.CloseForData(view, failureMechanism);

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
            bool closeForData = info.CloseForData(view, failureMechanism);

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
            bool closeForData = info.CloseForData(view, failureMechanismContext);

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
            bool closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var view = mocks.StrictMock<PipingCalculationsView>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroup = mocks.StrictMock<CalculationGroup>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingCalculationsGroup,
                                                                                  null,
                                                                                  Enumerable.Empty<PipingSurfaceLine>(),
                                                                                  Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                  pipingFailureMechanism, assessmentSection);

            view.Expect(v => v.AssessmentSection = assessmentSection);
            view.Expect(v => v.PipingFailureMechanism = pipingFailureMechanism);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(view, pipingCalculationGroupContext);

            // Assert
            mocks.VerifyAll();
        }
    }
}