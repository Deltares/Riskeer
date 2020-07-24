// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingCalculationsViewInfoTest
    {
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
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
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, info.Image);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var mocks = new MockRepository();
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
        public void GetViewName_WithContext_ReturnsCalculationGroupName()
        {
            // Setup
            const string calculationGroupName = "Test";

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Name = calculationGroupName
            };

            var failureMechanism = new PipingFailureMechanism();
            using (var calculationsView = new PipingCalculationsView(calculationGroup, failureMechanism, assessmentSection))
            {
                var pipingCalculationGroupContext = new PipingCalculationGroupContext(calculationGroup,
                                                                                      null,
                                                                                      Enumerable.Empty<PipingSurfaceLine>(),
                                                                                      Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                      failureMechanism,
                                                                                      assessmentSection);

                // Call
                string name = info.GetViewName(calculationsView, pipingCalculationGroupContext);

                // Assert
                Assert.AreEqual(calculationGroupName, name);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_PipingCalculationGroupContextWithPipingFailureMechanismParent_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
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
            var mocks = new MockRepository();
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
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());
            var assessmentSectionToRemove = mocks.Stub<IAssessmentSection>();
            assessmentSectionToRemove.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                new PipingFailureMechanism()
            });
            mocks.ReplayAll();

            using (var view = new PipingCalculationsView(new CalculationGroup(), new PipingFailureMechanism(), assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSectionToRemove);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            using (var view = new PipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());
            mocks.ReplayAll();

            using (var view = new PipingCalculationsView(new CalculationGroup(), new PipingFailureMechanism(), assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, new PipingFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            using (var view = new PipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());
            mocks.ReplayAll();

            using (var view = new PipingCalculationsView(new CalculationGroup(), new PipingFailureMechanism(), assessmentSection))
            {
                var failureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            using (var view = new PipingCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection))
            {
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }
    }
}