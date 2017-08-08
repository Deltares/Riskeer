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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private StabilityStoneCoverPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new StabilityStoneCoverPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(StabilityStoneCoverFailureMechanismView));
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
            Assert.AreEqual(typeof(StabilityStoneCoverFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(StabilityStoneCoverFailureMechanismContext), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, info.Image);
        }

        [Test]
        public void GetViewName_WithStabilityStoneCoverFailureMechanism_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var stabilityStoneCoverFailureMechanismContext = new StabilityStoneCoverFailureMechanismContext(stabilityStoneCoverFailureMechanism, assessmentSection);

            using (var view = new StabilityStoneCoverFailureMechanismView())
            {
                // Call
                string viewName = info.GetViewName(view, stabilityStoneCoverFailureMechanismContext);

                // Assert
                Assert.AreEqual(stabilityStoneCoverFailureMechanism.Name, viewName);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var stabilityStoneCoverFailureMechanismContext = new StabilityStoneCoverFailureMechanismContext(stabilityStoneCoverFailureMechanism, assessmentSection);

            using (var view = new StabilityStoneCoverFailureMechanismView
            {
                Data = stabilityStoneCoverFailureMechanismContext
            })
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
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var stabilityStoneCoverFailureMechanismContext = new StabilityStoneCoverFailureMechanismContext(stabilityStoneCoverFailureMechanism, assessmentSection);

            using (var view = new StabilityStoneCoverFailureMechanismView
            {
                Data = stabilityStoneCoverFailureMechanismContext
            })
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
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var otherStabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var stabilityStoneCoverFailureMechanismContext = new StabilityStoneCoverFailureMechanismContext(stabilityStoneCoverFailureMechanism, assessmentSection);

            using (var view = new StabilityStoneCoverFailureMechanismView
            {
                Data = stabilityStoneCoverFailureMechanismContext
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, otherStabilityStoneCoverFailureMechanism);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var stabilityStoneCoverFailureMechanismContext = new StabilityStoneCoverFailureMechanismContext(stabilityStoneCoverFailureMechanism, assessmentSection);

            using (var view = new StabilityStoneCoverFailureMechanismView
            {
                Data = stabilityStoneCoverFailureMechanismContext
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, stabilityStoneCoverFailureMechanism);

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

            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                IsRelevant = isRelevant
            };

            var context = new StabilityStoneCoverFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool result = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(isRelevant, result);
            mocks.VerifyAll();
        }
    }
}