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
using System.Threading;
using System.Windows.Forms;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailurePathViewInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GrassCoverErosionInwardsFailurePathView));
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
            Assert.AreEqual(typeof(GrassCoverErosionInwardsFailurePathContext), info.DataType);
            Assert.AreEqual(typeof(GrassCoverErosionInwardsFailurePathContext), info.ViewDataType);
        }

        [Test]
        public void GetViewName_WithGrassCoverErosionInwardsFailureMechanismContext_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var context = new GrassCoverErosionInwardsFailurePathContext(failureMechanism, assessmentSection);

            // Call
            string viewName = info.GetViewName(null, context);

            // Assert
            Assert.AreEqual(failureMechanism.Name, viewName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AdditionalDataCheck_Always_ReturnTrueOnlyIfFailureMechanismInAssembly(bool inAssembly)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                InAssembly = inAssembly
            };

            var context = new GrassCoverErosionInwardsFailurePathContext(failureMechanism, assessmentSection);

            // Call
            bool result = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(inAssembly, result);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var view = new GrassCoverErosionInwardsFailurePathView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, otherAssessmentSection);

            // Assert
            Assert.IsFalse(closeForData);

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var view = new GrassCoverErosionInwardsFailurePathView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var otherGrassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var view = new GrassCoverErosionInwardsFailurePathView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, otherGrassCoverErosionInwardsFailureMechanism);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var view = new GrassCoverErosionInwardsFailurePathView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CreateInstance_WithContext_ReturnGrassCoverErosionInwardsFailurePathView()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var context = new GrassCoverErosionInwardsFailurePathContext(failureMechanism, assessmentSection);

            using (var testForm = new Form())
            {
                // Call
                var view = info.CreateInstance(context) as GrassCoverErosionInwardsFailurePathView;

                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }
    }
}