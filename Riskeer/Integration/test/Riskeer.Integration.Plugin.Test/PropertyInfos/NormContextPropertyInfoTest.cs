// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Linq;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class NormContextPropertyInfoTest
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
                Assert.AreEqual(typeof(NormContext), info.DataType);
                Assert.AreEqual(typeof(NormProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsFailureMechanismContributionAsData()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var viewCommands = Substitute.For<IViewCommands>();
            IGui gui = StubFactory.CreateGuiStub();
            gui.ViewCommands.Returns(viewCommands);
            using (var plugin = new RiskeerPlugin())
            {
                plugin.Gui = gui;

                FailureMechanismContribution failureMechanismContribution = FailureMechanismContributionTestFactory.CreateFailureMechanismContribution();
                var context = new NormContext(failureMechanismContribution, assessmentSection);

                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<NormProperties>(objectProperties);
                Assert.AreSame(failureMechanismContribution, objectProperties.Data);
            }
        }

        private static PropertyInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(NormProperties));
        }
    }
}