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
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.Views;

namespace Riskeer.StabilityPointStructures.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class StabilityPointStructuresCalculationsViewInfoTest
    {
        private StabilityPointStructuresPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new StabilityPointStructuresPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(StabilityPointStructuresCalculationsView));
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
            Assert.AreEqual(typeof(StabilityPointStructuresCalculationGroupContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
        }

        [Test]
        public void GetViewData_ContextNotNull_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new StabilityPointStructuresCalculationGroupContext(calculationGroup,
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

            var calculationGroupContext = new StabilityPointStructuresCalculationGroupContext(calculationGroup,
                                                                                              null,
                                                                                              new StabilityPointStructuresFailureMechanism(),
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculationGroupContext = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new StabilityPointStructuresCalculationGroupContext(calculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(calculationGroupContext);

            // Assert
            Assert.IsFalse(additionalDataCheck);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            using (var view = new StabilityPointStructuresCalculationsView(new CalculationGroup(), new StabilityPointStructuresFailureMechanism(), assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
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

            using (var view = new StabilityPointStructuresCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationsContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            using (var view = new StabilityPointStructuresCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection))
            {
                var context = new StabilityPointStructuresCalculationsContext(new StabilityPointStructuresFailureMechanism(), assessmentSection);

                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculationsContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            using (var view = new StabilityPointStructuresCalculationsView(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection))
            {
                var context = new StabilityPointStructuresCalculationsContext(failureMechanism, assessmentSection);

                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }
    }
}