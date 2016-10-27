using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.ClosingStructures;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Create.ClosingStructures
{
    [TestFixture]
    public class ClosingStructureCalculationCreateExtensionsTest
    {
        [Test]
        public void Create_RegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            // Call
            TestDelegate call = () => calculation.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_ValidCalculation_ReturnClosingStructuresCalculationEntity()
        {
            // Setup
            var random = new Random(45);
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = "A",
                Comments = "B",
                InputParameters =
                {
                    StormDuration =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                    },
                    StructureNormalOrientation = (RoundedDouble) random.NextDouble(),
                    FailureProbabilityStructureWithErosion = (RoundedDouble) random.NextDouble(),
                    UseForeshore = random.NextBoolean(),
                    UseBreakWater = random.NextBoolean(),
                    BreakWater =
                    {
                        Type = BreakWaterType.Dam,
                        Height = (RoundedDouble) random.NextDouble()
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = (RoundedDouble) random.NextDouble()
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        CoefficientOfVariation = (RoundedDouble) random.NextDouble()
                    },
                    InflowModelType = ClosingStructureInflowModelType.VerticalWall,
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    DeviationWaveDirection = (RoundedDouble) random.NextDouble(),
                    DrainCoefficient =
                    {
                        Mean = (RoundedDouble) random.NextDouble()
                    },
                    FactorStormDurationOpenStructure = (RoundedDouble) random.NextDouble(),
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    FailureProbabilityOpenStructure = (RoundedDouble) random.NextDouble(),
                    FailureProbabilityReparation = (RoundedDouble) random.NextDouble(),
                    IdenticalApertures = random.Next(),
                    LevelCrestStructureNotClosing =
                    {
                        Mean = (RoundedDouble) random.NextDouble(),
                        StandardDeviation = (RoundedDouble) random.NextDouble()
                    },
                    ProbabilityOpenStructureBeforeFlooding = (RoundedDouble) random.NextDouble()
                }
            };

            var registry = new PersistenceRegistry();

            const int order = 67;

            // Call
            ClosingStructuresCalculationEntity entity = calculation.Create(registry, order);

            // Assert
            Assert.AreEqual(calculation.Name, entity.Name);
            Assert.AreEqual(calculation.Comments, entity.Comments);

            ClosingStructuresInput inputParameters = calculation.InputParameters;
            Assert.AreEqual(inputParameters.StormDuration.Mean.Value, entity.StormDurationMean);
            Assert.AreEqual(inputParameters.StructureNormalOrientation.Value, entity.StructureNormalOrientation);
            Assert.AreEqual(inputParameters.FailureProbabilityStructureWithErosion, entity.FailureProbabilityStructureWithErosion);
            Assert.IsNull(entity.ClosingStructureEntity);
            Assert.IsNull(entity.HydraulicLocationEntity);
            Assert.IsNull(entity.ForeshoreProfileEntity);
            Assert.AreEqual(Convert.ToByte(inputParameters.UseForeshore), entity.UseForeshore);
            Assert.AreEqual(Convert.ToByte(inputParameters.UseBreakWater), entity.UseBreakWater);
            Assert.AreEqual(Convert.ToInt16(inputParameters.BreakWater.Type), entity.BreakWaterType);
            Assert.AreEqual(inputParameters.BreakWater.Height.Value, entity.BreakWaterHeight);
            Assert.AreEqual(inputParameters.AllowedLevelIncreaseStorage.Mean.Value, entity.AllowedLevelIncreaseStorageMean);
            Assert.AreEqual(inputParameters.AllowedLevelIncreaseStorage.StandardDeviation.Value, entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.AreEqual(inputParameters.StorageStructureArea.Mean.Value, entity.StorageStructureAreaMean);
            Assert.AreEqual(inputParameters.StorageStructureArea.CoefficientOfVariation.Value, entity.StorageStructureAreaCoefficientOfVariation);
            Assert.AreEqual(inputParameters.FlowWidthAtBottomProtection.Mean.Value, entity.FlowWidthAtBottomProtectionMean);
            Assert.AreEqual(inputParameters.FlowWidthAtBottomProtection.StandardDeviation.Value, entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.AreEqual(inputParameters.CriticalOvertoppingDischarge.Mean.Value, entity.CriticalOvertoppingDischargeMean);
            Assert.AreEqual(inputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation.Value, entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.AreEqual(inputParameters.ModelFactorSuperCriticalFlow.Mean.Value, entity.ModelFactorSuperCriticalFlowMean);
            Assert.AreEqual(inputParameters.WidthFlowApertures.Mean.Value, entity.WidthFlowAperturesMean);
            Assert.AreEqual(inputParameters.WidthFlowApertures.CoefficientOfVariation.Value, entity.WidthFlowAperturesCoefficientOfVariation);
            Assert.AreEqual(Convert.ToByte(inputParameters.InflowModelType), entity.InflowModelType);
            Assert.AreEqual(inputParameters.InsideWaterLevel.Mean.Value, entity.InsideWaterLevelMean);
            Assert.AreEqual(inputParameters.InsideWaterLevel.StandardDeviation.Value, entity.InsideWaterLevelStandardDeviation);
            Assert.AreEqual(inputParameters.DeviationWaveDirection.Value, entity.DeviationWaveDirection);
            Assert.AreEqual(inputParameters.DrainCoefficient.Mean.Value, entity.DrainCoefficientMean);
            Assert.AreEqual(inputParameters.FactorStormDurationOpenStructure.Value, entity.FactorStormDurationOpenStructure);
            Assert.AreEqual(inputParameters.ThresholdHeightOpenWeir.Mean.Value, entity.ThresholdHeightOpenWeirMean);
            Assert.AreEqual(inputParameters.ThresholdHeightOpenWeir.StandardDeviation.Value, entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.AreEqual(inputParameters.AreaFlowApertures.Mean.Value, entity.AreaFlowAperturesMean);
            Assert.AreEqual(inputParameters.AreaFlowApertures.StandardDeviation.Value, entity.AreaFlowAperturesStandardDeviation);
            Assert.AreEqual(inputParameters.FailureProbabilityOpenStructure, entity.FailureProbabilityOpenStructure);
            Assert.AreEqual(inputParameters.FailureProbabilityReparation, entity.FailureProbablityReparation);
            Assert.AreEqual(inputParameters.IdenticalApertures, entity.IdenticalApertures);
            Assert.AreEqual(inputParameters.LevelCrestStructureNotClosing.Mean.Value, entity.LevelCrestStructureNotClosingMean);
            Assert.AreEqual(inputParameters.LevelCrestStructureNotClosing.StandardDeviation.Value, entity.LevelCrestStructureNotClosingStandardDeviation);
            Assert.AreEqual(inputParameters.ProbabilityOpenStructureBeforeFlooding, entity.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_CalculationWithParametersNaN_ReturnEntityWithNullParameters()
        {
            // Setup
            var random = new Random(45);
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    StormDuration =
                    {
                        Mean = RoundedDouble.NaN,
                    },
                    StructureNormalOrientation = RoundedDouble.NaN,
                    BreakWater =
                    {
                        Height = RoundedDouble.NaN
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    StorageStructureArea =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = RoundedDouble.NaN
                    },
                    WidthFlowApertures =
                    {
                        Mean = RoundedDouble.NaN,
                        CoefficientOfVariation = RoundedDouble.NaN
                    },
                    InsideWaterLevel =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    DeviationWaveDirection = RoundedDouble.NaN,
                    DrainCoefficient =
                    {
                        Mean = RoundedDouble.NaN
                    },
                    FactorStormDurationOpenStructure = RoundedDouble.NaN,
                    ThresholdHeightOpenWeir =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    AreaFlowApertures =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    },
                    LevelCrestStructureNotClosing =
                    {
                        Mean = RoundedDouble.NaN,
                        StandardDeviation = RoundedDouble.NaN
                    }
                }
            };

            var registry = new PersistenceRegistry();

            const int order = 67;

            // Call
            ClosingStructuresCalculationEntity entity = calculation.Create(registry, order);

            // Assert
            Assert.IsNull(entity.StormDurationMean);
            Assert.IsNull(entity.StructureNormalOrientation);
            Assert.IsNull(entity.BreakWaterHeight);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.IsNull(entity.StorageStructureAreaMean);
            Assert.IsNull(entity.StorageStructureAreaCoefficientOfVariation);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionMean);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.IsNull(entity.CriticalOvertoppingDischargeMean);
            Assert.IsNull(entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.IsNull(entity.ModelFactorSuperCriticalFlowMean);
            Assert.IsNull(entity.WidthFlowAperturesMean);
            Assert.IsNull(entity.WidthFlowAperturesCoefficientOfVariation);
            Assert.IsNull(entity.InsideWaterLevelMean);
            Assert.IsNull(entity.InsideWaterLevelStandardDeviation);
            Assert.IsNull(entity.DeviationWaveDirection);
            Assert.IsNull(entity.DrainCoefficientMean);
            Assert.IsNull(entity.FactorStormDurationOpenStructure);
            Assert.IsNull(entity.ThresholdHeightOpenWeirMean);
            Assert.IsNull(entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.IsNull(entity.AreaFlowAperturesMean);
            Assert.IsNull(entity.AreaFlowAperturesStandardDeviation);
            Assert.IsNull(entity.LevelCrestStructureNotClosingMean);
            Assert.IsNull(entity.LevelCrestStructureNotClosingStandardDeviation);
        }

        [Test]
        public void Create_CalculationWithStructure_ReturnEntityWithStructureEntity()
        {
            // Setup
            ClosingStructure alreadyRegisteredStructure = new TestClosingStructure();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = alreadyRegisteredStructure
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new ClosingStructureEntity(), alreadyRegisteredStructure);

            // Call
            ClosingStructuresCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity.ClosingStructureEntity);
        }

        [Test]
        public void Create_CalculationWithHydraulicBoundaryLocation_ReturnEntityWithHydraulicLocationEntity()
        {
            // Setup
            var alreadyRegisteredHydroLocation = new HydraulicBoundaryLocation(1, "A", 2, 3);
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = alreadyRegisteredHydroLocation
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new HydraulicLocationEntity(), alreadyRegisteredHydroLocation);

            // Call
            ClosingStructuresCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_CalculationWithForeshoreProfile_ReturnEntityWithForeshoreProfileEntity()
        {
            // Setup
            var alreadyRegisteredForeshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                                         new Point2D[0],
                                                                         null,
                                                                         new ForeshoreProfile.ConstructionProperties());
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = alreadyRegisteredForeshoreProfile
                }
            };

            var registry = new PersistenceRegistry();
            registry.Register(new ForeshoreProfileEntity(), alreadyRegisteredForeshoreProfile);

            // Call
            ClosingStructuresCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity.ForeshoreProfileEntity);
        }
    }
}