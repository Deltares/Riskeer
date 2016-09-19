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
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNull(info.AdditionalDataCheck);
                Assert.IsNull(info.AfterCreate);
                Assert.AreEqual(typeof(WaveImpactAsphaltCoverFailureMechanismContext), info.DataType);
                Assert.AreEqual(typeof(WaveImpactAsphaltCoverFailureMechanismProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void GetObjectPropertiesData_Always_ReturnsHydraulicBoundaryDatabase()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var context = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, assessmentSectionMock);

            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                var objectPropertiesData = info.GetObjectPropertiesData(context);

                // Assert
                Assert.AreSame(failureMechanism, objectPropertiesData);
            }
            mocks.VerifyAll();
        }

        private static PropertyInfo GetInfo(WaveImpactAsphaltCoverPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(WaveImpactAsphaltCoverFailureMechanismContext));
        }
    }
}