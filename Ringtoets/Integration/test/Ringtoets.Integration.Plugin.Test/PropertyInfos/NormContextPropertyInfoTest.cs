// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class NormContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
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
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var viewCommands = mocks.Stub<IViewCommands>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ViewCommands).Return(viewCommands);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
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

            mocks.VerifyAll();
        }

        private static PropertyInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(NormProperties));
        }
    }
}