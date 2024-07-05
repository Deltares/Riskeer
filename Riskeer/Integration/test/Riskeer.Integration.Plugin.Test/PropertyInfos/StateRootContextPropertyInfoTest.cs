﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class StateRootContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(StateRootContext), info.DataType);
                Assert.AreEqual(typeof(AssessmentSectionProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var viewCommands = mocks.Stub<IViewCommands>();
            IGui gui = StubFactory.CreateGuiStub(mocks);
            gui.Stub(g => g.ViewCommands).Return(viewCommands);
            mocks.ReplayAll();

            using (var plugin = new RiskeerPlugin())
            {
                plugin.Gui = gui;

                var context = new TestStateRootContext(assessmentSection);

                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<AssessmentSectionProperties>(objectProperties);
                Assert.AreSame(assessmentSection, objectProperties.Data);
            }

            mocks.VerifyAll();
        }

        private static PropertyInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(AssessmentSectionProperties));
        }

        private class TestStateRootContext : StateRootContext
        {
            public TestStateRootContext(AssessmentSection wrappedData) : base(wrappedData) {}
        }
    }
}