namespace Wti.Calculation.Piping
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
        private readonly double reductionFactorSellmeijer;
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
        private readonly double exitPointXCoordinate;

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

        public double ReductionFactorSellmeijer
        {
            get
            {
                return reductionFactorSellmeijer;
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

        public double ExitPointXCoordinate
        {
            get
            {
                return exitPointXCoordinate;
            }
        }

        #endregion

        /// <summary>
        /// Constructs a new <see cref="PipingCalculationInput"/>, which contains values for the parameters used
        /// in the piping sub calculations.
        /// </summary>
        /// <param name="waterVolumetricWeight">The volumetric weight of water.</param>
        /// <param name="upliftModelFactor">Calculation value used to account for uncertainty in the model for uplift.</param>
        /// <param name="effectiveStress">The effective stress of a layer.</param>
        /// <param name="assessmentLevel">Outside high water level.</param>
        /// <param name="piezometricHeadExit">Piezometric head at the exit point.</param>
        /// <param name="dampingFactorExit">Damping factor at the exit point.</param>
        /// <param name="phreaticLevelExit">Phreatic level at the exit point.</param>
        /// <param name="piezometricHeadPolder">Piezometric head in the hinterland.</param>
        /// <param name="criticalHeaveGradient">Critical exit gradient for heave.</param>
        /// <param name="thicknessCoverageLayer">The total thickness of the coverage layer at the exit point.</param>
        /// <param name="sellmeijerModelFactor">Calculation value used to account for uncertainty in the model for Sellmeijer.</param>
        /// <param name="reductionFactorSellmeijer">Reduction factor Sellmeijer.</param>
        /// <param name="seepageLength">Horizontal distance between entree and exit point.</param>
        /// <param name="sandParticlesVolumicWeight">The (lowerbound) volumic weight of sand grain material of a sand layer under water.</param>
        /// <param name="whitesDragCoefficient">White's drag coefficient.</param>
        /// <param name="diameter70">Sieve size through which 70% fraction of the grains of the top part of the aquifer passes.</param>
        /// <param name="darcyPermeability">Darcy-speed with which water flows through the aquifer layer.</param>
        /// <param name="waterKinematicViscosity">The kinematic viscosity of water at 10 degrees Celsius.</param>
        /// <param name="gravity">Gravitational acceleration.</param>
        /// <param name="thicknessAquiferLayer">The thickness of the aquifer layer.</param>
        /// <param name="meanDiameter70">Mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.</param>
        /// <param name="beddingAngle">Angle of the force balance representing the amount in which sand grains resist rolling.</param>
        /// <param name="exitPointXCoordinate">X coordinate of the exit point.</param>
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
            double reductionFactorSellmeijer, 
            double seepageLength, 
            double sandParticlesVolumicWeight, 
            double whitesDragCoefficient, 
            double diameter70, 
            double darcyPermeability, 
            double waterKinematicViscosity, 
            double gravity, 
            double thicknessAquiferLayer, 
            double meanDiameter70, 
            double beddingAngle, 
            double exitPointXCoordinate)
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
            this.reductionFactorSellmeijer = reductionFactorSellmeijer;
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
            this.exitPointXCoordinate = exitPointXCoordinate;
        }
    }
}