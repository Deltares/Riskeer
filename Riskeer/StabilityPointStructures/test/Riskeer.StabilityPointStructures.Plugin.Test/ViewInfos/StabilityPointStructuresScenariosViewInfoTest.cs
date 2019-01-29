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

using System.Drawing;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class StabilityPointStructuresScenariosViewInfoTest
    {
        private StabilityPointStructuresPlugin plugin;
        private ViewInfo info;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            plugin = new StabilityPointStructuresPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(StabilityPointStructuresScenariosView));
            mocks = new MockRepository();
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
            Assert.AreEqual(typeof(StabilityPointStructuresScenariosContext), info.DataType);
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var context = new StabilityPointStructuresScenariosContext(calculationGroup, failureMechanism);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculationGroup, viewData);
        }

        [Test]
        public void Image_Always_ReturnScenariosIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ScenariosIcon, image);
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            using (var view = new StabilityPointStructuresScenariosView())
            {
                var group = new CalculationGroup();
                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var context = new StabilityPointStructuresScenariosContext(group, failureMechanism);

                // Call
                info.AfterCreate(view, context);

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnFalse()
        {
            // Setup
            using (var view = new StabilityPointStructuresScenariosView
            {
                Data = new CalculationGroup()
            })
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
                mocks.ReplayAll();

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnFalse()
        {
            // Setup
            using (var view = new StabilityPointStructuresScenariosView
            {
                Data = new CalculationGroup()
            })
            {
                var unrelatedFailureMechanism = new StabilityPointStructuresFailureMechanism();

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
                {
                    unrelatedFailureMechanism
                });
                mocks.ReplayAll();

                // Precondition
                Assert.AreNotSame(view.Data, unrelatedFailureMechanism.CalculationsGroup);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnTrue()
        {
            // Setup
            var relatedFailureMechanism = new StabilityPointStructuresFailureMechanism();

            using (var view = new StabilityPointStructuresScenariosView
            {
                Data = relatedFailureMechanism.CalculationsGroup
            })
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
                {
                    relatedFailureMechanism
                });
                mocks.ReplayAll();

                // Precondition
                Assert.AreSame(view.Data, relatedFailureMechanism.CalculationsGroup);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnFalse()
        {
            // Setup
            using (var view = new StabilityPointStructuresScenariosView
            {
                Data = new CalculationGroup()
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, new StabilityPointStructuresFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnTrue()
        {
            // Setup
            var correspondingFailureMechanism = new StabilityPointStructuresFailureMechanism();
            using (var view = new StabilityPointStructuresScenariosView
            {
                Data = correspondingFailureMechanism.CalculationsGroup
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, correspondingFailureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutStabilityPointStructuresFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
            mocks.ReplayAll();

            var view = new StabilityPointStructuresScenariosView
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
                new StabilityPointStructuresFailureMechanism()
            });

            mocks.ReplayAll();

            var view = new StabilityPointStructuresScenariosView
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            var view = new StabilityPointStructuresScenariosView
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
            var view = new StabilityPointStructuresScenariosView();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            view.Data = new CalculationGroup();

            // Call
            bool closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var view = new StabilityPointStructuresScenariosView();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            view.Data = failureMechanism.CalculationsGroup;

            // Call
            bool closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            var view = new StabilityPointStructuresScenariosView();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(new StabilityPointStructuresFailureMechanism(), assessmentSection);

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

            var view = new StabilityPointStructuresScenariosView();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            view.Data = failureMechanism.CalculationsGroup;

            // Call
            bool closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }
    }
}