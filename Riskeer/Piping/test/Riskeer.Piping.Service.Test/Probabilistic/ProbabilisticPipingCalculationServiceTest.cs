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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service.Probabilistic;

namespace Riskeer.Piping.Service.Test.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingCalculationServiceTest
    {
        private const string averagingSoilLayerPropertiesMessage = "Meerdere aaneengesloten deklagen gevonden. De grondeigenschappen worden bepaald door het nemen van een gewogen gemiddelde, mits de standaardafwijkingen en verschuivingen voor alle lagen gelijk zijn.";
        
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        private double testSurfaceLineTopLevel;
        private ProbabilisticPipingCalculation calculation;

        [SetUp]
        public void Setup()
        {
            calculation = SemiProbabilisticPipingCalculationScenarioTestFactory.CreateProbabilisticPipingCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            testSurfaceLineTopLevel = calculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
        }

        [Test]
        public void Validate_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();
            
            // Call
            void Call() => ProbabilisticPipingCalculationService.Validate(null, new GeneralPipingInput(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_GeneralInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();
            
            // Call
            void Call() => ProbabilisticPipingCalculationService.Validate(new TestProbabilisticPipingCalculation(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ProbabilisticPipingCalculationService.Validate(new TestProbabilisticPipingCalculation(), new GeneralPipingInput(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
        
        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();

            // Call
            void Call() => ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsMessageAndReturnFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = null;

            // Call
            var isValid = false;
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Er is geen hydraulische belastingenlocatie geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsMessageAndReturnFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            string invalidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, invalidFilePath);
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2);

            // Call
            var isValid = true;
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_InvalidPreprocessorDirectory_LogsMessageAndReturnFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = "NonExistingPreprocessorDirectory";
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2);

            // Call
            var isValid = true;
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsMessageAndReturnFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            string invalidFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, invalidFilePath);
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2);

            // Call
            var isValid = false;
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_WithoutImportedHydraulicBoundaryDatabase_LogsMessageAndReturnFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks);
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2);

            // Call
            var isValid = false;
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische belastingendatabase geïmporteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }
        
        [Test]
        public void Validate_InvalidCalculationInput_LogsErrorAndReturnsFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();

            // Call
            var isValid = false;

            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(new TestProbabilisticPipingCalculation(), new GeneralPipingInput(), assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(7, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische belastingenlocatie geselecteerd.", msgs[1]);
                Assert.AreEqual("Er is geen profielschematisatie geselecteerd.", msgs[2]);
                Assert.AreEqual("Er is geen ondergrondschematisatie geselecteerd.", msgs[3]);
                Assert.AreEqual("De waarde voor 'uittredepunt' moet een concreet getal zijn.", msgs[4]);
                Assert.AreEqual("De waarde voor 'intredepunt' moet een concreet getal zijn.", msgs[5]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[6]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_WithoutAquiferLayer_LogsErrorAndReturnsFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var aquitardLayer = new PipingSoilLayer(2.0)
            {
                IsAquifer = false
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    aquitardLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[1]);
                Assert.AreEqual("Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                Assert.AreEqual("Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.", msgs[3]);
                Assert.AreEqual("Geen watervoerende laag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[4]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[5]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_WithoutAquitardLayer_LogsWarningsAndReturnsTrue()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var aquiferLayer = new PipingSoilLayer(10.56)
            {
                IsAquifer = true,
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    aquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[1]);
                Assert.AreEqual("Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.IsTrue(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_WithoutCoverageLayer_LogsWarningsAndReturnsTrue()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var coverageLayerAboveSurfaceLine = new PipingSoilLayer(13.0)
            {
                IsAquifer = false
            };
            var bottomAquiferLayer = new PipingSoilLayer(11.0)
            {
                IsAquifer = true,
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    coverageLayerAboveSurfaceLine,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[1]);
                Assert.AreEqual("Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.IsTrue(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_MultipleCoverageLayer_LogsWarningAndReturnsTrue()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var random = new Random(21);
            const double belowPhreaticLevelDeviation = 0.5;
            const int belowPhreaticLevelShift = 1;
            const double belowPhreaticLevelMeanBase = 15.0;

            var topCoverageLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) (belowPhreaticLevelMeanBase + belowPhreaticLevelShift + random.NextDouble()),
                    StandardDeviation = (RoundedDouble) belowPhreaticLevelDeviation,
                    Shift = (RoundedDouble) belowPhreaticLevelShift
                }
            };
            var middleCoverageLayer = new PipingSoilLayer(8.5)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) (belowPhreaticLevelMeanBase + belowPhreaticLevelShift + random.NextDouble()),
                    StandardDeviation = (RoundedDouble) belowPhreaticLevelDeviation,
                    Shift = (RoundedDouble) belowPhreaticLevelShift
                }
            };
            var bottomAquiferLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    topCoverageLayer,
                                                    middleCoverageLayer,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(averagingSoilLayerPropertiesMessage, msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsTrue(isValid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Validate_IncompleteDiameterD70Definition_LogsErrorAndReturnsFalse(bool meanSet, bool coefficientOfVariationSet)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = meanSet
                               ? random.NextRoundedDouble(1, double.MaxValue)
                               : RoundedDouble.NaN,
                    CoefficientOfVariation = coefficientOfVariationSet
                                                 ? random.NextRoundedDouble()
                                                 : RoundedDouble.NaN
                }
            };

            var completeLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(15.0, 999.999),
                    StandardDeviation = random.NextRoundedDouble(1e-6, 5.0),
                    Shift = random.NextRoundedDouble(1e-6, 10)
                }
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    completeLayer,
                                                    incompletePipingSoilLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kan de definitie voor het 70%-fraktiel van de korreldiameter van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(6.2e-5)]
        [TestCase(5.1e-3)]
        public void Validate_InvalidDiameterD70Value_LogsWarningAndReturnsTrue(double diameter70Value)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var random = new Random(21);
            var coverageLayerInvalidD70 = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) diameter70Value,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };
            var validLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(15.0, 999.999),
                    StandardDeviation = random.NextRoundedDouble(1e-6, 5.0),
                    Shift = random.NextRoundedDouble(1e-6, 10)
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    validLayer,
                                                    coverageLayerInvalidD70
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"Rekenwaarde voor d70 ({new RoundedDouble(6, diameter70Value)} m) ligt buiten het geldigheidsbereik van dit model. Geldige waarden liggen tussen 0.000063 m en 0.0005 m.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsTrue(isValid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Validate_IncompletePermeabilityDefinition_LogsErrorAndReturnsFalse(bool meanSet, bool coefficientOfVariationSet)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = meanSet
                               ? random.NextRoundedDouble(1, double.MaxValue)
                               : RoundedDouble.NaN,
                    CoefficientOfVariation = coefficientOfVariationSet
                                                 ? random.NextRoundedDouble()
                                                 : RoundedDouble.NaN
                }
            };

            var completeLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(15.0, 999.999),
                    StandardDeviation = random.NextRoundedDouble(1e-6, 999.999),
                    Shift = random.NextRoundedDouble(1e-6, 10)
                }
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    completeLayer,
                                                    incompletePipingSoilLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kan de definitie voor de doorlatendheid van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        public void Validate_IncompleteSaturatedVolumicWeightDefinition_LogsErrorAndReturnsFalse(bool meanSet, bool deviationSet, bool shiftSet)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = meanSet
                               ? random.NextRoundedDouble(1, double.MaxValue)
                               : RoundedDouble.NaN,
                    StandardDeviation = deviationSet
                                            ? random.NextRoundedDouble()
                                            : RoundedDouble.NaN,
                    Shift = shiftSet
                                ? random.NextRoundedDouble()
                                : RoundedDouble.NaN
                }
            };

            var completeLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1e-4,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    incompletePipingSoilLayer,
                                                    completeLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_SaturatedCoverageLayerVolumicWeightLessThanWaterVolumicWeight_LogsErrorAndReturnsFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var coverageLayerInvalidSaturatedVolumicWeight = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) 9.81,
                    StandardDeviation = (RoundedDouble) 2,
                    Shift = (RoundedDouble) 0
                }
            };
            var validLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.5
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 0.0002,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    coverageLayerInvalidSaturatedVolumicWeight,
                                                    validLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            var isValid = false;

            // Call
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(
                    "Het verzadigd volumetrisch gewicht van de deklaag moet groter zijn dan het volumetrisch gewicht van water.",
                    msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_SaturatedCoverageLayerLessThanWaterLayerAndMissingSaturatedParameter_LogsErrorOnlyForIncompleteDefinition()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();
            
            var topCoverageLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) 5,
                    StandardDeviation = (RoundedDouble) 2,
                    Shift = (RoundedDouble) 0
                }
            };
            var middleCoverageLayerMissingParameter = new PipingSoilLayer(8.5)
            {
                IsAquifer = false,
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = (RoundedDouble) 5,
                    StandardDeviation = (RoundedDouble) 2,
                    Shift = RoundedDouble.NaN
                }
            };
            var bottomAquiferLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 0.3,
                    CoefficientOfVariation = (RoundedDouble) 0.6
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = (RoundedDouble) 0.0002,
                    CoefficientOfVariation = (RoundedDouble) 0
                }
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    topCoverageLayer,
                                                    middleCoverageLayerMissingParameter,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D);

            calculation.InputParameters.StochasticSoilProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            void Call() => ProbabilisticPipingCalculationService.Validate(calculation, failureMechanism.GeneralInput, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(averagingSoilLayerPropertiesMessage, msgs[1]);
                Assert.AreEqual(
                    "Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.",
                    msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            mocks.VerifyAll();
        }
    }
}