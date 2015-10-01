using Deltares.WTIPiping;

namespace Wti.Calculation.Piping
{
    /// <summary>
    /// This class represents a combination of piping sub-calculations, which together can be used
    /// to assess based on piping.
    /// </summary>
    public class PipingCalculation {

        private readonly PipingCalculationInput input;

        /// <summary>
        /// Constructs a new <see cref="PipingCalculation"/>. The <paramref name="input"/> is used to
        /// obtain the parameters used in the different sub calculations.
        /// </summary>
        /// <param name="input">The <see cref="PipingCalculationInput"/> containing all the values required
        /// for performing a piping calculation.</param>
        public PipingCalculation(PipingCalculationInput input)
        {
            this.input = input;
        }

        /// <summary>
        /// Performs the actual sub calculations and returns a <see cref="PipingCalculationResult"/>, which
        /// contains the results of all sub calculations.
        /// </summary>
        /// <returns>A <see cref="PipingCalculationResult"/> containing the results of the sub calculations.</returns>
        public PipingCalculationResult Calculate()
        {
            try
            {
                var upliftResultContainer = CalculateUplift();
                var heaveResultContainer = CalulateHeave();
                var sellmejerResultContainer = CalulateSellmeijer();

                return new PipingCalculationResult(
                    upliftResultContainer.Zu,
                    upliftResultContainer.FoSu,
                    heaveResultContainer.Zh,
                    heaveResultContainer.FoSh,
                    sellmejerResultContainer.Zp,
                    sellmejerResultContainer.FoSp
                    );
            }
            catch (UpliftCalculatorException e)
            {
                throw new PipingCalculationException(e.Message);
            }
            catch (PipingException<HeaveCalculator> e)
            {
                throw new PipingCalculationException(e.Message);
            }
            catch (PipingException<EffectiveThicknessCalculator> e)
            {
                throw new PipingCalculationException(e.Message);
            }
            catch (PipingException<Sellmeijer2011Calculator> e)
            {
                throw new PipingCalculationException(e.Message);
            }
        }
        
        private HeaveCalculator CalulateHeave()
        {
            var calculator = new HeaveCalculator
            {
                Ich = input.CriticalHeaveGradient,
                PhiExit = input.PiezometricHeadExit,
                DTotal = input.ThicknessCoverageLayer,
                PhiPolder = input.PiezometricHeadPolder,
                RExit = input.DampingFactorExit,
                HExit = input.PhreaticLevelExit
            };
            calculator.Calculate();
            return calculator;
        }

        private WTIUpliftCalculator CalculateUplift()
        {
            var effectiveStressResult = CalculateEffectiveThickness();

            var calculator = new WTIUpliftCalculator
            {
                VolumetricWeightOfWater = input.WaterVolumetricWeight,
                ModelFactorUplift = input.UpliftModelFactor,
                EffectiveStress = effectiveStressResult.EffectiveStress,
                HRiver = input.AssessmentLevel,
                PhiExit = input.PiezometricHeadExit,
                RExit = input.DampingFactorExit,
                HExit = input.PhreaticLevelExit,
                PhiPolder = input.PiezometricHeadPolder
            };
            calculator.Calculate();
            return calculator;
        }

        private Sellmeijer2011Calculator CalulateSellmeijer()
        {
            var calculator = new Sellmeijer2011Calculator
            {
                ModelFactorPiping = input.SellmeijerModelFactor,
                HRiver = input.AssessmentLevel,
                HExit = input.PhreaticLevelExit,
                Rc = input.SellmeijerReductionFactor,
                DTotal = input.ThicknessCoverageLayer,
                SeepageLength = input.SeepageLength,
                GammaSubParticles = input.SandParticlesVolumicWeight,
                WhitesDragCoefficient = input.WhitesDragCoefficient,
                D70 = input.Diameter70,
                VolumetricWeightOfWater = input.WaterVolumetricWeight,
                DarcyPermeability = input.DarcyPermeability,
                KinematicViscosityWater = input.WaterKinematicViscosity,
                Gravity = input.Gravity,
                DAquifer = input.ThicknessAquiferLayer,
                D70Mean = input.MeanDiameter70,
                BeddingAngle = input.BeddingAngle
            };
            calculator.Calculate();
            return calculator;
        }

        private EffectiveThicknessCalculator CalculateEffectiveThickness()
        {
            var calculator = new EffectiveThicknessCalculator
            {
                ExitPointXCoordinate = input.ExitPointXCoordinate,
                PhreaticLevel = input.PhreaticLevelExit,
                SoilProfile = new PipingProfileCreator().Create(),
                SurfaceLine = new PipingSurfaceLineCreator().Create(),
                VolumicWeightOfWater = input.WaterVolumetricWeight
            };
            calculator.Calculate();
            return calculator;
        }
    }
}