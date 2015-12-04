﻿using System;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui
{
    /// <summary>
    /// Enumeration for the all the possible color themes in the application.
    /// </summary>
    public enum ColorTheme
    {
        Dark,
        Light,
        Metro,
        Aero,
        VS2010,
        Generic
    }

    /// <summary>
    /// Extension methods for the <see cref="ColorTheme"/> class
    /// </summary>
    public static class ColorThemeExtensions
    {
        /// <summary>
        /// Gets the localized string from the <see cref="Resources"/> based on the 
        /// name of the <see cref="ColorTheme"/>.
        /// </summary>
        /// <param name="theme">The <see cref="ColorTheme"/> for which to get the localized name.</param>
        /// <returns>The localized name from the <see cref="Resources"/>.</returns>
        public static string Localized(this ColorTheme theme)
        {
            return Resources.ResourceManager.GetString(Enum.GetName(typeof(ColorTheme), theme));
        }
    }
}