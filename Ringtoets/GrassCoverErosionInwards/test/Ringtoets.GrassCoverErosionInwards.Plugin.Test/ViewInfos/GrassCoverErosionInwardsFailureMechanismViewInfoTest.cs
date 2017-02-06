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

using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GrassCoverErosionInwardsFailureMechanismView));
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
            Assert.AreEqual(typeof(GrassCoverErosionInwardsFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(GrassCoverErosionInwardsFailureMechanismContext), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, info.Image);
        }

        [Test]
        public void GetViewName_WithGrassCoverErosionInwardsFailureMechanism_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var grassCoverErosionInwardsFailureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(grassCoverErosionInwardsFailureMechanism, assessmentSectionStub);

            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                // Call
                string viewName = info.GetViewName(view, grassCoverErosionInwardsFailureMechanismContext);

                // Assert
                Assert.AreEqual(grassCoverErosionInwardsFailureMechanism.Name, viewName);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var grassCoverErosionInwardsFailureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(grassCoverErosionInwardsFailureMechanism, assessmentSection);

            using (var view = new GrassCoverErosionInwardsFailureMechanismView
            {
                Data = grassCoverErosionInwardsFailureMechanismContext
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
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var grassCoverErosionInwardsFailureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(grassCoverErosionInwardsFailureMechanism, assessmentSectionStub);

            using (var view = new GrassCoverErosionInwardsFailureMechanismView
            {
                Data = grassCoverErosionInwardsFailureMechanismContext
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSectionStub);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var otherGrassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var grassCoverErosionInwardsFailureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(grassCoverErosionInwardsFailureMechanism, assessmentSectionStub);

            using (var view = new GrassCoverErosionInwardsFailureMechanismView
            {
                Data = grassCoverErosionInwardsFailureMechanismContext
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, otherGrassCoverErosionInwardsFailureMechanism);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var grassCoverErosionInwardsFailureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(grassCoverErosionInwardsFailureMechanism, assessmentSectionStub);

            using (var view = new GrassCoverErosionInwardsFailureMechanismView
            {
                Data = grassCoverErosionInwardsFailureMechanismContext
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, grassCoverErosionInwardsFailureMechanism);

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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                IsRelevant = isRelevant
            };

            var context = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool result = info.AdditionalDataCheck(context);

            // Assert
            Assert.AreEqual(isRelevant, result);
            mocks.VerifyAll();
        }
    }
}