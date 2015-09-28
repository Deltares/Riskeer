using System.Drawing;
using DelftTools.Shell.Gui;
using DeltaShell.Plugins.DemoApplicationPlugin.Factories;

namespace DeltaShell.Plugins.DemoGuiPlugin.Commands
{
    public class WTIProjectCommand : IGuiCommand
    {
        public void Execute(params object[] arguments)
        {
            Gui.Application.Project.Items.Add(WTIProjectFactory.CreateWTIProject());
            Gui.Application.Project.NotifyObservers();
        }

        public void Unexecute() {}

        public string Name
        {
            get
            {
                return "WTI project command";
            }
        }

        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public Image Image { get; set; }

        public bool Checked { get; set; }

        public IGui Gui { get; set; }
    }
}