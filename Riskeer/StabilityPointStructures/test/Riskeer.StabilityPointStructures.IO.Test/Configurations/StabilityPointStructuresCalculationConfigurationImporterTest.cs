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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.IO.Configurations;

namespace Riskeer.StabilityPointStructures.IO.Test.Configurations
{
    [TestFixture]
    public class StabilityPointStructuresCalculationConfigurationImporterTest
    {
        private readonly string importerPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.StabilityPointStructures.IO,
                                                                          nameof(StabilityPointStructuresCalculationConfigurationImporter));

        private static IEnumerable<TestCaseData> ValidConfigurationInvalidData
        {
            get
            {
                const string testNameFormat = "Import_InvalidData({0:80})";
                yield return new TestCaseData(
                        "validConfigurationAllowedLevelIncreaseStorageMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'peilverhogingkomberging'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationAllowedLevelIncreaseStorageStandardDeviationInvalid.xml",
                        "Een standaardafwijking van '-0,01' is ongeldig voor stochast 'peilverhogingkomberging'. " +
                        "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationAllowedLevelIncreaseStorageVariationCoefficient.xml",
                        "Indien voor parameter 'peilverhogingkomberging' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                        "Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationAreaFlowAperturesMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'doorstroomoppervlak'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationAreaFlowAperturesStandardDeviationInvalid.xml",
                        "Een standaardafwijking van '-0,01' is ongeldig voor stochast 'doorstroomoppervlak'. " +
                        "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationAreaFlowAperturesVariationCoefficient.xml",
                        "Indien voor parameter 'doorstroomoppervlak' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                        "Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationBankWidthStandardDeviationInvalid.xml",
                        "Een standaardafwijking van '-0,01' is ongeldig voor stochast 'bermbreedte'. " +
                        "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationBankWidthVariationCoefficient.xml",
                        "Indien voor parameter 'bermbreedte' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                        "Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationConstructiveStrengthLinearLoadModelMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'lineairebelastingschematiseringsterkte'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationConstructiveStrengthLinearLoadModelVariationCoeffInvalid.xml",
                        "Een variatiecoëfficiënt van '-0,01' is ongeldig voor stochast 'lineairebelastingschematiseringsterkte'. " +
                        "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationConstructiveStrengthLinearLoadModelStandardDeviation.xml",
                        "Indien voor parameter 'lineairebelastingschematiseringsterkte' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                        "Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationConstructiveStrengthQuadraticLoadModelMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'kwadratischebelastingschematiseringsterkte'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationConstructiveStrengthQuadraticLoadModelVariationCoefInvalid.xml",
                        "Een variatiecoëfficiënt van '-0,01' is ongeldig voor stochast 'kwadratischebelastingschematiseringsterkte'. " +
                        "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationConstructiveStrengthQuadraticLoadModelStandardDeviation.xml",
                        "Indien voor parameter 'kwadratischebelastingschematiseringsterkte' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                        "Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationCriticalOvertoppingDischargeMeanInvalid.xml",
                        "Een gemiddelde van '-2' is ongeldig voor stochast 'kritiekinstromenddebiet'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationCriticalOvertoppingDischargeStandardDeviation.xml",
                        "Indien voor parameter 'kritiekinstromenddebiet' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                        "Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationCriticalOvertoppingDischargeVariationCoefficientInvalid.xml",
                        "Een variatiecoëfficiënt van '-0,1' is ongeldig voor stochast 'kritiekinstromenddebiet'. " +
                        "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationDrainCoefficientStandardDeviation.xml",
                        "Er kan geen spreiding voor stochast 'afvoercoefficient' opgegeven worden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationDrainCoefficientVariationCoefficient.xml",
                        "Er kan geen spreiding voor stochast 'afvoercoefficient' opgegeven worden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationEvaluationLevelWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om analysehoogte aan toe te voegen.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationFailureCollisionEnergyMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'bezwijkwaardeaanvaarenergie'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationFailureCollisionEnergyVariationCoefficientInvalid.xml",
                        "Een variatiecoëfficiënt van '-0,01' is ongeldig voor stochast 'bezwijkwaardeaanvaarenergie'. " +
                        "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationFailureCollisionEnergyStandardDeviation.xml",
                        "Indien voor parameter 'bezwijkwaardeaanvaarenergie' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                        "Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationFailureProbabilityRepairClosureInvalid.xml",
                        "Een waarde van '1,1' als faalkans herstel van gefaalde situatie is ongeldig. " +
                        "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationFailureProbabilityRepairClosureWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om faalkans herstel van gefaalde situatie aan toe te voegen.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationFailureProbabilityStructureErosionInvalid.xml",
                        "Een waarde van '1,1' als faalkans gegeven erosie bodem is ongeldig. " +
                        "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationFlowVelocityStructureClosableStandardDeviation.xml",
                        "Er kan geen spreiding voor stochast 'kritiekestroomsnelheid' opgegeven worden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationFlowVelocityStructureClosableVariationCoefficient.xml",
                        "Er kan geen spreiding voor stochast 'kritiekestroomsnelheid' opgegeven worden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationFlowWidthAtBottomProtectionMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'breedtebodembescherming'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationFlowWidthAtBottomProtectionStandardDeviationInvalid.xml",
                        "Een standaardafwijking van '-0,01' is ongeldig voor stochast 'breedtebodembescherming'. " +
                        "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationFlowWidthAtBottomProtectionVariationCoefficient.xml",
                        "Indien voor parameter 'breedtebodembescherming' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                        "Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationInflowModelTypeWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om instroommodel aan toe te voegen.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationInsideWaterLevelStandardDeviationInvalid.xml",
                        "Een standaardafwijking van '-0,01' is ongeldig voor stochast 'binnenwaterstand'. " +
                        "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationInsideWaterLevelVariationCoefficient.xml",
                        "Indien voor parameter 'binnenwaterstand' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                        "Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationInsideWaterLevelFailureConstructionStandardDeviatioInvalid.xml",
                        "Een standaardafwijking van '-0,01' is ongeldig voor stochast 'binnenwaterstandbijfalen'. " +
                        "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationInsideWaterLevelFailureConstructionVariationCoefficient.xml",
                        "Indien voor parameter 'binnenwaterstandbijfalen' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                        "Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationLevelCrestStructureStandardDeviationInvalid.xml",
                        "Een standaardafwijking van '-0,01' is ongeldig voor stochast 'kerendehoogte'. " +
                        "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationLevelCrestStructureVariationCoefficient.xml",
                        "Indien voor parameter 'kerendehoogte' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                        "Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationLevellingCountWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om het aantal nivelleringen per jaar aan toe te voegen.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationLoadSchematizationTypeWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om belastingschematisering aan toe te voegen.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationProbabilityCollisionSecondaryStructureWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de kans op aanvaring tweede keermiddel per nivellering aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationProbabilityCollisionSecondaryStructureInvalid.xml",
                        "Een waarde van '1,1' als de kans op aanvaring tweede keermiddel per nivellering is ongeldig. " +
                        "Kans moet in het bereik [0,0, 1,0] liggen.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationShipMassVariationCoefficientInvalid.xml",
                        "Een variatiecoëfficiënt van '-0,01' is ongeldig voor stochast 'massaschip'. " +
                        "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationShipMassStandardDeviation.xml",
                        "Indien voor parameter 'massaschip' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                        "Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationShipVelocityVariationCoefficientInvalid.xml",
                        "Een variatiecoëfficiënt van '-0,01' is ongeldig voor stochast 'aanvaarsnelheid'. " +
                        "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationShipVelocityStandardDeviation.xml",
                        "Indien voor parameter 'aanvaarsnelheid' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                        "Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationStabilityLinearLoadModelMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'lineairebelastingschematiseringstabiliteit'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStabilityLinearLoadModelVariationCoefficientInvalid.xml",
                        "Een variatiecoëfficiënt van '-0,01' is ongeldig voor stochast 'lineairebelastingschematiseringstabiliteit'. " +
                        "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStabilityLinearLoadModelStandardDeviation.xml",
                        "Indien voor parameter 'lineairebelastingschematiseringstabiliteit' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                        "Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationStabilityQuadraticLoadModelMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'kwadratischebelastingschematiseringstabiliteit'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStabilityQuadraticLoadModelVariationCoefficientInvalid.xml",
                        "Een variatiecoëfficiënt van '-0,01' is ongeldig voor stochast 'kwadratischebelastingschematiseringstabiliteit'. " +
                        "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStabilityQuadraticLoadModelStandardDeviation.xml",
                        "Indien voor parameter 'kwadratischebelastingschematiseringstabiliteit' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                        "Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationStormDurationMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'stormduur'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStormDurationVariationCoefficient.xml",
                        "Er kan geen spreiding voor stochast 'stormduur' opgegeven worden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStormDurationStandardDeviation.xml",
                        "Er kan geen spreiding voor stochast 'stormduur' opgegeven worden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationStructureNormalOrientationWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om oriëntatie aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStructureNormalOrientationInvalid.xml",
                        "Een waarde van '-12' als oriëntatie is ongeldig. De waarde voor de oriëntatie moet in het bereik [0,00, 360,00] liggen.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationStorageStructureAreaMeanInvalid.xml",
                        "Een gemiddelde van '-0,2' is ongeldig voor stochast 'kombergendoppervlak'. " +
                        "Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStorageStructureAreaVariationCoefficientInvalid.xml",
                        "Een variatiecoëfficiënt van '-0,01' is ongeldig voor stochast 'kombergendoppervlak'. " +
                        "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStorageStructureAreaStandardDeviation.xml",
                        "Indien voor parameter 'kombergendoppervlak' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. " +
                        "Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationThresholdHeightOpenWeirStandardDeviationInvalid.xml",
                        "Een standaardafwijking van '-0,01' is ongeldig voor stochast 'drempelhoogte'. " +
                        "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationThresholdHeightOpenWeirVariationCoefficient.xml",
                        "Indien voor parameter 'drempelhoogte' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                        "Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationVerticalDistanceWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de afstand onderkant wand en teen van de dijk/berm aan toe te voegen.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationWidthFlowAperturesStandardDeviationInvalid.xml",
                        "Een standaardafwijking van '-0,1' is ongeldig voor stochast 'breedtedoorstroomopening'. " +
                        "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationWidthFlowAperturesVariationCoefficient.xml",
                        "Indien voor parameter 'breedtedoorstroomopening' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. " +
                        "Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData(
                        "validConfigurationStabilityQuadraticLoadModelWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'kwadratischebelastingschematiseringstabiliteit' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStabilityLinearLoadModelWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'lineairebelastingschematiseringstabiliteit' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationShipVelocityWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'aanvaarsnelheid' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationShipMassWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'massaschip' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationFailureCollisionEnergyWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'bezwijkwaardeaanvaarenergie' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationBankWidthWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'bermbreedte' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationConstructiveStrengthLinearLoadModelWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'lineairebelastingschematiseringsterkte' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationConstructiveStrengthQuadraticLoadModelWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'kwadratischebelastingschematiseringsterkte' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationFlowVelocityStructureClosableWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'kritiekestroomsnelheid' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationCriticalOvertoppingDischargeWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'kritiekinstromenddebiet' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationThresholdHeightOpenWeirWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'drempelhoogte' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationLevelCrestStructureWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'kerendehoogte' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationAllowedLevelIncreaseStorageWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'peilverhogingkomberging' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationStorageStructureAreaWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'kombergendoppervlak' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationFlowWidthAtBottomProtectionWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'breedtebodembescherming' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationAreaFlowAperturesWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'doorstroomoppervlak' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationWidthFlowAperturesWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'breedtedoorstroomopening' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationInsideWaterLevelFailureConstructionWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'binnenwaterstandbijfalen' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData(
                        "validConfigurationInsideWaterLevelWithoutStructure.xml",
                        "Er is geen kunstwerk opgegeven om de stochast 'binnenwaterstand' aan toe te voegen.")
                    .SetName(testNameFormat);
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                "", new CalculationGroup(), Enumerable.Empty<HydraulicBoundaryLocation>(), Enumerable.Empty<ForeshoreProfile>(),
                Enumerable.Empty<StabilityPointStructure>(), new StabilityPointStructuresFailureMechanism());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<StabilityPointStructuresCalculationConfigurationReader,
                StabilityPointStructuresCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new StabilityPointStructuresCalculationConfigurationImporter(
                "", new CalculationGroup(), null, Enumerable.Empty<ForeshoreProfile>(),
                Enumerable.Empty<StabilityPointStructure>(), new StabilityPointStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new StabilityPointStructuresCalculationConfigurationImporter(
                "", new CalculationGroup(), Enumerable.Empty<HydraulicBoundaryLocation>(), null,
                Enumerable.Empty<StabilityPointStructure>(), new StabilityPointStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
        }

        [Test]
        public void Constructor_StructuresNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new StabilityPointStructuresCalculationConfigurationImporter(
                "", new CalculationGroup(), Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(), null, new StabilityPointStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("structures", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new StabilityPointStructuresCalculationConfigurationImporter(
                "", new CalculationGroup(), Enumerable.Empty<HydraulicBoundaryLocation>(), 
                Enumerable.Empty<ForeshoreProfile>(), Enumerable.Empty<StabilityPointStructure>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(ValidConfigurationInvalidData))]
        public void Import_ValidConfigurationInvalidData_LogMessageAndContinueImport(string file, string expectedErrorMessage)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var structure = new TestStabilityPointStructure("kunstwerk1", "kunstwerk1");
            var foreshoreProfile = new TestForeshoreProfile("profiel 1");

            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new ForeshoreProfile[]
                {
                    foreshoreProfile
                },
                new StabilityPointStructure[]
                {
                    structure
                },
                new StabilityPointStructuresFailureMechanism());
            var successful = false;

            // Call
            void Call() => successful = importer.Import();

            // Assert
            string expectedMessage = $"{expectedErrorMessage} Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_UseForeshoreButForeshoreProfileWithoutGeometry_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationUseForeshoreWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            var foreshoreProfile = new TestForeshoreProfile("Voorlandprofiel");
            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new[]
                {
                    foreshoreProfile
                },
                Enumerable.Empty<StabilityPointStructure>(),
                new StabilityPointStructuresFailureMechanism());

            var successful = false;

            // Call
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het opgegeven voorlandprofiel 'Voorlandprofiel' heeft geen voorlandgeometrie en kan daarom niet gebruikt worden. " +
                                           "Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochastWithMeanOnly_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationStochastMeansOnly.xml");

