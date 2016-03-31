using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

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
            }, SoilProfileType.SoilProfile1D, 0);
            var testHydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(0.0);

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = testHydraulicBoundaryLocation
            };
            inputParameters.SurfaceLine = surfaceLine;
            inputParameters.SoilProfile = (soilProfile);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
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

            var inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.Attach(projectObserver);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
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
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var projectObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 8;
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            var inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.Attach(projectObserver);

            Random random = new Random(22);

            double assessmentLevel = random.NextDouble();

            var dampingFactorExit = new LognormalDistribution(3);
            var phreaticLevelExit = new NormalDistribution(2);
            var diameter70 = new LognormalDistribution(2);
            var darcyPermeability = new LognormalDistribution(3);
            var saturatedVolumicWeightOfCoverageLoayer = new ShiftedLognormalDistribution(2);

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            PipingSoilProfile soilProfile = new TestPipingSoilProfile();

            // Call
            new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              assessmentSectionMock),
                DampingFactorExit = new LognormalDistributionDesignVariable(dampingFactorExit),
                PhreaticLevelExit = new NormalDistributionDesignVariable(phreaticLevelExit),
                Diameter70 = new LognormalDistributionDesignVariable(diameter70),
                DarcyPermeability = new LognormalDistributionDesignVariable(darcyPermeability),
                SaturatedVolumicWeightOfCoverageLayer = new ShiftedLognormalDistributionDesignVariable(saturatedVolumicWeightOfCoverageLoayer),
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
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
            Assert.AreEqual(soilProfile, inputParameters.SoilProfile);

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
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var inputObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 2;
            inputObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };
            inputParameters.Attach(inputObserver);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
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
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var inputObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 2;
            inputObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.SurfaceLine = surfaceLine;
            inputParameters.Attach(inputObserver);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
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
        public void EntryPointL_SetResultInInvalidSeePage_SeepageLengthValueNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              assessmentSectionMock)
            };

            const double l = 2.0;
            properties.ExitPointL = (RoundedDouble) l;

            inputParameters.Attach(inputObserver);

            // Call
            properties.EntryPointL = (RoundedDouble) l;

            // Assert
            Assert.IsNaN(properties.SeepageLength.GetDesignValue());

            mocks.VerifyAll();
        }

        [Test]
        public void ExitPointL_SetResultInInvalidSeePage_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.SurfaceLine = surfaceLine;

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              assessmentSectionMock)
            };

            const double l = 2.0;
            properties.EntryPointL = (RoundedDouble) l;

            inputParameters.Attach(inputObserver);

            // Call
            properties.ExitPointL = (RoundedDouble) l;

            // Assert
            Assert.IsNaN(properties.SeepageLength.GetDesignValue());

            mocks.VerifyAll();
        }

        [Test]
        public void HydraulicBoundaryLocation_DesignWaterLevelIsNaN_AssessmentLevelSetToNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            double assessmentLevel = new Random(21).NextDouble();
            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, String.Empty, 0.0, 0.0)
                {
                    DesignWaterLevel = assessmentLevel
                }
            };
            inputParameters.Attach(projectObserver);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              assessmentSectionMock)
            };

            string testName = "TestName";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, testName, 0, 0)
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
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(1);
            mocks.ReplayAll();

            var inputParameters = new PipingInput(new GeneralPipingInput());
            inputParameters.Attach(projectObserver);

            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
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
            Assert.AreEqual(testLevel, properties.AssessmentLevel, 1e-2);

            mocks.VerifyAll();
        }

        [Test]
        public void SurfaceLine_NewSurfaceLine_SoilProfileSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var inputParameters = new PipingInput(new GeneralPipingInput());
            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              assessmentSectionMock)
            };
            inputParameters.SoilProfile = new TestPipingSoilProfile();

            // Call
            properties.SurfaceLine = ValidSurfaceLine(0, 2);

            // Assert
            Assert.IsNull(inputParameters.SoilProfile);
        }

        [Test]
        public void SurfaceLine_SameSurfaceLine_SoilProfileUnchanged()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var testSurfaceLine = ValidSurfaceLine(0, 2);
            var testPipingSoilProfile = new TestPipingSoilProfile();

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = testSurfaceLine,
                SoilProfile = testPipingSoilProfile
            };
            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              assessmentSectionMock)
            };

            // Call
            properties.SurfaceLine = testSurfaceLine;

            // Assert
            Assert.AreSame(testPipingSoilProfile, inputParameters.SoilProfile);
        }

        [Test]
        public void SurfaceLine_DifferentSurfaceLine_SoilProfileSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var testPipingSoilProfile = new TestPipingSoilProfile();

            var inputParameters = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = ValidSurfaceLine(0, 2),
                SoilProfile = testPipingSoilProfile
            };
            var properties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              assessmentSectionMock)
            };

            // Call
            properties.SurfaceLine = ValidSurfaceLine(0, 2);

            // Assert
            Assert.IsNull(inputParameters.SoilProfile);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenCompletePipingInputContextProperties_WhenPhreaticLevelExitPropertiesSetThroughProperties_ThenPiezometricHeadExitUpdated(int propertyIndexToChange)
        {
            // Given
            var mocks = new MockRepository();
            var typeDescriptorContextMock = mocks.StrictMock<ITypeDescriptorContext>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            var inputParameters = new PipingInput(new GeneralPipingInput());
            var contextProperties = new PipingInputContextProperties
            {
                Data = new PipingInputContext(inputParameters,
                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                              Enumerable.Empty<StochasticSoilModel>(),
                                              assessmentSectionMock)
            };
            inputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0)
            {
                DesignWaterLevel = 1.0
            };

            DesignVariable<NormalDistribution> phreaticLevelExitProperty = contextProperties.PhreaticLevelExit;
            var dynamicPropertyBag = new DynamicPropertyBag(contextProperties);
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
            public TestHydraulicBoundaryLocation(double designWaterLevel) : base(0, string.Empty, 0, 0)
            {
                DesignWaterLevel = designWaterLevel;
            }
        }
    }
}