﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.PresentationObjects.StandAlone;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class WaterPressureAsphaltCoverFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsPlugin();
            info = plugin.GetViewInfos().First(
                tni => tni.ViewType == typeof(FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism,
                           WaterPressureAsphaltCoverFailureMechanismSectionResult>));
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
            Assert.AreEqual(typeof(WaterPressureAsphaltCoverFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(WaterPressureAsphaltCoverFailureMechanism), info.ViewDataType);
        }

        [Test]
        public void GetViewName_WithFailureMechanismContext_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();
            var failureMechanismContext = new WaterPressureAsphaltCoverFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            string viewName = info.GetViewName(null, failureMechanismContext);

            // Assert
            Assert.AreEqual(failureMechanism.Name, viewName);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, image);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

            using (FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult> view =
                CreateView(failureMechanism, assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, otherAssessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

            using (FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult> view =
                CreateView(failureMechanism, assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();
            var otherFailureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

            using (FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult> view =
                CreateView(failureMechanism, assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, otherFailureMechanism);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

            using (FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult> view =
                CreateView(failureMechanism, assessmentSection))
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AdditionalDataCheck_Always_ReturnTrueOnlyIfFailureMechanismRelevant(bool isRelevant)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism
            {
                IsRelevant = isRelevant
            };

            var context = new WaterPressureAsphaltCoverFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool result = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(isRelevant, result);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithData_ReturnFailureMechanismView()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

            var context = new WaterPressureAsphaltCoverFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism,
                WaterPressureAsphaltCoverFailureMechanismSectionResult>>(view);

            var failureMechanismView = (FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism,
                WaterPressureAsphaltCoverFailureMechanismSectionResult>) view;
            Assert.AreSame(failureMechanism, failureMechanismView.FailureMechanism);
            Assert.AreSame(assessmentSection, failureMechanismView.AssessmentSection);
        }

        private static FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult> CreateView(
            WaterPressureAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism,
                WaterPressureAsphaltCoverFailureMechanismSectionResult>(
                failureMechanism,
                assessmentSection,
                Enumerable.Empty<MapFeature>,
                Enumerable.Empty<MapFeature>,
                Enumerable.Empty<MapFeature>);
        }
    }
}