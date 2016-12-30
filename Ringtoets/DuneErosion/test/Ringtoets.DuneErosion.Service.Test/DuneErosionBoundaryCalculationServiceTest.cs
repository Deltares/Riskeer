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
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.DuneErosion.Service.Test
{
    [TestFixture]
    public class DuneErosionBoundaryCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Calculate_WithZeroContributionForFailureMechanism_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 0
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
                                                                         {
                                                                             failureMechanism
                                                                         });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
                                                                                                                {
                                                                                                                    failureMechanism
                                                                                                                }, 1, 1.0/300));
            mocks.ReplayAll();

            var duneLocation = new TestDuneLocation();
            bool exceptionThrown = false;

            // Call
            Action call = () =>
            {
                try
                {
                    new DuneErosionBoundaryCalculationService().Calculate(duneLocation, failureMechanism, assessmentSection, validFilePath);
                }
                catch (Exception)
                {
                    exceptionThrown = true;
                }
            };

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
                                         {
                                             var msgs = messages.ToArray();
                                             Assert.AreEqual(3, msgs.Length);
                                             var name = duneLocation.Name;
                                             StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", name), msgs[0]);
                                             Assert.AreEqual("De bijdrage van dit toetsspoor is nul. Daardoor kunnen de berekeningen niet worden uitgevoerd.", msgs[1]);
                                             StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", name), msgs[2]);
                                         });
            Assert.IsTrue(exceptionThrown);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidData_CalculationStartedWithRightParameters()
        {
            // Setup
            const double norm = 1.0 / 200;
            const double contribution = 10;
            const string ringId = "1";
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = contribution
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();            
            assessmentSection.Stub(a => a.Id).Return(ringId);
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, norm));
            mocks.ReplayAll();

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;

                // Call
                new DuneErosionBoundaryCalculationService().Calculate(duneLocation, failureMechanism, assessmentSection, validFilePath);

                // Assert
                Assert.AreEqual(testDataPath, testCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(ringId, testCalculator.RingId);

                var expectedInput = CreateInput(duneLocation, failureMechanism.GetMechanismSpecificNorm(assessmentSection));
                AssertInput(expectedInput, testCalculator.ReceivedInputs.First());
                Assert.IsFalse(testCalculator.IsCanceled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationRan_SetOutput()
        {
            // Setup
            const double norm = 1.0/200;
            const double contribution = 10;
            const string ringId = "1";
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = contribution
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(ringId);
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
                                                                         {
                                                                             failureMechanism
                                                                         });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
                                                                                                                {
                                                                                                                    failureMechanism
                                                                                                                }, 1, norm));
            mocks.ReplayAll();

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            // Precondition
            Assert.IsNull(duneLocation.Output);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                testCalculator.ReliabilityIndex = 3.27052;
                testCalculator.WaterLevel = 4.82912;
                testCalculator.WaveHeight = 2.88936;
                testCalculator.WavePeriod = 10.65437;

                // Call
                Action test = () => new DuneErosionBoundaryCalculationService().Calculate(duneLocation, failureMechanism, assessmentSection, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(test, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", duneLocation.Name), msgs[0]);
                    StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", duneLocation.Name), msgs[2]);
                });
                double mechanismSpecificNorm = failureMechanism.GetMechanismSpecificNorm(assessmentSection);
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
            const double norm = 1.0/200;
            const double contribution = 10;
            const string ringId = "1";
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = contribution
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(ringId);
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
                                                                         {
                                                                             failureMechanism
                                                                         });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
                                                                                                                {
                                                                                                                    failureMechanism
                                                                                                                }, 1, norm));
            mocks.ReplayAll();

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                testCalculator.ReliabilityIndex = 0.01;                

                // Call
                Action test = () => new DuneErosionBoundaryCalculationService().Calculate(duneLocation, failureMechanism, assessmentSection, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(test, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", duneLocation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Duinafslag berekening voor locatie '{0}' is niet geconvergeerd.", duneLocation.Name), msgs[1]);
                    StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", duneLocation.Name), msgs[3]);
                });
            }
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            const double norm = 1.0 / 200;
            const double contribution = 10;
            const string ringId = "1";
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = contribution
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(ringId);
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, norm));
            mocks.ReplayAll();

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testCalculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;

                var service = new DuneErosionBoundaryCalculationService();
                testCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(duneLocation, failureMechanism, assessmentSection, validFilePath);

                // Assert
                Assert.IsTrue(testCalculator.IsCanceled);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            const double norm = 1.0 / 200;
            const double contribution = 10;
            const string ringId = "1";
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = contribution
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(ringId);
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, norm));
            mocks.ReplayAll();

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                calculator.LastErrorFileContent = "An error occurred";
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DuneErosionBoundaryCalculationService().Calculate(duneLocation, failureMechanism, assessmentSection, validFilePath);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", duneLocation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("De berekening voor duinafslag '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", duneLocation.Name), msgs[1]);
                    StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", duneLocation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(duneLocation.Output);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            const double norm = 1.0 / 200;
            const double contribution = 10;
            const string ringId = "1";
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = contribution
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(ringId);
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, norm));
            mocks.ReplayAll();

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                calculator.EndInFailure = true;

                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DuneErosionBoundaryCalculationService().Calculate(duneLocation, failureMechanism, assessmentSection, validFilePath);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", duneLocation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("De berekening voor duinafslag '{0}' is niet gelukt. Er is geen foutrapport beschikbaar.", duneLocation.Name), msgs[1]);
                    StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", duneLocation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(duneLocation.Output);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            const double norm = 1.0 / 200;
            const double contribution = 10;
            const string ringId = "1";
            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = contribution
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(ringId);
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, norm));
            mocks.ReplayAll();

            var duneLocation = new DuneLocation(1300001, "test", new Point2D(0, 0), 3, 0, 0, 0.000007);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).DunesBoundaryConditionsCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorFileContent = "An error occurred";

                var exceptionThrown = false;
                var exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DuneErosionBoundaryCalculationService().Calculate(duneLocation, failureMechanism, assessmentSection, validFilePath);
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
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", duneLocation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("De berekening voor duinafslag '{0}' is niet gelukt. Bekijk het foutrapport door op details te klikken.",
                                                          duneLocation.Name), msgs[1]);
                    StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", duneLocation.Name), msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(duneLocation.Output);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }
            mocks.VerifyAll();
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
            return new DunesBoundaryConditionsCalculationInput(1, duneLocation.Id, norm, duneLocation.Orientation);
        }
    }
}