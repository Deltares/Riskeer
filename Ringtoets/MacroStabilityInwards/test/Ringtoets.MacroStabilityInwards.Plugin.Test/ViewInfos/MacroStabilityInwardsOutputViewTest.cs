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
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Plugin.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Forms.Views;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class MacroStabilityInwardsOutputViewTest : ShouldCloseViewWithCalculationDataTester
    {
        private MockRepository mocks;
        private MacroStabilityInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(MacroStabilityInwardsOutputView));
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
            Assert.AreEqual(typeof(MacroStabilityInwardsOutputContext), info.DataType);
            Assert.AreEqual(typeof(MacroStabilityInwardsCalculationScenario), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(Resources.MacroStabilityInwardsInputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsInputResourceName()
        {
            // Setup
            using (var view = new MacroStabilityInwardsOutputView())
            {
                var calculationScenario = new MacroStabilityInwardsCalculationScenario();

                // Call
                string viewName = info.GetViewName(view, calculationScenario);

                // Assert
                Assert.AreEqual("Resultaat", viewName);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculation()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var context = new MacroStabilityInwardsOutputContext(calculation);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculation, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_Always_CreatesMacroStabilityInwardsOutputView()
        {
            // Call
            IView view = info.CreateInstance(null);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsOutputView>(view);
        }

        protected override bool ShouldCloseMethod(IView view, object o)
        {
            return info.CloseForData(view, o);
        }

        protected override IView GetView()
        {
            return new MacroStabilityInwardsOutputView();
        }
    }
}