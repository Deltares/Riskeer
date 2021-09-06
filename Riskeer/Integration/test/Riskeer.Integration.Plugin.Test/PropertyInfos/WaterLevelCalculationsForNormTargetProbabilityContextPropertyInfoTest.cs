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
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class WaterLevelCalculationsForNormTargetProbabilityContextPropertyInfoTest
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
                Assert.AreEqual(typeof(WaterLevelCalculationsForNormTargetProbabilityProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsDataCorrectly()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);

            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(calculationsForTargetProbability,
                                                                                    assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<WaterLevelCalculationsForNormTargetProbabilityProperties>(objectProperties);
                Assert.AreSame(calculationsForTargetProbability, objectProperties.Data);
            }

            mockRepository.VerifyAll();
        }

        private static PropertyInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(WaterLevelCalculationsForNormTargetProbabilityContext));
        }
    }
}