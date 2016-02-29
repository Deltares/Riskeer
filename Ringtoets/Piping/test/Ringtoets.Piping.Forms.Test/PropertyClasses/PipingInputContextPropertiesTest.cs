using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingInputContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingInputContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingInputContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var random = new Random(22);

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var soilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var testHydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var inputParameters = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                HydraulicBoundaryLocation = testHydraulicBoundaryLocation
            };

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock)
            };

            // Call & Assert
            Assert.AreSame(inputParameters.PhreaticLevelExit, properties.PhreaticLevelExitHeave.Distribution);
            Assert.AreSame(inputParameters.PhreaticLevelExit, properties.PhreaticLevelExitSellmeijer.Distribution);
            Assert.AreSame(inputParameters.PhreaticLevelExit, properties.PhreaticLevelExitUplift.Distribution);
            Assert.AreSame(inputParameters.DampingFactorExit, properties.DampingFactorExitUplift.Distribution);
            Assert.AreSame(inputParameters.DampingFactorExit, properties.DampingFactorExitHeave.Distribution);
            Assert.AreSame(inputParameters.ThicknessCoverageLayer, properties.ThicknessCoverageLayerHeave.Distribution);
            Assert.AreSame(inputParameters.ThicknessCoverageLayer, properties.ThicknessCoverageLayerSellmeijer.Distribution);
            Assert.AreSame(inputParameters.Diameter70, properties.Diameter70.Distribution);
            Assert.AreSame(inputParameters.DarcyPermeability, properties.DarcyPermeability.Distribution);
            Assert.AreSame(inputParameters.ThicknessAquiferLayer, properties.ThicknessAquiferLayer.Distribution);

            Assert.AreEqual(inputParameters.UpliftModelFactor, properties.UpliftModelFactor);
            Assert.AreEqual(inputParameters.PiezometricHeadExit, properties.PiezometricHeadExitHeave);
            Assert.AreEqual(inputParameters.PiezometricHeadExit, properties.PiezometricHeadExitUplift);
            Assert.AreEqual(inputParameters.PiezometricHeadPolder, properties.PiezometricHeadPolderHeave);
            Assert.AreEqual(inputParameters.PiezometricHeadPolder, properties.PiezometricHeadPolderUplift);
            Assert.AreEqual(inputParameters.AssessmentLevel, properties.AssessmentLevelSellmeijer);
            Assert.AreEqual(inputParameters.AssessmentLevel, properties.AssessmentLevelUplift);
            Assert.AreEqual(inputParameters.SellmeijerModelFactor, properties.SellmeijerModelFactor);

            Assert.AreEqual(inputParameters.CriticalHeaveGradient, properties.CriticalHeaveGradient);
            Assert.AreEqual(inputParameters.SellmeijerReductionFactor, properties.SellmeijerReductionFactor);
            Assert.AreEqual(inputParameters.Gravity, properties.Gravity);
            Assert.AreEqual(inputParameters.WaterKinematicViscosity, properties.WaterKinematicViscosity);
            Assert.AreEqual(inputParameters.WaterVolumetricWeight, properties.WaterVolumetricWeightSellmeijer);
            Assert.AreEqual(inputParameters.WaterVolumetricWeight, properties.WaterVolumetricWeightUplift);
            Assert.AreEqual(inputParameters.SandParticlesVolumicWeight, properties.SandParticlesVolumicWeight);
            Assert.AreEqual(inputParameters.WhitesDragCoefficient, properties.WhitesDragCoefficient);
            Assert.AreEqual(inputParameters.BeddingAngle, properties.BeddingAngle);
            Assert.AreEqual(inputParameters.MeanDiameter70, properties.MeanDiameter70);

            Assert.AreSame(inputParameters.SeepageLength, properties.SeepageLength.Distribution);
            Assert.AreEqual(inputParameters.SeepageLength.Mean, properties.ExitPointL - properties.EntryPointL);
            Assert.AreEqual(inputParameters.ExitPointL, properties.ExitPointL);

            Assert.AreSame(surfaceLine, properties.SurfaceLine);
            Assert.AreSame(soilProfile, properties.SoilProfile);
            Assert.AreSame(testHydraulicBoundaryLocation, properties.HydraulicBoundaryLocation);

            mocks.ReplayAll();
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var inputParameters = new PipingInput();
            inputParameters.Attach(projectObserver);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock)
            };

            // Call & Assert
            const double assessmentLevel = 0.12;
            properties.AssessmentLevelSellmeijer = assessmentLevel;
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel);
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var projectObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 25;
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            var inputParameters = new PipingInput();
            inputParameters.Attach(projectObserver);

            Random random = new Random(22);

            double assessmentLevel = random.NextDouble();
            double waterVolumetricWeight = random.NextDouble();
            double upliftModelFactor = random.NextDouble();
            double piezometricHeadExit = random.NextDouble();
            double piezometricHeadPolder = random.NextDouble();
            double sellmeijerModelFactor = random.NextDouble();
            double sellmeijerReductionFactor = random.NextDouble();
            double sandParticlesVolumicWeight = random.NextDouble();
            double whitesDragCoefficient = random.NextDouble();
            double waterKinematicViscosity = random.NextDouble();
            double gravity = random.NextDouble();
            double meanDiameter70 = random.NextDouble();
            double beddingAngle = random.NextDouble();
            double entryPointL = random.NextDouble();
            double exitPointL = entryPointL + random.NextDouble() + 0.001;
            
            var dampingFactorExit = new LognormalDistribution();
            var phreaticLevelExit = new NormalDistribution();
            var thicknessCoverageLayer = new LognormalDistribution();
            var seepageLength = new LognormalDistribution();
            var diameter70 = new LognormalDistribution();
            var darcyPermeability = new LognormalDistribution();
            var thicknessAquiferLayer = new LognormalDistribution();

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingSoilProfile soilProfile = new TestPipingSoilProfile();
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            var inputContext = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock),
                AssessmentLevelSellmeijer = assessmentLevel,
                WaterVolumetricWeightUplift = waterVolumetricWeight,
                UpliftModelFactor = upliftModelFactor,
                PiezometricHeadExitUplift = piezometricHeadExit,
                DampingFactorExitHeave = new LognormalDistributionDesignVariable(dampingFactorExit),
                PhreaticLevelExitHeave = new NormalDistributionDesignVariable(phreaticLevelExit),
                PiezometricHeadPolderHeave = piezometricHeadPolder,
                ThicknessCoverageLayerSellmeijer = new LognormalDistributionDesignVariable(thicknessCoverageLayer),
                SellmeijerModelFactor = sellmeijerModelFactor,
                SellmeijerReductionFactor = sellmeijerReductionFactor,
                SeepageLength = new LognormalDistributionDesignVariable(seepageLength),
                SandParticlesVolumicWeight = sandParticlesVolumicWeight,
                WhitesDragCoefficient = whitesDragCoefficient,
                Diameter70 = new LognormalDistributionDesignVariable(diameter70),
                DarcyPermeability = new LognormalDistributionDesignVariable(darcyPermeability),
                WaterKinematicViscosity = waterKinematicViscosity,
                Gravity = gravity,
                ThicknessAquiferLayer = new LognormalDistributionDesignVariable(thicknessAquiferLayer),
                MeanDiameter70 = meanDiameter70,
                BeddingAngle = beddingAngle,
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                HydraulicBoundaryLocation = hydraulicBoundaryLocation
            };

            // Assert I
            Assert.AreEqual(assessmentLevel, inputParameters.AssessmentLevel);
            Assert.AreEqual(waterVolumetricWeight, inputParameters.WaterVolumetricWeight);
            Assert.AreEqual(upliftModelFactor, inputParameters.UpliftModelFactor);
            Assert.AreEqual(piezometricHeadExit, inputParameters.PiezometricHeadExit);
            Assert.AreEqual(dampingFactorExit, inputParameters.DampingFactorExit);
            Assert.AreEqual(phreaticLevelExit, inputParameters.PhreaticLevelExit);
            Assert.AreEqual(piezometricHeadPolder, inputParameters.PiezometricHeadPolder);
            Assert.AreEqual(thicknessCoverageLayer, inputParameters.ThicknessCoverageLayer);
            Assert.AreEqual(sellmeijerModelFactor, inputParameters.SellmeijerModelFactor);
            Assert.AreEqual(sellmeijerReductionFactor, inputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(seepageLength, inputParameters.SeepageLength);
            Assert.AreEqual(sandParticlesVolumicWeight, inputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(whitesDragCoefficient, inputParameters.WhitesDragCoefficient);
            Assert.AreEqual(diameter70, inputParameters.Diameter70);
            Assert.AreEqual(darcyPermeability, inputParameters.DarcyPermeability);
            Assert.AreEqual(waterKinematicViscosity, inputParameters.WaterKinematicViscosity);
            Assert.AreEqual(gravity, inputParameters.Gravity);
            Assert.AreEqual(thicknessAquiferLayer, inputParameters.ThicknessAquiferLayer);
            Assert.AreEqual(meanDiameter70, inputParameters.MeanDiameter70);
            Assert.AreEqual(beddingAngle, inputParameters.BeddingAngle);
            Assert.AreEqual(soilProfile, inputParameters.SoilProfile);
            Assert.AreEqual(hydraulicBoundaryLocation, inputParameters.HydraulicBoundaryLocation);
            
            // Call
            inputContext.ExitPointL = exitPointL;
            inputContext.EntryPointL = entryPointL;

            // Assert II
            Assert.AreEqual(exitPointL - entryPointL, inputParameters.SeepageLength.Mean);
            Assert.AreEqual(exitPointL, inputParameters.ExitPointL);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(2, 2, 0)]
        [TestCase(2, 4, -2)]
        [TestCase(0.5, 4, -3.5)]
        [TestCase(1e-6, 4, -(4 - 1e-6))]
        [TestCase(3, 1e-6, 3 - 1e-6)]
        [TestCase(0.5, 1e-6, 0.5-1e-6)]
        public void EntryPointL_ExitPointAndSeepageLengthSet_ExpectedValue(double exitPoint, double seepageLength, double entryPoint)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var random = new Random(22);

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var soilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var inputParameters = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock)
            };

            properties.ExitPointL = exitPoint;
            properties.SeepageLength.Distribution.Mean = seepageLength;

            // Call & Assert
            Assert.AreEqual(entryPoint, properties.EntryPointL);
            Assert.AreEqual(properties.ExitPointL, inputParameters.ExitPointL);
            Assert.AreEqual(properties.SeepageLength.Distribution.Mean, inputParameters.SeepageLength.Mean);

            mocks.ReplayAll();
        }

        [Test]
        [TestCase(0, 3, 3)]
        [TestCase(2, 4, 2)]
        [TestCase(1e-6, 4, 4 - 1e-6)]
        [TestCase(1e-6, 3, 3 - 1e-6)]
        [TestCase(0, 1e-6, 1e-6)]
        public void SeepageLength_ExitPointAndEntryPointSet_ExpectedValue(double entryPoint, double exitPoint, double seepageLength)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var random = new Random(22);

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var soilProfile = new PipingSoilProfile(string.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var inputParameters = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock)
            };

            properties.ExitPointL = exitPoint;
            properties.EntryPointL = entryPoint;

            // Call & Assert
            Assert.AreEqual(seepageLength, properties.SeepageLength.Distribution.Mean);
            Assert.AreEqual(properties.ExitPointL, inputParameters.ExitPointL);
            Assert.AreEqual(properties.SeepageLength.Distribution.Mean, inputParameters.SeepageLength.Mean);

            mocks.ReplayAll();
        }

        [Test]
        public void SeepageLength_EntryPointAndThenExitPointSet_ExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var random = new Random(22);

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var soilProfile = new PipingSoilProfile(string.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var inputParameters = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock)
            };

            properties.EntryPointL = -1;
            properties.ExitPointL = 2;

            // Call & Assert
            Assert.AreEqual(3, properties.SeepageLength.Distribution.Mean);
            Assert.AreEqual(properties.ExitPointL, inputParameters.ExitPointL);
            Assert.AreEqual(properties.SeepageLength.Distribution.Mean, inputParameters.SeepageLength.Mean);

            mocks.ReplayAll();
        }

        [Test]
        public void EntryPointL_SetResultInInvalidSeePage_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var random = new Random(22);

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var soilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var inputParameters = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock)
            };

            var l = 2.0;
            properties.ExitPointL = l;

            // Call
            TestDelegate test = () => properties.EntryPointL = l;

            // Assert
            var message = string.Format(Resources.PipingInputContextProperties_EntryPointL_Value_0_results_in_invalid_seepage_length, l);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);

            mocks.ReplayAll();
        }

        [Test]
        public void ExitPointL_SetResultInInvalidSeePage_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var random = new Random(22);

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var soilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var inputParameters = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock)
            };

            var l = -2.0;
            properties.EntryPointL = l;

            // Call
            TestDelegate test = () => properties.ExitPointL = l;

            // Assert
            var message = string.Format(Resources.PipingInputContextProperties_ExitPointL_Value_0_results_in_invalid_seepage_length, l);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);

            mocks.ReplayAll();
        }

        [Test]
        public void HydraulicBoundaryLocation_DesignWaterLevelIsNaN_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var projectObserver = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var inputParameters = new PipingInput();
            inputParameters.Attach(projectObserver);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock)
            };

            string testName = "TestName";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, testName, 0, 0)
            {
                DesignWaterLevel = double.NaN
            };

            // Call
            TestDelegate test = () => properties.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Assert
            var message = string.Format("Voor locatie '{0}' is geen toetspeil berekend.", testName);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);

            Assert.AreEqual(0, properties.AssessmentLevelSellmeijer);
            Assert.AreEqual(0, properties.AssessmentLevelUplift);

            mocks.VerifyAll();
        }

        [Test]
        public void HydraulicBoundaryLocation_DesignWaterLevelSet_SetsAssessmentLevelToDesignWaterLevelAndNotifiesOnce()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(1);
            mocks.ReplayAll();

            var inputParameters = new PipingInput();
            inputParameters.Attach(projectObserver);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<PipingSoilProfile>(),
                                              assessmentSectionMock)
            };

            double testLevel = new Random(21).NextDouble();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0)
            {
                DesignWaterLevel = testLevel
            };

            // Call
            properties.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Assert
            Assert.AreEqual(testLevel, properties.AssessmentLevelSellmeijer);
            Assert.AreEqual(testLevel, properties.AssessmentLevelUplift);

            mocks.VerifyAll();
        }

        private static RingtoetsPipingSurfaceLine ValidSurfaceLine(double xMin, double xMax)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(xMin, 0.0, 0.0),
                new Point3D(xMax, 0.0, 1.0)
            });
            return surfaceLine;
        }

        private class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
        {
            public TestHydraulicBoundaryLocation() : base(0, string.Empty, 0, 0) { }
        }
    }
}