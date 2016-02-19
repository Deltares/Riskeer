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

using System;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HydraRing.Calculation.Test
{
    [TestFixture]
    public class HydraRingConfigurationTest
    {
        [Test]
        public void GenerateDataBaseCreationScript_HydraRingTimeIntegrationSchemeTypeUnspecified_ThrowsInvalidOperationException()
        {
            var hydraRingConfiguration = new HydraRingConfiguration
            {
                HydraRingUncertaintiesType = HydraRingUncertaintiesType.Model,
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(700003, "", 0, 0),
                HydraRingFailureMechanismType = HydraRingFailureMechanismType.AssessmentLevel
            };

            // Call
            TestDelegate test = () => hydraRingConfiguration.GenerateDataBaseCreationScript();

            // Assert
            Assert.Throws<InvalidOperationException>(test, "Cannot generate database creation script: HydraRingTimeIntegrationSchemeType unspecified.");
        }

        [Test]
        public void GenerateDataBaseCreationScript_HydraRingUncertaintiesTypeUnspecified_ThrowsInvalidOperationException()
        {
            var hydraRingConfiguration = new HydraRingConfiguration
            {
                HydraRingTimeIntegrationSchemeType = HydraRingTimeIntegrationSchemeType.FBC,
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(700003, "", 0, 0),
                HydraRingFailureMechanismType = HydraRingFailureMechanismType.AssessmentLevel
            };

            // Call
            TestDelegate test = () => hydraRingConfiguration.GenerateDataBaseCreationScript();

            // Assert
            Assert.Throws<InvalidOperationException>(test, "Cannot generate database creation script: HydraRingUncertaintiesType unspecified.");
        }

        [Test]
        public void GenerateDataBaseCreationScript_HydraulicBoundaryLocationUnspecified_ThrowsInvalidOperationException()
        {
            var hydraRingConfiguration = new HydraRingConfiguration
            {
                HydraRingTimeIntegrationSchemeType = HydraRingTimeIntegrationSchemeType.FBC,
                HydraRingUncertaintiesType = HydraRingUncertaintiesType.Model,
                HydraRingFailureMechanismType = HydraRingFailureMechanismType.AssessmentLevel
            };

            // Call
            TestDelegate test = () => hydraRingConfiguration.GenerateDataBaseCreationScript();

            // Assert
            Assert.Throws<InvalidOperationException>(test, "Cannot generate database creation script: HydraulicBoundaryLocation unspecified.");
        }

        [Test]
        public void GenerateDataBaseCreationScript_HydraRingFailureMechanismTypeUnspecified_ThrowsInvalidOperationException()
        {
            var hydraRingConfiguration = new HydraRingConfiguration
            {
                HydraRingTimeIntegrationSchemeType = HydraRingTimeIntegrationSchemeType.FBC,
                HydraRingUncertaintiesType = HydraRingUncertaintiesType.Model,
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(700003, "", 0, 0)
            };

            // Call
            TestDelegate test = () => hydraRingConfiguration.GenerateDataBaseCreationScript();

            // Assert
            Assert.Throws<InvalidOperationException>(test, "Cannot generate database creation script: HydraRingFailureMechanismType unspecified.");
        }

        [Test]
        public void GenerateDataBaseCreationScript_NonDefaultHydraRingConfiguration_ReturnsExpectedCreationScript()
        {
            var hydraRingConfiguration = new HydraRingConfiguration
            {
                HydraRingTimeIntegrationSchemeType = HydraRingTimeIntegrationSchemeType.NTI,
                HydraRingUncertaintiesType = HydraRingUncertaintiesType.Model,
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(700003, "", 0, 0),
                HydraRingFailureMechanismType = HydraRingFailureMechanismType.AssessmentLevel
            };

            var expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                         "INSERT INTO [HydraulicModels] VALUES (3, 2, 'WTI 2017');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Sections];" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (999, 1, 1, 'HydraRingLocation', 'HydraRingLocation', NULL, NULL, NULL, NULL, 700003, 700003, 100, NULL, NULL);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [DesignTables];" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (999, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Areas];" + Environment.NewLine +
                                         "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Projects];" + Environment.NewLine +
                                         "INSERT INTO [Projects] VALUES (1, 'Sprint', 'Hydra-Ring Sprint');" + Environment.NewLine;

            var creationScript = hydraRingConfiguration.GenerateDataBaseCreationScript();

            Assert.AreEqual(expectedCreationScript, creationScript);
        }
    }
}