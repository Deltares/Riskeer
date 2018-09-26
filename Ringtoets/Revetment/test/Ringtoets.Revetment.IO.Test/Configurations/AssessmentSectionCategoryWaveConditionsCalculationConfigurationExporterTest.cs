// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.IO.Configurations;

namespace Ringtoets.Revetment.IO.Test.Configurations
{
    [TestFixture]
    public class AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporterTest : CustomCalculationConfigurationExporterDesignGuidelinesTestFixture<
        AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter,
        AssessmentSectionCategoryWaveConditionsCalculationConfigurationWriter,
        ICalculation<AssessmentSectionCategoryWaveConditionsInput>,
        AssessmentSectionCategoryWaveConditionsCalculationConfiguration>
    {
        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            var calculation1 = new TestWaveConditionsCalculation<AssessmentSectionCategoryWaveConditionsInput>(new AssessmentSectionCategoryWaveConditionsInput())
            {
                Name = "Calculation A",
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile("ForeshoreA"),
                    CategoryType = AssessmentSectionCategoryType.FactorizedSignalingNorm
                }
            };

            var calculation2 = new TestWaveConditionsCalculation<AssessmentSectionCategoryWaveConditionsInput>(new AssessmentSectionCategoryWaveConditionsInput())
            {
                Name = "PK001_0002 W1-6_4_1D1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HydraulicLocationA", 0, 0),
                    UseBreakWater = true,
                    CategoryType = AssessmentSectionCategoryType.FactorizedLowerLimitNorm
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

            string expectedXmlFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO,
                                                                    Path.Combine(
                                                                        nameof(AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter),
                                                                        "fullValidConfiguration.xml"));
            // Call and Assert
            WriteAndValidate(new[]
            {
                calculationGroup
            }, expectedXmlFilePath);
        }

        protected override ICalculation<AssessmentSectionCategoryWaveConditionsInput> CreateCalculation()
        {
            var random = new Random(21);
            return new TestWaveConditionsCalculation<AssessmentSectionCategoryWaveConditionsInput>(new AssessmentSectionCategoryWaveConditionsInput
            {
                CategoryType = random.NextEnumValue<AssessmentSectionCategoryType>()
            });
        }

        protected override AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter CallConfigurationFilePathConstructor(IEnumerable<ICalculationBase> calculations, string filePath)
        {
            return new AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter(calculations, filePath);
        }
    }
}