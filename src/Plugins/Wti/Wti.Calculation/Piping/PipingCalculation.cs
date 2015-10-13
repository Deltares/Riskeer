using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Deltares.WTIPiping;

namespace Wti.Calculation.Piping
{
    /// <summary>
    /// This class represents a combination of piping sub-calculations, which together can be used
    /// to assess based on piping.
    /// </summary>
    public class PipingCalculation
    {
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
        /// <exception cref="PipingCalculationException">Thrown when any of the invocations of the sub-calculations from the kernel throws an Exception.</exception>
        public PipingCalculationResult Calculate()
        {
            var upliftResult = CalculateUplift();
            var heaveResult = CalculateHeave();
            var sellmeijerResult = CalculateSellmeijer();

            return new PipingCalculationResult(
                upliftResult.Zu,
                upliftResult.FoSu,
                heaveResult.Zh,
                heaveResult.FoSh,
                sellmeijerResult.Zp,
                sellmeijerResult.FoSp
            );
        }

        /// <summary>
        /// Returns a list of validation messages. The validation messages are based on the values of the <see cref="PipingCalculationInput"/>
        /// which was provided to this <see cref="PipingCalculation"/> and are determined by the Piping kernel.
        /// </summary>
        public List<string> Validate()
        {
            List<string> upliftCalculatorValidationResults = CreateUpliftCalculator().Validate();
            List<string> heaveCalculatorValidationResults = CreateHeaveCalculator().Validate();
            List<string> sellmeijerCalculatorValidationResults = CreateSellmeijerCalculator().Validate();

            return upliftCalculatorValidationResults.Concat(heaveCalculatorValidationResults).Concat(sellmeijerCalculatorValidationResults).ToList();
        }

        private Sellmeijer2011Calculator CalculateSellmeijer()
        {
            Sellmeijer2011Calculator sellmeijerCalculator = CreateSellmeijerCalculator();

            try
            {
                sellmeijerCalculator.Calculate();
            }
            catch (PipingException<Sellmeijer2011Calculator> e)
            {
                throw new PipingCalculationException(e.Message);
            }
            catch (PipingException<SellmeijerBaseCalculator> e)
            {
                throw new PipingCalculationException(e.Message);
            }

            return sellmeijerCalculator;
        }

        private HeaveCalculator CalculateHeave()
        {
            var heaveCalculator = CreateHeaveCalculator();

            try
            {
                heaveCalculator.Calculate();
            }
            catch (PipingException<HeaveCalculator> e)
            {
                throw new PipingCalculationException(e.Message);
            }

            return heaveCalculator;
        }

        private WTIUpliftCalculator CalculateUplift()
        {
            WTIUpliftCalculator upliftCalculator = CreateUpliftCalculator();
            
            try
            {
                upliftCalculator.Calculate();
            }
            catch (WTIUpliftCalculatorException e)
            {
                throw new PipingCalculationException(e.Message);
            }
            catch (PipingException<EffectiveThicknessCalculator> e)
            {
                throw new PipingCalculationException(e.Message);
            }

            return upliftCalculator;
        }

        private HeaveCalculator CreateHeaveCalculator()
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
            return calculator;
        }

        private WTIUpliftCalculator CreateUpliftCalculator()
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
            return calculator;
        }

        private Sellmeijer2011Calculator CreateSellmeijerCalculator()
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
            return calculator;
        }

        private EffectiveThicknessCalculator CalculateEffectiveThickness()
        {
            var calculator = new EffectiveThicknessCalculator
            {
                ExitPointXCoordinate = input.ExitPointXCoordinate,
                PhreaticLevel = input.PhreaticLevelExit,
                SoilProfile = PipingProfileCreator.Create(),
                SurfaceLine = PipingSurfaceLineCreator.Create(),
                VolumicWeightOfWater = input.WaterVolumetricWeight
            };
            calculator.Calculate();
            return calculator;
        }
    }
}