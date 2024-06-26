﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveConditionsInputContext), info.DataType);
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveConditionsInputContextProperties), info.PropertyObjectType);
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
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            configureInputAction(calculation.InputParameters);

            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(calculation.InputParameters,
                                                                                  calculation,
                                                                                  assessmentSection,
                                                                                  Enumerable.Empty<ForeshoreProfile>());

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveConditionsInputContextProperties>(objectProperties);
                Assert.AreSame(context, objectProperties.Data);

                Assert.AreEqual(expectedAssessmentLevel, ((GrassCoverErosionOutwardsWaveConditionsInputContextProperties) objectProperties).AssessmentLevel);
            }
        }

        private static PropertyInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsInputContext));
        }
    }
}