using System;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui
{
    public enum ColorTheme
    {
        Dark,
        Light,
        Metro,
        Aero,
        VS2010,
        Generic
    }

    public static class ColorThemeExtensions
    {
        public static string Localized(this ColorTheme theme)
        {
            return Resources.ResourceManager.GetString(Enum.GetName(typeof(ColorTheme), theme));
        }
    }
}