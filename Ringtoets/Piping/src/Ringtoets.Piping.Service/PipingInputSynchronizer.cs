using System;
using Core.Common.Base;
using Core.Common.Base.Data;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.InputParameterCalculation;
using Resources = Ringtoets.Piping.Service.Properties.Resources;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// Class responsible for synchronizing piping input.
    /// </summary>
    public class PipingInputSynchronizer : IObserver
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PipingInputSynchronizer));

        private const double seepageLengthStandardDeviationFraction = 0.1;

        private readonly PipingInput input;

        private PipingInputSynchronizer(PipingInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "Cannot create PipingInputSynchronizer without PipingInput.");
            }
            input.Attach(this);
            this.input = input;

            SynchronizeDerivedProperties();
        }

        /// <summary>
        /// Starts the synchronization of the given <see cref="PipingInput"/>.
        /// </summary>
        /// <param name="input">The input to synchronize the values for.</param>
        public static void Synchronize(PipingInput input)
        {
            new PipingInputSynchronizer(input);
        }

        public void UpdateObserver()
        {
            SynchronizeDerivedProperties();
        }

        private void SynchronizeDerivedProperties()
        {
            UpdateAssessmentLevel();
            UpdateSeepageLength();
            UpdateThicknessCoverageLayer();
            UpdateThicknessAquiferLayer();
            UpdatePiezometricHeadExit();
        }

        private void UpdateThicknessAquiferLayer()
        {
            PipingSoilProfile soilProfile = input.SoilProfile;
            RingtoetsPipingSurfaceLine surfaceLine = input.SurfaceLine;
            double exitPointL = input.ExitPointL;

            if (soilProfile != null && surfaceLine != null && !double.IsNaN(exitPointL))
            {
                double thicknessTopAquiferLayer = GetThicknessTopAquiferLayer(soilProfile, surfaceLine, exitPointL);
                TrySetThicknessAquiferLayerMean(input, thicknessTopAquiferLayer);

                if (double.IsNaN(input.ThicknessAquiferLayer.Mean))
                {
                    log.Warn(Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer);
                }
            }
            else
            {
                input.ThicknessAquiferLayer.Mean = (RoundedDouble) double.NaN;
            }
        }

        private static void TrySetThicknessAquiferLayerMean(PipingInput input, double thicknessTopAquiferLayer)
        {
            try
            {
                input.ThicknessAquiferLayer.Mean = (RoundedDouble) thicknessTopAquiferLayer;
            }
            catch (ArgumentOutOfRangeException)
            {
                input.ThicknessAquiferLayer.Mean = (RoundedDouble) double.NaN;
            }
        }

        private static double GetThicknessTopAquiferLayer(PipingSoilProfile soilProfile, RingtoetsPipingSurfaceLine surfaceLine, double exitPointL)
        {
            try
            {
                var zAtL = surfaceLine.GetZAtL(exitPointL);
                return soilProfile.GetTopAquiferLayerThicknessBelowLevel(zAtL);
            }
            catch (ArgumentOutOfRangeException)
            {
                return double.NaN;
            }
        }

        private void UpdateThicknessCoverageLayer()
        {
            if (input.SurfaceLine != null && input.SoilProfile != null & !double.IsNaN(input.ExitPointL))
            {
                TrySetThicknessCoverageLayer();

                if (double.IsNaN(input.ThicknessCoverageLayer.Mean))
                {
                    log.Warn(Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
                }
            }
            else
            {
                input.ThicknessCoverageLayer.Mean = (RoundedDouble)double.NaN;
            }
        }

        private void TrySetThicknessCoverageLayer()
        {
            try
            {
                input.ThicknessCoverageLayer.Mean = (RoundedDouble)InputParameterCalculationService.CalculateThicknessCoverageLayer(input.WaterVolumetricWeight, PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), input.ExitPointL, input.SurfaceLine, input.SoilProfile);
            }
            catch (ArgumentOutOfRangeException)
            {
                input.ThicknessCoverageLayer.Mean = (RoundedDouble)double.NaN;
            }
        }

        private void UpdatePiezometricHeadExit()
        {
            try
            {
                var assessmentLevel = input.AssessmentLevel;
                var dampingFactorExit = PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue();
                var phreaticLevelExit = PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue();

                input.PiezometricHeadExit = (RoundedDouble)InputParameterCalculationService.CalculatePiezometricHeadAtExit(assessmentLevel, dampingFactorExit, phreaticLevelExit);
            }
            catch (ArgumentOutOfRangeException)
            {
                input.PiezometricHeadExit = (RoundedDouble) double.NaN;
            }
        }

        private void UpdateAssessmentLevel()
        {
            input.AssessmentLevel =
                input.HydraulicBoundaryLocation == null ?
                    (RoundedDouble) double.NaN :
                    (RoundedDouble) input.HydraulicBoundaryLocation.DesignWaterLevel;
        }

        private void UpdateSeepageLength()
        {
            try
            {
                input.SeepageLength.Mean = input.ExitPointL - input.EntryPointL;
            }
            catch (ArgumentOutOfRangeException)
            {
                input.SeepageLength.Mean = (RoundedDouble) double.NaN;
            }
            input.SeepageLength.StandardDeviation = input.SeepageLength.Mean*seepageLengthStandardDeviationFraction;
        }
    }
}