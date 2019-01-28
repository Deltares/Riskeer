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
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Integration.Data.StandAlone;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class MacroStabilityOutwardsAssemblyCategoriesContextPropertyInfoTest
    {
        private RingtoetsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(FailureMechanismSectionAssemblyCategoriesProperties));
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
            Assert.AreEqual(typeof(MacroStabilityOutwardsAssemblyCategoriesContext), info.DataType);
        }

        [Test]
        public void CreateInstance_ValidArguments_ReturnProperties()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            var context = new MacroStabilityOutwardsAssemblyCategoriesContext(failureMechanism, assessmentSection,
                                                                              () => 0);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCategoriesProperties>(objectProperties);

                var properties = (FailureMechanismSectionAssemblyCategoriesProperties) objectProperties;
                AssemblyCategoryPropertiesTestHelper.AssertFailureMechanismSectionAssemblyCategoryProperties(
                    context.GetFailureMechanismSectionAssemblyCategoriesFunc(),
                    properties);
            }
        }
    }
}