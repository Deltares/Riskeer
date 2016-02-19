using System;

namespace Core.Common.Gui.Forms.ViewManager
{
    /// <summary>
    /// Flags on where views can be located.
    /// </summary>
    [Flags]
    public enum ViewLocation
    {
        /// <summary>
        /// The location reserved for Document Views.
        /// </summary>
        Document = 0x0,
        /// <summary>
        /// Left of the location reserved for Document Views.
        /// </summary>
        Left = 0x1,
        /// <summary>
        /// Right of the location reserved for Document Views.
        /// </summary>
        Right = 0x2,
        /// <summary>
        /// Above the location reserved for Document Views.
        /// </summary>
        Top = 0x4,
        /// <summary>
        /// Below the location reserved for Document Views.
        /// </summary>
        Bottom = 0x8,
        /// <summary>
        /// Floating panel.
        /// </summary>
        Floating = 0x16
    };
}