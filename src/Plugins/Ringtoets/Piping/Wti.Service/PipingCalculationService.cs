using System;

using log4net;

using Ringtoets.Piping.Service.Properties;

using Wti.Calculation.Piping;
using Wti.Data;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="PipingCalculation"/>. Error and status information is 
    /// logged during the execution of the operation. At the end of an operation, a <see cref="PipingCalculationResult"/> is returned,
    /// representing the result of the operation.
    /// </summary>
    public static class PipingCalculationService
    {
        private static readonly ILog PipingDataLogger = LogManager.GetLogger(typeof(PipingData));

        /// <summary>
        /// Performs a piping calculation based on the supplied <see cref="PipingData"/> and sets <see cref="PipingData.Output"/>
        /// to the <see cref="PipingCalculationResult"/> if the calculation was successful. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="pipingData">The <see cref="PipingData"/> to base the input for the calculation upon.</param>
        /// <returns>If <paramref name="pipingData"/> contains validation errors, then <see cref="PipingCalculationResult.ValidationErrors"/> is returned.
        /// If problems were encountered during the calculation, <see cref="PipingCalculationResult.CalculationErrors"/> is returned. 
        /// Otherwise, <see cref="PipingCalculationResult.Successful"/> is returned.</returns>
        public static PipingCalculationResult PerfromValidatedCalculation(PipingData pipingData)
        {
            PipingCalculationResult validationResult = Validate(pipingData);

            if (validationResult == PipingCalculationResult.Successful)
            {
                ClearOutput(pipingData);
                return Calculate(pipingData);
            }

            return validationResult;
        }

        /// <summary>
        /// Performs validation over the values on the given <paramref name="pipingData"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="pipingData">The <see cref="PipingData"/> for which to validate the values.</param>
        /// <returns>If <paramref name="pipingData"/> contains validation errors, then <see cref="PipingCalculationResult.ValidationErrors"/> is returned.
        /// Otherwise, <see cref="PipingCalculationResult.Successful"/> is returned.</returns>
        public static PipingCalculationResult Validate(PipingData pipingData)
        {
            PipingDataLogger.Info(String.Format(Resources.ValidationStarted_0, DateTimeService.CurrentTimeAsString));

            var validationResults = new PipingCalculation(CreateInputFromData(pipingData)).Validate();
            LogMessagesAsError(Resources.ErrorInPipingValidation_0, validationResults.ToArray());

            PipingDataLogger.Info(String.Format(Resources.ValidationEnded_0, DateTimeService.CurrentTimeAsString));

            return validationResults.Count > 0 ? PipingCalculationResult.ValidationErrors : PipingCalculationResult.Successful;
        }

        private static void LogMessagesAsError(string format, params string[] errorMessages)
        {
            foreach (var errorMessage in errorMessages)
            {
                PipingDataLogger.Error(string.Format(format, errorMessage));
            }
        }

        private static void ClearOutput(PipingData pipingData)
        {
            pipingData.Output = null;
        }

        private static PipingCalculationResult Calculate(PipingData pipingData)
        {
            PipingDataLogger.Info(String.Format(Resources.CalculationStarted_0, DateTimeService.CurrentTimeAsString));

            try
            {
                var pipingResult = new PipingCalculation(CreateInputFromData(pipingData)).Calculate();

                pipingData.Output = new PipingOutput(pipingResult.UpliftZValue,
                                                     pipingResult.UpliftFactorOfSafety,
                                                     pipingResult.HeaveZValue,
                                                     pipingResult.HeaveFactorOfSafety,
                                                     pipingResult.SellmeijerZValue,
                                                     pipingResult.SellmeijerFactorOfSafety);
            }
            catch (PipingCalculationException e)
            {
                LogMessagesAsError(Resources.ErrorInPipingCalculation_0, e.Message);
                return PipingCalculationResult.CalculationErrors;
            }
            finally
            {
                PipingDataLogger.Info(String.Format(Resources.CalculationEnded_0, DateTimeService.CurrentTimeAsString));
            }
            return PipingCalculationResult.Successful;
        }

        private static PipingCalculationInput CreateInputFromData(PipingData pipingData)
        {
            return new PipingCalculationInput(
                pipingData.WaterVolumetricWeight,
                pipingData.UpliftModelFactor,
                pipingData.AssessmentLevel,
                pipingData.PiezometricHeadExit,
                pipingData.DampingFactorExit,
                pipingData.PhreaticLevelExit,
                pipingData.PiezometricHeadPolder,
                pipingData.CriticalHeaveGradient,
                pipingData.ThicknessCoverageLayer,
                pipingData.SellmeijerModelFactor,
                pipingData.SellmeijerReductionFactor,
                pipingData.SeepageLength,
                pipingData.SandParticlesVolumicWeight,
                pipingData.WhitesDragCoefficient,
                pipingData.Diameter70,
                pipingData.DarcyPermeability,
                pipingData.WaterKinematicViscosity,
                pipingData.Gravity,
                pipingData.ThicknessAquiferLayer,
                pipingData.MeanDiameter70,
                pipingData.BeddingAngle,
                pipingData.ExitPointXCoordinate,
                pipingData.SurfaceLine
                );
        }
    }
}