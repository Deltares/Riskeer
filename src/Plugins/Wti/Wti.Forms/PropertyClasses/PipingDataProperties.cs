using DelftTools.Shell.Gui;
using DelftTools.Utils;
using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingDataPropertiesDisplayName")]
    public class PipingDataProperties : ObjectProperties<PipingData>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataWaterVolumetricWeightDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataWaterVolumetricWeightDescription")]
        public double WaterVolumetricWeight
        {
            get
            {
                return data.WaterVolumetricWeight;
            }
            set
            {
                data.WaterVolumetricWeight = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataUpliftModelFactorDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataUpliftModelFactorDescription")]
        public double UpliftModelFactor
        {
            get
            {
                return data.UpliftModelFactor;
            }
            set
            {
                data.UpliftModelFactor = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataAssessmentLevelDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataAssessmentLevelDescription")]
        public double AssessmentLevel
        {
            get
            {
                return data.AssessmentLevel;
            }
            set
            {
                data.AssessmentLevel = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataPiezometricHeadExitDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataPiezometricHeadExitDescription")]
        public double PiezometricHeadExit
        {
            get
            {
                return data.PiezometricHeadExit;
            }
            set
            {
                data.PiezometricHeadExit = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataDampingFactorExitDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataDampingFactorExitDescription")]
        public double DampingFactorExit
        {
            get
            {
                return data.DampingFactorExit;
            }
            set
            {
                data.DampingFactorExit = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataPhreaticLevelExitDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataPhreaticLevelExitDescription")]
        public double PhreaticLevelExit
        {
            get
            {
                return data.PhreaticLevelExit;
            }
            set
            {
                data.PhreaticLevelExit = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataPiezometricHeadPolderDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataPiezometricHeadPolderDescription")]
        public double PiezometricHeadPolder
        {
            get
            {
                return data.PiezometricHeadPolder;
            }
            set
            {
                data.PiezometricHeadPolder = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataCriticalHeaveGradientDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataCriticalHeaveGradientDescription")]
        public double CriticalHeaveGradient
        {
            get
            {
                return data.CriticalHeaveGradient;
            }
            set
            {
                data.CriticalHeaveGradient = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataThicknessCoverageLayerDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataThicknessCoverageLayerDescription")]
        public double ThicknessCoverageLayer
        {
            get
            {
                return data.ThicknessCoverageLayer;
            }
            set
            {
                data.ThicknessCoverageLayer = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataSellmeijerModelFactorDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSellmeijerModelFactorDescription")]
        public double SellmeijerModelFactor
        {
            get
            {
                return data.SellmeijerModelFactor;
            }
            set
            {
                data.SellmeijerModelFactor = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataSellmeijerReductionFactorDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSellmeijerReductionFactorDescription")]
        public double SellmeijerReductionFactor
        {
            get
            {
                return data.SellmeijerReductionFactor;
            }
            set
            {
                data.SellmeijerReductionFactor = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataSeepageLengthDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSeepageLengthDescription")]
        public double SeepageLength
        {
            get
            {
                return data.SeepageLength;
            }
            set
            {
                data.SeepageLength = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataSandParticlesVolumicWeightDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSandParticlesVolumicWeightDescription")]
        public double SandParticlesVolumicWeight
        {
            get
            {
                return data.SandParticlesVolumicWeight;
            }
            set
            {
                data.SandParticlesVolumicWeight = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataWhitesDragCoefficientDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataWhitesDragCoefficientDescription")]
        public double WhitesDragCoefficient
        {
            get
            {
                return data.WhitesDragCoefficient;
            }
            set
            {
                data.WhitesDragCoefficient = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataDiameter70DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataDiameter70Description")]
        public double Diameter70
        {
            get
            {
                return data.Diameter70;
            }
            set
            {
                data.Diameter70 = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataDarcyPermeabilityDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataDarcyPermeabilityDescription")]
        public double DarcyPermeability
        {
            get
            {
                return data.DarcyPermeability;
            }
            set
            {
                data.DarcyPermeability = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataWaterKinematicViscosityDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataWaterKinematicViscosityDescription")]
        public double WaterKinematicViscosity
        {
            get
            {
                return data.WaterKinematicViscosity;
            }
            set
            {
                data.WaterKinematicViscosity = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataGravityDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataGravityDescription")]
        public double Gravity
        {
            get
            {
                return data.Gravity;
            }
            set
            {
                data.Gravity = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataThicknessAquiferLayerDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataThicknessAquiferLayerDescription")]
        public double ThicknessAquiferLayer
        {
            get
            {
                return data.ThicknessAquiferLayer;
            }
            set
            {
                data.ThicknessAquiferLayer = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataMeanDiameter70DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataMeanDiameter70Description")]
        public double MeanDiameter70
        {
            get
            {
                return data.MeanDiameter70;
            }
            set
            {
                data.MeanDiameter70 = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataBeddingAngleDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataBeddingAngleDescription")]
        public double BeddingAngle
        {
            get
            {
                return data.BeddingAngle;
            }
            set
            {
                data.BeddingAngle = value;
                data.NotifyObservers();
            }
        }
    }
}