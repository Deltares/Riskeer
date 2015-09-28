namespace DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.Domain
{
    public class HydraulicBoundariesDatabase
    {
        private readonly string dataBasePath;

        public HydraulicBoundariesDatabase(string dataBasePath)
        {
            this.dataBasePath = dataBasePath;
        }

        public string DataBasePath 
        {
            get
            {
                return dataBasePath;
            }
        }
    }
}