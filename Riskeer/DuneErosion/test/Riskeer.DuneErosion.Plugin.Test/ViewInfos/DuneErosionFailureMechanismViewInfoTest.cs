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
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.DuneErosion.Forms.Views;

namespace Riskeer.DuneErosion.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class DuneErosionFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private DuneErosionPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new DuneErosionPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(DuneErosionFailureMechanismView));
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
            Assert.AreEqual(typeof(DuneErosionHydraulicLoadsContext), info.DataType);
            Assert.AreEqual(typeof(DuneErosionHydraulicLoadsContext), info.ViewDataType);
        }

        [Test]
        public void GetViewName_WithContext_ReturnsNameOfFailureMechanism()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new DuneErosionHydraulicLoadsContext(failureMechanism, assessmentSection);

            // Call
            string viewName = info.GetViewName(null, context);

            // Assert
            Assert.AreEqual(failureMechanism.Name, viewName);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CreateInstance_WithContext_ReturnDuneFailureMechanismView()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();

            var context = new DuneErosionHydraulicLoadsContext(failureMechanism, assessmentSection);

            using (var testForm = new Form())
            {
                // Call
                var view = info.CreateInstance(context) as DuneErosionFailureMechanismView;

                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }
    }
}