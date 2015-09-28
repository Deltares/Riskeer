using System;

namespace SharpMap.Editors.Snapping
{
    [Flags]
    public enum SnapRole { 
        Free, 
        FreeAtObject,
        Start, 
        End, 
        StartEnd, 
        AllTrackers, 
        TrackersNoStartNoEnd, 
        None }
}