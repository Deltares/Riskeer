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
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PropertyClasses.StandAlone;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class StandAloneFailureMechanismPropertyInfoTest
    {
        private RingtoetsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(StandAloneFailureMechanismProperties));
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
            Assert.AreEqual(typeof(IFailureMechanismContext<IFailureMechanism>), info.DataType);
            Assert.AreEqual(typeof(StandAloneFailureMechanismProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_WithContext_NewPropertiesWithFailureMechanismAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var context = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            IObjectProperties objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<StandAloneFailureMechanismProperties>(objectProperties);
            Assert.AreSame(failureMechanism, objectProperties.Data);
            mocks.VerifyAll();
        }

        private class TestFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public TestFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) : base(wrappedFailureMechanism, parent) {}
        }
    }
}