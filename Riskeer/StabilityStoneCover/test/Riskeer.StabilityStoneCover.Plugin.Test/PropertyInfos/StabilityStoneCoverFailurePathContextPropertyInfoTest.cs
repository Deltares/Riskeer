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

using System.Linq;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;

namespace Riskeer.StabilityStoneCover.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class StabilityStoneCoverFailurePathContextPropertyInfoTest
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
                Assert.AreEqual(typeof(StabilityStoneCoverFailurePathContext), info.DataType);
                Assert.AreEqual(typeof(StabilityStoneCoverFailurePathProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsFailureMechanismAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverFailurePathContext(failureMechanism, assessmentSection);

            using (var plugin = new StabilityStoneCoverPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<StabilityStoneCoverFailurePathProperties>(objectProperties);
                Assert.AreSame(failureMechanism, objectProperties.Data);
            }

            mocks.VerifyAll();
        }

        private static PropertyInfo GetInfo(StabilityStoneCoverPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(StabilityStoneCoverFailurePathContext));
        }
    }
}