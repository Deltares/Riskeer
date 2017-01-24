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
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.DuneErosion.Service.Test
{
    [TestFixture]
    public class DuneErosionBoundaryCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Calculate_DuneLocationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneErosionBoundaryCalculationService().Calculate(null,
                                                                                            new DuneErosionFailureMechanism(),
                                                                                            "1",
                                                                                            1,
                                                                                            validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneLocation", exception.ParamName);
        }

        [Test]
        public void Calculate_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneErosionBoundaryCalculationService().Calculate(new TestDuneLocation(),
                                                                                            null,
                                                                                            "1",
                                                                                            1,
                                                                                            validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Calculate_ValidData_CalculationStartedWithRightParameters()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            var mechanismSpecificNorm = failureMechanism.GetMechanismSpecificNorm(1.0/200);

            const string ringId = "1";

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDunesBoundaryConditionsCalculator testCalculator =
                    ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;

                // Call
                new DuneErosionBoundaryCalculationService().Calculate(duneLocation,
                                                                      failureMechanism,
                                                                      "1",
                                                                      mechanismSpecificNorm,
                                                                      validFilePath);

                // Assert
                Assert.AreEqual(testDataPath, testCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(ringId, testCalculator.RingId);

                DunesBoundaryConditionsCalculationInput expectedInput = CreateInput(duneLocation, mechanismSpecificNorm);
                AssertInput(expectedInput, testCalculator.ReceivedInputs.First());
                Assert.IsFalse(testCalculator.IsCanceled);
            }
        }

        [Test]
        public void Calculate_CalculationRan_SetOutput()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            double mechanismSpecificNorm = failureMechanism.GetMechanismSpecificNorm(1.0/200);

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            // Precondition
            Assert.IsNull(duneLocation.Output);

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDunesBoundaryConditionsCalculator testCalculator =
                    ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                testCalculator.ReliabilityIndex = 3.27052;
                testCalculator.WaterLevel = 4.82912;
                testCalculator.WaveHeight = 2.88936;
                testCalculator.WavePeriod = 10.65437;

                // Call
                Action test = () => new DuneErosionBoundaryCalculationService().Calculate(duneLocation,
                                                                                          failureMechanism,
                                                                                          "1",
                                                                                          mechanismSpecificNorm,
                                                                                          validFilePath);

                // Assert
                TestHelper.AssertLogMessages(test, messages =>
                                             {
                                                 var msgs = messages.ToArray();
                                                 Assert.AreEqual(3, msgs.Length);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' gestart om: ", msgs[0]);
                                                 StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' beëindigd om: ", msgs[2]);
                                             });
                double targetReliability = StatisticsConverter.ProbabilityToReliability(mechanismSpecificNorm);
                double calculatedProbability = StatisticsConverter.ReliabilityToProbability(testCalculator.ReliabilityIndex);

                Assert.IsNotNull(duneLocation.Output);
                Assert.AreEqual(testCalculator.ReliabilityIndex, duneLocation.Output.CalculatedReliability.Value);
                Assert.AreEqual(calculatedProbability, duneLocation.Output.CalculatedProbability);
                Assert.AreEqual(mechanismSpecificNorm, duneLocation.Output.TargetProbability);
                Assert.AreEqual(targetReliability, duneLocation.Output.TargetReliability, duneLocation.Output.TargetReliability.GetAccuracy());
                Assert.AreEqual(testCalculator.WaterLevel, duneLocation.Output.WaterLevel, duneLocation.Output.WaterLevel.GetAccuracy());
                Assert.AreEqual(testCalculator.WaveHeight, duneLocation.Output.WaveHeight, duneLocation.Output.WaveHeight.GetAccuracy());
                Assert.AreEqual(testCalculator.WavePeriod, duneLocation.Output.WavePeriod, duneLocation.Output.WavePeriod.GetAccuracy());
            }
        }

        [Test]
        public void Calculate_CalculationRanNotConverged_LogMessage()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDunesBoundaryConditionsCalculator testCalculator =
                    ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                testCalculator.ReliabilityIndex = 0.01;

                // Call
                Action test = () => new DuneErosionBoundaryCalculationService().Calculate(duneLocation,
                                                                                          failureMechanism,
                                                                                          "1",
                                                                                          failureMechanism.GetMechanismSpecificNorm(1.0/200),
                                                                                          validFilePath);

                // Assert
                TestHelper.AssertLogMessages(test, messages =>
                                             {
                                                 var msgs = messages.ToArray();
                                                 Assert.AreEqual(4, msgs.Length);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' gestart om: ", msgs[0]);
                                                 StringAssert.StartsWith($"Duinafslag berekening voor locatie '{duneLocation.Name}' is niet geconvergeerd.", msgs[1]);
                                                 StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' beëindigd om: ", msgs[3]);
                                             });
            }
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDunesBoundaryConditionsCalculator testCalculator =
                    ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;

                var service = new DuneErosionBoundaryCalculationService();
                testCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(duneLocation,
                                  failureMechanism,
                                  "1",
                                  failureMechanism.GetMechanismSpecificNorm(1.0/200),
                                  validFilePath);

                // Assert
                Assert.IsTrue(testCalculator.IsCanceled);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDunesBoundaryConditionsCalculator calculator =
                    ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                calculator.LastErrorFileContent = "An error occurred";
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DuneErosionBoundaryCalculationService().Calculate(duneLocation,
                                                                              failureMechanism,
                                                                              "1",
                                                                              failureMechanism.GetMechanismSpecificNorm(1.0/200),
                                                                              validFilePath);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                                             {
                                                 var msgs = messages.ToArray();
                                                 Assert.AreEqual(4, msgs.Length);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' gestart om: ", msgs[0]);
                                                 StringAssert.StartsWith($"De berekening voor duinafslag '{duneLocation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                                                 StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' beëindigd om: ", msgs[3]);
                                             });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(duneLocation.Output);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDunesBoundaryConditionsCalculator calculator =
                    ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DuneErosionBoundaryCalculationService().Calculate(duneLocation,
                                                                              failureMechanism,
                                                                              "1",
                                                                              failureMechanism.GetMechanismSpecificNorm(1.0/200),
                                                                              validFilePath);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                                             {
                                                 var msgs = messages.ToArray();
                                                 Assert.AreEqual(4, msgs.Length);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' gestart om: ", msgs[0]);
                                                 StringAssert.StartsWith($"De berekening voor duinafslag '{duneLocation.Name}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                                                 StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' beëindigd om: ", msgs[3]);
                                             });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(duneLocation.Output);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 0,
                                                    Offset = 0,
                                                    Orientation = 0,
                                                    D50 = 0.000007
                                                });

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestDunesBoundaryConditionsCalculator calculator =
                    ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorFileContent = "An error occurred";

                var exceptionThrown = false;
                var exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DuneErosionBoundaryCalculationService().Calculate(duneLocation,
                                                                              failureMechanism,
                                                                              "1",
                                                                              failureMechanism.GetMechanismSpecificNorm(1.0/200),
                                                                              validFilePath);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exceptionThrown = true;
                        exceptionMessage = e.Message;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                                             {
                                                 var msgs = messages.ToArray();
                                                 Assert.AreEqual(4, msgs.Length);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' gestart om: ", msgs[0]);
                                                 StringAssert.StartsWith($"De berekening voor duinafslag '{duneLocation.Name}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                                                 StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                                                 StringAssert.StartsWith($"Berekening van '{duneLocation.Name}' beëindigd om: ", msgs[3]);
                                             });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(duneLocation.Output);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }
        }

        private static void AssertInput(DunesBoundaryConditionsCalculationInput expectedInput, DunesBoundaryConditionsCalculationInput actualInput)
        {
            Assert.AreEqual(expectedInput.Section.SectionId, actualInput.Section.SectionId);
            Assert.AreEqual(expectedInput.Section.CrossSectionNormal, actualInput.Section.CrossSectionNormal);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, actualInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Beta, actualInput.Beta);
        }

        private static DunesBoundaryConditionsCalculationInput CreateInput(DuneLocation duneLocation, double norm)
        {
            return new DunesBoundaryConditionsCalculationInput(1, duneLocation.Id, norm);
        }
    }
}