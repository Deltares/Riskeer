using System;
using System.Drawing;

namespace Core.GIS.SharpMap.Api
{
    public interface IThemeItem : IComparable
    {
        string Range { get; }
        string Label { get; set; }
        IStyle Style { get; set; }
        Bitmap Symbol { get; }
    }
}