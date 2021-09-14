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

using System;
using System.Linq;
using Core.Common.Base.Data;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;

namespace Riskeer.StabilityStoneCover.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsInputContextPropertyInfoTest
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
                Assert.AreEqual(typeof(StabilityStoneCoverWaveConditionsInputContext), info.DataType);
                Assert.AreEqual(typeof(StabilityStoneCoverWaveConditionsInputContextProperties), info.PropertyObjectType);
            }
        }

        [Test]
        [TestCaseSource(typeof(WaveConditionsInputTestHelper),
                        nameof(WaveConditionsInputTestHelper.GetAssessmentLevelConfigurationPerWaterLevelType))]
        public void CreateInstance_WithContextThatHasInputWithSpecificWaterLevelType_ExpectedProperties(
            IAssessmentSection assessmentSection,
            Action<WaveConditionsInput> configureInputAction,
            RoundedDouble expectedAssessmentLevel)
        {
            // Setup
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();

            configureInputAction(calculation.InputParameters);

            var context = new StabilityStoneCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                            calculation,
                                                                            assessmentSection,
                                                                            Array.Empty<ForeshoreProfile>());

            using (var plugin = new StabilityStoneCoverPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<StabilityStoneCoverWaveConditionsInputContextProperties>(objectProperties);
                Assert.AreSame(context, objectProperties.Data);

                Assert.AreEqual(expectedAssessmentLevel, ((StabilityStoneCoverWaveConditionsInputContextProperties) objectProperties).AssessmentLevel);
            }
        }

        private static PropertyInfo GetInfo(StabilityStoneCoverPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(StabilityStoneCoverWaveConditionsInputContext));
        }
    }
}