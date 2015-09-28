using DelftTools.Shell.Core;

using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects;
using DeltaShell.Plugins.DemoGuiPlugin.Views;

namespace DeltaShell.Plugins.DemoGuiPlugin.Controllers
{
    public class WTIProjectViewController : IObserver
    {
        private readonly WTIProjectView view;
        private WTIProject project;

        public WTIProjectViewController(WTIProjectView view)
        {
            this.view = view;
        }

        public void UpdateObserver()
        {
            view.SetProjectName(project.Name);
        }

        public object GetViewData()
        {
            return project;
        }

        public void SetViewData(object viewData)
        {
            if (project != null)
            {
                project.Detach(this);
            }
            project = viewData as WTIProject;
            if (project != null)
            {
                project.Attach(this);

                view.SetProjectName(project.Name);
            }
        }

        public void SetProjectName(string text)
        {
            project.Name = text;
            project.NotifyObservers();
        }
    }
}