using System.Collections.Generic;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Forms;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Plugin
{
    public class PipingGuiPlugin : GuiPlugin
    {
        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new PipingRibbon();
            }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<PipingCalculationContext, PipingCalculationContextProperties>();
            yield return new PropertyInfo<PipingCalculationGroupContext, PipingCalculationGroupContextProperties>();
            yield return new PropertyInfo<PipingInputContext, PipingInputContextProperties>();
            yield return new PropertyInfo<PipingOutput, PipingOutputProperties>();
            yield return new PropertyInfo<RingtoetsPipingSurfaceLine, RingtoetsPipingSurfaceLineProperties>();
            yield return new PropertyInfo<PipingSoilProfile, PipingSoilProfileProperties>();
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new PipingCalculationContextNodePresenter
            {
                RunActivityAction = Gui.ActivityRunner.Enqueue
            };
            yield return new PipingCalculationGroupContextNodePresenter();
            yield return new PipingInputContextNodePresenter();
            yield return new PipingFailureMechanismNodePresenter
            {
                RunActivityAction = Gui.ActivityRunner.Enqueue
            };
            yield return new PipingSurfaceLineCollectionNodePresenter
            {
                ImportSurfaceLinesAction = Gui.CommandHandler.ImportToGuiSelection
            };
            yield return new PipingSurfaceLineNodePresenter();
            yield return new PipingSoilProfileCollectionNodePresenter
            {
                ImportSoilProfilesAction = Gui.CommandHandler.ImportToGuiSelection
            };
            yield return new PipingSoilProfileNodePresenter();
            yield return new PipingOutputNodePresenter();
            yield return new EmptyPipingOutputNodePresenter();
            yield return new EmptyPipingCalculationReportNodePresenter();
        }
    }
}