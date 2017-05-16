﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingCalculationsGroup = new CalculationGroup();
            var pipingScenariosContext = new PipingScenariosContext(pipingCalculationsGroup, pipingFailureMechanism);

            // Call
            object viewData = info.GetViewData(pipingScenariosContext);

            // Assert
            Assert.AreEqual(pipingCalculationsGroup, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsScenarios()
        {
            // Setup
            var pipingCalculationsGroup = new CalculationGroup
            {
                Name = "Test"
            };

            using (var view = new PipingScenariosView())
            {
                // Call
                string viewName = info.GetViewName(view, pipingCalculationsGroup);

                // Assert
                Assert.AreEqual(RingtoetsCommonFormsResources.Scenarios_DisplayName, viewName);
            }
        }

        [Test]
        public void AdditionalDataCheck_PipingScenariosContextWithPipingFailureMechanismParent_ReturnsTrue()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingScenariosContext = new PipingScenariosContext(pipingFailureMechanism.CalculationsGroup, pipingFailureMechanism);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(pipingScenariosContext);

            // Assert
            Assert.IsTrue(additionalDataCheck);
        }

        [Test]
        public void AdditionalDataCheck_PipingScenariosContextWithoutPipingFailureMechanismParent_ReturnsFalse()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingCalculationsGroup = new CalculationGroup();
            var pipingScenariosContext = new PipingScenariosContext(pipingCalculationsGroup, pipingFailureMechanism);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(pipingScenariosContext);

            // Assert
            Assert.IsFalse(additionalDataCheck);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutPipingFailureMechanism_ReturnsFalse()
        {
            // Setup
            var pipingCalculationsGroup = new CalculationGroup();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            using (var view = new PipingScenariosView
            {
                Data = pipingCalculationsGroup
            })
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
            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingCalculationsGroup = new CalculationGroup();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                pipingFailureMechanism
            });

            mocks.ReplayAll();

            using (var view = new PipingScenariosView
            {
                Data = pipingCalculationsGroup
            })
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
            var pipingFailureMechanism = new PipingFailureMechanism();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                pipingFailureMechanism
            });

            mocks.ReplayAll();

            using (var view = new PipingScenariosView
            {
                Data = pipingFailureMechanism.CalculationsGroup
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
            using (var view = new PipingScenariosView
            {
                Data = new CalculationGroup()
            })
            {
                var failureMechanism = new PipingFailureMechanism();

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
            var failureMechanism = new PipingFailureMechanism();

            using (var view = new PipingScenariosView
            {
                Data = failureMechanism.CalculationsGroup
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

            using (var view = new PipingScenariosView
            {
                Data = failureMechanism.CalculationsGroup
            })
            {
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new PipingScenariosView
            {
                Data = failureMechanism.CalculationsGroup
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingCalculationsGroup = new CalculationGroup();
            var pipingScenariosContext = new PipingScenariosContext(pipingCalculationsGroup, pipingFailureMechanism);

            using (var view = new PipingScenariosView())
            {
                // Call
                info.AfterCreate(view, pipingScenariosContext);

                // Assert
                Assert.AreSame(pipingFailureMechanism, view.PipingFailureMechanism);
            }
        }
    }
}