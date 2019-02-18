// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.IO.Configurations;

namespace Riskeer.StabilityStoneCover.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationConfigurationExporterTest : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
        StabilityStoneCoverWaveConditionsCalculationConfigurationExporter, StabilityStoneCoverWaveConditionsCalculationConfigurationWriter,
        ICalculation<StabilityStoneCoverWaveConditionsInput>, StabilityStoneCoverWaveConditionsCalculationConfiguration>
    {
        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            var calculation1 = new TestWaveConditionsCalculation<StabilityStoneCoverWaveConditionsInput>(new StabilityStoneCoverWaveConditionsInput())
            {
                Name = "Calculation A",
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile("ForeshoreA"),
                    CategoryType = AssessmentSectionCategoryType.FactorizedSignalingNorm
                }
            };

            var calculation2 = new TestWaveConditionsCalculation<StabilityStoneCoverWaveConditionsInput>(new StabilityStoneCoverWaveConditionsInput())
            {
                Name = "PK001_0002 W1-6_4_1D1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HydraulicLocationA", 0, 0),
                    UseBreakWater = true,
                    CategoryType = AssessmentSectionCategoryType.FactorizedLowerLimitNorm,
                    CalculationType = StabilityStoneCoverWaveConditionsCalculationType.Blocks
                }
            };

            var calculationGroup2 = new CalculationGroup
            {
                Name = "PK001_0002",
                Children =
                {
                    calculation2
                }
            };

            var calculationGroup = new CalculationGroup
            {
                Name = "PK001_0001",
                Children =
                {
                    calculation1,
                    calculationGroup2
                }
            };

            string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.StabilityStoneCover.IO,
                                                                    Path.Combine(
                                                                        nameof(StabilityStoneCoverWaveConditionsCalculationConfigurationExporter),
                                                                        "fullValidConfiguration.xml"));
            // Call and Assert
            WriteAndValidate(new[]
            {
                calculationGroup
            }, expectedXmlFilePath);
        }

        protected override ICalculation<StabilityStoneCoverWaveConditionsInput> CreateCalculation()
        {
            var random = new Random(21);
            return new TestWaveConditionsCalculation<StabilityStoneCoverWaveConditionsInput>(new StabilityStoneCoverWaveConditionsInput
            {
                CategoryType = random.NextEnumValue<AssessmentSectionCategoryType>(),
                CalculationType = random.NextEnumValue<StabilityStoneCoverWaveConditionsCalculationType>()
            });
        }

        protected override StabilityStoneCoverWaveConditionsCalculationConfigurationExporter CallConfigurationFilePathConstructor(
            IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new StabilityStoneCoverWaveConditionsCalculationConfigurationExporter(calculations, filePath);
        }
    }
}