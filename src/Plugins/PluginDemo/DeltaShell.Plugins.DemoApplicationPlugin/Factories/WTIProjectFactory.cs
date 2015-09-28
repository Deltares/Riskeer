using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Domain;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Schematization;

namespace DeltaShell.Plugins.DemoApplicationPlugin.Factories
{
    public static class WTIProjectFactory
    {
        public static WTIProject CreateWTIProject()
        {
            return new WTIProject
                   {
                       Name = "WTI project",
                       ReferenceLine = new ReferenceLine(),
                       HydraulicBoundariesDatabase = new HydraulicBoundariesDatabase("<Path to hydraulic boundaries database>")
                   };
        }
    }
}