            var calculationGroup = new CalculationGroup();
            var structure = new TestStabilityPointStructure("kunstwerk1");
            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new[]
                {
                    structure,
                    new TestStabilityPointStructure("other structure")
                },
                new StabilityPointStructuresFailureMechanism());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            var expectedCalculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    Structure = structure,
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 0.2
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) 80.5
                    },
                    BankWidth =
                    {
                        Mean = (RoundedDouble) 1.2
                    },
                    ConstructiveStrengthLinearLoadModel =
                    {
                        Mean = (RoundedDouble) 2
                    },
                    ConstructiveStrengthQuadraticLoadModel =
                    {
                        Mean = (RoundedDouble) 2
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 2
                    },
                    DrainCoefficient =
                    {
                        Mean = (RoundedDouble) 0.1
                    },
                    FailureCollisionEnergy =
                    {
                        Mean = (RoundedDouble) 1.2
                    },
                    FlowVelocityStructureClosable =
                    {
                        Mean = (RoundedDouble) 1.1
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 15.2
                    },
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) 0.5
                    },
                    InsideWaterLevelFailureConstruction =
                    {
                        Mean = (RoundedDouble) 0.7
                    },
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) 4.3
                    },
                    ShipMass =
                    {
                        Mean = (RoundedDouble) 16000
                    },
                    ShipVelocity =
                    {
                        Mean = (RoundedDouble) 1.2
                    },
                    StabilityLinearLoadModel =
                    {
                        Mean = (RoundedDouble) 1.2
                    },
                    StabilityQuadraticLoadModel =
                    {
                        Mean = (RoundedDouble) 1.2
                    },
                    StormDuration =
                    {
                        Mean = (RoundedDouble) 6.0
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 15000
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) 1.2
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 15.2
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculationScenario<StabilityPointStructuresInput>) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_StochastWithStandardDeviationOrVariationCoefficientOnly_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationStochastStandardDeviationVariationCoefficientOnly.xml");

            var calculationGroup = new CalculationGroup();
            var structure = new TestStabilityPointStructure("kunstwerk1");
            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new[]
                {
                    structure,
                    new TestStabilityPointStructure("other structure")
                },
                new StabilityPointStructuresFailureMechanism());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            var expectedCalculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    Structure = structure,
                    AllowedLevelIncreaseStorage =
                    {
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    AreaFlowApertures =
                    {
                        StandardDeviation = (RoundedDouble) 1
                    },
                    BankWidth =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    ConstructiveStrengthLinearLoadModel =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    ConstructiveStrengthQuadraticLoadModel =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    CriticalOvertoppingDischarge =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    FailureCollisionEnergy =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    FlowWidthAtBottomProtection =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    InsideWaterLevel =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    InsideWaterLevelFailureConstruction =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    LevelCrestStructure =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    ShipMass =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    ShipVelocity =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StabilityLinearLoadModel =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StabilityQuadraticLoadModel =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StorageStructureArea =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.01
                    },
                    ThresholdHeightOpenWeir =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    WidthFlowApertures =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculationScenario<StabilityPointStructuresInput>) calculationGroup.Children[0]);
        }

        [Test]
        [TestCase("validConfigurationEmptyCalculation.xml")]
        [TestCase("validConfigurationEmptyStochasts.xml")]
        [TestCase("validConfigurationEmptyStochastElements.xml")]
        [TestCase("validConfigurationEmptyWaveReduction.xml")]
        public void Import_EmptyConfigurations_DataAddedToModel(string file)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var structure = new TestStabilityPointStructure("kunstwerk1", "kunstwerk1");
            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new[]
                {
                    structure
                },
                new StabilityPointStructuresFailureMechanism());

            var expectedCalculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Name = "Berekening 1"
            };

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculationScenario<StabilityPointStructuresInput>) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_FullCalculationConfiguration_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validFullConfiguration.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Locatie1");
            var foreshoreProfile = new TestForeshoreProfile("profiel1", new List<Point2D>
            {
                new Point2D(0, 3)
            });
            var structure = new TestStabilityPointStructure("kunstwerk1");
            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new[]
                {
                    hydraulicBoundaryLocation,
                    new TestHydraulicBoundaryLocation("Other location")
                },
                new[]
                {
                    foreshoreProfile,
                    new TestForeshoreProfile("Other profile")
                },
                new[]
                {
                    structure,
                    new TestStabilityPointStructure("other structure")
                },
                new StabilityPointStructuresFailureMechanism());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
            Assert.IsTrue(successful);
            var expectedCalculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    Structure = structure,
                    ForeshoreProfile = foreshoreProfile,
                    ShouldIllustrationPointsBeCalculated = true,
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) 80.5,
                        StandardDeviation = (RoundedDouble) 1
                    },
                    BankWidth =
                    {
                        Mean = (RoundedDouble) 1.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    BreakWater =
                    {
                        Height = (RoundedDouble) 1.23,
                        Type = BreakWaterType.Dam
                    },
                    ConstructiveStrengthLinearLoadModel =
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    ConstructiveStrengthQuadraticLoadModel =
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    DrainCoefficient =
                    {
                        Mean = (RoundedDouble) 0.1
                    },
                    EvaluationLevel = (RoundedDouble) 0.1,
                    FactorStormDurationOpenStructure = (RoundedDouble) 0.002,
                    FailureCollisionEnergy =
                    {
                        Mean = (RoundedDouble) 1.2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    FailureProbabilityRepairClosure = 0.001,
                    FailureProbabilityStructureWithErosion = 0.0001,
                    FlowVelocityStructureClosable =
                    {
                        Mean = (RoundedDouble) 1.1
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) 0.5,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    InsideWaterLevelFailureConstruction =
                    {
                        Mean = (RoundedDouble) 0.7,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) 4.3,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    LevellingCount = 1,
                    LoadSchematizationType = LoadSchematizationType.Quadratic,
                    ProbabilityCollisionSecondaryStructure = 0.00001,
                    UseBreakWater = true,
                    UseForeshore = false,
                    ShipMass =
                    {
                        Mean = (RoundedDouble) 16000,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    ShipVelocity =
                    {
                        Mean = (RoundedDouble) 1.2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StabilityLinearLoadModel =
                    {
                        Mean = (RoundedDouble) 1.2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StabilityQuadraticLoadModel =
                    {
                        Mean = (RoundedDouble) 1.2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    },
                    StormDuration =
                    {
                        Mean = (RoundedDouble) 6.0
                    },
                    StructureNormalOrientation = (RoundedDouble) 7,
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 15000,
                        CoefficientOfVariation = (RoundedDouble) 0.01
                    },
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) 1.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    VerticalDistance = (RoundedDouble) 2,
                    VolumicWeightWater = (RoundedDouble) 9.91,
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculationScenario<StabilityPointStructuresInput>) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ConfigurationWithUnroundedValues_RoundedValuesAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationUnrounded.xml");

            var calculationGroup = new CalculationGroup();
            var structure = new TestStabilityPointStructure("kunstwerk1", "kunstwerk1");
            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new HydraulicBoundaryLocation[0],
                new ForeshoreProfile[0],
                new[]
                {
                    structure
                },
                new StabilityPointStructuresFailureMechanism());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            Assert.AreEqual(1, calculationGroup.Children.Count);
            var calculation = (StructuresCalculation<StabilityPointStructuresInput>) calculationGroup.Children[0];
            double expectedValue = (RoundedDouble) 1.2e-108;
            Assert.AreEqual(expectedValue, calculation.InputParameters.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(expectedValue, calculation.InputParameters.ProbabilityCollisionSecondaryStructure);
        }

        [Test]
        public void DoPostImport_CalculationWithStructureInSection_AssignsCalculationToSectionResult()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationFullCalculation.xml");
            var calculationGroup = new CalculationGroup();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(10, 10)
                })
            });

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure(new Point2D(5, 5))
                }
            };
            failureMechanism.CalculationsGroup.Children.Add(
                calculation);

            var importer = new StabilityPointStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                Enumerable.Empty<StabilityPointStructure>(),
                failureMechanism);

            // Preconditions
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.IsNull(failureMechanism.SectionResults.ElementAt(0).Calculation);

            // Call
            importer.DoPostImport();

            // Assert
            Assert.AreSame(calculation, failureMechanism.SectionResults.ElementAt(0).Calculation);
        }

        [TestCase("validConfigurationUnknownForeshoreProfile.xml",
                  "Het voorlandprofiel met ID 'unknown' bestaat niet.",
                  TestName = "Import_UnknownData({0:80})")]
        [TestCase("validConfigurationUnknownHydraulicBoundaryLocation.xml",
                  "De hydraulische belastingenlocatie 'unknown' bestaat niet.",
                  TestName = "Import_UnknownData({0:80})")]
        [TestCase("validConfigurationUnknownStructure.xml",
                  "Het kunstwerk met ID 'unknown' bestaat niet.",
                  TestName = "Import_UnknownData({0:80})")]
        public void Import_ValidConfigurationUnknownData_LogMessageAndContinueImport(string file, string expectedErrorMessage)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();

            var importer = new StabilityPointStructuresCalculationConfigurationImporter(filePath,
                                                                                        calculationGroup,
                                                                                        Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                        Enumerable.Empty<ForeshoreProfile>(),
                                                                                        Enumerable.Empty<StabilityPointStructure>(),
                                                                                        new StabilityPointStructuresFailureMechanism());
            var successful = false;

            // Call
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = $"{expectedErrorMessage} Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        private static void AssertCalculation(StructuresCalculationScenario<StabilityPointStructuresInput> expectedCalculation,
                                              StructuresCalculationScenario<StabilityPointStructuresInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreEqual(expectedCalculation.IsRelevant, actualCalculation.IsRelevant);
            Assert.AreEqual(expectedCalculation.Contribution, actualCalculation.Contribution);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Height, actualCalculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Type, actualCalculation.InputParameters.BreakWater.Type);
            Assert.AreEqual(expectedCalculation.InputParameters.EvaluationLevel, actualCalculation.InputParameters.EvaluationLevel);

            Assert.AreEqual(expectedCalculation.InputParameters.FactorStormDurationOpenStructure, actualCalculation.InputParameters.FactorStormDurationOpenStructure);
            Assert.AreEqual(expectedCalculation.InputParameters.FailureProbabilityRepairClosure, actualCalculation.InputParameters.FailureProbabilityRepairClosure);
            Assert.AreEqual(expectedCalculation.InputParameters.FailureProbabilityStructureWithErosion, actualCalculation.InputParameters.FailureProbabilityStructureWithErosion);
            Assert.AreSame(expectedCalculation.InputParameters.ForeshoreProfile, actualCalculation.InputParameters.ForeshoreProfile);

            Assert.AreEqual(expectedCalculation.InputParameters.InflowModelType, actualCalculation.InputParameters.InflowModelType);
            Assert.AreEqual(expectedCalculation.InputParameters.LevellingCount, actualCalculation.InputParameters.LevellingCount);
            Assert.AreEqual(expectedCalculation.InputParameters.LoadSchematizationType, actualCalculation.InputParameters.LoadSchematizationType);
            Assert.AreEqual(expectedCalculation.InputParameters.ProbabilityCollisionSecondaryStructure, actualCalculation.InputParameters.ProbabilityCollisionSecondaryStructure);

            Assert.AreSame(expectedCalculation.InputParameters.Structure, actualCalculation.InputParameters.Structure);
            Assert.AreEqual(expectedCalculation.InputParameters.StructureNormalOrientation, actualCalculation.InputParameters.StructureNormalOrientation);
            Assert.AreEqual(expectedCalculation.InputParameters.UseForeshore, actualCalculation.InputParameters.UseForeshore);
            Assert.AreEqual(expectedCalculation.InputParameters.UseBreakWater, actualCalculation.InputParameters.UseBreakWater);
            Assert.AreEqual(expectedCalculation.InputParameters.ShouldIllustrationPointsBeCalculated, actualCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);

            Assert.AreEqual(expectedCalculation.InputParameters.VerticalDistance, actualCalculation.InputParameters.VerticalDistance);
            Assert.AreEqual(expectedCalculation.InputParameters.VolumicWeightWater, actualCalculation.InputParameters.VolumicWeightWater);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.AllowedLevelIncreaseStorage, actualCalculation.InputParameters.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.AreaFlowApertures, actualCalculation.InputParameters.AreaFlowApertures);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.BankWidth, actualCalculation.InputParameters.BankWidth);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ConstructiveStrengthLinearLoadModel, actualCalculation.InputParameters.ConstructiveStrengthLinearLoadModel);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ConstructiveStrengthQuadraticLoadModel, actualCalculation.InputParameters.ConstructiveStrengthQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.CriticalOvertoppingDischarge, actualCalculation.InputParameters.CriticalOvertoppingDischarge);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.DrainCoefficient, actualCalculation.InputParameters.DrainCoefficient);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.FailureCollisionEnergy, actualCalculation.InputParameters.FailureCollisionEnergy);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.FlowVelocityStructureClosable, actualCalculation.InputParameters.FlowVelocityStructureClosable);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.FlowWidthAtBottomProtection, actualCalculation.InputParameters.FlowWidthAtBottomProtection);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.InsideWaterLevel, actualCalculation.InputParameters.InsideWaterLevel);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.InsideWaterLevelFailureConstruction, actualCalculation.InputParameters.InsideWaterLevelFailureConstruction);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.LevelCrestStructure, actualCalculation.InputParameters.LevelCrestStructure);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ShipMass, actualCalculation.InputParameters.ShipMass);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ShipVelocity, actualCalculation.InputParameters.ShipVelocity);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StabilityLinearLoadModel, actualCalculation.InputParameters.StabilityLinearLoadModel);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StabilityQuadraticLoadModel, actualCalculation.InputParameters.StabilityQuadraticLoadModel);

            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StorageStructureArea, actualCalculation.InputParameters.StorageStructureArea);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StormDuration, actualCalculation.InputParameters.StormDuration);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.WidthFlowApertures, actualCalculation.InputParameters.WidthFlowApertures);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ThresholdHeightOpenWeir, actualCalculation.InputParameters.ThresholdHeightOpenWeir);
        }
    }
}