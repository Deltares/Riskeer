using System;

namespace Core.Common.Utils.Diagnostics
{
    /// <summary>
    /// This class is used for debugging purposes, to inform some global listeners about large resource allocation / deallocation (bitmaps, etc.).
    /// </summary>
    public static class ResourceMonitor
    {
        public static event Action<object, object> ResourceAllocated;

        public static event Action<object, object> ResourceDeallocated;

        public static void OnResourceAllocated(object sender, object resource)
        {
            if (ResourceAllocated != null)
            {
                ResourceAllocated(sender, resource);
            }
        }

        public static void OnResourceDeallocated(object sender, object resource)
        {
            if (ResourceDeallocated != null)
            {
                ResourceDeallocated(sender, resource);
            }
        }
    }
}