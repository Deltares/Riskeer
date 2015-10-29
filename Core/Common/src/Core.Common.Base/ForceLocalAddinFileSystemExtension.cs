using Mono.Addins.Database;

namespace Core.Common.Base
{
    /// <summary>
    /// Extension for Mono Addins plugin(addin) detection, to force scanning in local Appdomain
    /// </summary>
    internal class ForceLocalAddinFileSystemExtension : AddinFileSystemExtension
    {
        public override bool RequiresIsolation
        {
            get
            {
                return false;
            }
        }
    }
}