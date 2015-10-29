using System;

namespace Core.GIS.SharpMap.Editors.Snapping
{
    [Flags]
    public enum SnapRole
    {
        Free,
        FreeAtObject,
        Start,
        End,
        StartEnd,
        AllTrackers,
        TrackersNoStartNoEnd,
        None
    }
}