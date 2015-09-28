namespace Wti.Calculation
{
    /// <summary>
    /// This class contains all the parameters that are required to perform a piping assessment.
    /// </summary>
    public class PipingCalculationInput
    {
        private readonly double waterVolumetricWeight;
        private readonly double upliftModelFactor;
        private readonly double effectiveStress;
        private readonly double assessmentLevel;
        private readonly double piezometricHeadExit;
        private readonly double dampingFactorExit;
        private readonly double phreaticLevelExit;
        private readonly double piezometricHeadPolder;
        private readonly double criticalHeaveGradient;
        private readonly double thicknessCoverageLayer;
        private readonly double sellmeijerModelFactor;
        private readonly double reductionFactor;
        private readonly double seepageLength;
        private readonly double sandParticlesVolumicWeight;
        private readonly double whitesDragCoefficient;
        private readonly double diameter70;
        private readonly double darcyPermeability;
        private readonly double waterKinematicViscosity;
        private readonly double gravity;
        private readonly double thicknessAquiferLayer;
        private readonly double meanDiameter70;
        private readonly double beddingAngle;

        #region properties

        public double WaterVolumetricWeight
        {
            get
            {
                return waterVolumetricWeight;
            }
        }

        public double UpliftModelFactor
        {
            get
            {
                return upliftModelFactor;
            }
        }

        public double EffectiveStress
        {
            get
            {
                return effectiveStress;
            }
        }

        public double AssessmentLevel
        {
            get
            {
                return assessmentLevel;
            }
        }

        public double PiezometricHeadExit
        {
            get
            {
                return piezometricHeadExit;
            }
        }

        public double DampingFactorExit
        {
            get
            {
                return dampingFactorExit;
            }
        }

        public double PhreaticLevelExit
        {
            get
            {
                return phreaticLevelExit;
            }
        }

        public double PiezometricHeadPolder
        {
            get
            {
                return piezometricHeadPolder;
            }
        }

        public double CriticalHeaveGradient
        {
            get
            {
                return criticalHeaveGradient;
            }
        }

        public double ThicknessCoverageLayer
        {
            get
            {
                return thicknessCoverageLayer;
            }
        }

        public double SellmeijerModelFactor {
            get
            {
                return sellmeijerModelFactor;
            }
        }

        public double ReductionFactor
        {
            get
            {
                return reductionFactor;
            }
        }

        public double SeepageLength
        {
            get
            {
                return seepageLength;
            }
        }

        public double SandParticlesVolumicWeight
        {
            get
            {
                return sandParticlesVolumicWeight;
            }
        }

        public double WhitesDragCoefficient
        {
            get
            {
                return whitesDragCoefficient;
            }
        }

        public double Diameter70
        {
            get
            {
                return diameter70;
            }
        }

        public double DarcyPermeability
        {
            get
            {
                return darcyPermeability;
            }
        }

        public double WaterKinematicViscosity
        {
            get
            {
                return waterKinematicViscosity;
            }
        }

        public double Gravity
        {
            get
            {
                return gravity;
            }
        }

        public double ThicknessAquiferLayer
        {
            get
            {
                return thicknessAquiferLayer;
            }
        }

        public double MeanDiameter70
        {
            get
            {
                return meanDiameter70;
            }
        }

        public double BeddingAngle
        {
            get
            {
                return beddingAngle;
            }
        }

        #endregion

        /// <summary>
        /// Constructs a new <see cref="PipingCalculationInput"/>, which contains values for the parameters used
        /// in the piping sub calculations.
        /// </summary>
        /// <param name="waterVolumetricWeight">The volumetric weight of water.</param>
        /// <param name="upliftModelFactor">A model factor used in the uplift sub calculations.</param>
        /// <param name="effectiveStress">The effective stress of a layer.</param>
        /// <param name="assessmentLevel">The water level to assess for.</param>
        /// <param name="piezometricHeadExit"></param>
        /// <param name="dampingFactorExit"></param>
        /// <param name="phreaticLevelExit"></param>
        /// <param name="piezometricHeadPolder"></param>
        /// <param name="criticalHeaveGradient"></param>
        /// <param name="thicknessCoverageLayer">The thickness of the coverage layer in the soil profile.</param>
        /// <param name="sellmeijerModelFactor">A model factor used in the Sellmeijer sub calculation.</param>
        /// <param name="reductionFactor"></param>
        /// <param name="seepageLength"></param>
        /// <param name="sandParticlesVolumicWeight"></param>
        /// <param name="whitesDragCoefficient"></param>
        /// <param name="diameter70"></param>
        /// <param name="darcyPermeability"></param>
        /// <param name="waterKinematicViscosity">The kinematic viscosity of water.</param>
        /// <param name="gravity">The ammount of gravitation to calculate with.</param>
        /// <param name="thicknessAquiferLayer">The thickness of the aquifer layer in the soil profile.</param>
        /// <param name="meanDiameter70"></param>
        /// <param name="beddingAngle"></param>
        public PipingCalculationInput(
            double waterVolumetricWeight,
            double upliftModelFactor,
            double effectiveStress,
            double assessmentLevel,
            double piezometricHeadExit,
            double dampingFactorExit,
            double phreaticLevelExit,
            double piezometricHeadPolder,
            double criticalHeaveGradient,
            double thicknessCoverageLayer, 
            double sellmeijerModelFactor, 
            double reductionFactor, 
            double seepageLength, 
            double sandParticlesVolumicWeight, 
            double whitesDragCoefficient, 
            double diameter70, 
            double darcyPermeability, 
            double waterKinematicViscosity, 
            double gravity, 
            double thicknessAquiferLayer, 
            double meanDiameter70, 
            double beddingAngle
            )
        {
            this.waterVolumetricWeight = waterVolumetricWeight;
            this.upliftModelFactor = upliftModelFactor;
            this.effectiveStress = effectiveStress;
            this.assessmentLevel = assessmentLevel;
            this.piezometricHeadExit = piezometricHeadExit;
            this.dampingFactorExit = dampingFactorExit;
            this.phreaticLevelExit = phreaticLevelExit;
            this.piezometricHeadPolder = piezometricHeadPolder;
            this.criticalHeaveGradient = criticalHeaveGradient;
            this.thicknessCoverageLayer = thicknessCoverageLayer;
            this.sellmeijerModelFactor = sellmeijerModelFactor;
            this.reductionFactor = reductionFactor;
            this.seepageLength = seepageLength;
            this.sandParticlesVolumicWeight = sandParticlesVolumicWeight;
            this.whitesDragCoefficient = whitesDragCoefficient;
            this.diameter70 = diameter70;
            this.darcyPermeability = darcyPermeability;
            this.waterKinematicViscosity = waterKinematicViscosity;
            this.gravity = gravity;
            this.thicknessAquiferLayer = thicknessAquiferLayer;
            this.meanDiameter70 = meanDiameter70;
            this.beddingAngle = beddingAngle;
        }
    }
}