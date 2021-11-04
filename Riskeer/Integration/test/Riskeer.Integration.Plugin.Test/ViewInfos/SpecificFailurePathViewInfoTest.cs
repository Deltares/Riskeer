// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Properties;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class SpecificFailurePathViewInfoTest
    {
        private MockRepository mocks;
        private RiskeerPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RiskeerPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(SpecificFailurePathView));
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
            Assert.AreEqual(typeof(SpecificFailurePathContext), info.DataType);
            Assert.AreEqual(typeof(SpecificFailurePathContext), info.ViewDataType);
        }

        [Test]
        public void GetViewName_WithContext_ReturnsNameOfFailurePath()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePath = new SpecificFailurePath();
            var context = new SpecificFailurePathContext(failurePath, assessmentSection);

            // Call
            string viewName = info.GetViewName(null, context);

            // Assert
            Assert.AreEqual(failurePath.Name, viewName);
        }

        [Test]
        public void Image_Always_ReturnsGenericFailureMechanismIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.FailureMechanismIcon, image);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CreateInstance_WithContext_ReturnsSpecificFailurePathView()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failurePath = new SpecificFailurePath();
            var context = new SpecificFailurePathContext(failurePath, assessmentSection);

            using (var testForm = new Form())
            {
                // Call
                var view = info.CreateInstance(context) as SpecificFailurePathView;

                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreSame(failurePath, view.FailurePath);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AdditionalDataCheck_WithContext_ReturnsFailurePathInAssemblyValue(bool inAssembly)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failurePath = new SpecificFailurePath
            {
                InAssembly = inAssembly
            };
            var context = new SpecificFailurePathContext(failurePath, assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(inAssembly, additionalDataCheck);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            otherAssessmentSection.Stub(ass => ass.SpecificFailurePaths).Return(new ObservableList<IFailurePath>());
            otherAssessmentSection.Stub(ass => ass.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            mocks.ReplayAll();

            var failurePath = new SpecificFailurePath();
            var assessmentSection = new AssessmentSectionStub();
            var view = new SpecificFailurePathView(failurePath, assessmentSection);

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
            var failurePath = new SpecificFailurePath();
            var assessmentSection = new AssessmentSectionStub
            {
                SpecificFailurePaths =
                {
                    failurePath
                }
            };
            var view = new SpecificFailurePathView(failurePath, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedContext_ReturnsFalse()
        {
            // Setup
            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePath = new SpecificFailurePath();
            var assessmentSection = new AssessmentSectionStub();
            var view = new SpecificFailurePathView(failurePath, assessmentSection);

            var context = new SpecificFailurePathContext(new SpecificFailurePath(), otherAssessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, context);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedContext_ReturnsTrue()
        {
            // Setup
            var failurePath = new SpecificFailurePath();
            var assessmentSection = new AssessmentSectionStub();
            var view = new SpecificFailurePathView(failurePath, assessmentSection);

            var context = new SpecificFailurePathContext(failurePath, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, context);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToFailurePath_ReturnsFalse()
        {
            // Setup
            var failurePath = new SpecificFailurePath();
            var assessmentSection = new AssessmentSectionStub();
            var view = new SpecificFailurePathView(failurePath, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, new SpecificFailurePath());

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailurePath_ReturnsTrue()
        {
            // Setup
            var failurePath = new SpecificFailurePath();
            var assessmentSection = new AssessmentSectionStub();
            var view = new SpecificFailurePathView(failurePath, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, failurePath);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }
    }
}