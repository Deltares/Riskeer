using System;
using System.Collections.Generic;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
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

        /// <summary>
        /// Get the <see cref="ITreeNodePresenter"/> defined for the <see cref="PipingGuiPlugin"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ITreeNodePresenter"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="IGui.ContextMenuProvider"/> is <c>null</c>.</exception>
        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new PipingFailureMechanismNodePresenter(Gui.ContextMenuProvider)
            {
                RunActivitiesAction = activities => ActivityProgressDialogRunner.Run(Gui.MainWindow, activities)
            };
            yield return new PipingCalculationContextNodePresenter(Gui.ContextMenuProvider)
            {
                RunActivityAction = activity => ActivityProgressDialogRunner.Run(Gui.MainWindow, activity)
            };
            yield return new PipingCalculationGroupContextNodePresenter(Gui.ContextMenuProvider)
            {
                RunActivitiesAction = activities => ActivityProgressDialogRunner.Run(Gui.MainWindow, activities)
            };
            yield return new PipingInputContextNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingSurfaceLineCollectionNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingSurfaceLineNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingSoilProfileCollectionNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingSoilProfileNodePresenter(Gui.ContextMenuProvider);
            yield return new PipingOutputNodePresenter(Gui.ContextMenuProvider);
            yield return new EmptyPipingOutputNodePresenter(Gui.ContextMenuProvider);
            yield return new EmptyPipingCalculationReportNodePresenter(Gui.ContextMenuProvider);
        }
    }
}