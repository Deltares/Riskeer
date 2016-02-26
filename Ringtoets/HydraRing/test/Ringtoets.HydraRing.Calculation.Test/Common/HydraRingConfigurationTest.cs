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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Common;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Types;

namespace Ringtoets.HydraRing.Calculation.Test.Common
{
    [TestFixture]
    public class HydraRingConfigurationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hydraRingConfiguration = new HydraRingConfiguration(HydraRingTimeIntegrationSchemeType.NTI, HydraRingUncertaintiesType.Model);

            // Assert
            Assert.AreEqual(HydraRingTimeIntegrationSchemeType.NTI, hydraRingConfiguration.TimeIntegrationSchemeType);
            Assert.AreEqual(HydraRingUncertaintiesType.Model, hydraRingConfiguration.UncertaintiesType);
        }

        [Test]
        public void GenerateDataBaseCreationScript_NonDefaultHydraRingConfiguration_ReturnsExpectedCreationScript()
        {
            var hydraRingConfiguration = new HydraRingConfiguration(HydraRingTimeIntegrationSchemeType.NTI, HydraRingUncertaintiesType.Model);

            hydraRingConfiguration.AddHydraRingCalculation(new AssessmentLevelCalculationData(700004, 2.2));

            var expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                         "INSERT INTO [HydraulicModels] VALUES (3, 2, 'WTI 2017');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Sections];" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (999, 1, 1, 'HydraRingLocation', 'HydraRingLocation', NULL, NULL, NULL, NULL, 700004, 700004, 100, NULL, NULL);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [DesignTables];" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (999, 1, NULL, NULL, 2, 26, NULL, NULL, NULL, NULL, 0, 50, 2.2);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Numerics];" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (999, 1, NULL, NULL, 1, 1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 10000, 20000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (999, 1, NULL, NULL, 26, 0, 0, NULL, NULL, NULL, NULL, 0, NULL, 300);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Areas];" + Environment.NewLine +
                                         "INSERT INTO [Areas] VALUES (1, '1', 'Nederland');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Projects];" + Environment.NewLine +
                                         "INSERT INTO [Projects] VALUES (1, 'WTI 2017', 'Ringtoets calculation');" + Environment.NewLine;

            var creationScript = hydraRingConfiguration.GenerateDataBaseCreationScript();

            Assert.AreEqual(expectedCreationScript, creationScript);
        }
    }
}