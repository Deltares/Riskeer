using System.Collections.Generic;
using System.Drawing;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;
using DelftTools.Utils.Collections.Generic;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Domain;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.FailureMechanism;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Schematization;
using DeltaShell.Plugins.DemoGuiPlugin.NodePresenters;
using DeltaShell.Plugins.DemoGuiPlugin.NodePresenters.Domain;
using DeltaShell.Plugins.DemoGuiPlugin.NodePresenters.FailureMechanism;
using DeltaShell.Plugins.DemoGuiPlugin.NodePresenters.Schematization;
using DeltaShell.Plugins.DemoGuiPlugin.PropertClasses;
using DeltaShell.Plugins.DemoGuiPlugin.PropertClasses.Domain;
using DeltaShell.Plugins.DemoGuiPlugin.PropertClasses.FailureMechanism;
using DeltaShell.Plugins.DemoGuiPlugin.PropertClasses.Schematization;
using DeltaShell.Plugins.DemoGuiPlugin.Views;
using Mono.Addins;

namespace DeltaShell.Plugins.DemoGuiPlugin
{
    [Extension(typeof(IPlugin))]
    public class WTIGuiPlugin : GuiPlugin
    {
        public override string Name
        {
            get { return "WTI gui plugin"; }
        }

        public override string DisplayName
        {
            get { return "WTI gui plugin"; }
        }

        public override string Description
        {
            get { return "WTI gui plugin"; }
        }

        public override string Version
        {
            get { return "1.0.0.0"; }
        }

        public override Image Image
        {
            get { return Properties.Resources.plug; }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            // Domain
            yield return new PropertyInfo<HydraulicBoundariesDatabase, HydraulicBoundariesDatabaseProperties>();

            // Failure mechanisms
            yield return new PropertyInfo<IAssessment, AssessmentProperties>();
            yield return new PropertyInfo<IEventedList<IAssessment>, AssessmentsProperties>();

            // Schematization
            yield return new PropertyInfo<ReferenceLine, ReferenceLineProperties>();

            // Other
            yield return new PropertyInfo<WTIProject, WTIProjectProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<WTIProject, WTIProjectView>
                {
                    Description = "WTI project view"
                };
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            // Domain
            yield return new HydraulicBoundariesDatabaseNodePresenter();

            // Failure mechanism
            yield return new AssessmentsNodePresenter();
            yield return new AssessmentNodePresenter();

            // Schematization
            yield return new ReferenceLineNodePresenter();

            // Other
            yield return new WTIProjectNodePresenter();
        }

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get { return new Ribbon(); }
        }
    }
}
