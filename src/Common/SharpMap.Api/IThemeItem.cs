using System;
using System.Drawing;

namespace SharpMap.Api
{
    public interface IThemeItem : IComparable
    {
        string Range { get; }
        string Label { get; set; }
        Bitmap Symbol { get; set; }
        IStyle Style { get; set; }
    }
}