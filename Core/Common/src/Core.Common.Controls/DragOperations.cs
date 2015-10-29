using System;

namespace Core.Common.Controls
{
    /// <summary>
    /// Enum that specifies dragoperations. The numeric values are chosen to allow for bitwise comparison.
    /// </summary>
    [Flags]
    public enum DragOperations
    {
        None = 0,
        Move = 1
    }
}