using System;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using log4net;
using Wti.Calculation.Piping;
using Wti.Data;
using Wti.Forms.NodePresenters;
using Wti.Forms.Properties;

namespace Wti.Controller
{
    public class PipingDataNodeController : IContextMenuProvider
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(PipingCalculationInput));
        private readonly PipingDataNodePresenter nodePresenter = new PipingDataNodePresenter();

        public PipingDataNodeController()
        {
            nodePresenter.ContextMenu = GetContextMenu;
        }

        public PipingDataNodePresenter NodePresenter
        {
            get
            {
                return nodePresenter;
            }
        }

        public IMenuItem GetContextMenu(object pipingData)
        {
            var contextMenu = new PipingContextMenuStrip((PipingData) pipingData);
            contextMenu.OnCalculationClick += OnCalculationClick;
            var contextMenuAdapter = new MenuItemContextMenuStripAdapter(contextMenu);
            return contextMenuAdapter;
        }

        private void OnCalculationClick(PipingData pipingData)
        {
            try
            {
                var input = new PipingCalculationInput(
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
                    pipingData.ExitPointXCoordinate
                    );
                var pipingCalculation = new PipingCalculation(input);
                var pipingResult = pipingCalculation.Calculate();

                pipingData.Output = new PipingOutput(pipingResult.UpliftZValue,
                                                     pipingResult.UpliftFactorOfSafety,
                                                     pipingResult.HeaveZValue, pipingResult.HeaveFactorOfSafety, pipingResult.SellmeijerZValue, pipingResult.SellmeijerFactorOfSafety);
            }
            catch (PipingCalculationException e)
            {
                logger.Warn(String.Format(Resources.ErrorInPipingCalculation_0, e.Message));
            }
        }
    }
}