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

using System.Drawing;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsScenariosViewInfoTest
    {
        private GrassCoverErosionInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GrassCoverErosionInwardsScenariosView));
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
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ScenariosIcon, image);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnFalse()
        {
            // Setup
            var unrelatedFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                unrelatedFailureMechanism
            });
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionInwardsScenariosView(new CalculationGroup(), new GrassCoverErosionInwardsFailureMechanism()))
            {
                // Precondition
                Assert.AreNotSame(view.Data, unrelatedFailureMechanism.CalculationsGroup);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnTrue()
        {
            // Setup
            var relatedFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                relatedFailureMechanism
            });
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionInwardsScenariosView(relatedFailureMechanism.CalculationsGroup, relatedFailureMechanism))
            {
                // Precondition
                Assert.AreSame(view.Data, relatedFailureMechanism.CalculationsGroup);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnFalse()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsScenariosView(new CalculationGroup(), new GrassCoverErosionInwardsFailureMechanism()))
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
            using (var view = new GrassCoverErosionInwardsScenariosView(correspondingFailureMechanism.CalculationsGroup, correspondingFailureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, correspondingFailureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutGrassCoverErosionInwardsFailureMechanism_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
            mocks.ReplayAll();

            using (var view = new GrassCoverErosionInwardsScenariosView(new CalculationGroup(), new GrassCoverErosionInwardsFailureMechanism()))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            using (var view = new GrassCoverErosionInwardsScenariosView(failureMechanism.CalculationsGroup, failureMechanism))
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new GrassCoverErosionInwardsScenariosView(failureMechanism.CalculationsGroup, failureMechanism))
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
            using (var view = new GrassCoverErosionInwardsScenariosView(new CalculationGroup(), new GrassCoverErosionInwardsFailureMechanism()))
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