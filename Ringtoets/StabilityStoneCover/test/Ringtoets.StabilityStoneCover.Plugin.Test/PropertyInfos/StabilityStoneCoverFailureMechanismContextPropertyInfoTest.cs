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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;

namespace Ringtoets.StabilityStoneCover.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new StabilityStoneCoverPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(StabilityStoneCoverFailureMechanismContext), info.DataType);
                Assert.AreEqual(typeof(StabilityStoneCoverFailureMechanismProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_Always_SetsFailureMechanismAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverFailureMechanismContext(failureMechanism, assessmentSection);

            using (var plugin = new StabilityStoneCoverPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                var objectProperties= info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<StabilityStoneCoverFailureMechanismProperties>(objectProperties);
                Assert.AreSame(failureMechanism, objectProperties.Data);
            }
            mocks.VerifyAll();
        }

        private static PropertyInfo GetInfo(StabilityStoneCoverPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(StabilityStoneCoverFailureMechanismContext));
        }
    }
}