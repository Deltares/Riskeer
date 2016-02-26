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
using System.Collections.Generic;
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

            hydraRingConfiguration.AddHydraRingCalculation(new HydraulicCalculationDataImplementation(700004, 1.1));

            var expectedCreationScript = "DELETE FROM [HydraulicModels];" + Environment.NewLine +
                                         "INSERT INTO [HydraulicModels] VALUES (3, 2, 'WTI 2017');" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Sections];" + Environment.NewLine +
                                         "INSERT INTO [Sections] VALUES (999, 1, 1, 'HydraRingLocation', 'HydraRingLocation', NULL, NULL, NULL, NULL, 700004, 700004, 100, NULL, NULL);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [DesignTables];" + Environment.NewLine +
                                         "INSERT INTO [DesignTables] VALUES (999, 1, NULL, NULL, 2, 26, NULL, NULL, NULL, NULL, 0, 50, 1.1);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [Numerics];" + Environment.NewLine +
                                         "INSERT INTO [Numerics] VALUES (999, 1, NULL, NULL, 1, 1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 1, 10000, 20000, 0.1, -6, 6, 25);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [VariableDatas];" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (999, 1, NULL, NULL, 26, 2.2, 0, NULL, NULL, NULL, NULL, 0, NULL, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (999, 1, NULL, NULL, 26, 22.2, 0, NULL, NULL, NULL, NULL, 1, NULL, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (999, 1, NULL, NULL, 26, NULL, 2, 333.3, 444.4, NULL, NULL, 0, NULL, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (999, 1, NULL, NULL, 26, NULL, 2, 3333.3, NULL, NULL, NULL, 1, 4444.4, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (999, 1, NULL, NULL, 26, NULL, 4, 33333.3, 44444.4, 55555.5, NULL, 0, NULL, 300);" + Environment.NewLine +
                                         "INSERT INTO [VariableDatas] VALUES (999, 1, NULL, NULL, 26, NULL, 4, 333333.3, NULL, 555555.5, NULL, 1, 444444.4, 300);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [CalculationProfiles];" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (999, 1, 1.1, 2.2, 3.3);" + Environment.NewLine +
                                         "INSERT INTO [CalculationProfiles] VALUES (999, 2, 11.1, 22.2, 33.3);" + Environment.NewLine +
                                         Environment.NewLine +
                                         "DELETE FROM [SectionFaultTreeModels];" + Environment.NewLine +
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
                                         "INSERT INTO [Projects] VALUES (1, 'WTI 2017', 'Ringtoets calculation');" + Environment.NewLine;

            var creationScript = hydraRingConfiguration.GenerateDataBaseCreationScript();

            Assert.AreEqual(expectedCreationScript, creationScript);
        }

        private class HydraulicCalculationDataImplementation : HydraulicCalculationData
        {
            public HydraulicCalculationDataImplementation(int hydraulicBoundaryLocationId, double beta) : base(hydraulicBoundaryLocationId, beta)
            {
                
            }

            public override HydraRingFailureMechanismType FailureMechanismType
            {
                get
                {
                    return HydraRingFailureMechanismType.AssessmentLevel;
                }
            }

            public override IEnumerable<HydraRingVariable> Variables
            {
                get
                {
                    yield return new HydraRingVariableImplementation(26, HydraRingDistributionType.Deterministic, 2.2, HydraRingDeviationType.Standard, 3.3, 4.4, 5.5);
                    yield return new HydraRingVariableImplementation(26, HydraRingDistributionType.Deterministic, 22.2, HydraRingDeviationType.Variation, 33.3, 44.4, 55.5);
                    yield return new HydraRingVariableImplementation(26, HydraRingDistributionType.Normal, 222.2, HydraRingDeviationType.Standard, 333.3, 444.4, 555.5);
                    yield return new HydraRingVariableImplementation(26, HydraRingDistributionType.Normal, 2222.2, HydraRingDeviationType.Variation, 3333.3, 4444.4, 5555.5);
                    yield return new HydraRingVariableImplementation(26, HydraRingDistributionType.LogNormal, 22222.2, HydraRingDeviationType.Standard, 33333.3, 44444.4, 55555.5);
                    yield return new HydraRingVariableImplementation(26, HydraRingDistributionType.LogNormal, 222222.2, HydraRingDeviationType.Variation, 333333.3, 444444.4, 555555.5);
                }
            }

            public override IEnumerable<HydraRingProfilePoint> ProfilePoints
            {
                get
                {
                    yield return new HydraRingProfilePointDerivative(1.1, 2.2, 3.3);
                    yield return new HydraRingProfilePointDerivative(11.1, 22.2, 33.3);
                }
            }
        }

        private class HydraRingVariableImplementation : HydraRingVariable
        {
            public HydraRingVariableImplementation(int variableId, HydraRingDistributionType distributionType, double value, HydraRingDeviationType deviationType, double mean, double variability, double shift)
                : base(variableId, distributionType, value, deviationType, mean, variability, shift) {}
        }

        private class HydraRingProfilePointDerivative : HydraRingProfilePoint
        {
            private readonly double roughness;

            public HydraRingProfilePointDerivative(double x, double z, double roughness) : base(x, z)
            {
                this.roughness = roughness;
            }

            public override double Roughness
            {
                get
                {
                    return roughness;
                }
            }
        }
    }
}