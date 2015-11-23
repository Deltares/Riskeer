using System;

using log4net;

using Ringtoets.Piping.Calculation.Piping;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service.Properties;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="PipingCalculation"/>. Error and status information is 
    /// logged during the execution of the operation. At the end of an operation, a <see cref="PipingCalculationResult"/> is returned,
    /// representing the result of the operation.
    /// </summary>
    public static class PipingCalculationService
    {
        private static readonly ILog pipingDataLogger = LogManager.GetLogger(typeof(PipingCalculationData));

        /// <summary>
        /// Performs validation over the values on the given <paramref name="pipingData"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="pipingData">The <see cref="PipingCalculationData"/> for which to validate the values.</param>
        /// <returns>False if <paramref name="pipingData"/> contains validation errors; True otherwise.</returns>
        public static bool Validate(PipingCalculationData pipingData)
        {
            pipingDataLogger.Info(String.Format(Resources.Validation_Subject_0_started_Time_1_,
                                                pipingData.Name, DateTimeService.CurrentTimeAsString));

            var validationResults = new PipingCalculation(CreateInputFromData(pipingData.InputParameters)).Validate();
            LogMessagesAsError(Resources.Error_in_piping_validation_0, validationResults.ToArray());

            pipingDataLogger.Info(String.Format(Resources.Validation_Subject_0_ended_Time_1_,
                                                pipingData.Name, DateTimeService.CurrentTimeAsString));

            return validationResults.Count == 0;
        }

        /// <summary>
        /// Performs a piping calculation based on the supplied <see cref="PipingCalculationData"/> and sets <see cref="PipingCalculationData.Output"/>
        /// to the <see cref="PipingCalculationResult"/> if the calculation was successful. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="pipingData">The <see cref="PipingCalculationData"/> to base the input for the calculation upon.</param>
        /// <remarks>Consider calling <see cref="Validate"/> first to see if calculation is possible.</remarks>
        public static void Calculate(PipingCalculationData pipingData)
        {
            pipingDataLogger.Info(String.Format(Resources.Calculation_Subject_0_started_Time_1_,
                                                pipingData.Name, DateTimeService.CurrentTimeAsString));

            try
            {
                var pipingResult = new PipingCalculation(CreateInputFromData(pipingData.InputParameters)).Calculate();

                pipingData.Output = new PipingOutput(pipingResult.UpliftZValue,
                                                     pipingResult.UpliftFactorOfSafety,
                                                     pipingResult.HeaveZValue,
                                                     pipingResult.HeaveFactorOfSafety,
                                                     pipingResult.SellmeijerZValue,
                                                     pipingResult.SellmeijerFactorOfSafety);
            }
            catch (PipingCalculationException e)
            {
                LogMessagesAsError(Resources.Error_in_piping_calculation_0, e.Message);
            }
            finally
            {
                pipingDataLogger.Info(String.Format(Resources.Calculation_Subject_0_ended_Time_1_,
                                                    pipingData.Name, DateTimeService.CurrentTimeAsString));
            }
        }

        private static void LogMessagesAsError(string format, params string[] errorMessages)
        {
            foreach (var errorMessage in errorMessages)
            {
                pipingDataLogger.Error(string.Format(format, errorMessage));
            }
        }

        private static PipingCalculationInput CreateInputFromData(PipingInputParameters inputParameters)
        {
            return new PipingCalculationInput(
                inputParameters.WaterVolumetricWeight,
                inputParameters.UpliftModelFactor,
                inputParameters.AssessmentLevel,
                inputParameters.PiezometricHeadExit,
                PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(inputParameters).GetDesignValue(),
                PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(inputParameters).GetDesignValue(),
                inputParameters.PiezometricHeadPolder,
                inputParameters.CriticalHeaveGradient,
                PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(inputParameters).GetDesignValue(),
                inputParameters.SellmeijerModelFactor,
                inputParameters.SellmeijerReductionFactor,
                PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(inputParameters).GetDesignValue(),
                inputParameters.SandParticlesVolumicWeight,
                inputParameters.WhitesDragCoefficient,
                PipingSemiProbabilisticDesignValueFactory.GetDiameter70(inputParameters).GetDesignValue(),
                PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(inputParameters).GetDesignValue(),
                inputParameters.WaterKinematicViscosity,
                inputParameters.Gravity,
                PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(inputParameters).GetDesignValue(),
                inputParameters.MeanDiameter70,
                inputParameters.BeddingAngle,
                inputParameters.ExitPointXCoordinate,
                inputParameters.SurfaceLine,
                inputParameters.SoilProfile
                );
        }
    }
}