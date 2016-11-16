// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.Views;
using ClosingStructuresDataResources = Ringtoets.ClosingStructures.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private ClosingStructuresPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(ClosingStructuresFailureMechanismView));
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
            Assert.AreEqual(typeof(ClosingStructuresFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(ClosingStructuresFailureMechanismContext), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, info.Image);
        }

        [Test]
        public void GetViewName_WithClosingStructuresFailureMechanism_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            using (var view = new ClosingStructuresFailureMechanismView())
            {
                // Call
                string viewName = info.GetViewName(view, failureMechanismContext);

                // Assert
                Assert.AreEqual(failureMechanism.Name, viewName);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            var otherAssessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            using (var view = new ClosingStructuresFailureMechanismView
            {
                Data = failureMechanismContext
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, otherAssessmentSectionMock);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            using (var view = new ClosingStructuresFailureMechanismView
            {
                Data = failureMechanismContext
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSectionMock);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var otherClosingStructuresFailureMechanism = new ClosingStructuresFailureMechanism();

            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            using (var view = new ClosingStructuresFailureMechanismView
            {
                Data = failureMechanismContext
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, otherClosingStructuresFailureMechanism);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            using (var view = new ClosingStructuresFailureMechanismView
            {
                Data = failureMechanismContext
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AdditionalDataCheck_Always_ReturnTrueOnlyIfFailureMechanismRelevant(bool isRelevant)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                IsRelevant = isRelevant
            };

            var context = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool result = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(isRelevant, result);
            mocks.VerifyAll();
        }
    }
}