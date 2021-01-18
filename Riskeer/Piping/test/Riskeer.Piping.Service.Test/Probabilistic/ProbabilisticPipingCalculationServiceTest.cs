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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Piping;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service.Probabilistic;

namespace Riskeer.Piping.Service.Test.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingCalculationServiceTest
    {
        private const string averagingSoilLayerPropertiesMessage = "Meerdere aaneengesloten deklagen gevonden. De grondeigenschappen worden bepaald door het nemen van een gewogen gemiddelde, mits de standaardafwijkingen en verschuivingen voor alle lagen gelijk zijn.";

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "Hlcd.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        private double testSurfaceLineTopLevel;
        private ProbabilisticPipingCalculation calculation;

        [SetUp]
        public void Setup()
        {
            calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<TestProbabilisticPipingCalculation>(
                new TestHydraulicBoundaryLocation());
            testSurfaceLineTopLevel = calculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
        }

        #region Validate

        [Test]
        public void Validate_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ProbabilisticPipingCalculationService.Validate(null,
                                                                          new PipingFailureMechanism(),
                                                                          assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ProbabilisticPipingCalculationService.Validate(new TestProbabilisticPipingCalculation(),
                                                                          null,
                                                                          assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ProbabilisticPipingCalculationService.Validate(new TestProbabilisticPipingCalculation(),
                                                                          new PipingFailureMechanism(),
                                                                          null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Validate_Always_LogsStartAndEndOfValidation()
        {
            // Setup
            var failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            mocks.ReplayAll();

            // Call
            void Call() => ProbabilisticPipingCalculationService.Validate(calculation,
                                                                          failureMechanism,
                                                                          assessmentSection);

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
        public void Validate_NoHydraulicBoundaryLocation_LogsMessageAndReturnsFalse()
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFailureMechanismWithSections();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = null;

            // Call
            var isValid = false;

            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsMessageAndReturnsFalse()
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

            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        public void Validate_InvalidPreprocessorDirectory_LogsMessageAndReturnsFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = "NonExistingPreprocessorDirectory";
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2);

            // Call
            var isValid = true;

            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsMessageAndReturnsFalse()
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

            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        public void Validate_WithoutImportedHydraulicBoundaryDatabase_LogsMessageAndReturnsFalse()
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

            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        public void Validate_InvalidCalculationInput_LogsMessagesAndReturnsFalse()
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFailureMechanismWithSections();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            mocks.ReplayAll();

            // Call
            var isValid = false;

            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(new TestProbabilisticPipingCalculation(),
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        public void Validate_WithoutAquiferLayer_LogsMessagesAndReturnsFalse()
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFailureMechanismWithSections();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
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
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt. Er wordt een deklaagdikte gebruikt gelijk aan 0.", msgs[1]);
                Assert.AreEqual("Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.", msgs[2]);
                Assert.AreEqual("Geen watervoerende laag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[3]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[4]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_WithoutAquitardLayer_LogsMessageAndReturnsTrue()
        {
            // Setup
            var failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
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
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt. Er wordt een deklaagdikte gebruikt gelijk aan 0.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsTrue(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_WithoutCoverageLayer_LogsMessageAndReturnsTrue()
        {
            // Setup
            var failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
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
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt. Er wordt een deklaagdikte gebruikt gelijk aan 0.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsTrue(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_MultipleCoverageLayers_LogsMessageAndReturnsTrue()
        {
            // Setup
            var failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            mocks.ReplayAll();

            var random = new Random(21);
            const double belowPhreaticLevelDeviation = 0.5;
            const int belowPhreaticLevelShift = 10;
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
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        public void Validate_IncompleteDiameterD70Definition_LogsMessageAndReturnsFalse(bool meanSet, bool coefficientOfVariationSet)
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFailureMechanismWithSections();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
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
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Validate_IncompletePermeabilityDefinition_LogsMessageAndReturnsFalse(bool meanSet, bool coefficientOfVariationSet)
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFailureMechanismWithSections();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
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
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        public void Validate_IncompleteSaturatedVolumicWeightDefinition_LogsMessageAndReturnsFalse(bool meanSet, bool deviationSet, bool shiftSet)
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFailureMechanismWithSections();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
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
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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
        public void Validate_SaturatedCoverageLayerLessThanWaterLayerAndMissingSaturatedParameter_LogsMessageOnlyForIncompleteDefinition()
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFailureMechanismWithSections();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
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
            void Call() => ProbabilisticPipingCalculationService.Validate(calculation,
                                                                          failureMechanism,
                                                                          assessmentSection);

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

        [Test]
        public void Validate_SaturatedCoverageLayerVolumicWeightShiftLessThanWaterVolumicWeight_LogsMessageAndReturnsFalse()
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFailureMechanismWithSections();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
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
            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(
                    "De verschuiving van het verzadigd volumetrisch gewicht van de deklaag moet groter zijn dan het volumetrisch gewicht van water.",
                    msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_FailureMechanismWithoutSections_LogsMessageAndReturnsFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            mocks.ReplayAll();

            // Call
            var isValid = false;

            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Er is geen vakindeling geïmporteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_CalculationInMultipleSections_LogsMessageAndReturnsFalse()
        {
            // Setup
            PipingFailureMechanism failureMechanism = CreateFailureMechanismWithSections();

            calculation.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(1.0, 0.0);

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            mocks.ReplayAll();

            // Call
            var isValid = false;

            void Call() => isValid = ProbabilisticPipingCalculationService.Validate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Kan geen vak bepalen voor deze berekening: de locatie van de profielschematisatie bevindt zich op de scheiding van twee vakken.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
            mocks.VerifyAll();
        }

        private static PipingFailureMechanism CreateFailureMechanismWithSections()
        {
            var failureMechanism = new PipingFailureMechanism();

            failureMechanism.SetSections(new[]
            {
                new FailureMechanismSection("Section1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(1.0, 0.0)
                }),
                new FailureMechanismSection("Section2", new[]
                {
                    new Point2D(1.0, 0.0),
                    new Point2D(2.0, 0.0)
                })
            }, "path/to/sections");

            return failureMechanism;
        }

        #endregion

        #region Calculate

        [Test]
        public void Calculate_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticPipingCalculationService().Calculate(
                null, new GeneralPipingInput(), CreateCalculationSettings(), 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Calculate_GeneralInputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticPipingCalculationService().Calculate(
                new TestProbabilisticPipingCalculation(), null, CreateCalculationSettings(), 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
        }

        [Test]
        public void Calculate_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticPipingCalculationService().Calculate(
                new TestProbabilisticPipingCalculation(), new GeneralPipingInput(), null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_CalculationValidWithOrWithoutCoverageLayer_InputPropertiesCorrectlySentToCalculator(bool withCoverageLayer)
        {
            // Setup
            double sectionLength = new Random(21).NextDouble();
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);

            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            if (!withCoverageLayer)
            {
                calculation.InputParameters.StochasticSoilProfile =
                    new PipingStochasticSoilProfile(0.0, new PipingSoilProfile(string.Empty, 0.0, new[]
                    {
                        new PipingSoilLayer(10)
                    }, SoilProfileType.SoilProfile1D));
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), sectionLength);

                // Assert
                PipingCalculationInput[] profileSpecificInputs = profileSpecificCalculator.ReceivedInputs.ToArray();
                PipingCalculationInput[] sectionSpecificInputs = sectionSpecificCalculator.ReceivedInputs.ToArray();

                Assert.AreEqual(1, profileSpecificInputs.Length);
                Assert.AreEqual(1, sectionSpecificInputs.Length);

                AssertCalculatorInput(failureMechanism.GeneralInput, calculation.InputParameters, 0, withCoverageLayer, profileSpecificInputs[0]);
                AssertCalculatorInput(failureMechanism.GeneralInput, calculation.InputParameters, sectionLength, withCoverageLayer, sectionSpecificInputs[0]);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_Always_LogsStartAndEndOfCalculation()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);

            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator);
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator);
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void Calculate_ValidCalculationWithCoverageLayer_OutputSetOnCalculation(bool shouldProfileSpecificIllustrationPointsBeCalculated,
                                                                                       bool shouldSectionSpecificIllustrationPointsBeCalculated)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestPipingCalculator
                             {
                                 IllustrationPointsResult = new TestGeneralResult()
                             }).Repeat.Twice();
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = shouldProfileSpecificIllustrationPointsBeCalculated;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = shouldSectionSpecificIllustrationPointsBeCalculated;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                IPartialProbabilisticPipingOutput profileSpecificOutput = calculation.Output.ProfileSpecificOutput;
                IPartialProbabilisticPipingOutput sectionSpecificOutput = calculation.Output.SectionSpecificOutput;

                Assert.IsInstanceOf<PartialProbabilisticFaultTreePipingOutput>(profileSpecificOutput);
                Assert.IsInstanceOf<PartialProbabilisticFaultTreePipingOutput>(sectionSpecificOutput);

                Assert.IsFalse(double.IsNaN(profileSpecificOutput.Reliability));
                Assert.IsFalse(double.IsNaN(sectionSpecificOutput.Reliability));

                Assert.AreEqual(shouldProfileSpecificIllustrationPointsBeCalculated, profileSpecificOutput.HasGeneralResult);
                Assert.AreEqual(shouldSectionSpecificIllustrationPointsBeCalculated, sectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void Calculate_ValidCalculationWithoutCoverageLayer_OutputSetOnCalculation(bool shouldProfileSpecificIllustrationPointsBeCalculated,
                                                                                          bool shouldSectionSpecificIllustrationPointsBeCalculated)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestPipingCalculator
                             {
                                 IllustrationPointsResult = new TestGeneralResult()
                             }).Repeat.Twice();
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);
            calculation.InputParameters.StochasticSoilProfile =
                new PipingStochasticSoilProfile(0.0, new PipingSoilProfile(string.Empty, 0.0, new[]
                {
                    new PipingSoilLayer(10)
                }, SoilProfileType.SoilProfile1D));

            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = shouldProfileSpecificIllustrationPointsBeCalculated;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = shouldSectionSpecificIllustrationPointsBeCalculated;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                IPartialProbabilisticPipingOutput profileSpecificOutput = calculation.Output.ProfileSpecificOutput;
                IPartialProbabilisticPipingOutput sectionSpecificOutput = calculation.Output.SectionSpecificOutput;

                Assert.IsInstanceOf<PartialProbabilisticSubMechanismPipingOutput>(profileSpecificOutput);
                Assert.IsInstanceOf<PartialProbabilisticSubMechanismPipingOutput>(sectionSpecificOutput);

                Assert.IsFalse(double.IsNaN(profileSpecificOutput.Reliability));
                Assert.IsFalse(double.IsNaN(sectionSpecificOutput.Reliability));

                Assert.AreEqual(shouldProfileSpecificIllustrationPointsBeCalculated, profileSpecificOutput.HasGeneralResult);
                Assert.AreEqual(shouldSectionSpecificIllustrationPointsBeCalculated, sectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ProfileSpecificCalculationFailedWithExceptionAndNoLastErrorPresent_LogsMessagesAndThrowsException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);

            var profileSpecificCalculator = new TestPipingCalculator
            {
                EndInFailure = true
            };

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestPipingCalculator())
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                void Call()
                {
                    try
                    {
                        new ProbabilisticPipingCalculationService().Calculate(
                            calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede '{calculation.Name}' is mislukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(exceptionThrown);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ProfileSpecificCalculationFailedWithExceptionAndLastErrorPresent_LogsMessagesAndThrowsException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);

            var profileSpecificCalculator = new TestPipingCalculator
            {
                EndInFailure = true,
                LastErrorFileContent = "An error occurred"
            };

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestPipingCalculator())
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                void Call()
                {
                    try
                    {
                        new ProbabilisticPipingCalculationService().Calculate(
                            calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede '{calculation.Name}' is mislukt. " +
                                    $"Bekijk het foutrapport door op details te klikken.\r\n{profileSpecificCalculator.LastErrorFileContent}", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(exceptionThrown);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_SectionSpecificCalculationFailedWithExceptionAndNoLastErrorPresent_LogsMessagesAndThrowsException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);

            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                EndInFailure = true
            };

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                void Call()
                {
                    try
                    {
                        new ProbabilisticPipingCalculationService().Calculate(
                            calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak '{calculation.Name}' is mislukt. Er is geen foutrapport beschikbaar.", msgs[2]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(exceptionThrown);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_SectionSpecificCalculationFailedWithExceptionAndLastErrorPresent_LogsMessagesAndThrowsException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);

            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                EndInFailure = true,
                LastErrorFileContent = "An error occurred"
            };

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                void Call()
                {
                    try
                    {
                        new ProbabilisticPipingCalculationService().Calculate(
                            calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak '{calculation.Name}' is mislukt. " +
                                    $"Bekijk het foutrapport door op details te klikken.\r\n{sectionSpecificCalculator.LastErrorFileContent}", msgs[2]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(exceptionThrown);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButProfileSpecificGeneralResultNull_IllustrationPointsNotSetAndLogsMessages()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            const string parserError = "Parser error message";
            var mocks = new MockRepository();
            var profileSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = true;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new ProbabilisticPipingCalculationService().Calculate(calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(5, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual(parserError, msgs[2]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.ProfileSpecificOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.SectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputCalculateProfileSpecificIllustrationPointsFalseAndIllustrationPointsParserErrorMessageNotNull_DoesNotLog()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            const string parserError = "Parser error message";
            var mocks = new MockRepository();
            var profileSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = false;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new ProbabilisticPipingCalculationService().Calculate(calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(4, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.ProfileSpecificOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.SectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButSectionSpecificGeneralResultNull_IllustrationPointsNotSetAndLogsMessages()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            const string parserError = "Parser error message";
            var mocks = new MockRepository();
            var profileSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = true;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new ProbabilisticPipingCalculationService().Calculate(calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(5, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual(parserError, msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.ProfileSpecificOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.SectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputCalculateSectionSpecificIllustrationPointsFalseAndIllustrationPointsParserErrorMessageNotNull_DoesNotLog()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            const string parserError = "Parser error message";
            var mocks = new MockRepository();
            var profileSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsParserErrorMessage = parserError
            };

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = false;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new ProbabilisticPipingCalculationService().Calculate(calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();

                    Assert.AreEqual(4, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.ProfileSpecificOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.SectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButProfileSpecificIllustrationPointResultsOfIncorrectType_IllustrationPointsNotSetAndLogsMessage()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var profileSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            };
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = true;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(5, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual("Het uitlezen van illustratiepunten is mislukt.", msgs[2]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);

                    Assert.IsInstanceOf<IllustrationPointConversionException>(tupleArray[2].Item3);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.ProfileSpecificOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.SectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ProfileSpecificCalculationRanErrorInSettingIllustrationPoints_IllustrationPointsNotSetAndLogsMessage()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var profileSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts()
            };
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = true;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(5, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual($"Fout bij het uitlezen van de illustratiepunten voor berekening {calculation.Name} (doorsnede): " +
                                    "Een of meerdere stochasten hebben dezelfde naam. Het uitlezen van illustratiepunten wordt overgeslagen.", msgs[2]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.ProfileSpecificOutput.HasGeneralResult);
                Assert.IsTrue(calculation.Output.SectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButSectionSpecificIllustrationPointResultsOfIncorrectType_IllustrationPointsNotSetAndLogsMessage()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var profileSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            };

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = true;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(5, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual("Het uitlezen van illustratiepunten is mislukt.", msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);

                    Assert.IsInstanceOf<IllustrationPointConversionException>(tupleArray[3].Item3);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.ProfileSpecificOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.SectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_SectionSpecificCalculationRanErrorInSettingIllustrationPoints_IllustrationPointsNotSetAndLogsMessage()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var profileSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = new TestGeneralResult()
            };
            var sectionSpecificCalculator = new TestPipingCalculator
            {
                IllustrationPointsResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts()
            };

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated = true;
            calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                    Assert.AreEqual(5, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De piping sterkte berekening voor doorsnede is uitgevoerd op de tijdelijke locatie '{profileSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    Assert.AreEqual($"De piping sterkte berekening voor vak is uitgevoerd op de tijdelijke locatie '{sectionSpecificCalculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    Assert.AreEqual($"Fout bij het uitlezen van de illustratiepunten voor berekening {calculation.Name} (vak): " +
                                    "Een of meerdere stochasten hebben dezelfde naam. Het uitlezen van illustratiepunten wordt overgeslagen.", msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });

                Assert.IsNotNull(calculation.Output);
                Assert.IsTrue(calculation.Output.ProfileSpecificOutput.HasGeneralResult);
                Assert.IsFalse(calculation.Output.SectionSpecificOutput.HasGeneralResult);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculators()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            HydraulicBoundaryCalculationSettings calculationSettings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase);

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestPipingCalculator())
                             .Repeat.Twice();
            mocks.ReplayAll();

            calculation.InputParameters.HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, calculationSettings, 0);
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_ExpectedPreprocessorDirectorySetToCalculators()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = validPreprocessorDirectory;

            HydraulicBoundaryCalculationSettings calculationSettings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase);

            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, calculationSettings, 0);
            }

            // Assert
            Assert.IsTrue(profileSpecificCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            Assert.IsTrue(sectionSpecificCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculators()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydraulicBoundaryDatabaseFilePath);
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = "NonExistingPreprocessorDirectory";

            HydraulicBoundaryCalculationSettings calculationSettings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase);

            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ProbabilisticPipingCalculationService().Calculate(
                    calculation, failureMechanism.GeneralInput, calculationSettings, 0);
            }

            // Assert
            Assert.IsFalse(profileSpecificCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            Assert.IsFalse(sectionSpecificCalculator.ReceivedInputs.Single().PreprocessorSetting.RunPreprocessor);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CancelAfterProfileSpecificCalculation_CancelsCalculatorAndOutputNotSet()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator();

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            var service = new ProbabilisticPipingCalculationService();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                profileSpecificCalculator.CalculationFinishedHandler += (sender, args) => service.Cancel();

                // Call
                service.Calculate(calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);
            }

            // Assert
            Assert.IsTrue(profileSpecificCalculator.IsCanceled);
            Assert.IsTrue(sectionSpecificCalculator.IsCanceled);
            Assert.IsNull(calculation.Output);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CancelAfterSectionSpecificCalculation_CancelsCalculatorAndOutputNotSet()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var profileSpecificCalculator = new TestPipingCalculator();
            var sectionSpecificCalculator = new TestPipingCalculator();

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(profileSpecificCalculator)
                             .Repeat.Once();
            calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                             .IgnoreArguments()
                             .Return(sectionSpecificCalculator)
                             .Repeat.Once();
            mocks.ReplayAll();

            var service = new ProbabilisticPipingCalculationService();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                sectionSpecificCalculator.CalculationFinishedHandler += (sender, args) => service.Cancel();

                // Call
                service.Calculate(calculation, failureMechanism.GeneralInput, CreateCalculationSettings(), 0);
            }

            // Assert
            Assert.IsTrue(profileSpecificCalculator.IsCanceled);
            Assert.IsTrue(sectionSpecificCalculator.IsCanceled);
            Assert.IsNull(calculation.Output);
            mocks.VerifyAll();
        }

        private static HydraulicBoundaryCalculationSettings CreateCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                            validHlcdFilePath,
                                                            false,
                                                            string.Empty);
        }

        private static void AssertCalculatorInput(GeneralPipingInput generalInput,
                                                  ProbabilisticPipingInput input,
                                                  double sectionLength,
                                                  bool withCoverageLayer,
                                                  PipingCalculationInput actualInput)
        {
            VariationCoefficientLogNormalDistribution seepageLength = DerivedPipingInput.GetSeepageLength(input);
            LogNormalDistribution thicknessAquiferLayer = DerivedPipingInput.GetThicknessAquiferLayer(input);
            VariationCoefficientLogNormalDistribution darcyPermeability = DerivedPipingInput.GetDarcyPermeability(input);
            VariationCoefficientLogNormalDistribution diameterD70 = DerivedPipingInput.GetDiameterD70(input);

            PipingCalculationInput expectedInput;

            if (withCoverageLayer)
            {
                LogNormalDistribution effectiveThicknessCoverageLayer = DerivedPipingInput.GetEffectiveThicknessCoverageLayer(input, generalInput);
                LogNormalDistribution saturatedVolumicWeightOfCoverageLayer = DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(input);

                expectedInput = new PipingCalculationInput(
                    1300001,
                    sectionLength,
                    input.PhreaticLevelExit.Mean, input.PhreaticLevelExit.StandardDeviation,
                    generalInput.WaterVolumetricWeight,
                    effectiveThicknessCoverageLayer.Mean, effectiveThicknessCoverageLayer.StandardDeviation,
                    saturatedVolumicWeightOfCoverageLayer.Mean, saturatedVolumicWeightOfCoverageLayer.StandardDeviation,
                    saturatedVolumicWeightOfCoverageLayer.Shift,
                    generalInput.UpliftModelFactor.Mean, generalInput.UpliftModelFactor.StandardDeviation,
                    input.DampingFactorExit.Mean, input.DampingFactorExit.StandardDeviation,
                    seepageLength.Mean, seepageLength.CoefficientOfVariation,
                    thicknessAquiferLayer.Mean, thicknessAquiferLayer.StandardDeviation,
                    generalInput.SandParticlesVolumicWeight,
                    generalInput.SellmeijerModelFactor.Mean, generalInput.SellmeijerModelFactor.StandardDeviation,
                    generalInput.BeddingAngle,
                    generalInput.WhitesDragCoefficient,
                    generalInput.WaterKinematicViscosity,
                    darcyPermeability.Mean, darcyPermeability.CoefficientOfVariation,
                    diameterD70.Mean, diameterD70.CoefficientOfVariation,
                    generalInput.Gravity,
                    generalInput.CriticalHeaveGradient.Mean, generalInput.CriticalHeaveGradient.StandardDeviation);
            }
            else
            {
                expectedInput = new PipingCalculationInput(
                    1300001,
                    sectionLength,
                    input.PhreaticLevelExit.Mean, input.PhreaticLevelExit.StandardDeviation,
                    generalInput.WaterVolumetricWeight,
                    generalInput.UpliftModelFactor.Mean, generalInput.UpliftModelFactor.StandardDeviation,
                    input.DampingFactorExit.Mean, input.DampingFactorExit.StandardDeviation,
                    seepageLength.Mean, seepageLength.CoefficientOfVariation,
                    thicknessAquiferLayer.Mean, thicknessAquiferLayer.StandardDeviation,
                    generalInput.SandParticlesVolumicWeight,
                    generalInput.SellmeijerModelFactor.Mean, generalInput.SellmeijerModelFactor.StandardDeviation,
                    generalInput.BeddingAngle,
                    generalInput.WhitesDragCoefficient,
                    generalInput.WaterKinematicViscosity,
                    darcyPermeability.Mean, darcyPermeability.CoefficientOfVariation,
                    diameterD70.Mean, diameterD70.CoefficientOfVariation,
                    generalInput.Gravity,
                    generalInput.CriticalHeaveGradient.Mean, generalInput.CriticalHeaveGradient.StandardDeviation);
            }

            HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
        }

        #endregion
    }
}