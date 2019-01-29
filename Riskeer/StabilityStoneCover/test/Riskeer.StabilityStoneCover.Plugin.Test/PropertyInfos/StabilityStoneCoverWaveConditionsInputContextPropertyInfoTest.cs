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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
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
        [TestCaseSource(
            typeof(AssessmentSectionTestHelper),
            nameof(AssessmentSectionTestHelper.GetHydraulicBoundaryLocationCalculationConfigurationPerAssessmentSectionCategoryType))]
        public void CreateInstance_WithContextThatHasInputWithSpecificCategoryType_ExpectedProperties(
            IAssessmentSection assessmentSection,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            AssessmentSectionCategoryType categoryType,
            HydraulicBoundaryLocationCalculation expectedHydraulicBoundaryLocationCalculation)
        {
            // Setup
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = categoryType
                }
            };

            var context = new StabilityStoneCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                            calculation,
                                                                            assessmentSection,
                                                                            new ForeshoreProfile[0]);

            using (var plugin = new StabilityStoneCoverPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<StabilityStoneCoverWaveConditionsInputContextProperties>(objectProperties);
                Assert.AreSame(context, objectProperties.Data);

                Assert.AreEqual(expectedHydraulicBoundaryLocationCalculation.Output.Result,
                                ((StabilityStoneCoverWaveConditionsInputContextProperties) objectProperties).AssessmentLevel);
            }
        }

        private static PropertyInfo GetInfo(StabilityStoneCoverPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(StabilityStoneCoverWaveConditionsInputContext));
        }
    }
}