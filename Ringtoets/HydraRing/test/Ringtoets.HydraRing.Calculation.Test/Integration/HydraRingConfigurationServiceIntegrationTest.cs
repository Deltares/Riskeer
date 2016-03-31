// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.Test.Integration
{
    [TestFixture]
    public class HydraRingConfigurationServiceIntegrationTest
    {
        [Test]
        public void GenerateDataBaseCreationScript_HydraRingConfigurationWithAssessmentLevelCalculation_ReturnsExpectedCreationScript()
        {
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All);

            hydraRingConfigurationService.AddHydraRingCalculationInput(new AssessmentLevelCalculationInput(700004, 10000));

            var expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                         "INSERT INTO [HydraulicModels] VALUES (1, 1, 'WTI 2017');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Sections];" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (700004, 1, 1, '700004', '700004', 0, 0, 0, 0, 700004, 700004, 100, 0, 0);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [DesignTables];" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (700004, 1, 1, 1, 2, 26, 0, 0, 0, 0, 5, 15, 3.71901648545568);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Numerics];" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (700004, 1, 1, 1, 1, 1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 20000, 100000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (700004, 1, 1, 1, 26, 0, 0, 0, 0, 0, 0, 0, 0, 300);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                         "INSERT INTO [SectionFaultTreeModels] VALUES (700004, 1, 1, 1, 1);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Fetches];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Profiles];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Forelands];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [ProbabilityAlternatives];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SetUpHeights];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalcWindDirections];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Swells];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Areas];" + Environment.NewLine +
                                         "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Projects];" + Environment.NewLine +
                                         "INSERT INTO [Projects] VALUES (1, 'WTI 2017', 'Ringtoets calculation');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Breakwaters];" + Environment.NewLine;

            var creationScript = hydraRingConfigurationService.GenerateDataBaseCreationScript();

            Assert.AreEqual(expectedCreationScript, creationScript);
        }

        [Test]
        public void GenerateDataBaseCreationScript_HydraRingConfigurationWithOvertoppingCalculation_ReturnsExpectedCreationScript()
        {
            var hydraRingConfigurationService = new HydraRingConfigurationService("34-1", HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.None);
            int hydraulicBoundaryLocationId = 700004;

            var hydraRingSection = new HydraRingSection(hydraulicBoundaryLocationId, "700004", 2.2, 3.3, 4.4, 5.5, 6.6, 7.7);
            var profilePoints = new List<HydraRingProfilePoint>
            {
                new HydraRingProfilePoint(1.1, 2.2)
            };
            var forelandPoints = new List<HydraRingForelandPoint>
            {
                new HydraRingForelandPoint(1.1, 2.2)
            };

            hydraRingConfigurationService.AddHydraRingCalculationInput(new OvertoppingCalculationInput(hydraulicBoundaryLocationId, hydraRingSection, profilePoints, forelandPoints));

            var expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                         "INSERT INTO [HydraulicModels] VALUES (1, 0, 'WTI 2017');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Sections];" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (700004, 1, 1, '700004', '700004', 2.2, 3.3, 5.5, 6.6, 700004, 700004, 100, 7.7, 4.4);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [DesignTables];" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (700004, 101, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Numerics];" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (700004, 101, 1, 1, 102, 1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 20000, 100000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (700004, 101, 1, 1, 103, 1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 20000, 100000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (700004, 1, 1.1, 2.2, 1);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
                                         "INSERT INTO [SectionFaultTreeModels] VALUES (700004, 101, 1, 1, 1017);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionSubMechanismModels];" + Environment.NewLine +
                                         "INSERT INTO [SectionSubMechanismModels] VALUES (700004, 101, 1, 1, 102, 94);" + Environment.NewLine +
                                         "INSERT INTO [SectionSubMechanismModels] VALUES (700004, 101, 1, 1, 103, 95);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Fetches];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [AreaPoints];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [PresentationSections];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Profiles];" + Environment.NewLine +
                                         "INSERT INTO [Profiles] VALUES (700004, 1, 1.1, 2.2, 1);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [ForelandModels];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Forelands];" + Environment.NewLine +
                                         "INSERT INTO [Forelands] VALUES (700004, 1, 1.1, 2.2);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [ProbabilityAlternatives];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SetUpHeights];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalcWindDirections];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Swells];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [WaveReductions];" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Areas];" + Environment.NewLine +
                                         "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Projects];" + Environment.NewLine +
                                         "INSERT INTO [Projects] VALUES (1, 'WTI 2017', 'Ringtoets calculation');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Breakwaters];" + Environment.NewLine;

            var creationScript = hydraRingConfigurationService.GenerateDataBaseCreationScript();

            Assert.AreEqual(expectedCreationScript, creationScript);
        }
    }
}