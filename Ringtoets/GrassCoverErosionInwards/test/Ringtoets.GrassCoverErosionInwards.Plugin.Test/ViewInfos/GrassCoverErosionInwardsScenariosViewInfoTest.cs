﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsScenariosViewInfoTest
    {
        private GrassCoverErosionInwardsPlugin plugin;
        private ViewInfo info;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GrassCoverErosionInwardsScenariosView));
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
            Assert.AreEqual(typeof(GrassCoverErosionInwardsScenariosContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
        }

        [Test]
        public void GetViewName_Always_ReturnViewName()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsScenariosView())
            {
                var viewData = new CalculationGroup();

                // Call
                string viewName = info.GetViewName(view, viewData);

                // Assert
                Assert.AreEqual("Scenario's", viewName);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnWrappedData()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var context = new GrassCoverErosionInwardsScenariosContext(calculationGroup, failureMechanism);

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
            TestHelper.AssertImagesAreEqual(Resources.ScenariosIcon, image);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnFalse()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsScenariosView
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
            using (var view = new GrassCoverErosionInwardsScenariosView
            {
                Data = new CalculationGroup()
            })
            {
                var unrelatedFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
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
            var relatedFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            using (var view = new GrassCoverErosionInwardsScenariosView
            {
                Data = relatedFailureMechanism.CalculationsGroup
            })
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
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
            using (var view = new GrassCoverErosionInwardsScenariosView
            {
                Data = new CalculationGroup()
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, new GrassCoverErosionInwardsFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnTrue()
        {
            // Setup
            var correspondingFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            using (var view = new GrassCoverErosionInwardsScenariosView
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
        public void CloseForData_AssessmentSectionRemovedWithoutGrassCoverErosionInwardsFailureMechanism_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<GrassCoverErosionInwardsScenariosView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculationsGroupMock = mocks.StrictMock<CalculationGroup>();

            viewMock.Expect(vm => vm.Data).Return(calculationsGroupMock);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<GrassCoverErosionInwardsScenariosView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mocks.Stub<GrassCoverErosionInwardsFailureMechanism>();
            var calculationsGroupMock = mocks.StrictMock<CalculationGroup>();

            viewMock.Expect(vm => vm.Data).Return(calculationsGroupMock);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanismMock
            });

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<GrassCoverErosionInwardsScenariosView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mocks.Stub<GrassCoverErosionInwardsFailureMechanism>();

            viewMock.Expect(vm => vm.Data).Return(failureMechanismMock.CalculationsGroup);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanismMock
            });

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var view = new GrassCoverErosionInwardsScenariosView();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            view.Data = new CalculationGroup();

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(view, failureMechanism));
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var view = new GrassCoverErosionInwardsScenariosView();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            view.Data = failureMechanism.CalculationsGroup;

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.CloseForData(view, failureMechanism));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var view = new GrassCoverErosionInwardsScenariosView();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSectionMock);

            view.Data = failureMechanism.CalculationsGroup;

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(view, failureMechanismContext));
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var view = new GrassCoverErosionInwardsScenariosView();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock);

            view.Data = failureMechanism.CalculationsGroup;

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.CloseForData(view, failureMechanismContext));
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsScenariosView())
            {
                var group = new CalculationGroup();
                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var context = new GrassCoverErosionInwardsScenariosContext(group, failureMechanism);

                // Call
                info.AfterCreate(view, context);

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }
    }
}