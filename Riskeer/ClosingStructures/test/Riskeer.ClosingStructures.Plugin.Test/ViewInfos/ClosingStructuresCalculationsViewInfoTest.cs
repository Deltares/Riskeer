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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.Forms.Views;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.ClosingStructures.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class ClosingStructuresCalculationsViewInfoTest
    {
        private MockRepository mocks;
        private ClosingStructuresPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(ClosingStructuresCalculationsView));
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
            Assert.AreEqual(typeof(ClosingStructuresCalculationGroupContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, info.Image);
        }

        [Test]
        public void GetViewData_ContextNotNull_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new ClosingStructuresCalculationGroupContext(calculationGroup,
                                                                                       null,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            // Call
            object viewData = info.GetViewData(calculationGroupContext);

            // Assert
            Assert.AreSame(calculationGroup, viewData);
        }

        [Test]
        public void GetViewName_WithCalculationGroupContext_ReturnsCalculationGroupName()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            const string calculationGroupName = "Test";

            var calculationGroup = new CalculationGroup
            {
                Name = calculationGroupName
            };

            var calculationGroupContext = new ClosingStructuresCalculationGroupContext(calculationGroup,
                                                                                       null,
                                                                                       new ClosingStructuresFailureMechanism(),
                                                                                       assessmentSection);
            // Call
            string name = info.GetViewName(null, calculationGroupContext);

            // Assert
            Assert.AreEqual(calculationGroupName, name);
        }

        [Test]
        public void AdditionalDataCheck_CalculationGroupContextWithFailureMechanismParent_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculationGroupContext = new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                       null,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(calculationGroupContext);

            // Assert
            Assert.IsTrue(additionalDataCheck);
        }

        [Test]
        public void AdditionalDataCheck_CalculationGroupContextWithoutFailureMechanismParent_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new ClosingStructuresCalculationGroupContext(calculationGroup,
                                                                                       null,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(calculationGroupContext);

            // Assert
            Assert.IsFalse(additionalDataCheck);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            using (var calculationsView = new ClosingStructuresCalculationsView(new CalculationGroup(), new ClosingStructuresFailureMechanism(), assessmentSection))
            {
                bool closeForData = info.CloseForData(calculationsView, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            using (var calculationsView = new ClosingStructuresCalculationsView(new CalculationGroup(), new ClosingStructuresFailureMechanism(), assessmentSection))
            {
                bool closeForData = info.CloseForData(calculationsView, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            mocks = new MockRepository();
            var calculationGroup = new CalculationGroup();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new TestFailureMechanism(),
                failureMechanism
            });
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "Location 1", 1.1, 2.2),
                    new HydraulicBoundaryLocation(2, "Location 2", 3.3, 4.4)
                }
            });
            mocks.ReplayAll();

            using (var view = new ClosingStructuresCalculationsView(calculationGroup, failureMechanism, assessmentSection)
            {
                Data = failureMechanism.CalculationsGroup
            })
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
            var assessmentSection = new AssessmentSectionStub();

            using (var view = new ClosingStructuresCalculationsView(new CalculationGroup(), new ClosingStructuresFailureMechanism(), assessmentSection))
            {
                var failureMechanism = new ClosingStructuresFailureMechanism();

                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            using (var calculationsView = new ClosingStructuresCalculationsView(failureMechanism.CalculationsGroup, new ClosingStructuresFailureMechanism(), assessmentSection))
            {
                calculationsView.Data = failureMechanism.CalculationsGroup;

                // Call
                bool closeForData = info.CloseForData(calculationsView, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            using (var calculationsView = new ClosingStructuresCalculationsView(new CalculationGroup(), failureMechanism, assessmentSection))
            {
                var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                bool closeForData = info.CloseForData(calculationsView, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            using (var calculationsView = new ClosingStructuresCalculationsView(failureMechanism.CalculationsGroup, new ClosingStructuresFailureMechanism(), assessmentSection))
            {
                var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                bool closeForData = info.CloseForData(calculationsView, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }
    }
}