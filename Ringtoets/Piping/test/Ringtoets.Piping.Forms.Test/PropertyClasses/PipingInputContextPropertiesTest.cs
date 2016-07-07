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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

using RingtoetsPipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingInputContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            PipingInputContextProperties properties = new PipingInputContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingInputContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var random = new Random(22);

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            StochasticSoilProfile stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
                {
                    new PipingSoilLayer(random.NextDouble())
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };
            StochasticSoilModel stochasticSoilModel = new StochasticSoilModel(0, "StochasticSoilModelName", "StochasticSoilModelSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            HydraulicBoundaryLocation testHydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(0.0);

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = testHydraulicBoundaryLocation
            };
            inputParameters.SurfaceLine = surfaceLine;
            inputParameters.StochasticSoilModel = stochasticSoilModel;
            inputParameters.StochasticSoilProfile = (stochasticSoilProfile);

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            // Call & Assert
            Assert.AreSame(inputParameters.PhreaticLevelExit, properties.PhreaticLevelExit.Distribution);
            Assert.AreSame(inputParameters.DampingFactorExit, properties.DampingFactorExit.Distribution);
            Assert.AreEqual(inputParameters.ThicknessCoverageLayer.Mean, properties.ThicknessCoverageLayer.Distribution.Mean);
            Assert.AreEqual(inputParameters.ThicknessCoverageLayer.StandardDeviation, properties.ThicknessCoverageLayer.Distribution.StandardDeviation);
            Assert.AreSame(inputParameters.Diameter70, properties.Diameter70.Distribution);
            Assert.AreSame(inputParameters.DarcyPermeability, properties.DarcyPermeability.Distribution);
            Assert.AreEqual(inputParameters.ThicknessAquiferLayer.Mean, properties.ThicknessAquiferLayer.Distribution.Mean);
            Assert.AreEqual(inputParameters.ThicknessAquiferLayer.StandardDeviation, properties.ThicknessAquiferLayer.Distribution.StandardDeviation);
            Assert.AreSame(inputParameters.SaturatedVolumicWeightOfCoverageLayer, properties.SaturatedVolumicWeightOfCoverageLayer.Distribution);

            Assert.AreEqual(inputParameters.AssessmentLevel, properties.AssessmentLevel);
            Assert.AreEqual(inputParameters.PiezometricHeadExit, properties.PiezometricHeadExit);

            Assert.AreEqual(inputParameters.SeepageLength.Mean, properties.SeepageLength.Distribution.Mean);
            Assert.AreEqual(inputParameters.SeepageLength.StandardDeviation, properties.SeepageLength.Distribution.StandardDeviation);
            Assert.AreEqual(inputParameters.SeepageLength.Mean, properties.ExitPointL - properties.EntryPointL);
            Assert.AreEqual(inputParameters.ExitPointL, properties.ExitPointL);

            Assert.AreSame(surfaceLine, properties.SurfaceLine);
            Assert.AreSame(stochasticSoilProfile, properties.StochasticSoilProfile);
            Assert.AreSame(stochasticSoilModel, properties.StochasticSoilModel);
            Assert.AreSame(testHydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);

            mocks.ReplayAll();
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.Attach(projectObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            const double entryPointL = 0.12;

            // Call
            properties.EntryPointL = (RoundedDouble) entryPointL;

            // Assert
            Assert.AreEqual(entryPointL, inputParameters.EntryPointL.Value);
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var projectObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 9;
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0.0, 4.0)
                }
            };
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = calculationItem.InputParameters;
            inputParameters.Attach(projectObserver);

            Random random = new Random(22);

            double assessmentLevel = random.NextDouble();

            LogNormalDistribution dampingFactorExit = new LogNormalDistribution(3);
            NormalDistribution phreaticLevelExit = new NormalDistribution(2);
            LogNormalDistribution diameter70 = new LogNormalDistribution(2);
            LogNormalDistribution darcyPermeability = new LogNormalDistribution(3);
            ShiftedLogNormalDistribution saturatedVolumicWeightOfCoverageLoayer = new ShiftedLogNormalDistribution(2);

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            StochasticSoilModel stochasticSoilModel1 = ValidStochasticSoilModel(0.0, 4.0);

            StochasticSoilModel stochasticSoilModel2 = ValidStochasticSoilModel(0.0, 4.0);
            StochasticSoilProfile stochasticSoilProfile2 = stochasticSoilModel2.StochasticSoilProfiles.First();
            stochasticSoilModel2.StochasticSoilProfiles.Add(new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 1234));

            // Call
            new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              new[]
                                              {
                                                  stochasticSoilModel1,
                                                  stochasticSoilModel2
                                              },
                                              failureMechanism,
                                              assessmentSectionMock),
                DampingFactorExit = new LogNormalDistributionDesignVariable(dampingFactorExit),
                PhreaticLevelExit = new NormalDistributionDesignVariable(phreaticLevelExit),
                Diameter70 = new LogNormalDistributionDesignVariable(diameter70),
                DarcyPermeability = new LogNormalDistributionDesignVariable(darcyPermeability),
                SaturatedVolumicWeightOfCoverageLayer = new ShiftedLogNormalDistributionDesignVariable(saturatedVolumicWeightOfCoverageLoayer),
                SurfaceLine = surfaceLine,
                StochasticSoilModel = stochasticSoilModel2,
                StochasticSoilProfile = stochasticSoilProfile2,
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(assessmentLevel)
            };

            // Assert
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel, inputParameters.AssessmentLevel.GetAccuracy());

            Assert.AreEqual(dampingFactorExit.Mean, inputParameters.DampingFactorExit.Mean,
                            inputParameters.DampingFactorExit.GetAccuracy());
            Assert.AreEqual(dampingFactorExit.StandardDeviation, inputParameters.DampingFactorExit.StandardDeviation,
                            inputParameters.DampingFactorExit.GetAccuracy());

            Assert.AreEqual(phreaticLevelExit.Mean, inputParameters.PhreaticLevelExit.Mean,
                            inputParameters.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(phreaticLevelExit.StandardDeviation, inputParameters.PhreaticLevelExit.StandardDeviation,
                            inputParameters.PhreaticLevelExit.GetAccuracy());

            Assert.AreEqual(diameter70.Mean, inputParameters.Diameter70.Mean,
                            inputParameters.Diameter70.GetAccuracy());
            Assert.AreEqual(diameter70.StandardDeviation, inputParameters.Diameter70.StandardDeviation,
                            inputParameters.Diameter70.GetAccuracy());

            Assert.AreEqual(darcyPermeability.Mean, inputParameters.DarcyPermeability.Mean,
                            inputParameters.DarcyPermeability.GetAccuracy());
            Assert.AreEqual(darcyPermeability.StandardDeviation, inputParameters.DarcyPermeability.StandardDeviation,
                            inputParameters.DarcyPermeability.GetAccuracy());

            Assert.AreEqual(saturatedVolumicWeightOfCoverageLoayer.Mean, inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean,
                            inputParameters.SaturatedVolumicWeightOfCoverageLayer.GetAccuracy());
            Assert.AreEqual(saturatedVolumicWeightOfCoverageLoayer.StandardDeviation, inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation,
                            inputParameters.SaturatedVolumicWeightOfCoverageLayer.GetAccuracy());
            Assert.AreEqual(saturatedVolumicWeightOfCoverageLoayer.Shift, inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift,
                            inputParameters.SaturatedVolumicWeightOfCoverageLayer.GetAccuracy());

            Assert.AreEqual(surfaceLine, inputParameters.SurfaceLine);
            Assert.AreEqual(stochasticSoilModel2, inputParameters.StochasticSoilModel);
            Assert.AreEqual(stochasticSoilProfile2, inputParameters.StochasticSoilModel.StochasticSoilProfiles.First());

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0, 3, 3)]
        [TestCase(2, 4, 2)]
        [TestCase(1e-2, 4, 4 - 1e-2)]
        [TestCase(1e-2, 3, 3 - 1e-2)]
        [TestCase(1, 1 + 1e-2, 1e-2)]
        public void SeepageLength_ExitPointAndEntryPointSet_ExpectedValue(double entryPoint, double exitPoint, double seepageLength)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 2;
            inputObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };
            inputParameters.Attach(inputObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock),
                ExitPointL = (RoundedDouble) exitPoint,
                EntryPointL = (RoundedDouble) entryPoint
            };

            // Call & Assert
            Assert.AreEqual(seepageLength, properties.SeepageLength.Distribution.Mean, 1e-6);
            Assert.AreEqual(properties.ExitPointL, inputParameters.ExitPointL);
            Assert.AreEqual(properties.SeepageLength.Distribution.Mean, inputParameters.SeepageLength.Mean);

            mocks.VerifyAll();
        }

        [Test]
        public void SeepageLength_EntryPointAndThenExitPointSet_ExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 2;
            inputObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.SurfaceLine = surfaceLine;
            inputParameters.Attach(inputObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock),
                EntryPointL = (RoundedDouble) 0.5,
                ExitPointL = (RoundedDouble) 2
            };

            // Call & Assert
            Assert.AreEqual(1.5, properties.SeepageLength.Distribution.Mean.Value);
            Assert.AreEqual(properties.ExitPointL, inputParameters.ExitPointL);
            Assert.AreEqual(properties.SeepageLength.Distribution.Mean, inputParameters.SeepageLength.Mean);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(2.0)]
        [TestCase(-5.0)]
        public void ExitPointL_InvalidValue_ThrowsArgumentOutOfRangeException(double newExitPoint)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.SurfaceLine = surfaceLine;

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            properties.EntryPointL = (RoundedDouble) 2.0;

            inputParameters.Attach(inputObserver);

            // Call
            TestDelegate call = () => properties.ExitPointL = (RoundedDouble) newExitPoint;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(
                call,
                RingtoetsPipingDataResources.PipingInput_EntryPointL_greater_or_equal_to_ExitPointL);

            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        [TestCase(2.0)]
        [TestCase(5.0)]
        public void EntryPointL_InvalidValue_ThrowsArgumentOutOfRangeException(double newEntryPoint)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.SurfaceLine = surfaceLine;

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            properties.ExitPointL = (RoundedDouble) 2.0;

            inputParameters.Attach(inputObserver);

            // Call
            TestDelegate call = () => properties.EntryPointL = (RoundedDouble) newEntryPoint;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(
                call,
                RingtoetsPipingDataResources.PipingInput_EntryPointL_greater_or_equal_to_ExitPointL);

            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void EntryPointL_NotOnSurfaceline_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.SurfaceLine = surfaceLine;

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            properties.ExitPointL = (RoundedDouble) 2.0;

            inputParameters.Attach(inputObserver);

            // Call
            TestDelegate call = () => properties.EntryPointL = (RoundedDouble)(-15.0);

            // Assert
            var expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 4]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);

            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void ExitPointL_NotOnSurfaceline_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var inputObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.SurfaceLine = surfaceLine;

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            properties.EntryPointL = (RoundedDouble) 2.0;

            inputParameters.Attach(inputObserver);

            // Call
            TestDelegate call = () => properties.ExitPointL = (RoundedDouble) 10.0;

            // Assert
            var expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 4]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);

            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void HydraulicBoundaryLocation_DesignWaterLevelIsNaN_AssessmentLevelSetToNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            double assessmentLevel = new Random(21).NextDouble();
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, String.Empty, 0.0, 0.0)
                {
                    DesignWaterLevel = assessmentLevel
                }
            };
            inputParameters.Attach(projectObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            string testName = "TestName";
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, testName, 0, 0)
            {
                DesignWaterLevel = double.NaN
            };

            // Call
            properties.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Assert
            Assert.IsNaN(properties.AssessmentLevel.Value);

            mocks.VerifyAll();
        }

        [Test]
        public void HydraulicBoundaryLocation_DesignWaterLevelSet_SetsAssessmentLevelToDesignWaterLevelAndNotifiesOnce()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(1);
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.Attach(projectObserver);

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            double testLevel = new Random(21).NextDouble();
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0)
            {
                DesignWaterLevel = testLevel
            };

            // Call
            properties.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Assert
            Assert.AreEqual(testLevel, properties.AssessmentLevel, 1e-2);

            mocks.VerifyAll();
        }

        [Test]
        public void SurfaceLine_NewSurfaceLine_StochasticSoilModelAndSoilProfileSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0.0, 4.0)
                }
            };
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = calculationItem.InputParameters;
            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };
            inputParameters.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };

            // Call
            properties.SurfaceLine = ValidSurfaceLine(0, 2);

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
        }

        [Test]
        public void SurfaceLine_SameSurfaceLine_SoilProfileUnchanged()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine testSurfaceLine = ValidSurfaceLine(0, 2);
            StochasticSoilProfile stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            StochasticSoilModel stochasticSoilModel = new StochasticSoilModel(0, "StochasticSoilModelName", "StochasticSoilModelSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(stochasticSoilProfile);

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = testSurfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile
            };
            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              new[]
                                              {
                                                  stochasticSoilModel
                                              },
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            // Call
            properties.SurfaceLine = testSurfaceLine;

            // Assert
            Assert.AreSame(stochasticSoilModel, inputParameters.StochasticSoilModel);
            Assert.AreSame(stochasticSoilProfile, inputParameters.StochasticSoilProfile);
        }

        [Test]
        public void SurfaceLine_DifferentSurfaceLine_StochasticSoilModelAndSoilProfileSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            StochasticSoilProfile testPipingSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            StochasticSoilModel stochasticSoilModel = new StochasticSoilModel(0, "StochasticSoilModelName", "StochasticSoilModelSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(testPipingSoilProfile);
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = ValidSurfaceLine(0, 4),
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = testPipingSoilProfile
                }
            };

            PipingInput inputParameters = calculationItem.InputParameters;
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              new[]
                                              {
                                                  stochasticSoilModel
                                              },
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            // Call
            properties.SurfaceLine = ValidSurfaceLine(0, 2);

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
        }

        [Test]
        public void StochasticSoilProfile_DifferentStochasticSoilModel_SoilProfileSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine testSurfaceLine = ValidSurfaceLine(0, 2);
            StochasticSoilProfile stochasticSoilProfile1 = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            StochasticSoilModel stochasticSoilModel1 = new StochasticSoilModel(0, "StochasticSoilModel1Name", "StochasticSoilModelSegment1Name");
            stochasticSoilModel1.StochasticSoilProfiles.Add(stochasticSoilProfile1);

            StochasticSoilProfile stochasticSoilProfile2 = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            StochasticSoilModel stochasticSoilModel2 = new StochasticSoilModel(0, "StochasticSoilModel2Name", "StochasticSoilModelSegment2Name");
            stochasticSoilModel1.StochasticSoilProfiles.Add(stochasticSoilProfile2);

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = testSurfaceLine,
                StochasticSoilModel = stochasticSoilModel1,
                StochasticSoilProfile = stochasticSoilProfile1
            };
            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInputContextProperties properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };

            // Call
            properties.StochasticSoilModel = stochasticSoilModel2;

            // Assert
            Assert.IsNull(inputParameters.StochasticSoilProfile);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenCompletePipingInputContextProperties_WhenPhreaticLevelExitPropertiesSetThroughProperties_ThenPiezometricHeadExitUpdated(int propertyIndexToChange)
        {
            // Given
            var mocks = new MockRepository();
            var typeDescriptorContextMock = mocks.StrictMock<ITypeDescriptorContext>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            PipingCalculationScenario calculationItem = new PipingCalculationScenario(new GeneralPipingInput());
            PipingFailureMechanism failureMechanism = new PipingFailureMechanism();

            PipingInput inputParameters = new PipingInput(new GeneralPipingInput());
            PipingInputContextProperties contextProperties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              calculationItem,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              failureMechanism,
                                              assessmentSectionMock)
            };
            inputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0)
            {
                DesignWaterLevel = 1.0
            };

            DesignVariable<NormalDistribution> phreaticLevelExitProperty = contextProperties.PhreaticLevelExit;
            DynamicPropertyBag dynamicPropertyBag = new DynamicPropertyBag(contextProperties);
            typeDescriptorContextMock.Expect(tdc => tdc.Instance).Return(dynamicPropertyBag).Repeat.Twice();
            typeDescriptorContextMock.Stub(tdc => tdc.PropertyDescriptor).Return(dynamicPropertyBag.GetProperties()["PhreaticLevelExit"]);
            mocks.ReplayAll();

            PropertyDescriptorCollection properties = new NormalDistributionDesignVariableTypeConverter().GetProperties(typeDescriptorContextMock, phreaticLevelExitProperty);
            Assert.NotNull(properties);

            // When
            properties[propertyIndexToChange].SetValue(phreaticLevelExitProperty, (RoundedDouble) 2.3);

            // Then
            Assert.IsFalse(double.IsNaN(inputParameters.PiezometricHeadExit));
            mocks.VerifyAll();
        }

        private static StochasticSoilModel ValidStochasticSoilModel(double xMin, double xMax)
        {
            StochasticSoilModel stochasticSoilModel = new StochasticSoilModel(0, "StochasticSoilModelName", "StochasticSoilModelSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 1234)
            {
                SoilProfile = new TestPipingSoilProfile()
            });
            stochasticSoilModel.Geometry.Add(new Point2D(xMin, 1.0));
            stochasticSoilModel.Geometry.Add(new Point2D(xMax, 0.0));
            return stochasticSoilModel;
        }

        private static RingtoetsPipingSurfaceLine ValidSurfaceLine(double xMin, double xMax)
        {
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(xMin, 0.0, 0.0),
                new Point3D(xMax, 0.0, 1.0)
            });
            return surfaceLine;
        }

        private class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
        {
            public TestHydraulicBoundaryLocation(double designWaterLevel) : base(0, string.Empty, 0, 0)
            {
                DesignWaterLevel = designWaterLevel;
            }
        }
    }
}