using System;

namespace DelftTools.Utils
{
    public static class VersionExtensions
    {
        /// <summary>
        /// Replaces -1 in version by 0. Allowing 1.3 == 1.3.0.0 
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static Version GetFullVersion(this Version version)
        {
            return new Version(version.Major == -1 ? 0 : version.Major,
                              version.Minor == -1 ? 0 : version.Minor,
                              version.Build == -1 ? 0 : version.Build,
                              version.Revision == -1 ? 0 : version.Revision);
        }
    }
}
