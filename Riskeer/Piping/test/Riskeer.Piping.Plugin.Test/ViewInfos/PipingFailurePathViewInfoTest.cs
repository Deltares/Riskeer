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

using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingFailurePathViewInfoTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingFailurePathView));
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
            Assert.AreEqual(typeof(PipingFailurePathContext), info.DataType);
            Assert.AreEqual(typeof(PipingFailurePathContext), info.ViewDataType);
        }

        [Test]
        public void GetViewName_WithPipingFailureMechanismContext_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            var context = new PipingFailurePathContext(failureMechanism, assessmentSection);

            // Call
            string viewName = info.GetViewName(null, context);

            // Assert
            Assert.AreEqual(failureMechanism.Name, viewName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AdditionalDataCheck_Always_ReturnTrueOnlyIfFailureMechanismInAssembly(bool inAssembly)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                InAssembly = inAssembly
            };

            var context = new PipingFailurePathContext(failureMechanism, assessmentSection);

            // Call
            bool result = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(inAssembly, result);
            mocks.VerifyAll();
        }        

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CreateInstance_WithContext_ReturnPipingFailurePathView()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new PipingFailureMechanism();

            var context = new PipingFailurePathContext(failureMechanism, assessmentSection);

            using (var testForm = new Form())
            {
                // Call
                var view = info.CreateInstance(context) as PipingFailurePathView;

                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new PipingFailureMechanism();

            var view = new PipingFailurePathView(failureMechanism, assessmentSection);

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
            var failureMechanism = new PipingFailureMechanism();

            var view = new PipingFailurePathView(failureMechanism, assessmentSection);

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
            var failureMechanism = new PipingFailureMechanism();
            var otherPipingFailureMechanism = new PipingFailureMechanism();

            var view = new PipingFailurePathView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, otherPipingFailureMechanism);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new PipingFailureMechanism();

            var view = new PipingFailurePathView(failureMechanism, assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }
    }
}