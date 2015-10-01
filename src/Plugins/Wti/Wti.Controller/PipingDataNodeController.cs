using System;
using System.Linq.Expressions;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using log4net;
using log4net.Core;
using Wti.Calculation.Piping;
using Wti.Data;
using Wti.Forms.NodePresenters;
using Wti.Forms.Properties;

namespace Wti.Controller
{
    public class PipingDataNodeController : IContextMenuProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(PipingCalculationInput));
        private PipingDataNodePresenter nodePresenter = new PipingDataNodePresenter();

        public PipingDataNodePresenter NodePresenter
        {
            get
            {
                return nodePresenter;
            }
        }

        public PipingDataNodeController()
        {
            nodePresenter.ContextMenu = GetContextMenu;
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
                logger.Info(String.Format("Veiligheidsfactor heave: {0}",pipingResult.HeaveFactorOfSafety));
                logger.Info(String.Format("Z heave: {0}",pipingResult.HeaveZValue));
                logger.Info(String.Format("Veiligheidsfactor uplift: {0}",pipingResult.UpliftFactorOfSafety));
                logger.Info(String.Format("Z uplift: {0}", pipingResult.UpliftZValue));
                logger.Info(String.Format("Veiligheidsfactor sellmeijer: {0}",pipingResult.SellmeijerFactorOfSafety));
                logger.Info(String.Format("Z sellmeijer: {0}", pipingResult.SellmeijerZValue));
            }
            catch (PipingCalculationException e)
            {
                logger.Warn(String.Format(Resources.ErrorInPipingCalculation_0, e));
            }
        }
    }
}