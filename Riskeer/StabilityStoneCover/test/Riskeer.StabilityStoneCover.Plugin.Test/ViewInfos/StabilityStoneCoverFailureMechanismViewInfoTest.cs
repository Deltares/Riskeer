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
using System.Threading;
using System.Windows.Forms;
using Core.Common.TestUtil;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Plugin.Test.ViewInfos
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
            Assert.AreEqual(typeof(StabilityStoneCoverHydraulicLoadsContext), info.DataType);
            Assert.AreEqual(typeof(StabilityStoneCoverHydraulicLoadsContext), info.ViewDataType);
        }

        [Test]
        public void GetViewName_WithContext_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverHydraulicLoadsContext(failureMechanism, assessmentSection);

            // Call
            string viewName = info.GetViewName(null, context);

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
        [Apartment(ApartmentState.STA)]
        public void CreateInstance_WithContext_ReturnStabilityStoneCoverFailureMechanismView()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var context = new StabilityStoneCoverHydraulicLoadsContext(failureMechanism, assessmentSection);

            using (var testForm = new Form())
            {
                // Call
                var view = info.CreateInstance(context) as StabilityStoneCoverFailureMechanismView;

                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }
    }
}