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

using System.Drawing;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Components.Gis.Features;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Forms.PresentationObjects.StandAlone;
using Riskeer.Integration.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingStructureFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private RiskeerPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RiskeerPlugin();
            info = plugin.GetViewInfos().First(
                tni => tni.ViewType == typeof(FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism,
                           PipingStructureFailureMechanismSectionResult>));
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
            Assert.AreEqual(typeof(PipingStructureFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(PipingStructureFailureMechanism), info.ViewDataType);
        }

        [Test]
        public void GetViewName_WithFailureMechanismContext_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingStructureFailureMechanism();
            var failureMechanismContext = new PipingStructureFailureMechanismContext(failureMechanism, assessmentSection);

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
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.FailureMechanismIcon, image);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingStructureFailureMechanism();

            using (FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult> view =
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
            var failureMechanism = new PipingStructureFailureMechanism();

            using (FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult> view =
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
            var failureMechanism = new PipingStructureFailureMechanism();
            var otherFailureMechanism = new PipingStructureFailureMechanism();

            using (FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult> view =
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
            var failureMechanism = new PipingStructureFailureMechanism();

            using (FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult> view =
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

            var failureMechanism = new PipingStructureFailureMechanism
            {
                IsRelevant = isRelevant
            };

            var context = new PipingStructureFailureMechanismContext(failureMechanism, assessmentSection);

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
            var failureMechanism = new PipingStructureFailureMechanism();

            var context = new PipingStructureFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism,
                PipingStructureFailureMechanismSectionResult>>(view);

            var failureMechanismView = (FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism,
                PipingStructureFailureMechanismSectionResult>) view;
            Assert.AreSame(failureMechanism, failureMechanismView.FailureMechanism);
            Assert.AreSame(assessmentSection, failureMechanismView.AssessmentSection);
        }

        private static FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult> CreateView(
            PipingStructureFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism,
                PipingStructureFailureMechanismSectionResult>(
                failureMechanism,
                assessmentSection,
                Enumerable.Empty<MapFeature>,
                Enumerable.Empty<MapFeature>,
                Enumerable.Empty<MapFeature>,
                Enumerable.Empty<MapFeature>);
        }
    }
}